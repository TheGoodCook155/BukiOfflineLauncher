using BukiOffline.Const;
using BukiOffline.Errors;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace BukiOffline.Services
{
    public class PythonInstaller : Iinstaller
    {
        private ILogger logger;

        private IConfiguration configuration;

        private string pythonUrl => this.configuration
            .GetSection("Python")
            .Get<string>();

        public PythonInstaller(IConfiguration configuration,ILogger logger)
        {
            this.configuration = configuration;
            this.logger = logger.ForContext<PythonInstaller>();
        }

        public async Task<bool> Install()
        {
            string output = string.Empty;

            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "py",
                    Arguments = "-0",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(processInfo);

                string processError = await process!.StandardError.ReadToEndAsync();

                output = await process!.StandardOutput.ReadToEndAsync();

                await process.WaitForExitAsync();

                logger.Information(output);

                logger.Error(processError);

            }
            catch (Exception e) when (e.HResult == (int) PythonError.NotFound)
            {
                await DownloadPythonExecutable();

                await InstallPythonExecutable();

                return true;
            }
           return output.Contains(DependencyVersions.Python);
        }

        private async Task InstallPythonExecutable() 
        {
            if (!File.Exists(ProjectPath.PythonInstallationPath)) 
            {
                throw new Exception("Python installation executable did not download properly. Please try again");
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = ProjectPath.PythonInstallationPath,
                UseShellExecute = true,
                Verb = "runas"
            };

            try
            {
                using var process = Process.Start(processStartInfo);

                await process.WaitForExitAsync();

                logger.Information("Python installer exited with code {ExitCode}", process.ExitCode);

            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == (int) RunAsError.AdminRightsNotGranted)
            {
                logger.Warning("Python installation cancelled by the user.");
            }
        }



        private async Task DownloadPythonExecutable() 
        {

            logger.Information($"Downloading Python from: {this.pythonUrl}");

            using HttpClient client = new HttpClient
            {
                Timeout = Timeout.InfiniteTimeSpan
            };

            using HttpResponseMessage response = await client.GetAsync(
                pythonUrl,
                HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            await using Stream input = await response.Content.ReadAsStreamAsync();

            await using FileStream output = new FileStream(
                ProjectPath.PythonInstallationPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                81920,
                useAsync: true);

            byte[] buffer = new byte[81920];

            int bytesRead;

            while ((bytesRead = await input.ReadAsync(buffer)) > 0)
            {
                await output.WriteAsync(buffer.AsMemory(0, bytesRead));
            }
        } 
    }
}


using Serilog;
using System.Diagnostics;

namespace BukiOffline.Services
{
    public class PythonInstaller : Iinstaller
    {
        private ILogger logger;

        public PythonInstaller(ILogger logger)
        {
            this.logger = logger.ForContext<PythonInstaller>();
        }

        public bool Install(out string error)
        {
            logger.Information("Installing Python");
            //winget install Python.Python.3.10
            var processInfo = new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = "install Python.Python.3.10 --silent --accept-package-agreements --accept-source-agreements",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

           var process = Process.Start(processInfo);

           string processError = process!.StandardError.ReadToEnd();

           process.WaitForExit(3000);

           error = processError;

           logger.Error(error);

           return string.IsNullOrEmpty(processError);
        }

    }
}

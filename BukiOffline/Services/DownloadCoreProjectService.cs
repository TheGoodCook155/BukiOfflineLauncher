
using BukiOffline.Const;
using BukiOffline.EventArguments;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.IO;
using System.Net.Http;

namespace BukiOffline.Services
{
    public class DownloadCoreProjectService
    {
        private ILogger logger;
        private IConfiguration configuration;

        public DownloadCoreProjectService(IConfiguration configuration, ILogger logger)
        {
            this.logger = logger.ForContext<DownloadCoreProjectService>();
            this.configuration = configuration;
        }

        public event EventHandler<DownloadedContentEventArgs> OnDownloadedContentChanged;
        
        public async Task DownloadCoreProject()
        {
            string downloadUrl = configuration
                .GetSection("Core")
                .Get<string>();

            logger.Information($"Project download started from {downloadUrl}");
            
            using HttpClient client = new HttpClient
            {
                Timeout = Timeout.InfiniteTimeSpan
            };

            using HttpResponseMessage response = await client.GetAsync(
                downloadUrl,
                HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            long totalBytes = response.Content.Headers.ContentLength ?? -1;

            if (Directory.Exists(ProjectPath.ExtractedDirectory) || File.Exists(ProjectPath.ZipPath) && GetLocalFileLength(ProjectPath.ZipPath) == totalBytes)
            {
                logger.Information("Project already ready");

                return;
            }

            long downloadedBytes = 0;

            await using Stream input = await response.Content.ReadAsStreamAsync();

            await using FileStream output = new FileStream(
                ProjectPath.ZipPath,
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

                downloadedBytes += bytesRead;

                if (totalBytes > 0)
                {
                    double percentage =
                        (double)downloadedBytes / totalBytes * 100;

                    string formattedPercentage = percentage.ToString("F2");

                    OnDownloadedContentChanged?.Invoke(this, new DownloadedContentEventArgs() { DownloadedSize = formattedPercentage});
                }
            }

            logger.Information("Project download done");
        }

        private static long GetLocalFileLength(string zipPath)
        {
            FileInfo fileInfo = new FileInfo(zipPath);

            long sizeInBytes = fileInfo.Length;

            return sizeInBytes;
        }
    }
}


using BukiOffline.EventArguments;
using System.IO;
using System.Net.Http;

namespace BukiOffline.Services
{
    public class DownloadCoreProjectService : CoreProjectBaseClass
    {
        public event EventHandler<DownloadedContentEventArgs> OnDownloadedContentChanged;
        
        public async Task DownloadCoreProject()
        {
            //https://archive.org/download/buki-updated/buki-updated.zip

            const string downloadUrl = "https://archive.org/download/buki-updated/buki-updated.zip";

            using HttpClient client = new HttpClient
            {
                Timeout = Timeout.InfiniteTimeSpan
            };

            using HttpResponseMessage response = await client.GetAsync(
                downloadUrl,
                HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            long totalBytes = response.Content.Headers.ContentLength ?? -1;

            if (Directory.Exists(this.extractedDirectory) || File.Exists(zipPath) && GetLocalFileLength(zipPath) == totalBytes)
            {
                return;
            }

            long downloadedBytes = 0;

            await using Stream input = await response.Content.ReadAsStreamAsync();

            await using FileStream output = new FileStream(
                zipPath,
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
        }

        private static long GetLocalFileLength(string zipPath)
        {
            FileInfo fileInfo = new FileInfo(zipPath);

            long sizeInBytes = fileInfo.Length;

            return sizeInBytes;
        }
    }
}

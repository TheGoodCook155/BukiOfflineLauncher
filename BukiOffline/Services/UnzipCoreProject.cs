using System.IO;
using System.IO.Compression;

namespace BukiOffline.Services
{
    public class UnzipCoreProject
    {
        //for stash
        string extractPath;
        string fileName;
        string currentDirectory;
        string zipPath;
        string extractedDirectory;

        public UnzipCoreProject()
        {
            this.currentDirectory = AppContext.BaseDirectory;
            this.zipPath = Path.Combine(currentDirectory, @"Core\buki-updated.zip");
            this.extractPath = Path.Combine(currentDirectory, @"Core");
            this.fileName = "buki-updated.zip";
            this.extractedDirectory = Path.Combine(currentDirectory,extractPath, "buki-updated");
        }

        public bool IsZipped { get; set; }

        public void Unzip() 
        {

            if (!File.Exists(zipPath))
            {
                IsZipped = false;
                return;
            }

            if (Directory.Exists(this.extractedDirectory)) 
            {
                Directory.Delete(this.extractedDirectory, true);
            }

                ZipFile.ExtractToDirectory(zipPath, extractPath);
        }

        public void RemoveZip() 
        {
            File.Delete(zipPath);
        }

    }
}

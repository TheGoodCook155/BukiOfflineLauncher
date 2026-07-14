using System.IO;

namespace BukiOffline.Services
{
    public class CoreProjectBaseClass
    {
       protected string extractPath;
       protected string fileName;
       protected string currentDirectory;
       protected string zipPath;
       protected string extractedDirectory;

        public CoreProjectBaseClass()
        {
            this.currentDirectory = AppContext.BaseDirectory;
            this.zipPath = Path.Combine(currentDirectory, @"Core\buki-updated.zip");
            this.extractPath = Path.Combine(currentDirectory, @"Core");
            this.fileName = "buki-updated.zip";
            this.extractedDirectory = Path.Combine(currentDirectory, extractPath, "buki-updated");
        }
    }
}

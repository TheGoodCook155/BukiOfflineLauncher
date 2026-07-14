using System.IO;
using System.IO.Compression;

namespace BukiOffline.Services
{
    public class UnzipCoreProject : CoreProjectBaseClass
    {
        public void Unzip() 
        {

            if (!File.Exists(zipPath))
            {
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

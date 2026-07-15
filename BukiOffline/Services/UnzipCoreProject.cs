using BukiOffline.Const;
using System.IO;
using System.IO.Compression;

namespace BukiOffline.Services
{
    public class UnzipCoreProject
    {
        public void Unzip() 
        {

            if (!File.Exists(ProjectPath.ZipPath))
            {
                return;
            }

            if (Directory.Exists(ProjectPath.ExtractedDirectory)) 
            {
                Directory.Delete(ProjectPath.ExtractedDirectory, true);
            }

                ZipFile.ExtractToDirectory(ProjectPath.ZipPath, ProjectPath.ExtractPath);
        }

        public void RemoveZip() 
        {
            File.Delete(ProjectPath.ZipPath);
        }

    }
}

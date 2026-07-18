using BukiOffline.Const;
using Serilog;
using System.IO;
using System.IO.Compression;

namespace BukiOffline.Services
{
    public class UnzipCoreProject
    {
        private ILogger logger;

        public UnzipCoreProject(ILogger logger)
        {
            this.logger = logger.ForContext<UnzipCoreProject>();
        }
        public void Unzip() 
        {
            logger.Information("Unziping project");

            if (!File.Exists(ProjectPath.ZipPath))
            {
                return;
            }

            if (Directory.Exists(ProjectPath.ExtractedDirectory)) 
            {
                Directory.Delete(ProjectPath.ExtractedDirectory, true);
            }

            ZipFile.ExtractToDirectory(ProjectPath.ZipPath, ProjectPath.ExtractPath);

            logger.Information("Extraction done");

        }

        public void RemoveZip() 
        {
            File.Delete(ProjectPath.ZipPath);

            logger.Information("Removing source zip file");
        }

    }
}

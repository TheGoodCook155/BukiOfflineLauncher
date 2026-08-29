using BukiOffline.Const;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Diagnostics;
using System.IO;

namespace BukiOffline.Services
{
    public class PrerequisitesChecker
    {

        private string[] pipExeArray => configuration
            .GetSection("Pip")
            .Get<string[]>();


        //speechbrain
        private string[] speechbrainExeArray => configuration
            .GetSection("Speechbrain")
            .Get<string[]>();

        //transformers
        private string[] transformersExeArray => configuration
            .GetSection("Transformers")
            .Get<string[]>();

        //librosa
        private string[] librosaExeArray => configuration
            .GetSection("Librosa")
            .Get<string[]>();

        //Torch
        private string torch => configuration
            .GetSection("Torch")
            .Get<string>();

        //Numpy
        private string numpy => configuration
            .GetSection("Numpy")
            .Get<string>();

        //HuggingFace Hub
        private string huggingFaceHub => configuration
            .GetSection("HuggingFaceHub")
            .Get<string>();

        public string ErrorState { get; set; } = string.Empty;

        private ILogger logger;

        private IConfiguration configuration;

        public PrerequisitesChecker(IConfiguration configruation, ILogger logger)
        {
            this.configuration = configruation;
            this.logger = logger.ForContext<PrerequisitesChecker>();
        }

        public async Task<bool> Check(Dictionary<Dependency,ProcessStartInfo> processStartInfoInfoList) 
        {
            foreach (var kvp in processStartInfoInfoList)
            {
                var processStartInfo = kvp.Value;

                bool dependencyPresent = await CheckDependency(kvp.Key);

                logger.Information($"Dependency is present: {dependencyPresent}");

                if (dependencyPresent) 
                {
                    continue;
                }

                logger.Information("Starting process");

                using var process = Process.Start(processStartInfo);

                if (process == null)
                {
                    ErrorState += " Failed to start process.";
                    logger.Information("Process is null");
                    return false;
                }

                Task<string> outputTask = process.StandardOutput.ReadToEndAsync();

                Task<string> errorTask = process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                string output = await outputTask;

                logger.Information($"{output}");

                string error = await errorTask;

                logger.Error(error);

                if (kvp.Key == Dependency.VenvCreate) 
                {
                    continue;
                }

                bool result = false;

                switch (kvp.Key)
                {
                    case Dependency.VenvCreate:
                        result = CheckVersion(output, DependencyVersions.VenvCreate);
                        ErrorState += "\n" + error;
                        break;
                    case Dependency.Torch:
                        result = CheckVersion(output, DependencyVersions.Torch);
                        ErrorState += "\n" + error;
                        break;

                    case Dependency.Speechbrain:
                        result = CheckVersion(output, DependencyVersions.Speechbrain);
                        ErrorState += "\n" + error;
                        break;

                    case Dependency.Transformers:
                        result = CheckVersion(output, DependencyVersions.Transformers);
                        ErrorState += "\n" + error;
                        break;

                    case Dependency.Tokenizers:
                        result = CheckVersion(output, DependencyVersions.Tokenizers);
                        ErrorState += "\n" + error;
                        break;

                    case Dependency.Numpy:
                        result = CheckVersion(output, DependencyVersions.Numpy);
                        ErrorState += "\n" + error;
                        break;

                    case Dependency.Requests:
                        result = CheckVersion(output, DependencyVersions.Requests);
                        ErrorState += "\n" + error;
                        break;

                    case Dependency.Librosa:
                        result = CheckVersion(output, DependencyVersions.Librosa);
                        ErrorState += "\n" + error;
                        break;

                    case Dependency.HuggingFaceHub:
                        result = CheckVersion(output, DependencyVersions.HuggingFace);
                        ErrorState += "\n" + error;
                        break;
                }

                if (!result && !string.IsNullOrWhiteSpace(error) && string.IsNullOrEmpty(output))
                {
                    ErrorState = error;
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> CheckDependency(Dependency key)
        {
            logger.Information($"Checking dependency: {key}");

            if (!Directory.Exists(ProjectPath.SriptsDirectory)) 
            {
                return false;
            }

            var fileNames = Directory
                                .GetFiles(ProjectPath.SriptsDirectory)
                                .Select(Path.GetFileName)
                                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            switch (key) 
            {
                case Dependency.VenvCreate:
                    if (Directory.Exists(ProjectPath.VenvDirectory)) 
                    {
                        return true;
                    }
                    break;
                case Dependency.Pip:
                    return pipExeArray.All(fileNames.Contains);

                case Dependency.Torch:
                    return fileNames.Contains(torch);

                case Dependency.Speechbrain:
                    return speechbrainExeArray.All(fileNames.Contains);

                case Dependency.Transformers:
                    return transformersExeArray.All(fileNames.Contains);

                case Dependency.Tokenizers:
                    return true;
                case Dependency.Numpy:
                    return fileNames.Contains(numpy);
                case Dependency.Requests:
                    return true;
                case Dependency.Librosa:
                    return librosaExeArray.All(fileNames.Contains);
                case Dependency.HuggingFaceHub:
                    return fileNames.Contains(huggingFaceHub);
                case Dependency.SentencePiece:
                    return false;
            }

            return false;
        }

        public static bool CheckVersion(string result, string dependencyVersion) 
        {
            if (!result.Contains(dependencyVersion)) 
            {
                return false;
            }
            return true;
        }
    }
}

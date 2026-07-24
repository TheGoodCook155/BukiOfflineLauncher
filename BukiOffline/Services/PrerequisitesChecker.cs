using BukiOffline.Const;
using Serilog;
using System.Diagnostics;
using System.IO;

namespace BukiOffline.Services
{
    public class PrerequisitesChecker
    {

        private string[] pipExeArray = { "pip.exe", "pip3.exe","pip3.10.exe" };

        //speechbrain
        private string[] speechbrainExeArray = {"f2py.exe",
                                                "hf.exe",
                                                "httpx.exe",
                                                "huggingface-cli.exe",
                                                "idna.exe",
                                                "numpy-config.exe",
                                                "tiny-agents.exe",
                                                "tqdm.exe",
                                                "convert-caffe2-to-onnx.exe",
                                                "convert-onnx-to-caffe2.exe" };

        //transformers
        private string[] transformersExeArray = {"normalizer.exe",
                                                "tiny-agents.exe",
                                                "huggingface-cli.exe",
                                                "hf.exe",
                                                "transformers-cli.exe" };
        //librosa
        private string[] librosaExeArray = { "cffi-gen-src.exe", "numba" };

        public string ErrorState { get; set; } = string.Empty;

        private ILogger logger;

        public PrerequisitesChecker(ILogger logger)
        {
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
                        ErrorState += " " + error;
                        break;
                    case Dependency.Torch:
                        result = CheckVersion(output, DependencyVersions.Torch);
                        ErrorState += " " + error;
                        break;

                    case Dependency.Speechbrain:
                        result = CheckVersion(output, DependencyVersions.Speechbrain);
                        ErrorState += " " + error;
                        break;

                    case Dependency.Transformers:
                        result = CheckVersion(output, DependencyVersions.Transformers);
                        ErrorState += " " + error;
                        break;

                    case Dependency.Tokenizers:
                        result = CheckVersion(output, DependencyVersions.Tokenizers);
                        ErrorState += " " + error;
                        break;

                    case Dependency.Numpy:
                        result = CheckVersion(output, DependencyVersions.Numpy);
                        ErrorState += " " + error;
                        break;

                    case Dependency.Requests:
                        result = CheckVersion(output, DependencyVersions.Requests);
                        ErrorState += " " + error;
                        break;

                    case Dependency.Librosa:
                        result = CheckVersion(output, DependencyVersions.Librosa);
                        ErrorState += " " + error;
                        break;

                    case Dependency.HuggingFaceHub:
                        result = CheckVersion(output, DependencyVersions.HuggingFace);
                        ErrorState += " " + error;
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
                    return fileNames.Contains("torchrun.exe");

                case Dependency.Speechbrain:
                    return speechbrainExeArray.All(fileNames.Contains);

                case Dependency.Transformers:
                    return transformersExeArray.All(fileNames.Contains);

                case Dependency.Tokenizers:
                    //no new dependencies installed
                    return true;
                case Dependency.Numpy:
                    return fileNames.Contains("f2py.exe");
                case Dependency.Requests:
                    return true;
                case Dependency.Librosa:
                    return librosaExeArray.All(fileNames.Contains);
                case Dependency.HuggingFaceHub:
                    return fileNames.Contains("huggingface-cli.exe");
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

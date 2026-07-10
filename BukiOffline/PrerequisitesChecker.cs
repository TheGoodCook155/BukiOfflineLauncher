using BukiOffline.Const;
using BukiOffline.Services;
using System.Diagnostics;

namespace BukiOffline
{
    public class PrerequisitesChecker
    {
        public string ErrorState { get; set; } = string.Empty;

        private Iinstaller pythonInstaller;

        public PrerequisitesChecker(Iinstaller pythonInstaller)
        {
            this.pythonInstaller = pythonInstaller;
        }
        public bool Check(Dictionary<Dependency,ProcessStartInfo> processStartInfoInfoList) 
        {
            foreach (var kvp in processStartInfoInfoList) 
            {
                var processStartInfo = kvp.Value;

                using var process = Process.Start(processStartInfo);

                string output = process!.StandardOutput.ReadToEnd();

                string error = process.StandardError.ReadToEnd();

                process.WaitForExit(3000);

                bool result = false;

                switch (kvp.Key) 
                {
                    case Dependency.Python:
                        result = CheckVersion(output, DependencyVersions.Python);
                        if (!result && string.IsNullOrEmpty(output)) 
                        {
                            bool installResult = pythonInstaller.Install(out var installError);
                            ErrorState = ErrorState + " " + installError;
                        }
                        break;
                }

                if (!string.IsNullOrWhiteSpace(error) && !result && string.IsNullOrEmpty(output))
                {
                    this.ErrorState = error;
                    return false;
                }
            }

            return true;
        }

        private static bool CheckVersion(string result, string dependencyVersion) 
        {
            if (!result.Contains(dependencyVersion)) 
            {
                return false;
            }
            return true;
        }

        
    }
}

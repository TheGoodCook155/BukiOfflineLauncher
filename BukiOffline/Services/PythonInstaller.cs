
using System.Diagnostics;

namespace BukiOffline.Services
{
    public class PythonInstaller : Iinstaller
    {
        public bool Install(out string error)
        {
            //winget install Python.Python.3.10
            var processInfo = new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = "install Python.Python.3.10 --silent --accept-package-agreements --accept-source-agreements",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

           var process = Process.Start(processInfo);

           string processError = process!.StandardError.ReadToEnd();

           process.WaitForExit(3000);

           error = processError;

           return string.IsNullOrEmpty(processError);
        }

    }
}

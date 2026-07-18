using BukiOffline.Const;
using Serilog;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace BukiOffline.Services
{
    public class AppLauncher
    {

        private ILogger logger;

        public AppLauncher(ILogger logger)
        {
            this.logger = logger.ForContext<AppLauncher>();
        }

        public Process process { get; private set; }

        public event EventHandler OnProcessStartEvent;

        public event EventHandler OnUACCancelledEvent;
        public async Task LaunchApp() 
        {

            try
            {
                logger.Information("Core process started");

                process = Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    WorkingDirectory = ProjectPath.ExtractedDirectory,
                    UseShellExecute = true,
                    Verb = "runas",
                    Arguments = $@"/k title BukiASR && cd /d ""{ProjectPath.ExtractedDirectory}"" && venv\Scripts\activate && python run.py"
                });
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == 1223)
            {
                this.OnUACCancelledEvent?.Invoke(this, EventArgs.Empty);

                logger.Information("Administrator rights not granted");
            }

            if (process == null)
            {
                return;
            }

            this.OnProcessStartEvent?.Invoke(this,EventArgs.Empty);

            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();

            Task<string> errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            string output = await outputTask;
            string error = await errorTask;
            
            logger.Error(error);
        }

        public void SetRunPyFile(string filePath, string device) 
        {
            logger.Information("Setting run.py");

            logger.Information($"Audio file path: {filePath}");

            logger.Information($"Device: {device}");

            var file = Path.Combine(ProjectPath.ExtractedDirectory, ProjectPath.RunPy);

            string [] lines = File.ReadAllLines(file);

            var removedComments = lines.Where(el => !el.StartsWith("#")).ToArray();

            for (int i = 0; i < removedComments.Length; i++)
            {
                string line = removedComments[i];

                if (line.StartsWith("#")) 
                {
                    removedComments[i] = string.Empty;
                }

                if (line.Contains("source")) 
                {
                    removedComments[i] = "source=\".\",";
                }


                if (line.Contains("audio_file_path ="))
                {
                    string path = Path.GetFullPath(filePath)
                        .Replace("\\", "\\\\");

                    removedComments[i] = $"    audio_file_path = \"{path}\"";
                }

                if (device == "GPU" && line.Contains("device ="))
                {
                    removedComments[i] = "device = torch.device(\"cuda\" if torch.cuda.is_available() else \"cpu\")";
                }
                else if (device == "CPU" && line.Contains("device ="))
                {
                    removedComments[i] = "device = torch.device(\"cpu\")";
                }
            }

            File.WriteAllLines(file, removedComments);

            logger.Information("Writing to run.py");
        }
    }
}

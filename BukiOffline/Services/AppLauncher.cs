using BukiOffline.Const;
using System.Diagnostics;
using System.IO;

namespace BukiOffline.Services
{
    public class AppLauncher
    {
        public Process process { get; private set; }
        public async Task LaunchApp() 
        {

           process = Process.Start(new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                WorkingDirectory = ProjectPath.ExtractedDirectory,
                UseShellExecute = true,
                Verb = "runas",
               Arguments = $@"/k title BukiASR && cd /d ""{ProjectPath.ExtractedDirectory}"" && venv\Scripts\activate && python run.py"
           });

            if (process == null)
            {
                return;
            }


            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();

            Task<string> errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            string output = await outputTask;
            string error = await errorTask;
            //log the output and error
        }

        public void SetRunPyFile(string filePath, string device) 
        {
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
        }
    }
}

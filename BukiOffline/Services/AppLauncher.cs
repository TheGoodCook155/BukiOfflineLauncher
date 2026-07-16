using BukiOffline.Const;
using System.Diagnostics;
using System.IO;

namespace BukiOffline.Services
{
    public class AppLauncher
    {
        public async Task LaunchApp() 
        {
            //foreach(var processStartInfo in startAppProcessStartInfoList) 
            //{
            //    using var process = Process.Start(processStartInfo);

            //    if (process == null)
            //    {
            //        return;
            //    }

            //    Task<string> outputTask = process.StandardOutput.ReadToEndAsync();

            //    Task<string> errorTask = process.StandardError.ReadToEndAsync();

            //    await process.WaitForExitAsync();

            //    string output = await outputTask;
            //    string error = await errorTask;
            //    //log the output and error
            //}

            using var process = Process.Start(new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                WorkingDirectory = ProjectPath.ExtractedDirectory,
                UseShellExecute = true,
                Verb = "runas",
                Arguments = $@"/k cd /d ""{ProjectPath.ExtractedDirectory}"" && venv\Scripts\activate && python run.py"
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

        public void SetRunPyFile() 
        {
            // this should be called first

            //change one line

            //source=".",# on fist execution this line should be: source="Macedonian-ASR/buki-wav2vec2-2.0" after that change it to the local folder . where the project resides

            var file = Path.Combine(ProjectPath.ExtractedDirectory, ProjectPath.RunPy);

            string [] lines = File.ReadAllLines(file);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line.Contains("source")) 
                {
                    lines[i] = "source=\".\",";
                }
            }

            File.WriteAllLines(file, lines);

            // choose file?

            // choose GPU vs CPU (this should be first maybe)
        }
    }
}

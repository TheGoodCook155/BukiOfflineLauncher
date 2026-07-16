using System.Diagnostics;
using System.IO;

namespace BukiOffline.Const
{
    public static class ProcessesStartInfoHolder
    {
        public static Dictionary<Dependency, ProcessStartInfo> DependencyProcessStartInfoList = new Dictionary<Dependency, ProcessStartInfo>()
        {
            {
                Dependency.Python,
                new ProcessStartInfo
                {
                    FileName = "py",
                    Arguments = "-0",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            },
              {
                Dependency.VenvCreate,
                new ProcessStartInfo
                {
                    FileName = "py",
                    Arguments = "-3.10 -m venv venv",
                    WorkingDirectory = ProjectPath.ExtractedDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            },
            {
                Dependency.Pip,
                new ProcessStartInfo
                {
                    FileName = Path.Combine(ProjectPath.ExtractedDirectory,"venv\\Scripts\\python.exe"),
                    Arguments = "-m pip install --upgrade pip",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            },
            {
                Dependency.Torch,
                new ProcessStartInfo
                {
                    FileName = Path.Combine(ProjectPath.ExtractedDirectory, "venv\\Scripts\\python.exe"),
                    Arguments = "-m pip install torch==2.1.2 torchaudio==2.1.2",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            },
            {
                Dependency.Speechbrain,
                new ProcessStartInfo
                {
                    FileName = Path.Combine(ProjectPath.ExtractedDirectory,"venv\\Scripts\\python.exe"),
                    Arguments = "-m pip install speechbrain==1.0.0",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            },
            {
                Dependency.Transformers,
                new ProcessStartInfo
                {
                    FileName = Path.Combine(ProjectPath.ExtractedDirectory,"venv\\Scripts\\python.exe"),
                    Arguments = "-m pip install transformers==4.30.2",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            },
            {
                Dependency.Tokenizers,
                new ProcessStartInfo
                {
                    FileName = Path.Combine(ProjectPath.ExtractedDirectory,"venv\\Scripts\\python.exe"),
                    Arguments = "-m pip install tokenizers==0.13.3",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            },
            {
                Dependency.Numpy,
                new ProcessStartInfo
                {
                    FileName = Path.Combine(ProjectPath.ExtractedDirectory,"venv\\Scripts\\python.exe"),
                    Arguments = "-m pip install \"numpy<2\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            },
            {
                Dependency.Requests,
                new ProcessStartInfo
                {
                    FileName = Path.Combine(ProjectPath.ExtractedDirectory,"venv\\Scripts\\python.exe"),
                    Arguments = "-m pip install requests",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            },
            {
                Dependency.Librosa,
                new ProcessStartInfo
                {
                    FileName = Path.Combine(ProjectPath.ExtractedDirectory,"venv\\Scripts\\python.exe"),
                    Arguments = "-m pip install librosa",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            },
            {
                Dependency.HuggingFaceHub,
                new ProcessStartInfo
                {
                    FileName = Path.Combine(ProjectPath.ExtractedDirectory,"venv\\Scripts\\python.exe"),
                    Arguments = "-m pip install huggingface_hub==0.19.4",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            },
        };
    }
}

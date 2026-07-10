
using System.Diagnostics;

namespace BukiOffline.Const
{
    public static class ProcessesStartInfoHolder
    {
        public static Dictionary<Dependency, ProcessStartInfo> ProcessStartInfoList = new Dictionary<Dependency, ProcessStartInfo>()
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
            }
        };


    }
}

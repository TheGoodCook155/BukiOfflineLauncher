using System.IO;

namespace BukiOffline.Const
{
    public static class ProjectPath
    {
       public static string CurrentDirectory = AppContext.BaseDirectory;
       public static string ZipPath = Path.Combine(CurrentDirectory, @"Core\buki-updated.zip");
       public static string ExtractPath = Path.Combine(CurrentDirectory, @"Core");
       public static string FileName = "buki-updated.zip";
       public static string ExtractedDirectory = Path.Combine(CurrentDirectory, ExtractPath, "buki-updated");
       public static string VenvDirectory = Path.Combine(CurrentDirectory, ExtractPath, "buki-updated","venv");
       public static string SriptsDirectory = Path.Combine(CurrentDirectory, ExtractPath, "buki-updated","venv","Scripts");
    }
}

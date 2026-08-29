using BukiOffline.Services;
using BukiOffline.ViewModels;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Windows;

namespace BukiOffline
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Iinstaller pythonInstaller;
        private UnzipCoreProject unzipCoreProject;
        private DownloadCoreProjectService downloadCoreProjectService;
        private AppLauncher appLauncher;
        private IConfiguration configuration;
        private void Init(ILogger logger) 
        {
            this.configuration = new ConfigurationBuilder()
                                .SetBasePath(AppContext.BaseDirectory)
                                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                                .Build();
            this.pythonInstaller = new PythonInstaller(configuration, logger);
            this.unzipCoreProject = new UnzipCoreProject(logger);
            this.downloadCoreProjectService = new DownloadCoreProjectService(configuration,logger);
            this.appLauncher = new AppLauncher(logger);  
        
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ILogger logger = new LoggerConfiguration()
                        .MinimumLevel.Information()
    .                    WriteTo.File(
                            path: "Logs\\log-.txt",
                            rollingInterval: RollingInterval.Day,
                            outputTemplate:"{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
    .                    CreateLogger();

            this.Init(logger);

            var prerequisitesChecker = new PrerequisitesChecker(configuration,logger);

            var mainViewModel = new MainViewModel(
                prerequisitesChecker,
                this.unzipCoreProject,
                this.downloadCoreProjectService,
                this.appLauncher,
                this.pythonInstaller,
                logger);

            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            mainWindow.Show();

            mainViewModel.Launch();

        }

        protected override void OnExit(ExitEventArgs e)
        {
            this.appLauncher.process?.Kill();
            base.OnExit(e);
        }
    }

}

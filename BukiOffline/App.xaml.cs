using BukiOffline.Services;
using BukiOffline.ViewModels;
using System.Configuration;
using System.Data;
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
        private void Init() 
        {
            this.pythonInstaller = new PythonInstaller();
            this.unzipCoreProject = new UnzipCoreProject();
            this.downloadCoreProjectService = new DownloadCoreProjectService();
            this.appLauncher = new AppLauncher();   
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.Init();

            var prerequisitesChecker = new PrerequisitesChecker(this.pythonInstaller);

            var mainViewModel = new MainViewModel(prerequisitesChecker,
                this.unzipCoreProject,
                this.downloadCoreProjectService,
                this.appLauncher);

            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            mainWindow.Show();

            mainViewModel.Launch();

        }
    }

}

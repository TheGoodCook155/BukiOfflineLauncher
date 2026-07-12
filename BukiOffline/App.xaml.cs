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
        private void Init() 
        {
            this.pythonInstaller = new PythonInstaller();
            this.unzipCoreProject = new UnzipCoreProject();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.Init();

            var prerequisitesChecker = new PrerequisitesChecker(this.pythonInstaller);

            var mainViewModel = new MainViewModel(prerequisitesChecker,this.unzipCoreProject);

            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            mainWindow.Show();

            mainViewModel.Launch();

        }
    }

}

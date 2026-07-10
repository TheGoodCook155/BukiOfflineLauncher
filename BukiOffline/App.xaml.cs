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

        private void Init() 
        {
            this.pythonInstaller = new PythonInstaller();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.Init();

            var prerequisitesChecker = new PrerequisitesChecker(this.pythonInstaller);

            var mainViewModel = new MainViewModel(prerequisitesChecker);

            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            mainWindow.Show();

            mainViewModel.Launch();

        }
    }

}

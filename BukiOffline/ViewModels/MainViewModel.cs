
using BukiOffline.Commands;
using BukiOffline.Const;
using BukiOffline.Services;
using Microsoft.Win32;
using Serilog;
using System.Windows;
using System.Windows.Input;

namespace BukiOffline.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private PrerequisitesChecker prerequisitesChecker;

        private UnzipCoreProject unzipCoreProject;

        private DownloadCoreProjectService downloadCoreProjectService;

        private AppLauncher appLauncher;

        private Iinstaller pythonInstaller;

        private ILogger logger;

        private bool appCanBeRestarted;
        public bool AppCanBeRestarted 
        {
            get => this.appCanBeRestarted; 
            private set
            {
                if (this.appCanBeRestarted != value)
                {
                    this.appCanBeRestarted = value;
                    OnPropertyChanged(nameof(this.AppCanBeRestarted));
                }
            }
        }

        public ICommand RestartApp {  get; set; }

        private string? file;

        private readonly TaskCompletionSource<string> deviceSelectionCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

        private string cpuGpuComboBoxSelectedValue;
        public string CpuGpuComboBoxSelectedValue
        {
            get => this.cpuGpuComboBoxSelectedValue;
            set
            {
                if (this.cpuGpuComboBoxSelectedValue != value)
                {
                    this.cpuGpuComboBoxSelectedValue = value;
                    OnPropertyChanged(nameof(this.CpuGpuComboBoxSelectedValue));

                    if (!string.IsNullOrEmpty(value))
                    {
                        deviceSelectionCompletionSource.TrySetResult(value);
                    }
                }
            }
        }

        private bool extractProjectVisibility;
        public bool ExtractProjectVisibility 
        {
            get => this.extractProjectVisibility;
            set 
            {
                if (this.extractProjectVisibility != value)
                {
                    this.extractProjectVisibility = value;
                    OnPropertyChanged(nameof(this.ExtractProjectVisibility));
                }
            }
        }

        private bool downloadingProjectVisibility;
        public bool DownloadingProjectVisibility
        {
            get => this.downloadingProjectVisibility;
            set
            {
                if (this.downloadingProjectVisibility != value)
                {
                    this.downloadingProjectVisibility = value;
                    OnPropertyChanged(nameof(this.DownloadingProjectVisibility));
                }
            }
        }

        private bool checkingDependenciesVisibility;
        public bool CheckingDependenciesVisibility
        {
            get => this.checkingDependenciesVisibility;
            set
            {
                if (this.checkingDependenciesVisibility != value)
                {
                    this.checkingDependenciesVisibility = value;
                    OnPropertyChanged(nameof(this.CheckingDependenciesVisibility));
                }
            }
        }

        private bool downloadPythonVisibility;
        public bool DownloadPythonVisibility
        {
            get => this.downloadPythonVisibility;
            set
            {
                if (this.downloadPythonVisibility != value)
                {
                    this.downloadPythonVisibility = value;
                    OnPropertyChanged(nameof(this.DownloadPythonVisibility));
                }
            }
        }

        private string downloadPythonMessage = "Python се симнува";
        public string DownloadPythonMessage
        {
            get => this.downloadPythonMessage;
            set
            {
                if (this.downloadPythonMessage != value)
                {
                    this.downloadPythonMessage = value;
                    OnPropertyChanged(nameof(this.DownloadPythonMessage));
                }
            }
        }

        private string unzipMessage = "Проектот се отпакува";
        public string UnzipMessage
        {
            get => this.unzipMessage;
            set
            {
                if (this.unzipMessage != value)
                {
                    this.unzipMessage = value;
                    OnPropertyChanged(nameof(this.UnzipMessage));
                }
            }
        }

        private string downloadMessage = "Проектот се симнува";
        public string DownloadMessage
        {
            get => this.downloadMessage;
            set
            {
                if (this.downloadMessage != value)
                {
                    this.downloadMessage = value;
                    OnPropertyChanged(nameof(this.DownloadMessage));
                }
            }
        }

        private string checkingDependenciesMessage = "Проверка на библиотеки";
        public string CheckingDependenciesMessage
        {
            get => this.checkingDependenciesMessage;
            set
            {
                if (this.checkingDependenciesMessage != value)
                {
                    this.checkingDependenciesMessage = value;
                    OnPropertyChanged(nameof(this.CheckingDependenciesMessage));
                }
            }
        }

        private string downloadPercent = "0.00";
        public string DownloadPercent
        {
            get => this.downloadPercent;
            set
            {
                if (this.downloadPercent != value)
                {
                    this.downloadPercent = value;
                    OnPropertyChanged(nameof(this.DownloadPercent));
                }
            }
        }

        public MainViewModel(
            PrerequisitesChecker prerequisitesChecker,
            UnzipCoreProject unzipCoreProject,
            DownloadCoreProjectService downloadCoreProjectService,
            AppLauncher appLauncher,
            Iinstaller pythonInstaller,
            ILogger logger)
        {
            this.prerequisitesChecker = prerequisitesChecker;
            this.unzipCoreProject = unzipCoreProject;
            this.downloadCoreProjectService = downloadCoreProjectService;
            this.appLauncher = appLauncher;
            this.logger = logger.ForContext<MainViewModel>();
            this.pythonInstaller = pythonInstaller;
            this.Init();
        }

        public async Task RestartApplicationCommand() 
        {
            logger.Information("Restarting app");

            this.appLauncher.process?.Kill();

            this.AppCanBeRestarted = false;

            CommandManager.InvalidateRequerySuggested();

            RestartLabels();

            await this.Launch();
        }

        private void RestartLabels() 
        {
            this.DownloadPercent = "0.00";
            this.CheckingDependenciesMessage = "Проверка на библиотеки";
            this.CheckingDependenciesVisibility = false;
            this.DownloadMessage = "Проектот се симнува";
            this.DownloadingProjectVisibility = false;
            this.UnzipMessage = "Проектот се отпакува";
            this.ExtractProjectVisibility = false;
            this.DownloadPythonMessage = "Python се симнува";
            this.DownloadPythonVisibility = false;
        }

        private void SubscribeToOnDownloadedContentChanged() 
        {
            this.downloadCoreProjectService.OnDownloadedContentChanged += DownloadCoreProjectService_OnDownloadedContentChanged;
        }

        private void SubscribeToAppLauncherOnProcessStartEvent() 
        {
            this.appLauncher.OnProcessStartEvent += AppLauncher_OnProcessStartEvent;
        }

        private void SubscribeToAppLauncherOnUACCancelledEvent()
        {
            this.appLauncher.OnUACCancelledEvent += AppLauncher_OnUACCancelledEvent;
        }

        private void AppLauncher_OnUACCancelledEvent(object? sender, EventArgs e)
        {
            MessageBox.Show("Потребен е администраторски пристап", "Инфо", MessageBoxButton.OK);

            this.AppCanBeRestarted = true;
            
            CommandManager.InvalidateRequerySuggested();
        }

        private void Init() 
        {
            this.SubscribeToOnDownloadedContentChanged();

            this.SubscribeToAppLauncherOnProcessStartEvent();

            this.SubscribeToAppLauncherOnUACCancelledEvent();

            this.RestartApp = new RestartAppCommand(this);

            CheckingDependenciesVisibility = false;
        }

        private void AppLauncher_OnProcessStartEvent(object? sender, EventArgs e)
        {
            this.AppCanBeRestarted = true;

            CommandManager.InvalidateRequerySuggested();
        }

        private void DownloadCoreProjectService_OnDownloadedContentChanged(object? sender, EventArguments.DownloadedContentEventArgs e)
        {
            this.DownloadPercent = e.DownloadedSize + "%";
        }

        private async Task ShowOpenDialog() 
        {
            FileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "WAV files (*.wav)|*.wav";

            openFileDialog.Title = "Одбери WAV фајл";

            bool? res = openFileDialog.ShowDialog();

            if (res == true)
            {
                file = openFileDialog.FileName;
            }
            else
            {
                MessageBox.Show("Одбери аудио фајл", "Инфо", MessageBoxButton.OK);
                await Task.Delay(5000);
                await ShowOpenDialog();
            }
        }

        public async Task Launch()
        {
            string device = await deviceSelectionCompletionSource.Task;

            await ShowOpenDialog();

            await DownloadCoreProject();

            await UnzipProject();

            this.appLauncher.SetRunPyFile(this.file!,device);

            await InstallPython();

            bool launchResult = await PrerequisitesChecker();

            if (launchResult)
            {
                await appLauncher.LaunchApp();

                return;
            }

            this.logger.Information("Listing errors");

            this.logger.Error(this.prerequisitesChecker.ErrorState);
        }

        private async Task InstallPython() 
        {
            this.DownloadPythonVisibility = true;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            CancellationToken token = cancellationTokenSource.Token;

            var messageProgressTask = MessageProgress(() => this.DownloadPythonMessage, value => this.DownloadPythonMessage = value, token);

            await this.pythonInstaller.Install();

            cancellationTokenSource.Cancel();

            this.DownloadPythonMessage = this.DownloadPythonMessage.Trim('.') + "... - Готово!";
        }

        private async Task<bool> PrerequisitesChecker() 
        {
            this.CheckingDependenciesVisibility = true;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            CancellationToken token = cancellationTokenSource.Token;

            var messageProgressTask = MessageProgress(() => CheckingDependenciesMessage, value => CheckingDependenciesMessage = value, token);

            bool launchResult = await prerequisitesChecker.Check(ProcessesStartInfoHolder.DependencyProcessStartInfoList);

            cancellationTokenSource.Cancel();

            CheckingDependenciesMessage = CheckingDependenciesMessage.Trim('.') + "... - Готово!";

            return launchResult;
        }

        private async Task DownloadCoreProject() 
        {
            this.DownloadingProjectVisibility = true;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            CancellationToken token = cancellationTokenSource.Token;

            var messageProgressTask = MessageProgress(() => DownloadMessage, value => DownloadMessage = value, token);
           
            await this.downloadCoreProjectService.DownloadCoreProject();

            cancellationTokenSource.Cancel();

            try
            {
                await messageProgressTask;
            }
            catch (OperationCanceledException)
            {

            }

            DownloadMessage = DownloadMessage.Trim('.') + "... - Готово!";
            this.DownloadPercent = "100%";
        }

        private async Task UnzipProject()
        {
            logger.Information("Unziping started");

            ExtractProjectVisibility = true;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            CancellationToken token = cancellationTokenSource.Token;

            var unzipAndDeleteTask = Task.Run(() =>
            {
                unzipCoreProject.Unzip();
                unzipCoreProject.RemoveZip();
            });

            var messageProgressTask = MessageProgress(() => UnzipMessage, value => UnzipMessage = value, token);

            await unzipAndDeleteTask;

            logger.Information("Unziping done, remove core zip done");

            cancellationTokenSource.Cancel();

            try
            {
                await messageProgressTask;
            }
            catch (OperationCanceledException)
            {

            }

            UnzipMessage = UnzipMessage.Trim('.') + "... - Готово!";
        }

        private async Task MessageProgress(Func<string> getValue, Action<string> setValue, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var message = getValue() + ".";

                if (message.Count(c => c == '.') > 3)
                {
                    message = message.Trim('.');
                }

                setValue(message);

                await Task.Delay(1000, token);

            }
        }
    }
}

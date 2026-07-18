
using BukiOffline.Commands;
using BukiOffline.Const;
using BukiOffline.Services;
using Microsoft.Win32;
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

        private string unzipMessage = "Extracting project";
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

        private string downloadMessage = "Downloading project";
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

        private string checkingDependenciesMessage = "Checking Dependencies";
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

        public MainViewModel(PrerequisitesChecker prerequisitesChecker,
            UnzipCoreProject unzipCoreProject,
            DownloadCoreProjectService downloadCoreProjectService,
            AppLauncher appLauncher)
        {
            this.prerequisitesChecker = prerequisitesChecker;
            this.unzipCoreProject = unzipCoreProject;
            this.downloadCoreProjectService = downloadCoreProjectService;
            this.appLauncher = appLauncher;
            this.Init();
        }

        public async Task RestartApplicationCommand() 
        {
            this.appLauncher.process?.Kill();

            this.AppCanBeRestarted = false;

            CommandManager.InvalidateRequerySuggested();

            RestartLabels();

            await this.Launch();
        }

        private void RestartLabels() 
        {
            this.DownloadPercent = "0.00";
            this.CheckingDependenciesMessage = "Checking Dependencies";
            this.CheckingDependenciesVisibility = false;
            this.DownloadMessage = "Downloading project";
            this.DownloadingProjectVisibility = false;
            this.UnzipMessage = "Extracting project";
            this.ExtractProjectVisibility = false;
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

            bool launchResult = await PrerequisitesChecker();

            if (launchResult)
            {
                // launch the app

                await appLauncher.LaunchApp();

                return;
            }

            // log the error codes and manually resolve...
        }

        private async Task<bool> PrerequisitesChecker() 
        {
            this.CheckingDependenciesVisibility = true;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            CancellationToken token = cancellationTokenSource.Token;

            var messageProgressTask = MessageProgress(() => CheckingDependenciesMessage, value => CheckingDependenciesMessage = value, token);

            bool launchResult = await prerequisitesChecker.Check(ProcessesStartInfoHolder.DependencyProcessStartInfoList);

            cancellationTokenSource.Cancel();

            CheckingDependenciesMessage = CheckingDependenciesMessage.Trim('.') + "... - Done!";

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

            DownloadMessage = DownloadMessage.Trim('.') + "... - Done!";
            this.DownloadPercent = "100%";
        }

        private async Task UnzipProject()
        {
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

            cancellationTokenSource.Cancel();

            try
            {
                await messageProgressTask;
            }
            catch (OperationCanceledException)
            {

            }

            UnzipMessage = UnzipMessage.Trim('.') + "... - Done!";
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

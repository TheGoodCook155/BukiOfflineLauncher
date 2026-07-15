
using BukiOffline.Const;
using BukiOffline.Services;

namespace BukiOffline.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private PrerequisitesChecker prerequisitesChecker;

        private UnzipCoreProject unzipCoreProject;

        private DownloadCoreProjectService downloadCoreProjectService;

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
            DownloadCoreProjectService downloadCoreProjectService)
        {
            this.prerequisitesChecker = prerequisitesChecker;
            this.unzipCoreProject = unzipCoreProject;
            this.downloadCoreProjectService = downloadCoreProjectService;
            this.Init();
        }

        private void Init() 
        {
            this.downloadCoreProjectService.OnDownloadedContentChanged -= DownloadCoreProjectService_OnDownloadedContentChanged;

            this.downloadCoreProjectService.OnDownloadedContentChanged += DownloadCoreProjectService_OnDownloadedContentChanged;
        }

        private void DownloadCoreProjectService_OnDownloadedContentChanged(object? sender, EventArguments.DownloadedContentEventArgs e)
        {
            this.DownloadPercent = e.DownloadedSize + "%";
        }

        public async Task Launch()
        {

            await DownloadCoreProject();

            await UnzipProject();

            bool launchResult = await PrerequisitesChecker();
            //bool launchResult = prerequisitesChecker.Check(ProcessesStartInfoHolder.ProcessStartInfoList);

            if (launchResult)
            {
                // launch the app
                return;
            }

            // log the error codes and manually resolve...

        }

        private async Task<bool> PrerequisitesChecker() 
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            CancellationToken token = cancellationTokenSource.Token;

            var messageProgressTask = MessageProgress(() => CheckingDependenciesMessage, value => CheckingDependenciesMessage = value, token);

            bool launchResult = await prerequisitesChecker.Check(ProcessesStartInfoHolder.ProcessStartInfoList);

            cancellationTokenSource.Cancel();

            CheckingDependenciesMessage = CheckingDependenciesMessage.Trim('.') + "... - Done!";

            return launchResult;
        }

        private async Task DownloadCoreProject() 
        {
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

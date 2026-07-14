
using BukiOffline.Const;
using BukiOffline.Services;

namespace BukiOffline.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private PrerequisitesChecker prerequisitesChecker;

        private UnzipCoreProject unzipCoreProject;

        private DownloadCoreProjectService downloadCoreProjectService;

        public bool ExtractProjectVisibility { get; set; }

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

            bool launchResult = prerequisitesChecker.Check(ProcessesStartInfoHolder.ProcessStartInfoList);

            if (launchResult)
            {
                // launch the app
                return;
            }

            // log the error codes and manually resolve...

        }

        private async Task DownloadCoreProject() 
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            CancellationToken token = cancellationTokenSource.Token;

            var messageProgressTask = MessageProgress(() => DownloadMessage, value => DownloadMessage = value, token);
           
            //var downloadProjectTask = Task.Run(() =>
            //{
               await this.downloadCoreProjectService.DownloadCoreProject();
            //});


            cancellationTokenSource.Cancel();

            try
            {
                await messageProgressTask;
            }
            catch (OperationCanceledException)
            {

            }

            DownloadMessage = DownloadMessage.Trim('.') + "... - Done!";
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

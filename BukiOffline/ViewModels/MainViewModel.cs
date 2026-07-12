
using BukiOffline.Const;
using BukiOffline.Services;

namespace BukiOffline.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private PrerequisitesChecker prerequisitesChecker;
        string test;

        private UnzipCoreProject unzipCoreProject;
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

        public MainViewModel(PrerequisitesChecker prerequisitesChecker, UnzipCoreProject unzipCoreProject)
        {
            this.prerequisitesChecker = prerequisitesChecker;
            this.unzipCoreProject = unzipCoreProject;
        }

        public async Task Launch()
        {
            await UnzipProject();

            bool launchResult = prerequisitesChecker.Check(ProcessesStartInfoHolder.ProcessStartInfoList);

            if (launchResult)
            {
                // launch the app
                return;
            }

            // log the error codes and manually resolve...

        }

        private async Task UnzipProject()
        {
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


using BukiOffline.Const;

namespace BukiOffline.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private PrerequisitesChecker prerequisitesChecker;

        public MainViewModel(PrerequisitesChecker prerequisitesChecker)
        {
            this.prerequisitesChecker = prerequisitesChecker;
        }

        public void Launch() 
        {
           bool launchResult = this.prerequisitesChecker.Check(ProcessesStartInfoHolder.ProcessStartInfoList);

            if (launchResult) 
            {
                //launch the app
            }

            //log the error codes and manually resolve...

        }
    }
}

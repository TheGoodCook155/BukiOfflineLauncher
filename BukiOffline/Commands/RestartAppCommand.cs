using BukiOffline.ViewModels;
using System.Windows.Input;

namespace BukiOffline.Commands
{
    public class RestartAppCommand : ICommand
    {
        private MainViewModel mainViewModel;

        public RestartAppCommand(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            this.mainViewModel.RestartApplicationCommand();
        }
    }
}

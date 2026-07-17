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

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            return this.mainViewModel.AppCanBeRestarted;
        }

        public void Execute(object? parameter)
        {
            this.mainViewModel.RestartApplicationCommand();
        }
    }
}

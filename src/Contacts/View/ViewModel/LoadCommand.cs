using System;
using System.Windows.Input;

namespace View.ViewModel
{
    public class LoadCommand : ICommand
    {
        private readonly MainVM _vm;

        public LoadCommand(MainVM vm) => _vm = vm;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _vm.Load();
        }
    }
}
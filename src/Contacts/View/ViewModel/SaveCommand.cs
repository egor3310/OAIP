using System;
using System.Windows.Input;

namespace View.ViewModel
{
    public class SaveCommand : ICommand
    {
        private readonly MainVM _vm;

        public SaveCommand(MainVM vm) => _vm = vm;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _vm.Save();
        }
    }
}
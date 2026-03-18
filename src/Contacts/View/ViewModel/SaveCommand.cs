using System;
using System.Windows.Input;

namespace View.ViewModel
{
    /// <summary>
    /// Класс сохраняет файл на компьютере. Привязан к кнопке Save
    /// </summary>
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
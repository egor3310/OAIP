using System;
using System.Windows.Input;
using View.ViewModel;

namespace View.ViewModel
{

    /// <summary>
    /// Команда для удаления выбранного контакта из коллекции.
    /// </summary>
    public class RemoveCommand : ICommand
    {
        private readonly MainVM _vm;

        public RemoveCommand(MainVM vm)
        {
            _vm = vm;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _vm.CanRemove;
        }

        /// <summary>
        /// Выполняет удаление выбранного контакта.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        /// 
        public void Execute(object? parameter)
        {
            _vm.Remove();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
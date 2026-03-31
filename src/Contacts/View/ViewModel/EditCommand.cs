using System;
using System.Windows.Input;
using View.ViewModel;

namespace View.ViewModel
{

    /// <summary>
    /// Команда для перехода в режим редактирования выбранного контакта.
    /// </summary>
    public class EditCommand : ICommand
    {
        private readonly MainVM _vm;

        public EditCommand(MainVM vm)
        {
            _vm = vm;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _vm.CanEdit;
        }

        /// <summary>
        /// Выполняет переход в режим редактирования контакта.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        public void Execute(object? parameter)
        {
            _vm.Edit();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
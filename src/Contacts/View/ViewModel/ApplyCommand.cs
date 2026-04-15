using System;
using System.Windows.Input;
using View.ViewModel;

namespace View.ViewModel
{

    /// <summary>
    /// Команда для применения изменений при создании или редактировании контакта.
    /// </summary>
    public class ApplyCommand : ICommand
    {
        private readonly MainVM _vm;

        public ApplyCommand(MainVM vm)
        {
            _vm = vm;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _vm.IsApplyVisible;
        }

        /// <summary>
        /// Команда для применения изменений при создании или редактировании контакта.
        /// </summary>
        /// 
        public void Execute(object? parameter)
        {
            _vm.Apply();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
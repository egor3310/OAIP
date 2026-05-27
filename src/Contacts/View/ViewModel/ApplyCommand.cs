using System;
using System.Windows.Input;

namespace View.ViewModel
{
    /// <summary>
    /// Команда для применения изменений при создании или редактировании контакта.
    /// </summary>
    public class ApplyCommand : ICommand
    {
        private readonly MainVM _vm;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ApplyCommand"/>.
        /// </summary>
        /// <param name="vm">Главная модель представления.</param>
        public ApplyCommand(MainVM vm)
        {
            _vm = vm;
        }

        /// <summary>
        /// Происходит при изменении условий выполнения команды.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Определяет, может ли команда быть выполнена.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        /// <returns><see langword="true"/>, если команда доступна; иначе <see langword="false"/>.</returns>
        public bool CanExecute(object? parameter)
        {
            return _vm.CanApply;
        }

        /// <summary>
        /// Выполняет применение изменений.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        public void Execute(object? parameter)
        {
            _vm.Apply();
        }

        /// <summary>
        /// Вызывает обновление состояния доступности команды.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
using System;
using System.Windows.Input;
using View.ViewModel;

namespace View.ViewModel
{
    /// <summary>
    /// Команда для перехода в режим создания нового контакта.
    /// </summary>
    public class AddCommand : ICommand
    {

        /// <summary>
        /// Ссылка на главную модель представления.
        /// </summary>
        private readonly MainVM _vm;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AddCommand"/>.
        /// </summary>
        /// <param name="vm">Главная модель представления.</param>
        /// 
        public AddCommand(MainVM vm)
        {
            _vm = vm;
        }

        /// <summary>
        /// Происходит при изменении условий выполнения команды.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Определяет, может ли команда быть выполнена в текущем состоянии.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        /// <returns><see langword="true"/>, если команду можно выполнить; иначе <see langword="false"/>.</returns>
        public bool CanExecute(object? parameter)
        {
            return _vm.CanAdd;
        }

        /// <summary>
        /// Определяет, может ли команда быть выполнена в текущем состоянии.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        /// <returns><see langword="true"/>, если команду можно выполнить; иначе <see langword="false"/>.</returns>
        public void Execute(object? parameter)
        {
            _vm.Add();
        }

        /// <summary>
        /// Выполняет переход в режим добавления нового контакта.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
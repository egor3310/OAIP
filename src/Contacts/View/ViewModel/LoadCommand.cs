using System;
using System.Windows.Input;

namespace View.ViewModel
{
    /// <summary>
    /// Класс LoadCommand отвечает за кнопку Load. Вытаскивание данных с файла Json на интерфейс. Привязан к кнопке Load
    /// </summary>
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
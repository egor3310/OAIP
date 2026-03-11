using System;

/// <summary>
/// Summary description for Class1
/// </summary>
internal class SaveCommand : ICommand
{
    private readonly MainVM _vm;

    public SaveCommand(MainVM vm)
    {
        _vm = vm;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        _vm.Save();
    }
}
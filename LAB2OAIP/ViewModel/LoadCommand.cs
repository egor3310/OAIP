using System;

/// <summary>
/// Summary description for Class1
/// </summary>
public class LoadCommand : ICommand
{
	public LoadCommand()
	{

         private readonly MainVM _vm;

    public LoadCommand(MainVM vm)
    {
        _vm = vm;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        _vm.Load();
    }
}
}

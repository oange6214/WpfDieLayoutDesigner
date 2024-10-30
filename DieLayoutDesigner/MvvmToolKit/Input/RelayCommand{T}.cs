using System.Windows.Input;

namespace DieLayoutDesigner.MvvmToolKit.Input;

public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Func<bool> _canExecute;

    public event EventHandler CanExecuteChanged;

    public RelayCommand(Action<T> execute, Func<bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

    public void Execute(object? parameter)
    {
        _execute((T)parameter);
    }
}
using System;
using System.Windows.Input;

namespace TeamTracker.Wpf.Commands;

public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Predicate<T>? _canExecute;
    
    public bool CanExecute(object? parameter)
    {
        return _canExecute is null || _canExecute((T)parameter!);
    }

    public void Execute(object? parameter)
    {
        _execute((T)parameter!);
    }

    public event EventHandler? CanExecuteChanged {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public RelayCommand(Action<T> execute, Predicate<T>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
}
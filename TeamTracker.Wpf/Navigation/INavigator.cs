using System.Windows.Input;

namespace TeamTracker.Wpf.Navigation;

public interface INavigator
{
    public ICommand UpdateCurrentViewTypeCommand { get; }
    public event EventHandler CurrentViewTypeChanged;
    public void UpdateCurrentViewType(ViewType newViewType, object? viewParameter);
}
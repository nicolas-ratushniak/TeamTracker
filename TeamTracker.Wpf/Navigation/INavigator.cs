using System.Windows.Input;

namespace TeamTracker.Wpf.Navigation;

public interface INavigator
{
    public event EventHandler<ViewTypeChangedEventArgs> CurrentViewTypeChanged;
    public ICommand UpdateCurrentViewTypeCommand { get; }
    public ViewType? CurrentViewType { get; }
    public void UpdateCurrentViewType(ViewType newViewType, object? viewParameter);
}
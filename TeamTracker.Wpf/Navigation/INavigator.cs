using System.Windows.Input;

namespace TeamTracker.Wpf.Navigation;

public interface INavigator
{
    public ViewType? CurrentViewType { get; }
    public ICommand UpdateCurrentViewTypeCommand { get; }
    public event EventHandler CurrentViewTypeChanged;
    public void SetCurrentViewType(ViewType newViewType);
}
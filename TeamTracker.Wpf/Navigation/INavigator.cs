using System.Windows.Input;

namespace TeamTracker.Wpf.Navigation;

public interface INavigator
{
    public ViewType? CurrentViewType { get; }
    public ICommand UpdateCurrentViewTypeCommand { get; }
    public event EventHandler CurrentViewTypeChanged;
    public void UpdateCurrentViewType(ViewType newViewType);
    public void UpdateCurrentViewType(ViewType newViewType, object viewParameter);
}
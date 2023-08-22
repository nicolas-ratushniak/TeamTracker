using System.Windows.Input;
using TeamTracker.Wpf.Commands;

namespace TeamTracker.Wpf.Navigation;

public class Navigator : INavigator
{
    public ViewType? CurrentViewType { get; private set; }
    public ICommand UpdateCurrentViewTypeCommand { get; }

    public event EventHandler? CurrentViewTypeChanged;

    public Navigator()
    {
        UpdateCurrentViewTypeCommand = new RelayCommand<ViewType>(SetCurrentViewType, (obj) => true);
    }

    public void SetCurrentViewType(ViewType newViewType)
    {
        var oldViewType = CurrentViewType;

        if (oldViewType != newViewType)
        {
            CurrentViewTypeChanged?.Invoke(this, new ViewTypeChangedEventArgs(oldViewType, newViewType));
            CurrentViewType = newViewType;
        }
    }
}
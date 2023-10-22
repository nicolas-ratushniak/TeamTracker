using System;
using System.Windows.Input;

namespace TeamTracker.Wpf.Navigation;

public interface INavigationService
{
    public event EventHandler<ViewTypeChangedEventArgs> CurrentViewTypeChanged;
    public ViewType? CurrentViewType { get; }
    public void NavigateTo(ViewType newViewType, object? viewParameter);
}
using TeamTracker.Wpf.ViewModels.Factories;

namespace TeamTracker.Wpf.Navigation;

public class ViewTypeChangedEventArgs : EventArgs
{
    public ViewType? OldViewType { get; set; }
    public ViewType NewViewType { get; set; }

    public ViewTypeChangedEventArgs(ViewType? oldViewType, ViewType newViewType)
    {
        OldViewType = oldViewType;
        NewViewType = newViewType;
    }
}
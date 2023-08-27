namespace TeamTracker.Wpf.Navigation;

public class ViewTypeChangedEventArgs : EventArgs
{
    public ViewType? OldViewType { get; set; }
    public ViewType NewViewType { get; set; }
    public object? ViewParameter { get; set; }

    public ViewTypeChangedEventArgs(ViewType? oldViewType, ViewType newViewType, object? viewParameter = null)
    {
        OldViewType = oldViewType;
        NewViewType = newViewType;
        ViewParameter = viewParameter;
    }
}
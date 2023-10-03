using System;

namespace TeamTracker.Wpf.Navigation;

public class ViewTypeChangedEventArgs : EventArgs
{
    public ViewType? OldViewType { get; set; }
    public ViewType NewViewType { get; set; }
    public object? ViewParameter { get; set; }

    public ViewTypeChangedEventArgs(ViewType? oldViewType, ViewType newViewType) : this(oldViewType, newViewType, null)
    {
    }

    public ViewTypeChangedEventArgs(ViewType? oldViewType, ViewType newViewType, object? viewParameter)
    {
        OldViewType = oldViewType;
        NewViewType = newViewType;
        ViewParameter = viewParameter;
    }
}
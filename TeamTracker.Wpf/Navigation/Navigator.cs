using System.Windows.Input;
using Microsoft.Extensions.Logging;
using TeamTracker.Wpf.Commands;

namespace TeamTracker.Wpf.Navigation;

public class Navigator : INavigator
{
    private readonly ILogger<Navigator> _logger;
    public ViewType? CurrentViewType { get; private set; }
    public ICommand UpdateCurrentViewTypeCommand { get; }

    public event EventHandler? CurrentViewTypeChanged;

    public Navigator(ILogger<Navigator> logger)
    {
        _logger = logger;
        UpdateCurrentViewTypeCommand = new RelayCommand<ViewType>(SetCurrentViewType, (obj) => true);
    }

    public void SetCurrentViewType(ViewType newViewType)
    {
        var oldViewType = CurrentViewType;

        if (oldViewType != newViewType)
        {
            _logger.LogInformation("Setting view type to {NewViewType}", newViewType);
            
            CurrentViewTypeChanged?.Invoke(this, new ViewTypeChangedEventArgs(oldViewType, newViewType));
            CurrentViewType = newViewType;
        }
    }
}
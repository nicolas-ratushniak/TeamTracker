using System;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using TeamTracker.Wpf.Commands;

namespace TeamTracker.Wpf.Navigation;

public class NavigationService : INavigationService
{
    public event EventHandler<ViewTypeChangedEventArgs> CurrentViewTypeChanged;
    
    private readonly ILogger<NavigationService> _logger;
    public ViewType? CurrentViewType { get; private set; }
    public ICommand UpdateCurrentViewTypeCommand { get; }

    public NavigationService(ILogger<NavigationService> logger)
    {
        _logger = logger;
        UpdateCurrentViewTypeCommand = new RelayCommand<ViewType>(viewType => NavigateTo(viewType, null));
    }

    public void NavigateTo(ViewType newViewType, object? viewParameter)
    {
        var oldViewType = CurrentViewType;

        if (oldViewType == newViewType)
        {
            return;
        }

        _logger.LogInformation("Setting view type to {NewViewType}", newViewType);
        
        CurrentViewType = newViewType;
        CurrentViewTypeChanged?.Invoke(this, new ViewTypeChangedEventArgs(oldViewType, newViewType, viewParameter));
    }
}
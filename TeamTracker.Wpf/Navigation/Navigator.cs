using System;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using TeamTracker.Wpf.Commands;

namespace TeamTracker.Wpf.Navigation;

public class Navigator : INavigator
{
    public event EventHandler<ViewTypeChangedEventArgs> CurrentViewTypeChanged;
    
    private readonly ILogger<Navigator> _logger;
    public ViewType? CurrentViewType { get; private set; }
    public ICommand UpdateCurrentViewTypeCommand { get; }

    public Navigator(ILogger<Navigator> logger)
    {
        _logger = logger;
        UpdateCurrentViewTypeCommand = new RelayCommand<ViewType>(viewType => UpdateCurrentViewType(viewType, null));
    }

    public void UpdateCurrentViewType(ViewType newViewType, object? viewParameter)
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
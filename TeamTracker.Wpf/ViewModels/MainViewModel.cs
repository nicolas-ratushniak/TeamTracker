using Microsoft.Extensions.Logging;
using TeamTracker.Wpf.Navigation;
using TeamTracker.Wpf.ViewModels.Factories;

namespace TeamTracker.Wpf.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly ILogger<MainViewModel> _logger;
    private ViewModelBase _currentViewModel;

    public INavigator Navigator { get; }

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            if (Equals(value, _currentViewModel)) return;
            
            _currentViewModel?.Dispose();
            _currentViewModel = value;
            OnPropertyChanged();
        }
    }

    public MainViewModel(INavigator navigator, IViewModelFactory viewModelFactory, ILogger<MainViewModel> logger)
    {
        _viewModelFactory = viewModelFactory;
        _logger = logger;
        Navigator = navigator;
        
        Navigator.CurrentViewTypeChanged += Navigator_OnCurrentViewTypeChanged;
        
        _logger.LogInformation("Setting default view");
        CurrentViewModel = _viewModelFactory.CreateViewModel(ViewType.Teams);
    }

    private void Navigator_OnCurrentViewTypeChanged(object? sender, EventArgs args)
    {
        var viewArgs = (ViewTypeChangedEventArgs)args;
        CurrentViewModel = _viewModelFactory.CreateViewModel(viewArgs.NewViewType, viewArgs.ViewParameter);
        
        _logger.LogInformation("Current view was successfully updated");
    }
}
using System;
using Microsoft.Extensions.Logging;
using TeamTracker.Wpf.Navigation;
using TeamTracker.Wpf.ViewModels.Factories;

namespace TeamTracker.Wpf.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly ILogger<MainViewModel> _logger;
    private BaseViewModel _currentViewModel;
    private ViewType _currentNavBarOption;

    public INavigationService NavigationService { get; }

    public BaseViewModel CurrentViewModel
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

    public ViewType CurrentNavBarOption
    {
        get => _currentNavBarOption;
        set
        {
            if (value == _currentNavBarOption) return;
            _currentNavBarOption = value;
            OnPropertyChanged();
        }
    }

    public MainViewModel(INavigationService navigationService, IViewModelFactory viewModelFactory, ILogger<MainViewModel> logger)
    {
        _viewModelFactory = viewModelFactory;
        _logger = logger;
        NavigationService = navigationService;

        NavigationService.CurrentViewTypeChanged += Navigator_OnCurrentViewTypeChanged;

        _logger.LogInformation("Setting default view");
        CurrentViewModel = _viewModelFactory.CreateViewModel(ViewType.Teams);
    }

    private void Navigator_OnCurrentViewTypeChanged(object? sender, EventArgs args)
    {
        var viewArgs = (ViewTypeChangedEventArgs)args;
        var newViewType = viewArgs.NewViewType;

        CurrentViewModel = _viewModelFactory.CreateViewModel(newViewType, viewArgs.ViewParameter);

        CurrentNavBarOption = newViewType switch
        {
            ViewType.Teams or ViewType.TeamCreate or ViewType.TeamUpdate => ViewType.Teams,
            ViewType.Games or ViewType.GameCreate => ViewType.Games,
            _ => ViewType.Help
        };

        _logger.LogInformation("Current view was successfully updated");
    }
}
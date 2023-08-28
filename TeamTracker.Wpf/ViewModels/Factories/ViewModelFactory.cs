using Microsoft.Extensions.Logging;
using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Navigation;

namespace TeamTracker.Wpf.ViewModels.Factories;

public class ViewModelFactory : IViewModelFactory
{
    private readonly ITeamService _teamService;
    private readonly IGameInfoService _gameInfoService;
    private readonly INavigator _navigator;
    private readonly ILogger<TeamCreateFormViewModel> _logger;

    public ViewModelFactory(ITeamService teamService, IGameInfoService gameInfoService, INavigator navigator,
        ILogger<TeamCreateFormViewModel> logger)
    {
        _teamService = teamService;
        _gameInfoService = gameInfoService;
        _navigator = navigator;
        _logger = logger;
    }

    public ViewModelBase CreateViewModel(ViewType viewType, object? viewParameter)
    {
        return viewType switch
        {
            ViewType.Teams => new TeamsViewModel(_teamService, _navigator),
            ViewType.TeamCreate => new TeamCreateFormViewModel(_teamService, _navigator, _logger),
            ViewType.TeamUpdate => new TeamUpdateFormViewModel((Guid)viewParameter!, _teamService, _navigator),
            ViewType.Games => new GamesViewModel(_gameInfoService),
            ViewType.Help => new HelpViewModel(),
            _ => throw new InvalidOperationException("Cannot create view model with this type.")
        };
    }
}
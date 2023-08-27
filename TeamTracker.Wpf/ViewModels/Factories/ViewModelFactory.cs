using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Navigation;

namespace TeamTracker.Wpf.ViewModels.Factories;

public class ViewModelFactory : IViewModelFactory
{
    private readonly ITeamService _teamService;
    private readonly IGameInfoService _gameInfoService;
    private readonly INavigator _navigator;

    public ViewModelFactory(ITeamService teamService, IGameInfoService gameInfoService, INavigator navigator)
    {
        _teamService = teamService;
        _gameInfoService = gameInfoService;
        _navigator = navigator;
    }

    public ViewModelBase CreateViewModel(ViewType viewType)
    {
        return viewType switch
        {
            ViewType.Teams => new TeamsViewModel(_teamService, _navigator),
            ViewType.TeamCreate => new AddTeamFormViewModel(_teamService, _navigator),
            ViewType.Games => new GamesViewModel(_gameInfoService),
            ViewType.Help => new HelpViewModel(),
            _ => throw new InvalidOperationException("Cannot create nonexistent view model")
        };
    }
}
using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Navigation;

namespace TeamTracker.Wpf.ViewModels.Factories;

public class ViewModelFactory : IViewModelFactory
{
    private readonly ITeamService _teamService;
    private readonly IGameInfoService _gameInfoService;

    public ViewModelFactory(ITeamService teamService, IGameInfoService gameInfoService)
    {
        _teamService = teamService;
        _gameInfoService = gameInfoService;
    }

    public ViewModelBase CreateViewModel(ViewType viewType)
    {
        return viewType switch
        {
            ViewType.Teams => new TeamsViewModel(_teamService),
            ViewType.Games => new GamesViewModel(_gameInfoService),
            ViewType.Help => new HelpViewModel(),
            _ => throw new InvalidOperationException("Cannot create nonexistent view model")
        };
    }
}
using TeamTracker.Wpf.Navigation;

namespace TeamTracker.Wpf.ViewModels.Factories;

public class ViewModelFactory : IViewModelFactory
{
    private readonly Func<TeamsViewModel> _createTeamsViewModel;
    private readonly Func<TeamCreateFormViewModel> _createTeamCreateViewModel;
    private readonly Func<Guid, TeamUpdateFormViewModel> _createTeamUpdateViewModel;
    private readonly Func<GamesViewModel> _createGamesViewModel;
    private readonly Func<GameCreateFormViewModel> _createGameCreateFormViewModel;
    private readonly Func<HelpViewModel> _createHelpViewModel;

    public ViewModelFactory(
        Func<TeamsViewModel> createTeamsViewModel,
        Func<TeamCreateFormViewModel> createTeamCreateViewModel,
        Func<Guid, TeamUpdateFormViewModel> createTeamUpdateViewModel,
        Func<GamesViewModel> createGamesViewModel,
        Func<GameCreateFormViewModel> createGameCreateFormViewModel,
        Func<HelpViewModel> createHelpViewModel
    )
    {
        _createTeamsViewModel = createTeamsViewModel;
        _createTeamCreateViewModel = createTeamCreateViewModel;
        _createTeamUpdateViewModel = createTeamUpdateViewModel;
        _createGamesViewModel = createGamesViewModel;
        _createGameCreateFormViewModel = createGameCreateFormViewModel;
        _createHelpViewModel = createHelpViewModel;
    }

    public ViewModelBase CreateViewModel(ViewType viewType, object? viewParameter = null)
    {
        return viewType switch
        {
            ViewType.Teams => _createTeamsViewModel(),
            ViewType.TeamCreate => _createTeamCreateViewModel(),
            ViewType.TeamUpdate => _createTeamUpdateViewModel((Guid)viewParameter!),
            ViewType.Games => _createGamesViewModel(),
            ViewType.GameCreate => _createGameCreateFormViewModel(),
            ViewType.Help => _createHelpViewModel(),
            _ => throw new InvalidOperationException("Cannot create view model with this type.")
        };
    }
}
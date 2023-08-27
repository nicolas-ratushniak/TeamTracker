using System.ComponentModel;
using System.Windows.Input;
using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Commands;
using TeamTracker.Wpf.Navigation;

namespace TeamTracker.Wpf.ViewModels;

public class TeamsViewModel : ViewModelBase
{
    private readonly ITeamService _teamService;
    private readonly INavigator _navigator;
    public TeamsListViewModel TeamsList { get; set; }
    public TeamDetailsViewModel SelectedTeamDetails { get; set; }

    public ICommand AddTeamCommand { get; }
    public ICommand EditTeamCommand { get; }

    public TeamsViewModel(ITeamService teamService, INavigator navigator)
    {
        _teamService = teamService;
        _navigator = navigator;
        TeamsList = new TeamsListViewModel(teamService);
        SelectedTeamDetails = new TeamDetailsViewModel();
        
        TeamsList.PropertyChanged += TeamsList_OnPropertyChanged;

        AddTeamCommand = new RelayCommand<object>(o => _navigator.UpdateCurrentViewType(ViewType.TeamCreate));
        EditTeamCommand = new RelayCommand<object>(o => _navigator.UpdateCurrentViewType(ViewType.TeamUpdate, SelectedTeamDetails.Team!.Id),
            o => SelectedTeamDetails.Team is not null);
    }

    private void TeamsList_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var selectedTeam = ((TeamsListViewModel)sender!).SelectedTeam;

        if (e.PropertyName != nameof(TeamsList.SelectedTeam))
        {
            return;
        }

        if (selectedTeam is null)
        {
            SelectedTeamDetails.Team = null;
        }
        else
        {
            var team = _teamService.Get(selectedTeam.Id);

            SelectedTeamDetails.Team = new TeamDetailsItemViewModel
            {
                Id = team.Id,
                Name = team.Name,
                OriginCity = team.OriginCity,
                GamesWon = team.GamesWon,
                GamesLost = team.GamesLost,
                GamesDrawn = team.GamesDrawn,
                MembersCount = team.MembersCount,
                TotalGames = _teamService.GetTotalGames(team),
                Points = _teamService.CalculatePoints(team)
            };
        }
    }
}
using System.ComponentModel;
using TeamTracker.Domain.Models;
using TeamTracker.Domain.Services;

namespace TeamTracker.Wpf.ViewModels;

public class TeamsViewModel : ViewModelBase
{
    private readonly ITeamService _teamService;
    public TeamsListViewModel TeamsList { get; set; }
    public TeamDetailsViewModel SelectedTeamDetails { get; set; }

    public TeamsViewModel(ITeamService teamService)
    {
        _teamService = teamService;
        TeamsList = new TeamsListViewModel(teamService);
        SelectedTeamDetails = new TeamDetailsViewModel();
        
        TeamsList.PropertyChanged += TeamsList_OnPropertyChanged;
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
            // var team = _teamService.Get(selectedTeam.Id);

            var team = new Team
            {
                Id = default,
                Name = "Super Team",
                OriginCity = "Chernivtsi",
                GamesWon = 3,
                GamesLost = 8,
                GamesDrawn = 0,
                MembersCount = 12
            };

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
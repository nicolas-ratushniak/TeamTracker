using TeamTracker.Domain.Models;
using TeamTracker.Domain.Services;

namespace TeamTracker.Wpf.ViewModels;

public class TeamsListViewModel : ViewModelBase
{
    private TeamListItemViewModel? _selectedTeam;
    
    public List<TeamListItemViewModel> Teams { get; set; }
    public TeamListItemViewModel? SelectedTeam
    {
        get => _selectedTeam;
        set
        {
            if (Equals(value, _selectedTeam)) return;
            _selectedTeam = value;
            OnPropertyChanged();
        }
    }

    public TeamsListViewModel(ITeamService teamService)
    {
        // var teamListItems = teamService.GetAll()
        //     .Select(t => new TeamListItemViewModel
        //     {
        //         Id = t.Id,
        //         FullName = $"{t.Name}-{t.OriginCity}"
        //     });
        
        var teams = new List<Team>()
        {
            new()
            {
                Id = Guid.Empty,
                Name = "Team 1",
                OriginCity = "City 1",
                GamesWon = 4,
                GamesLost = 1,
                GamesDrawn = 2,
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 2",
                OriginCity = "City 2",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 3",
                OriginCity = "City 3",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 4",
                OriginCity = "City 4",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 5",
                OriginCity = "City 5",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 1",
                OriginCity = "City 1",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 2",
                OriginCity = "City 2",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 3",
                OriginCity = "City 3",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 4",
                OriginCity = "City 4",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 5",
                OriginCity = "City 5",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 1",
                OriginCity = "City 1",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 2",
                OriginCity = "City 2",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 3",
                OriginCity = "City 3",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 4",
                OriginCity = "City 4",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 5",
                OriginCity = "City 5",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 1",
                OriginCity = "City 1",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 2",
                OriginCity = "City 2",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 3",
                OriginCity = "City 3",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 4",
                OriginCity = "City 4",
                MembersCount = 10
            },
            new()
            {
                Id = Guid.Empty,
                Name = "Team 5",
                OriginCity = "City 5",
                MembersCount = 10
            }
        };
        
        var teamListItems = teams
        .Select(t => new TeamListItemViewModel
        {
            Id = t.Id,
            FullName = $"{t.Name}-{t.OriginCity}"
        });
        
        Teams = teamListItems.ToList();
    }
}
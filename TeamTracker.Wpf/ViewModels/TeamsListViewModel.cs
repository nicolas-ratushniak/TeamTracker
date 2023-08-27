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
        var teamListItems = teamService.GetAll()
            .Select(t => new TeamListItemViewModel
            {
                Id = t.Id,
                FullName = $"{t.Name}-{t.OriginCity}"
            });

        Teams = teamListItems.ToList();
    }
}
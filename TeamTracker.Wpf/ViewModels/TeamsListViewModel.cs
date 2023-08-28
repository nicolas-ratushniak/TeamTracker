using TeamTracker.Domain.Services;

namespace TeamTracker.Wpf.ViewModels;

public class TeamsListViewModel : ViewModelBase
{
    private readonly ITeamService _teamService;
    private TeamListItemViewModel? _selectedTeam;
    private List<TeamListItemViewModel> _teams;

    public List<TeamListItemViewModel> Teams
    {
        get => _teams;
        set
        {
            if (Equals(value, _teams)) return;
            _teams = value;
            OnPropertyChanged();
        }
    }

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
        _teamService = teamService;
        Refresh();
    }

    public void Refresh()
    {
        Teams = _teamService.GetAll()
            .Select(t => new TeamListItemViewModel
            {
                Id = t.Id,
                FullName = $"{t.Name}-{t.OriginCity}"
            })
            .ToList();
    }
}
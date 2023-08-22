using TeamTracker.Domain.Services;

namespace TeamTracker.Wpf.ViewModels;

public class TeamsViewModel : ViewModelBase
{
    private List<TeamHeaderViewModel> _teams;
    private TeamDetailsViewModel? _selectedTeam;

    public List<TeamHeaderViewModel> Teams
    {
        get => _teams;
        set
        {
            if (Equals(value, _teams)) return;
            _teams = value;
            OnPropertyChanged();
        }
    }

    public TeamDetailsViewModel? SelectedTeam
    {
        get => _selectedTeam;
        set
        {
            if (Equals(value, _selectedTeam)) return;
            _selectedTeam = value;
            OnPropertyChanged();
        }
    }

    public TeamsViewModel(ITeamService teamService)
    {
        _teams = teamService.GetAll()
            .Select(t => new TeamHeaderViewModel(t))
            .ToList();
    }
}
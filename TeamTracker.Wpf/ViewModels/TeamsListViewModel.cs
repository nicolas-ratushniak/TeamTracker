using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using TeamTracker.Domain.Services;

namespace TeamTracker.Wpf.ViewModels;

public class TeamsListViewModel : ViewModelBase
{
    private readonly ITeamService _teamService;
    private readonly ObservableCollection<TeamListItemViewModel> _teams;
    private TeamListItemViewModel? _selectedTeam;
    private string _teamsFilter;

    public ICollectionView TeamsCollectionView { get; private set; }

    public string TeamsFilter
    {
        get => _teamsFilter;
        set
        {
            if (value == _teamsFilter) return;
            _teamsFilter = value ?? string.Empty;
            
            OnPropertyChanged();
            TeamsCollectionView.Refresh();
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
        _teamsFilter = string.Empty;
        _teams = new ObservableCollection<TeamListItemViewModel>(GetTeams());

        TeamsCollectionView = CollectionViewSource.GetDefaultView(_teams);
        TeamsCollectionView.Filter = FilterTeams;
    }

    private bool FilterTeams(object obj)
    {
        if (obj is TeamListItemViewModel team)
        {
            return team.FullName.ToLower().Contains(TeamsFilter.ToLower());
        }

        return false;
    }

    public void Refresh()
    {
        _teams.Clear();

        foreach (var team in GetTeams())
        {
            _teams.Add(team);
        }
    }

    private List<TeamListItemViewModel> GetTeams()
    {
        return _teamService.GetAll()
            .Select(t => new TeamListItemViewModel
            {
                Id = t.Id,
                FullName = $"{t.Name}-{t.OriginCity}"
            })
            .ToList();
    }
}
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
    private string _teamsSearchFilter;
    private string _sortStrategyName;
    private int _minMembers;
    private int _maxMembers;

    public string[] SortOptions { get; }
    public ICollectionView TeamsCollectionView { get; }

    public int MinMembers
    {
        get => _minMembers;
        set
        {
            if (value == _minMembers) return;
            _minMembers = value;
            
            OnPropertyChanged();
            UpdateFilters();
        }
    }

    public int MaxMembers
    {
        get => _maxMembers;
        set
        {
            if (value == _maxMembers) return;
            _maxMembers = value;
            
            OnPropertyChanged();
            UpdateFilters();
        }
    }

    public string SortStrategyName
    {
        get => _sortStrategyName;
        set
        {
            if (value == _sortStrategyName) return;
            _sortStrategyName = value;
            
            OnPropertyChanged();
            UpdateSortStrategy(GetSortStrategy(value));
        }
    }

    public string TeamsSearchFilter
    {
        get => _teamsSearchFilter;
        set
        {
            if (value == _teamsSearchFilter) return;
            _teamsSearchFilter = value ?? string.Empty;

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
        SortOptions = new[]
        {
            "Team Name Asc",
            "Team name Desc",
            "Points Asc",
            "Points Desc",
            "Members Asc",
            "Members Desc"
        };

        _teamService = teamService;
        _teamsSearchFilter = string.Empty;
        _teams = new ObservableCollection<TeamListItemViewModel>(GetTeams());

        TeamsCollectionView = CollectionViewSource.GetDefaultView(_teams);
        UpdateFilters();
        UpdateSortStrategy(new SortDescription("FullName", ListSortDirection.Ascending));
    }

    private SortDescription GetSortStrategy(string sortStrategyName)
    {
        return sortStrategyName switch
        {
            "Team Name Asc" => new SortDescription(nameof(SelectedTeam.FullName), ListSortDirection.Ascending),
            "Team name Desc" => new SortDescription(nameof(SelectedTeam.FullName), ListSortDirection.Descending),
            "Points Asc" => new SortDescription(nameof(SelectedTeam.Points), ListSortDirection.Ascending),
            "Points Desc" => new SortDescription(nameof(SelectedTeam.Points), ListSortDirection.Descending),
            "Members Asc" => new SortDescription(nameof(SelectedTeam.Members), ListSortDirection.Ascending),
            "Members Desc" => new SortDescription(nameof(SelectedTeam.Members), ListSortDirection.Descending),
            _ => new SortDescription()
        };
    }

    private void UpdateSortStrategy(SortDescription sortDescription)
    {
        TeamsCollectionView.SortDescriptions.Clear();
        TeamsCollectionView.SortDescriptions.Add(sortDescription);
    }

    private void UpdateFilters()
    {
        // if Max is not specified it is set to infinity 
        var actualMaxMembers = MaxMembers == 0 ? int.MaxValue : MaxMembers;
        
        if (MinMembers >= actualMaxMembers)
        {
            TeamsCollectionView.Filter = o =>
                o is TeamListItemViewModel t && FilterTeamsBySearch(t);
        }
        else
        {
            TeamsCollectionView.Filter = o =>
                o is TeamListItemViewModel t && FilterTeamsBySearch(t) && FilterTeamsByMembers(t);
        }

        bool FilterTeamsBySearch(TeamListItemViewModel team) => team.FullName.ToLower().Contains(TeamsSearchFilter.ToLower());
        
        bool FilterTeamsByMembers(TeamListItemViewModel team) => team.Members >= MinMembers && team.Members <= actualMaxMembers;
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
                FullName = $"{t.Name}-{t.OriginCity}",
                Points = _teamService.CalculatePoints(t),
                Members = t.MembersCount
            })
            .ToList();
    }
}
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Commands;

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
    private int _minPoints;
    private int _maxPoints;

    public ICommand ShowMostWinsCommand { get; }
    public ICommand ShowMostPointsCommand { get; }
    public ICommand ShowNewcomersCommand { get; }

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

    public int MinPoints
    {
        get => _minPoints;
        set
        {
            if (value == _minPoints) return;
            _minPoints = value;
            
            OnPropertyChanged();
            UpdateFilters();
        }
    }

    public int MaxPoints
    {
        get => _maxPoints;
        set
        {
            if (value == _maxPoints) return;
            _maxPoints = value;
            
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

        ShowMostWinsCommand = new RelayCommand<object>(ShowMostWins_Execute);
        ShowMostPointsCommand = new RelayCommand<object>(ShowMostPoints_Execute);
        ShowNewcomersCommand = new RelayCommand<object>(ShowNewcomers_Execute);
    }

    private void ShowNewcomers_Execute(object obj)
    {
        TeamsCollectionView.Filter = o =>
            o is TeamListItemViewModel t && FilterTeamsBySearch(t) && t.TotalGames == 0;
        ResetCommonFilters();
    }

    private void ShowMostPoints_Execute(object obj)
    {
        TeamsCollectionView.Filter = o =>
            o is TeamListItemViewModel t && FilterTeamsBySearch(t) && t.Points == _teams.Max(team => team.Points);
        ResetCommonFilters();
    }

    private void ShowMostWins_Execute(object obj)
    {
        TeamsCollectionView.Filter = o =>
            o is TeamListItemViewModel t && FilterTeamsBySearch(t) && t.Wins == _teams.Max(team => team.Wins);
        ResetCommonFilters();
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
        var actualMaxPoints = MaxPoints == 0 ? int.MaxValue : MaxPoints;

        Predicate<TeamListItemViewModel> filterTeamsBySearch = FilterTeamsBySearch;
        Predicate<TeamListItemViewModel> filterTeamsByMembers = FilterTeamsByMembers;
        Predicate<TeamListItemViewModel> filterTeamsByPoints = FilterTeamsByPoints;

        if (MinMembers > actualMaxMembers)
        {
            filterTeamsByMembers = _ => false;
        }

        if (MinPoints > actualMaxPoints)
        {
            filterTeamsByPoints = _ => false;
        }
        
        TeamsCollectionView.Filter = o =>
            o is TeamListItemViewModel t && filterTeamsBySearch(t) && filterTeamsByMembers(t) && filterTeamsByPoints(t);

        bool FilterTeamsByMembers(TeamListItemViewModel team) =>
            team.Members >= MinMembers && team.Members <= actualMaxMembers;
        
        bool FilterTeamsByPoints(TeamListItemViewModel team) =>
            team.Points >= MinPoints && team.Points <= actualMaxPoints;
    }

    private bool FilterTeamsBySearch(TeamListItemViewModel team) =>
        team.FullName.ToLower().Contains(TeamsSearchFilter.ToLower());

    private void ResetCommonFilters()
    {
        SortStrategyName = SortOptions[0];
        MinMembers = 0;
        MaxMembers = 0;
        MinPoints = 0;
        MaxPoints = 0;
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
                Name = t.Name,
                OriginCity = t.OriginCity,
                Points = _teamService.CalculatePoints(t),
                Members = t.MembersCount,
                TotalGames = _teamService.GetTotalGames(t),
                Wins = t.GamesWon
            })
            .ToList();
    }
}
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Commands;

namespace TeamTracker.Wpf.ViewModels;

public class TeamListViewModel : ViewModelBase
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
    private bool _isAdvancedFilterActive;

    private List<Predicate<TeamListItemViewModel>> _filters = new();

    public ICommand ShowMostWinsCommand { get; }
    public ICommand ShowMostPointsCommand { get; }
    public ICommand ShowNewcomersCommand { get; }
    public ICommand ResetAdvancedFiltersCommand { get; }

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
            TeamsCollectionView.Refresh();
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
            TeamsCollectionView.Refresh();
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
            TeamsCollectionView.Refresh();
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
            TeamsCollectionView.Refresh();
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

    public string SortStrategyName
    {
        get => _sortStrategyName;
        set
        {
            if (value == _sortStrategyName) return;
            _sortStrategyName = value;

            OnPropertyChanged();
            OnSortStrategyNameChanged();
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

    public TeamListViewModel(ITeamService teamService)
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
        AddDefaultFilters();
        ApplyFilters();
        TeamsCollectionView.SortDescriptions.Add(new SortDescription("FullName", ListSortDirection.Ascending));

        ShowMostWinsCommand = new RelayCommand<object>(ShowMostWins_Execute);
        ShowMostPointsCommand = new RelayCommand<object>(ShowMostPoints_Execute);
        ShowNewcomersCommand = new RelayCommand<object>(ShowNewcomers_Execute);
        ResetAdvancedFiltersCommand =
            new RelayCommand<object>(ResetAdvancedFilters_Execute, _ => _isAdvancedFilterActive);
    }

    private void ResetAdvancedFilters_Execute(object obj)
    {
        AddDefaultFilters();
        ApplyFilters();

        _isAdvancedFilterActive = false;
    }

    private void ShowNewcomers_Execute(object obj)
    {
        AddDefaultFilters();
        _filters.Add(t => t.TotalGames == 0);
        ApplyFilters();

        _isAdvancedFilterActive = true;
    }

    private void ShowMostPoints_Execute(object obj)
    {
        AddDefaultFilters();
        _filters.Add(t => t.Points == _teams.Max(team => team.Points));
        ApplyFilters();

        _isAdvancedFilterActive = true;
    }

    private void ShowMostWins_Execute(object obj)
    {
        AddDefaultFilters();
        _filters.Add(t => t.Wins == _teams.Max(team => team.Wins));
        ApplyFilters();

        _isAdvancedFilterActive = true;
    }

    private void AddDefaultFilters()
    {
        _filters.Clear();
        _filters.Add(FilterTeamsBySearch);
        _filters.Add(FilterTeamsByMembers);
        _filters.Add(FilterTeamsByPoints);
    }

    private void ApplyFilters()
    {
        TeamsCollectionView.Filter = o =>
            o is TeamListItemViewModel t && _filters.All(filter => filter(t));
    }

    private void OnSortStrategyNameChanged()
    {
        TeamsCollectionView.SortDescriptions.Clear();
        
        var newSortDescription = SortStrategyName switch
        {
            "Team Name Asc" => new SortDescription(nameof(SelectedTeam.FullName), ListSortDirection.Ascending),
            "Team name Desc" => new SortDescription(nameof(SelectedTeam.FullName), ListSortDirection.Descending),
            "Points Asc" => new SortDescription(nameof(SelectedTeam.Points), ListSortDirection.Ascending),
            "Points Desc" => new SortDescription(nameof(SelectedTeam.Points), ListSortDirection.Descending),
            "Members Asc" => new SortDescription(nameof(SelectedTeam.Members), ListSortDirection.Ascending),
            "Members Desc" => new SortDescription(nameof(SelectedTeam.Members), ListSortDirection.Descending),
            _ => new SortDescription()
        };
        
        TeamsCollectionView.SortDescriptions.Add(newSortDescription);
    }

    private bool FilterTeamsBySearch(TeamListItemViewModel team)
    {
        var lowerFilter = TeamsSearchFilter.ToLower();

        return team.FullName.ToLower().StartsWith(lowerFilter) ||
               team.OriginCity.ToLower().StartsWith(lowerFilter);
    }

    private bool FilterTeamsByMembers(TeamListItemViewModel team)
    {
        // if Max is not specified it is set to infinity 
        var actualMaxMembers = MaxMembers == 0 ? int.MaxValue : MaxMembers;

        if (MinMembers > actualMaxMembers)
        {
            return false;
        }

        return team.Members >= MinMembers && team.Members <= actualMaxMembers;
    }

    private bool FilterTeamsByPoints(TeamListItemViewModel team)
    {
        // if Max is not specified it is set to infinity 
        var actualMaxPoints = MaxPoints == 0 ? int.MaxValue : MaxPoints;

        if (MinPoints > actualMaxPoints)
        {
            return false;
        }

        return team.Points >= MinPoints && team.Points <= actualMaxPoints;
    }

    public void RefreshItemSource()
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
                Points = _teamService.GetPoints(t),
                Members = t.MembersCount,
                TotalGames = _teamService.GetTotalGames(t),
                Wins = _teamService.GetGamesWon(t)
            })
            .ToList();
    }
}
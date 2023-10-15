using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using TeamTracker.Wpf.Commands;
using TeamTracker.Wpf.ViewModels.Inners;

namespace TeamTracker.Wpf.ViewModels.Components;

public class TeamListViewModel : BaseViewModel
{
    private class TeamsFilter
    {
        public string? NameOrCity { get; set; }
        public int? MinMembers { get; set; }
        public int? MaxMembers { get; set; }
        public int? MinPoints { get; set; }
        public int? MaxPoints { get; set; }
        public int? TotalGames { get; set; }
        public int? TotalWins { get; set; }
        public int? ResultsToShowCount { get; set; }
    }

    private readonly TeamsFilter _teamsFilter;
    private TeamListItemViewModel? _selectedTeam;
    private string _teamsNameFilter;
    private string _sortStrategyName;
    private int _minMembers;
    private int _maxMembers;
    private int _minPoints;
    private int _maxPoints;
    private bool _isAdvancedFilterActive;

    public ICommand ShowMostWinsCommand { get; }
    public ICommand ShowMostPointsCommand { get; }
    public ICommand ShowNewcomersCommand { get; }
    public ICommand ResetAdvancedFiltersCommand { get; }

    public string[] SortOptions { get; }
    public ICollectionView TeamsCollectionView { get; }

    public ObservableCollection<TeamListItemViewModel> Teams { get; }

    public int MinMembers
    {
        get => _minMembers;
        set
        {
            if (value == _minMembers) return;
            _minMembers = value;
            _teamsFilter.MinMembers = value;

            OnPropertyChanged();
            FilterTeams();
        }
    }

    public int MaxMembers
    {
        get => _maxMembers;
        set
        {
            if (value == _maxMembers) return;
            _maxMembers = value;
            _teamsFilter.MaxMembers = value == 0 ? null : value;

            OnPropertyChanged();
            FilterTeams();
        }
    }

    public int MinPoints
    {
        get => _minPoints;
        set
        {
            if (value == _minPoints) return;
            _minPoints = value;
            _teamsFilter.MinPoints = value;

            OnPropertyChanged();
            FilterTeams();
        }
    }

    public int MaxPoints
    {
        get => _maxPoints;
        set
        {
            if (value == _maxPoints) return;
            _maxPoints = value;
            _teamsFilter.MaxPoints = value == 0 ? null : value;

            OnPropertyChanged();
            FilterTeams();
        }
    }

    public string TeamsNameFilter
    {
        get => _teamsNameFilter;
        set
        {
            if (value == _teamsNameFilter) return;
            _teamsNameFilter = value ?? string.Empty;
            _teamsFilter.NameOrCity = value;

            OnPropertyChanged();
            FilterTeams();
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

    public TeamListViewModel()
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

        _teamsFilter = new TeamsFilter();
        _teamsNameFilter = string.Empty;
        Teams = new ObservableCollection<TeamListItemViewModel>();

        TeamsCollectionView = CollectionViewSource.GetDefaultView(Teams);

        TeamsCollectionView.SortDescriptions.Add(
            new SortDescription("FullName",
                ListSortDirection.Ascending));

        ShowMostWinsCommand = new RelayCommand<object>(ShowMostWins_Execute);
        ShowMostPointsCommand = new RelayCommand<object>(ShowMostPoints_Execute);
        ShowNewcomersCommand = new RelayCommand<object>(ShowNewcomers_Execute);
        ResetAdvancedFiltersCommand =
            new RelayCommand<object>(_ => SetFiltersToDefault(), _ => _isAdvancedFilterActive);
    }

    private void ShowNewcomers_Execute(object obj)
    {
        RemoveAdvancedFilters();
        _isAdvancedFilterActive = true;

        _teamsFilter.TotalGames = 0;

        FilterTeams();
    }

    private void ShowMostPoints_Execute(object obj)
    {
        RemoveAdvancedFilters();
        _isAdvancedFilterActive = true;

        var mostPoints = Teams.Max(team => team.Points);
        _teamsFilter.MinPoints = mostPoints;
        _teamsFilter.MaxPoints = mostPoints;

        FilterTeams();
    }

    private void ShowMostWins_Execute(object obj)
    {
        RemoveAdvancedFilters();
        _isAdvancedFilterActive = true;

        var mostWins = Teams.Max(team => team.Wins);
        _teamsFilter.TotalWins = mostWins;

        FilterTeams();
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

    private void FilterTeams()
    {
        var remainsResults = _teamsFilter.ResultsToShowCount ?? int.MaxValue;

        TeamsCollectionView.Filter = o =>
        {
            if (o is not TeamListItemViewModel team)
            {
                return false;
            }

            if (_teamsFilter.NameOrCity != null &&
                !team.FullName.ToLower().StartsWith(_teamsFilter.NameOrCity) &&
                !team.OriginCity.ToLower().StartsWith(_teamsFilter.NameOrCity))
            {
                return false;
            }

            if (_teamsFilter.MinPoints != null && team.Points < _teamsFilter.MinPoints)
            {
                return false;
            }

            if (_teamsFilter.MaxPoints != null && team.Points > _teamsFilter.MaxPoints)
            {
                return false;
            }

            if (_teamsFilter.MinMembers != null && team.Members < _teamsFilter.MinMembers)
            {
                return false;
            }

            if (_teamsFilter.MaxMembers != null && team.Members > _teamsFilter.MaxMembers)
            {
                return false;
            }

            if (_teamsFilter.TotalGames != null && team.TotalGames != _teamsFilter.TotalGames)
            {
                return false;
            }

            if (_teamsFilter.TotalWins != null && team.Wins != _teamsFilter.TotalWins)
            {
                return false;
            }

            if (remainsResults <= 0)
            {
                return false;
            }

            remainsResults--;
            return true;
        };
    }

    private void SetFiltersToDefault()
    {
        _isAdvancedFilterActive = false;
        RemoveAdvancedFilters();

        FilterTeams();
    }

    private void RemoveAdvancedFilters()
    {
        _teamsFilter.TotalGames = null;
        _teamsFilter.TotalWins = null;
        _teamsFilter.MinPoints = MinPoints;
        _teamsFilter.MaxPoints = MaxPoints == 0 ? null : MaxPoints;
    }
}
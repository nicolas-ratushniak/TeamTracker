using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using TeamTracker.Wpf.Commands;
using TeamTracker.Wpf.ViewModels.Inners;

namespace TeamTracker.Wpf.ViewModels.Components;

public class GameListComponent : BaseViewModel
{
    private class GamesFilter
    {
        public string? HomeTeamFullName { get; set; }
        public string? AwayTeamFullName { get; set; }
        public int? GoalDifference { get; set; }
        public bool? IsSameCity { get; set; }
        public int? ResultsToShowCount { get; set; }
    }

    public event EventHandler SelectedGameChanged;

    private readonly GamesFilter _gamesFilter;
    private GameListItemViewModel? _selectedGame;
    private string _sortStrategyName;
    private string _homeTeamNameFilter = string.Empty;
    private string _awayTeamNameFilter = string.Empty;
    private bool _isAdvancedFilterActive;

    public ICommand ShowMostCrushingGameCommand { get; }
    public ICommand ShowDrawsCommand { get; }
    public ICommand ShowSameCityCommand { get; }
    public ICommand ResetAdvancedFiltersCommand { get; }

    public string[] SortOptions { get; }
    public ICollectionView GamesCollectionView { get; }

    public ObservableCollection<GameListItemViewModel> Games { get; }

    public string HomeTeamNameFilter
    {
        get => _homeTeamNameFilter;
        set
        {
            if (value == _homeTeamNameFilter) return;
            _homeTeamNameFilter = value;
            _gamesFilter.HomeTeamFullName = value;

            OnPropertyChanged();
            FilterGames();
        }
    }

    public string AwayTeamNameFilter
    {
        get => _awayTeamNameFilter;
        set
        {
            if (value == _awayTeamNameFilter) return;
            _awayTeamNameFilter = value;
            _gamesFilter.AwayTeamFullName = value;

            OnPropertyChanged();
            FilterGames();
        }
    }

    public GameListItemViewModel? SelectedGame
    {
        get => _selectedGame;
        set
        {
            if (Equals(value, _selectedGame)) return;
            _selectedGame = value;
            
            OnPropertyChanged();
            SelectedGameChanged?.Invoke(this, EventArgs.Empty);
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

    public GameListComponent()
    {
        SortOptions = new[]
        {
            "Recent first",
            "Older first"
        };

        _gamesFilter = new GamesFilter();
        Games = new ObservableCollection<GameListItemViewModel>();

        GamesCollectionView = CollectionViewSource.GetDefaultView(Games);
        
        GamesCollectionView.SortDescriptions.Add(
            new SortDescription(nameof(SelectedGame.Date), ListSortDirection.Ascending));

        ShowMostCrushingGameCommand = new RelayCommand<object>(ShowMostCrushingGame_Execute);
        ShowDrawsCommand = new RelayCommand<object>(ShowDrawsCommand_Execute);
        ShowSameCityCommand = new RelayCommand<object>(ShowSameCity_Execute);
        
        ResetAdvancedFiltersCommand =
            new RelayCommand<object>(_ => SetFiltersToDefault(), _ => _isAdvancedFilterActive);
    }

    public void SetFiltersToDefault()
    {
        _isAdvancedFilterActive = false;
        RemoveAdvancedFilters();

        FilterGames();
    }

    private void ShowMostCrushingGame_Execute(object obj)
    {
        RemoveAdvancedFilters();
        _isAdvancedFilterActive = true;

        var maxGoalDifference = Games.Max(game => Math.Abs(game.HomeTeamScore - game.AwayTeamScore));
        _gamesFilter.GoalDifference = maxGoalDifference;

        FilterGames();
    }

    private void ShowDrawsCommand_Execute(object obj)
    {
        RemoveAdvancedFilters();
        _isAdvancedFilterActive = true;

        _gamesFilter.GoalDifference = 0;
        FilterGames();
    }

    private void ShowSameCity_Execute(object obj)
    {
        RemoveAdvancedFilters();
        
        _isAdvancedFilterActive = true;
        _gamesFilter.IsSameCity = true;

        FilterGames();
    }

    private void OnSortStrategyNameChanged()
    {
        GamesCollectionView.SortDescriptions.Clear();

        var newSortDescription = SortStrategyName switch
        {
            "Recent first" => new SortDescription(nameof(SelectedGame.Date), ListSortDirection.Descending),
            "Older first" => new SortDescription(nameof(SelectedGame.Date), ListSortDirection.Ascending),
            _ => new SortDescription()
        };

        GamesCollectionView.SortDescriptions.Add(newSortDescription);
    }

    private void FilterGames()
    {
        var remainsResults = _gamesFilter.ResultsToShowCount ?? int.MaxValue;

        GamesCollectionView.Filter = o =>
        {
            if (o is not GameListItemViewModel game)
            {
                return false;
            }

            if (_gamesFilter.AwayTeamFullName != null &&
                !game.AwayTeamFullName.ToLower().StartsWith(_gamesFilter.AwayTeamFullName.ToLower()))
            {
                return false;
            }

            if (_gamesFilter.HomeTeamFullName != null &&
                !game.HomeTeamFullName.ToLower().StartsWith(_gamesFilter.HomeTeamFullName.ToLower()))
            {
                return false;
            }

            if (_gamesFilter.GoalDifference != null &&
                _gamesFilter.GoalDifference != Math.Abs(game.HomeTeamScore - game.AwayTeamScore))
            {
                return false;
            }

            if (_gamesFilter.IsSameCity != null && game.HomeTeamOriginCity != game.AwayTeamOriginCity)
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

    private void RemoveAdvancedFilters()
    {
        _gamesFilter.GoalDifference = null;
        _gamesFilter.IsSameCity = null;
    }
}
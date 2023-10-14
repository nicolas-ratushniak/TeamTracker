﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Commands;
using TeamTracker.Wpf.ViewModels.Inners;

namespace TeamTracker.Wpf.ViewModels.Components;

public class GameListViewModel : BaseViewModel
{
    private GameListItemViewModel? _selectedGame;
    private string _gamesSearchFilter = string.Empty;
    private string _sortStrategyName;
    private bool _isAdvancedFilterActive;


    public ICommand ShowMostCrushingGameCommand { get; }
    public ICommand ShowDrawsCommand { get; }
    public ICommand ResetAdvancedFiltersCommand { get; }

    public string[] SortOptions { get; }
    public ICollectionView GamesCollectionView { get; }

    public ObservableCollection<GameListItemViewModel> Games { get; }

    public string GamesSearchFilter
    {
        get => _gamesSearchFilter;
        set
        {
            if (value == _gamesSearchFilter) return;
            _gamesSearchFilter = value ?? string.Empty;

            OnPropertyChanged();
            GamesCollectionView.Refresh();
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

    public GameListViewModel()
    {
        SortOptions = new[]
        {
            "Recent first",
            "Older first"
        };

        Games = new ObservableCollection<GameListItemViewModel>();

        GamesCollectionView = CollectionViewSource.GetDefaultView(Games);
        SetDefaultFilters();
        UpdateSortStrategy(new SortDescription(nameof(SelectedGame.Date), ListSortDirection.Ascending));

        ShowMostCrushingGameCommand = new RelayCommand<object>(ShowMostCrushingGame_Execute);
        ShowDrawsCommand = new RelayCommand<object>(ShowDrawsCommand_Execute);
        ResetAdvancedFiltersCommand = new RelayCommand<object>(_ => SetDefaultFilters(), _ => _isAdvancedFilterActive);
    }

    private void ShowMostCrushingGame_Execute(object obj)
    {
        _isAdvancedFilterActive = true;

        GamesCollectionView.Filter = o =>
        {
            if (o is not GameListItemViewModel g)
            {
                return false;
            }

            var currentGoalsDiff = Math.Abs(g.HomeTeamScore - g.AwayTeamScore);
            var maxGoalsDiff = Games.Max(game => Math.Abs(game.HomeTeamScore - game.AwayTeamScore));

            return FilterGamesBySearch(g) && currentGoalsDiff == maxGoalsDiff;
        };
    }

    private void ShowDrawsCommand_Execute(object obj)
    {
        _isAdvancedFilterActive = true;

        GamesCollectionView.Filter = o => o is GameListItemViewModel g && FilterGamesBySearch(g) &&
                                          g.HomeTeamScore == g.AwayTeamScore;
    }

    private void SetDefaultFilters()
    {
        _isAdvancedFilterActive = false;
        GamesCollectionView.Filter = o => o is GameListItemViewModel g && FilterGamesBySearch(g);
    }

    private bool FilterGamesBySearch(GameListItemViewModel game)
    {
        var lowerFilter = GamesSearchFilter.ToLower();
        var underscoreIndex = lowerFilter.IndexOf('_');

        if (underscoreIndex == -1)
        {
            return game.HomeTeamFullName.ToLower().StartsWith(lowerFilter) ||
                   game.AwayTeamFullName.ToLower().StartsWith(lowerFilter);
        }

        var homeFilter = lowerFilter[..underscoreIndex];
        var awayFilter = lowerFilter[(underscoreIndex + 1)..];

        if (underscoreIndex == 0)
        {
            return game.AwayTeamFullName.ToLower().StartsWith(awayFilter);
        }

        if (underscoreIndex == lowerFilter.Length - 1)
        {
            return game.HomeTeamFullName.ToLower().StartsWith(homeFilter);
        }

        return game.HomeTeamFullName.ToLower().StartsWith(homeFilter) &&
               game.AwayTeamFullName.ToLower().StartsWith(awayFilter);
    }

    private SortDescription GetSortStrategy(string sortStrategyName)
    {
        return sortStrategyName switch
        {
            "Recent first" => new SortDescription(nameof(SelectedGame.Date), ListSortDirection.Descending),
            "Older first" => new SortDescription(nameof(SelectedGame.Date), ListSortDirection.Ascending),
            _ => new SortDescription()
        };
    }

    private void UpdateSortStrategy(SortDescription sortDescription)
    {
        GamesCollectionView.SortDescriptions.Clear();
        GamesCollectionView.SortDescriptions.Add(sortDescription);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Commands;
using TeamTracker.Wpf.Navigation;
using TeamTracker.Wpf.ViewModels.Components;
using TeamTracker.Wpf.ViewModels.Inners;

namespace TeamTracker.Wpf.ViewModels;

public class GamesViewModel : BaseViewModel
{
    private readonly IGameInfoService _gameInfoService;
    private readonly ITeamService _teamService;
    private readonly ILogger<GamesViewModel> _logger;
    private string _date;
    private string _homeTeamName;
    private string _homeTeamOriginCity;
    private int _homeTeamScore;
    private string _awayTeamName;
    private string _awayTeamOriginCity;
    private int _awayTeamScore;
    private bool _isGameSelected;
    private Guid? _selectedGameId;

    public ICommand GoToGameCreateCommand { get; }
    public ICommand DeleteGameCommand { get; }

    public GameListComponent GameList { get; }

    public bool IsGameSelected
    {
        get => _isGameSelected;
        set
        {
            if (value == _isGameSelected) return;
            _isGameSelected = value;
            OnPropertyChanged();
        }
    }

    public string Date
    {
        get => _date;
        set
        {
            if (value == _date) return;
            _date = value;
            OnPropertyChanged();
        }
    }

    public string HomeTeamName
    {
        get => _homeTeamName;
        set
        {
            if (value == _homeTeamName) return;
            _homeTeamName = value;
            OnPropertyChanged();
        }
    }

    public string HomeTeamOriginCity
    {
        get => _homeTeamOriginCity;
        set
        {
            if (value == _homeTeamOriginCity) return;
            _homeTeamOriginCity = value;
            OnPropertyChanged();
        }
    }

    public int HomeTeamScore
    {
        get => _homeTeamScore;
        set
        {
            if (value == _homeTeamScore) return;
            _homeTeamScore = value;
            OnPropertyChanged();
        }
    }

    public string AwayTeamName
    {
        get => _awayTeamName;
        set
        {
            if (value == _awayTeamName) return;
            _awayTeamName = value;
            OnPropertyChanged();
        }
    }

    public string AwayTeamOriginCity
    {
        get => _awayTeamOriginCity;
        set
        {
            if (value == _awayTeamOriginCity) return;
            _awayTeamOriginCity = value;
            OnPropertyChanged();
        }
    }

    public int AwayTeamScore
    {
        get => _awayTeamScore;
        set
        {
            if (value == _awayTeamScore) return;
            _awayTeamScore = value;
            OnPropertyChanged();
        }
    }

    public GamesViewModel(
        IGameInfoService gameInfoService,
        ITeamService teamService,
        INavigationService navigationService,
        ILogger<GamesViewModel> logger)
    {
        _gameInfoService = gameInfoService;
        _teamService = teamService;
        _logger = logger;

        GameList = new GameListComponent();
        GameList.SelectedGameChanged += OnSelectedGameChanged;

        GoToGameCreateCommand = new RelayCommand<object>(
            _ => navigationService.NavigateTo(ViewType.GameCreate, null));

        DeleteGameCommand = new RelayCommand<object>(
            DeleteGame_Execute,
            _ => GameList.SelectedGame is not null);

        LoadedCommand = new RelayCommand<object>(LoadData);
    }

    ~GamesViewModel()
    {
        _logger.LogDebug("The {ViewModel} was destroyed", nameof(GamesViewModel));
    }

    public override void Dispose()
    {
        GameList.SelectedGameChanged -= OnSelectedGameChanged;
        base.Dispose();
    }

    private void DeleteGame_Execute(object obj)
    {
        var messageBoxResult = MessageBox.Show("Are you sure, you want to delete this game?",
            "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (messageBoxResult != MessageBoxResult.Yes)
        {
            return;
        }

        _gameInfoService.Delete((Guid)_selectedGameId!);
        _logger.LogInformation("Successfully deleted a game with id {GameId}", (Guid)_selectedGameId!);

        RefreshGameListItems();
    }

    private void LoadData(object obj)
    {
        foreach (var game in GetGames())
        {
            GameList.Games.Add(game);
        }
    }

    private void OnSelectedGameChanged(object? sender, EventArgs e)
    {
        var selectedGame = GameList.SelectedGame;

        if (selectedGame is null)
        {
            IsGameSelected = false;
            _selectedGameId = null;
            return;
        }

        _selectedGameId = selectedGame.Id;

        Date = selectedGame.Date.ToShortDateString();
        HomeTeamName = selectedGame.HomeTeamName;
        HomeTeamOriginCity = selectedGame.HomeTeamOriginCity;
        HomeTeamScore = selectedGame.HomeTeamScore;
        AwayTeamName = selectedGame.AwayTeamName;
        AwayTeamOriginCity = selectedGame.AwayTeamOriginCity;
        AwayTeamScore = selectedGame.AwayTeamScore;
        IsGameSelected = true;
    }

    private void RefreshGameListItems()
    {
        GameList.Games.Clear();

        foreach (var game in GetGames())
        {
            GameList.Games.Add(game);
        }
    }

    private IEnumerable<GameListItemViewModel> GetGames()
    {
        try
        {
            return _gameInfoService.GetAll()
                .Select(g =>
                {
                    var homeTeam = _teamService.Get(g.TeamHomeId);
                    var awayTeam = _teamService.Get(g.TeamAwayId);

                    return new GameListItemViewModel
                    {
                        Id = g.Id,
                        Date = g.Date,
                        HomeTeamName = homeTeam.Name,
                        HomeTeamOriginCity = homeTeam.OriginCity,
                        HomeTeamScore = g.TeamHomeScore,
                        AwayTeamName = awayTeam.Name,
                        AwayTeamOriginCity = awayTeam.OriginCity,
                        AwayTeamScore = g.TeamAwayScore
                    };
                });
        }
        catch (InvalidDataException)
        {
            _logger.LogError("Failed to read games from the file");

            MessageBox.Show("The database file is broken. Please, contact the developer",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            throw;
        }
    }
}
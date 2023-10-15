using System.Collections.Generic;
using System.ComponentModel;
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
    private GameDetailsItemViewModel? _selectedGameDetails;

    public ICommand AddGameCommand { get; }
    public GameListComponent GameList { get; }

    public GameDetailsItemViewModel? SelectedGameDetails
    {
        get => _selectedGameDetails;
        set
        {
            if (Equals(value, _selectedGameDetails)) return;
            _selectedGameDetails = value;
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
        GameList.PropertyChanged += SelectedGame_OnPropertyChanged;

        AddGameCommand = new RelayCommand<object>(
            _ => navigationService.NavigateTo(ViewType.GameCreate, null));

        LoadedCommand = new RelayCommand<object>(LoadData);
    }

    private void LoadData(object obj)
    {
        foreach (var game in GetGames())
        {
            GameList.Games.Add(game);
        }
    }

    public override void Dispose()
    {
        GameList.PropertyChanged -= SelectedGame_OnPropertyChanged;
        base.Dispose();
    }

    ~GamesViewModel()
    {
        _logger.LogDebug("The {ViewModel} was destroyed", nameof(GamesViewModel));
    }

    private void SelectedGame_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(GameList.SelectedGame))
        {
            return;
        }

        var selectedGame = ((GameListComponent)sender!).SelectedGame;

        if (selectedGame is null)
        {
            SelectedGameDetails = null;
        }
        else
        {
            var game = _gameInfoService.Get(selectedGame.Id);

            SelectedGameDetails = new GameDetailsItemViewModel
            {
                Date = game.Date.ToShortDateString(),
                HomeTeamName = selectedGame.HomeTeamName,
                HomeTeamOriginCity = selectedGame.HomeTeamOriginCity,
                HomeTeamScore = selectedGame.HomeTeamScore,
                AwayTeamName = selectedGame.AwayTeamName,
                AwayTeamOriginCity = selectedGame.AwayTeamOriginCity,
                AwayTeamScore = selectedGame.AwayTeamScore
            };
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
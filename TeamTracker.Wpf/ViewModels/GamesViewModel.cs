using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Commands;
using TeamTracker.Wpf.Navigation;
using TeamTracker.Wpf.ViewModels.Inners;

namespace TeamTracker.Wpf.ViewModels;

public class GamesViewModel : ViewModelBase
{
    private readonly IGameInfoService _gameInfoService;
    private readonly ILogger<GamesViewModel> _logger;
    private GameDetailsItemViewModel? _selectedGameDetails;

    public ICommand AddGameCommand { get; }
    public GameListViewModel GameList { get; }

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

    public GamesViewModel(IGameInfoService gameInfoService, ITeamService teamService, INavigator navigator,
        ILogger<GamesViewModel> logger)
    {
        _gameInfoService = gameInfoService;
        _logger = logger;

        GameList = new GameListViewModel(gameInfoService, teamService);
        
        GameList.PropertyChanged += SelectedGame_OnPropertyChanged;

        AddGameCommand = new RelayCommand<object>(
            _ => navigator.UpdateCurrentViewType(ViewType.GameCreate, null));
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

        var selectedGame = ((GameListViewModel)sender!).SelectedGame;

        if (selectedGame is null)
        {
            SelectedGameDetails = null;
        }
        else
        {
            var game = _gameInfoService.Get(selectedGame.Id);

            SelectedGameDetails = new GameDetailsItemViewModel
            {
                Id = selectedGame.Id,
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
}
using System.ComponentModel;
using System.Windows.Input;
using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Commands;
using TeamTracker.Wpf.Navigation;

namespace TeamTracker.Wpf.ViewModels;

public class GamesViewModel : ViewModelBase
{
    private readonly IGameInfoService _gameInfoService;

    public ICommand AddGameCommand { get; }
    public GameListViewModel GameList { get; set; }
    public GameDetailsViewModel SelectedGameDetails { get; set; }

    public GamesViewModel(IGameInfoService gameInfoService, ITeamService teamService, INavigator navigator)
    {
        _gameInfoService = gameInfoService;
        
        GameList = new GameListViewModel(gameInfoService, teamService);
        SelectedGameDetails = new GameDetailsViewModel();
        
        GameList.PropertyChanged += SelectedGame_OnPropertyChanged;

        AddGameCommand = new RelayCommand<object>(
            _ => navigator.UpdateCurrentViewType(ViewType.GameCreate, null));
    }

    public override void Dispose()
    {
        GameList.PropertyChanged -= SelectedGame_OnPropertyChanged;
        base.Dispose();
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
            SelectedGameDetails.Game = null;
        }
        else
        {
            var game = _gameInfoService.Get(selectedGame.Id);

            SelectedGameDetails.Game = new GameDetailsItemViewModel
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
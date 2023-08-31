using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using TeamTracker.Domain.Services;

namespace TeamTracker.Wpf.ViewModels;

public class GamesListViewModel : ViewModelBase
{
    private readonly IGameInfoService _gameInfoService;
    private readonly ITeamService _teamService;
    private readonly ObservableCollection<GamesListItemViewModel> _games;
    private GamesListItemViewModel? _selectedGame;
    private string _gamesSearchFilter;

    public ICollectionView GamesCollectionView { get; }

    public ICommand ShowMostCrushingGameCommand { get; }
    public ICommand ShowDrawsCommand { get; }

    public string GamesSearchFilter
    {
        get => _gamesSearchFilter;
        set
        {
            if (value == _gamesSearchFilter) return;
            _gamesSearchFilter = value;
            OnPropertyChanged();
        }
    }

    public GamesListItemViewModel? SelectedGame
    {
        get => _selectedGame;
        set
        {
            if (Equals(value, _selectedGame)) return;
            _selectedGame = value;
            OnPropertyChanged();
        }
    }
    
    public GamesListViewModel(IGameInfoService gameInfoService, ITeamService teamService)
    {
        _gameInfoService = gameInfoService;
        _teamService = teamService;

        _games = new ObservableCollection<GamesListItemViewModel>(GetGames());

        GamesCollectionView = CollectionViewSource.GetDefaultView(_games);
    }

    private IEnumerable<GamesListItemViewModel> GetGames()
    {
        return _gameInfoService.GetAll()
            .Select(g =>
            {
                var homeTeam = _teamService.Get(g.TeamHomeId);
                var awayTeam = _teamService.Get(g.TeamAwayId);

                return new GamesListItemViewModel
                {
                    Id = g.Id,
                    HomeTeamName = homeTeam.Name,
                    HomeTeamOriginCity = homeTeam.OriginCity,
                    HomeTeamScore = g.TeamHomeScore,
                    AwayTeamName = awayTeam.Name,
                    AwayTeamOriginCity = awayTeam.OriginCity,
                    AwayTeamScore = g.TeamAwayScore
                };
            });
    }
}
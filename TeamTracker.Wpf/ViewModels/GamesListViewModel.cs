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
    private string _gamesSearchFilter = string.Empty;
    private string _sortStrategyName;

    
    public ICommand ShowMostCrushingGameCommand { get; }
    public ICommand ShowDrawsCommand { get; }
    public string[] SortOptions { get; }
    public ICollectionView GamesCollectionView { get; }

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
    
    public GamesListViewModel(IGameInfoService gameInfoService, ITeamService teamService)
    {
        SortOptions = new[]
        {
            "Recent first",
            "Older first"
        };
        
        _gameInfoService = gameInfoService;
        _teamService = teamService;

        _games = new ObservableCollection<GamesListItemViewModel>(GetGames());

        GamesCollectionView = CollectionViewSource.GetDefaultView(_games);
        GamesCollectionView.Filter =
            o => o is GamesListItemViewModel g && (g.HomeTeamName.ToLower().StartsWith(GamesSearchFilter.ToLower()) ||
                                                   g.AwayTeamName.ToLower().StartsWith(GamesSearchFilter.ToLower()));
        UpdateSortStrategy(new SortDescription(nameof(SelectedGame.Date), ListSortDirection.Ascending));
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
}
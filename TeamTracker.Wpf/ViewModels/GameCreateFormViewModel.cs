using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using TeamTracker.Domain.Dto;
using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Commands;
using TeamTracker.Wpf.Navigation;

namespace TeamTracker.Wpf.ViewModels;

public class GameCreateFormViewModel : ViewModelBase
{
    private readonly IGameInfoService _gameInfoService;
    private readonly ITeamService _teamService;
    private readonly INavigator _navigator;
    private readonly ILogger<GameCreateFormViewModel> _logger;
    private SelectTeamItemViewModel? _selectedHomeTeam;
    private SelectTeamItemViewModel? _selectedAwayTeam;
    private string _homeTeamSearchFilter = string.Empty;
    private string _awayTeamSearchFilter = string.Empty;
    private bool _areHomeCandidatesVisible;
    private bool _areAwayCandidatesVisible;
    private DateTime _date;
    private int _homeTeamScore; 
    private int _awayTeamScore;
    private string? _errorMessage;

    public ICommand SubmitCommand { get; }
    public ICommand CancelCommand { get; }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (value == _errorMessage) return;
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public DateTime Date
    {
        get => _date;
        set
        {
            if (value.Equals(_date)) return;
            _date = value;
            
            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
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
            CommandManager.InvalidateRequerySuggested();
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
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public SelectTeamItemViewModel? SelectedHomeTeam
    {
        get => _selectedHomeTeam;
        set
        {
            if (Equals(value, _selectedHomeTeam)) return;
            _selectedHomeTeam = value;

            OnPropertyChanged();
            OnSelectedHomeTeamChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public SelectTeamItemViewModel? SelectedAwayTeam
    {
        get => _selectedAwayTeam;
        set
        {
            if (Equals(value, _selectedAwayTeam)) return;
            _selectedAwayTeam = value;
            
            OnPropertyChanged();
            OnSelectedAwayTeamChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public string HomeTeamSearchFilter
    {
        get => _homeTeamSearchFilter;
        set
        {
            if (value == _homeTeamSearchFilter) return;
            _homeTeamSearchFilter = value ?? string.Empty;

            OnPropertyChanged();
            OnHomeTeamFilterChanged();
        }
    }

    public string AwayTeamSearchFilter
    {
        get => _awayTeamSearchFilter;
        set
        {
            if (value == _awayTeamSearchFilter) return;
            _awayTeamSearchFilter = value ?? string.Empty;
            
            OnPropertyChanged();
            OnAwayTeamFilterChanged();
        }
    }

    public bool AreHomeCandidatesVisible
    {
        get => _areHomeCandidatesVisible;
        set
        {
            if (value == _areHomeCandidatesVisible) return;
            _areHomeCandidatesVisible = value;
            OnPropertyChanged();
        }
    }

    public bool AreAwayCandidatesVisible
    {
        get => _areAwayCandidatesVisible;
        set
        {
            if (value == _areAwayCandidatesVisible) return;
            _areAwayCandidatesVisible = value;
            OnPropertyChanged();
        }
    }

    public ICollectionView HomeTeamCandidates { get; }
    public ICollectionView AwayTeamCandidates { get; }


    public GameCreateFormViewModel(IGameInfoService gameInfoService, ITeamService teamService, INavigator navigator,
        ILogger<GameCreateFormViewModel> logger)
    {
        _gameInfoService = gameInfoService;
        _teamService = teamService;
        _navigator = navigator;
        _logger = logger;
        _date = DateTime.Now;

        HomeTeamCandidates = CollectionViewSource.GetDefaultView(GetTeams());
        HomeTeamCandidates.SortDescriptions.Add(new SortDescription(nameof(SelectTeamItemViewModel.FullName), ListSortDirection.Ascending));
        HomeTeamCandidates.Filter = o => o is SelectTeamItemViewModel t && FilterTeamBySearch(t, HomeTeamSearchFilter);

        AwayTeamCandidates = CollectionViewSource.GetDefaultView(GetTeams());
        AwayTeamCandidates.SortDescriptions.Add(new SortDescription(nameof(SelectTeamItemViewModel.FullName), ListSortDirection.Ascending));
        AwayTeamCandidates.Filter = o => o is SelectTeamItemViewModel t && FilterTeamBySearch(t, AwayTeamSearchFilter);

        SubmitCommand = new RelayCommand<object>(AddGame_Execute, AddGame_CanExecute);
        CancelCommand = new RelayCommand<object>(_ => navigator.UpdateCurrentViewType(ViewType.Games, null));
    }

    private bool AddGame_CanExecute(object obj)
    {
        return Date != default && SelectedHomeTeam != null && SelectedAwayTeam != null &&
               HomeTeamScore >= 0 && AwayTeamScore >= 0;
    }

    private void AddGame_Execute(object obj)
    {
        var dto = new GameInfoCreateDto
        {
            TeamHomeId = SelectedHomeTeam!.Id,
            TeamAwayId = SelectedAwayTeam!.Id,
            TeamHomeScore = HomeTeamScore,
            TeamAwayScore = AwayTeamScore,
            Date = DateOnly.FromDateTime(Date)
        };

        try
        {
            _gameInfoService.PlayGame(dto);
            _logger.LogInformation("The Game was successfully created");
            
            _navigator.UpdateCurrentViewType(ViewType.Games, null);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation exception was caught: {Message}", ex.Message);
            ErrorMessage = ex.Message;
        }
    }

    private void OnSelectedHomeTeamChanged()
    {
        if (_selectedHomeTeam is null)
        {
            return;
        }

        HomeTeamSearchFilter = _selectedHomeTeam.FullName;
        AreHomeCandidatesVisible = false;
    }
    
    private void OnSelectedAwayTeamChanged()
    {
        if (_selectedAwayTeam is null)
        {
            return;
        }

        AwayTeamSearchFilter = _selectedAwayTeam.FullName;
        AreAwayCandidatesVisible = false;
    }

    private void OnHomeTeamFilterChanged()
    {
        var filter = HomeTeamSearchFilter;

        if (_selectedHomeTeam is null)
        {
            AreHomeCandidatesVisible = !string.IsNullOrEmpty(filter);
        }
        else
        {
            if (filter == _selectedHomeTeam.FullName)
            {
                AreHomeCandidatesVisible = false;
                return;
            }

            AreHomeCandidatesVisible = true;
            _selectedHomeTeam = null;
        }
        
        HomeTeamCandidates.Refresh();
    }
    
    private void OnAwayTeamFilterChanged()
    {
        var filter = AwayTeamSearchFilter;

        if (_selectedAwayTeam is null)
        {
            AreAwayCandidatesVisible = !string.IsNullOrEmpty(filter);
        }
        else
        {
            if (filter == _selectedAwayTeam.FullName)
            {
                AreAwayCandidatesVisible = false;
                return;
            }

            AreAwayCandidatesVisible = true;
            _selectedAwayTeam = null;
        }
        
        AwayTeamCandidates.Refresh();
    }

    private bool FilterTeamBySearch(SelectTeamItemViewModel team, string filter)
    {
        var lowerFilter = filter.ToLower();
        
        return team.FullName.ToLower().StartsWith(lowerFilter) ||
               team.OriginCity.ToLower().StartsWith(lowerFilter);
    }

    private IEnumerable<SelectTeamItemViewModel> GetTeams()
    {
        return _teamService.GetAll()
            .Select(t => new SelectTeamItemViewModel
            {
                Id = t.Id,
                Name = t.Name,
                OriginCity = t.OriginCity
            }).ToList();
    }
}
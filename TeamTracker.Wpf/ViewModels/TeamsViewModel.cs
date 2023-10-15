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

public class TeamsViewModel : BaseViewModel
{
    private readonly ITeamService _teamService;
    private readonly ILogger<TeamsViewModel> _logger;
    private Guid? _selectedTeamId;
    private string _name;
    private string _originCity;
    private int _gamesWon;
    private int _gamesLost;
    private int _gamesDrawn;
    private int _membersCount;
    private int _totalGames;
    private int _points;
    private bool _isTeamSelected;

    public TeamListComponent TeamList { get; }

    public ICommand AddTeamCommand { get; }
    public ICommand EditTeamCommand { get; }
    public ICommand DeleteTeamCommand { get; }

    public bool IsTeamSelected
    {
        get => _isTeamSelected;
        set
        {
            if (value == _isTeamSelected) return;
            _isTeamSelected = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (value == _name) return;
            _name = value;
            OnPropertyChanged();
        }
    }

    public string OriginCity
    {
        get => _originCity;
        set
        {
            if (value == _originCity) return;
            _originCity = value;
            OnPropertyChanged();
        }
    }

    public int GamesWon
    {
        get => _gamesWon;
        set
        {
            if (value == _gamesWon) return;
            _gamesWon = value;
            OnPropertyChanged();
        }
    }

    public int GamesLost
    {
        get => _gamesLost;
        set
        {
            if (value == _gamesLost) return;
            _gamesLost = value;
            OnPropertyChanged();
        }
    }

    public int GamesDrawn
    {
        get => _gamesDrawn;
        set
        {
            if (value == _gamesDrawn) return;
            _gamesDrawn = value;
            OnPropertyChanged();
        }
    }

    public int MembersCount
    {
        get => _membersCount;
        set
        {
            if (value == _membersCount) return;
            _membersCount = value;
            OnPropertyChanged();
        }
    }

    public int TotalGames
    {
        get => _totalGames;
        set
        {
            if (value == _totalGames) return;
            _totalGames = value;
            OnPropertyChanged();
        }
    }

    public int Points
    {
        get => _points;
        set
        {
            if (value == _points) return;
            _points = value;
            OnPropertyChanged();
        }
    }

    public TeamsViewModel(
        ITeamService teamService,
        INavigationService navigationService,
        ILogger<TeamsViewModel> logger)
    {
        _teamService = teamService;
        _logger = logger;
        TeamList = new TeamListComponent();

        TeamList.SelectedTeamChanged += OnSelectedTeamChanged;

        AddTeamCommand = new RelayCommand<object>(
            _ => navigationService.NavigateTo(ViewType.TeamCreate, null));

        EditTeamCommand = new RelayCommand<object>(
            _ => navigationService.NavigateTo(ViewType.TeamUpdate, _selectedTeamId),
            _ => TeamList.SelectedTeam is not null);

        DeleteTeamCommand = new RelayCommand<object>(
            DeleteTeam_Execute,
            _ => TeamList.SelectedTeam is not null);

        LoadedCommand = new RelayCommand<object>(LoadData);
    }

    private void LoadData(object obj)
    {
        foreach (var team in GetTeams())
        {
            TeamList.Teams.Add(team);
        }
    }

    public override void Dispose()
    {
        TeamList.SelectedTeamChanged -= OnSelectedTeamChanged;
        base.Dispose();
    }

    ~TeamsViewModel()
    {
        _logger.LogDebug("The {ViewModel} was destroyed", nameof(TeamsViewModel));
    }

    private void DeleteTeam_Execute(object obj)
    {
        var messageBoxResult = MessageBox.Show("Are you sure, you want to delete this team?",
            "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (messageBoxResult != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            _teamService.Delete((Guid)_selectedTeamId!);
            _logger.LogInformation("Successfully deleted a team with id {TeamId}", (Guid)_selectedTeamId!);

            RefreshTeamListItems();
        }
        catch (InvalidOperationException)
        {
            _logger.LogInformation(
                "The team with id {TeamId} is not a newcomer. The deletion is cancelled",
                (Guid)_selectedTeamId!);

            MessageBox.Show("Sorry, cannot delete this team. It's not a newcomer.", "Sorry",
                MessageBoxButton.OK, MessageBoxImage.Hand);
        }
    }

    private void OnSelectedTeamChanged(object? sender, EventArgs e)
    {
        var selectedTeam = TeamList.SelectedTeam;

        if (selectedTeam is null)
        {
            IsTeamSelected = false;
            _selectedTeamId = null;
            return;
        }

        _selectedTeamId = selectedTeam.Id;
        var team = _teamService.Get(selectedTeam.Id);

        Name = team.Name;
        OriginCity = team.OriginCity;
        GamesWon = _teamService.CountGamesWon(team.Id);
        GamesLost = _teamService.CountGamesLost(team.Id);
        GamesDrawn = _teamService.CountGamesDrawn(team.Id);
        MembersCount = team.MembersCount;
        TotalGames = _teamService.CountTotalGames(team.Id);
        Points = _teamService.CountPoints(team.Id);
        IsTeamSelected = true;
    }

    private void RefreshTeamListItems()
    {
        TeamList.Teams.Clear();

        foreach (var team in GetTeams())
        {
            TeamList.Teams.Add(team);
        }
    }

    private IEnumerable<TeamListItemViewModel> GetTeams()
    {
        try
        {
            return _teamService.GetAll()
                .Select(t => new TeamListItemViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    OriginCity = t.OriginCity,
                    Points = _teamService.CountPoints(t.Id),
                    Members = t.MembersCount,
                    TotalGames = _teamService.CountTotalGames(t.Id),
                    Wins = _teamService.CountGamesWon(t.Id)
                })
                .ToList();
        }
        catch (InvalidDataException)
        {
            _logger.LogError("Failed to read teams from the file");

            MessageBox.Show("The database file is broken. Please, contact the developer",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            throw;
        }
    }
}
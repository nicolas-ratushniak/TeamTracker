using System;
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

public class TeamsViewModel : BaseViewModel
{
    private readonly ITeamService _teamService;
    private readonly ILogger<TeamsViewModel> _logger;
    private Guid? _selectedTeamId;
    private TeamDetailsItemViewModel? _selectedTeamDetails;

    public TeamListViewModel TeamList { get; }

    public ICommand AddTeamCommand { get; }
    public ICommand EditTeamCommand { get; }
    public ICommand DeleteTeamCommand { get; }
    
    public TeamDetailsItemViewModel? SelectedTeamDetails
    {
        get => _selectedTeamDetails;
        set
        {
            if (Equals(value, _selectedTeamDetails)) return;
            _selectedTeamDetails = value;
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
        TeamList = new TeamListViewModel();

        TeamList.PropertyChanged += TeamsList_OnPropertyChanged;

        AddTeamCommand = new RelayCommand<object>(
            _ => navigationService.NavigateTo(ViewType.TeamCreate, null));

        EditTeamCommand = new RelayCommand<object>(
            _ => navigationService.NavigateTo(ViewType.TeamUpdate, _selectedTeamId),
            _ => SelectedTeamDetails is not null);

        DeleteTeamCommand = new RelayCommand<object>(
            DeleteTeam_Execute,
            _ => SelectedTeamDetails is not null);

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
        TeamList.PropertyChanged -= TeamsList_OnPropertyChanged;
        base.Dispose();
    }
    
    ~TeamsViewModel()
    {
        _logger.LogDebug("The {ViewModel} was destroyed", nameof(TeamsViewModel));
    }

    private void DeleteTeam_Execute(object obj)
    {
        var messageBoxResult = MessageBox.Show("Are you sure, you want to delete this team?", "Warning",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (messageBoxResult != MessageBoxResult.Yes)
        {
            return;
        }
        
        try
        {
            _teamService.Delete((Guid)_selectedTeamId!);
            RefreshTeamListItems();
        }
        catch (InvalidOperationException)
        {
            MessageBox.Show("Sorry, cannot delete this team. It's not a newcomer.", "Sorry", 
                MessageBoxButton.OK, MessageBoxImage.Hand);
        }
    }

    private void TeamsList_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(TeamList.SelectedTeam))
        {
            return;
        }

        var selectedTeam = ((TeamListViewModel)sender!).SelectedTeam;

        if (selectedTeam is null)
        {
            SelectedTeamDetails = null;
        }
        else
        {
            var team = _teamService.Get(selectedTeam.Id);
            _selectedTeamId = team.Id;

            SelectedTeamDetails = new TeamDetailsItemViewModel
            {
                Name = team.Name,
                OriginCity = team.OriginCity,
                GamesWon = _teamService.GetGamesWon(team),
                GamesLost = _teamService.GetGamesLost(team),
                GamesDrawn = _teamService.GetGamesDrawn(team),
                MembersCount = team.MembersCount,
                TotalGames = _teamService.GetTotalGames(team),
                Points = _teamService.GetPoints(team)
            };
        }
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
                    Points = _teamService.GetPoints(t),
                    Members = t.MembersCount,
                    TotalGames = _teamService.GetTotalGames(t),
                    Wins = _teamService.GetGamesWon(t)
                })
                .ToList();
        }
        catch (InvalidDataException ex)
        {
            MessageBox.Show("The database file is broken", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            throw;
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using TeamTracker.Domain.Abstract;
using TeamTracker.Domain.Dto;
using TeamTracker.Wpf.Commands;
using TeamTracker.Wpf.Navigation;
using TeamTracker.Wpf.ViewModels.Components;
using TeamTracker.Wpf.ViewModels.Inners;

namespace TeamTracker.Wpf.ViewModels;

public class GameCreateViewModel : BaseViewModel
{
    private readonly IGameInfoService _gameInfoService;
    private readonly ITeamService _teamService;
    private readonly INavigationService _navigationService;
    private readonly ILogger<GameCreateViewModel> _logger;
    private DateTime _date;
    private int _homeTeamScore;
    private int _awayTeamScore;
    private string? _errorMessage;

    public ICommand SubmitCommand { get; }
    public ICommand CancelCommand { get; }

    public TeamSelectorComponent HomeTeamSelector { get; }
    public TeamSelectorComponent AwayTeamSelector { get; }

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

    public GameCreateViewModel(IGameInfoService gameInfoService, ITeamService teamService,
        INavigationService navigationService,
        ILogger<GameCreateViewModel> logger)
    {
        _gameInfoService = gameInfoService;
        _teamService = teamService;
        _navigationService = navigationService;
        _logger = logger;
        _date = DateTime.Now;

        HomeTeamSelector = new TeamSelectorComponent("Select Home Team");
        AwayTeamSelector = new TeamSelectorComponent("Select Away Team");

        HomeTeamSelector.SelectedTeamChanged += (_, _) => CommandManager.InvalidateRequerySuggested();
        AwayTeamSelector.SelectedTeamChanged += (_, _) => CommandManager.InvalidateRequerySuggested();

        SubmitCommand = new RelayCommand<object>(AddGame_Execute, AddGame_CanExecute);
        CancelCommand = new RelayCommand<object>(_ => navigationService.NavigateTo(ViewType.Games, null));
        LoadedCommand = new RelayCommand<object>(LoadData);
    }

    private void LoadData(object obj)
    {
        foreach (var team in GetTeams())
        {
            HomeTeamSelector.Teams.Add(team);
            AwayTeamSelector.Teams.Add(team);
        }
    }

    private bool AddGame_CanExecute(object obj)
    {
        return Date != default &&
               HomeTeamSelector.SelectedTeam != null &&
               AwayTeamSelector.SelectedTeam != null &&
               HomeTeamScore >= 0 &&
               AwayTeamScore >= 0;
    }

    private void AddGame_Execute(object obj)
    {
        var dto = new GameInfoCreateDto
        {
            TeamHomeId = HomeTeamSelector.SelectedTeam!.Id,
            TeamAwayId = AwayTeamSelector.SelectedTeam!.Id,
            TeamHomeScore = HomeTeamScore,
            TeamAwayScore = AwayTeamScore,
            Date = DateOnly.FromDateTime(Date)
        };

        try
        {
            _gameInfoService.Add(dto);
            _logger.LogInformation("The Game was successfully created");

            _navigationService.NavigateTo(ViewType.Games, null);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation exception was caught: {Message}", ex.Message);
            ErrorMessage = ex.Message;
        }
    }

    private IEnumerable<TeamDropdownListItemViewModel> GetTeams()
    {
        try
        {
            return _teamService.GetAll()
                .Select(t => new TeamDropdownListItemViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    OriginCity = t.OriginCity
                }).ToList();
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
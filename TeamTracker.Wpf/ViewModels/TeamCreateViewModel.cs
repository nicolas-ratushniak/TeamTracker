using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using TeamTracker.Domain.Dto;
using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Commands;
using TeamTracker.Wpf.Navigation;

namespace TeamTracker.Wpf.ViewModels;

public class TeamCreateViewModel : ViewModelBase
{
    private readonly ITeamService _teamService;
    private readonly INavigationService _navigationService;
    private readonly ILogger<TeamCreateViewModel> _logger;
    private string? _errorMessage;
    private string _name = string.Empty;
    private string _originCity = string.Empty;
    private int _membersCount;
    
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

    public string Name
    {
        get => _name;
        set
        {
            if (value == _name) return;
            _name = value;
            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
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
            CommandManager.InvalidateRequerySuggested();
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
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public TeamCreateViewModel(ITeamService teamService, INavigationService navigationService, ILogger<TeamCreateViewModel> logger)
    {
        _teamService = teamService;
        _navigationService = navigationService;
        _logger = logger;

        SubmitCommand = new RelayCommand<object>(AddTeam, AddTeam_CanExecute);
        CancelCommand = new RelayCommand<object>(_ => _navigationService.UpdateCurrentViewType(ViewType.Teams, null));
    }

    private bool AddTeam_CanExecute(object obj)
    {
        return Name != string.Empty && OriginCity != string.Empty &&
               MembersCount > 0;
    }

    private void AddTeam(object obj)
    {
        var dto = new TeamCreateDto
        {
            Name = Name,
            OriginCity = OriginCity,
            MembersCount = MembersCount
        };

        try
        {
            _teamService.Add(dto);
            _logger.LogInformation("\"{NewTeamName}\" was successfully added", Name);
            
            _navigationService.UpdateCurrentViewType(ViewType.Teams, null);
        }
        catch (ValidationException ex)
        {
            _logger.LogInformation("Validation exception was caught: {Message}", ex.Message);
            ErrorMessage = ex.Message;
        }
    }
}
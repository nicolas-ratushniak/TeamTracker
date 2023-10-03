using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using TeamTracker.Domain.Dto;
using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Commands;
using TeamTracker.Wpf.Navigation;

namespace TeamTracker.Wpf.ViewModels;

public class TeamUpdateViewModel : BaseViewModel
{
    private readonly ITeamService _teamService;
    private readonly INavigationService _navigationService;
    private readonly ILogger<TeamUpdateViewModel> _logger;
    private readonly Guid _teamId;
    
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

    public TeamUpdateViewModel(Guid teamId, ITeamService teamService, INavigationService navigationService,
        ILogger<TeamUpdateViewModel> logger)
    {
        _teamService = teamService;
        _navigationService = navigationService;
        _logger = logger;
        _teamId = teamId;
        
        var team = _teamService.Get(teamId);

        Name = team.Name;
        OriginCity = team.OriginCity;
        MembersCount = team.MembersCount;
        
        SubmitCommand = new RelayCommand<object>(EditTeam, EditTeam_CanExecute);
        CancelCommand = new RelayCommand<object>(_ => _navigationService.UpdateCurrentViewType(ViewType.Teams, null));
    }

    private bool EditTeam_CanExecute(object obj)
    {
        return Name != string.Empty && OriginCity != string.Empty &&
               MembersCount > 0;
    }

    private void EditTeam(object obj)
    {
        var dto = new TeamUpdateDto()
        {
            Id = _teamId,
            Name = Name,
            OriginCity = OriginCity,
            MembersCount = MembersCount
        };

        try
        {
            _teamService.Update(dto);
            _logger.LogInformation("\"{TeamName}\" was successfully updated", dto.Name);

            _navigationService.UpdateCurrentViewType(ViewType.Teams, null);
        }
        catch (ValidationException ex)
        {
            _logger.LogInformation("Validation exception was caught: {Message}", ex.Message);
            ErrorMessage = ex.Message;
        }
    }
}
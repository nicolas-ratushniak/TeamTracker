using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using TeamTracker.Domain.Dto;
using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Commands;
using TeamTracker.Wpf.Navigation;

namespace TeamTracker.Wpf.ViewModels;

public class TeamUpdateFormViewModel : ViewModelBase
{
    private readonly ITeamService _teamService;
    private readonly INavigator _navigator;
    private TeamUpdateViewModel _teamToEdit;
    private string? _errorMessage;

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

    public TeamUpdateViewModel TeamToEdit
    {
        get => _teamToEdit;
        set
        {
            if (Equals(value, _teamToEdit)) return;
            _teamToEdit = value;
            OnPropertyChanged();
        }
    }

    public ICommand SubmitCommand { get; }
    public ICommand CancelCommand { get; }

    public TeamUpdateFormViewModel(Guid teamId, ITeamService teamService, INavigator navigator)
    {

        _teamService = teamService;
        _navigator = navigator;

        var team = _teamService.Get(teamId);
        
        _teamToEdit = new TeamUpdateViewModel
        {
            Id = team.Id,
            Name = team.Name,
            OriginCity = team.OriginCity,
            MembersCount = team.MembersCount
        };

        SubmitCommand = new RelayCommand<object>(EditTeam, EditTeam_CanExecute);
        CancelCommand = new RelayCommand<object>(_ => _navigator.UpdateCurrentViewType(ViewType.Teams, null));
    }

    private bool EditTeam_CanExecute(object obj)
    {
        return TeamToEdit.Name != string.Empty && TeamToEdit.OriginCity != string.Empty &&
               TeamToEdit.MembersCount > 0;
    }

    private void EditTeam(object obj)
    {
        var dto = new TeamUpdateDto()
        {
            Id = TeamToEdit.Id,
            Name = TeamToEdit.Name,
            OriginCity = TeamToEdit.OriginCity,
            MembersCount = TeamToEdit.MembersCount
        };
        
        try
        {
            _teamService.Update(dto);
            _navigator.UpdateCurrentViewType(ViewType.Teams, null);
        }
        catch (ValidationException)
        {
            ErrorMessage = "Some validation error occured";
        }
    }
}
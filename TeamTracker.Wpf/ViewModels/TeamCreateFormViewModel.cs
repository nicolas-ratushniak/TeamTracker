using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using TeamTracker.Domain.Dto;
using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Commands;
using TeamTracker.Wpf.Navigation;

namespace TeamTracker.Wpf.ViewModels;

public class TeamCreateFormViewModel : ViewModelBase
{
    private readonly ITeamService _teamService;
    private readonly INavigator _navigator;
    private TeamCreateViewModel _newTeam;
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

    public TeamCreateViewModel NewTeam
    {
        get => _newTeam;
        set
        {
            if (Equals(value, _newTeam)) return;
            _newTeam = value;
            OnPropertyChanged();
        }
    }

    public ICommand SubmitCommand { get; }
    public ICommand CancelCommand { get; }

    public TeamCreateFormViewModel(ITeamService teamService, INavigator navigator)
    {
        _teamService = teamService;
        _navigator = navigator;
        _newTeam = new TeamCreateViewModel();

        SubmitCommand = new RelayCommand<object>(AddTeam, AddTeam_CanExecute);
        CancelCommand = new RelayCommand<object>(o => _navigator.UpdateCurrentViewType(ViewType.Teams, null));
    }

    private bool AddTeam_CanExecute(object obj)
    {
        return NewTeam.Name != string.Empty && NewTeam.OriginCity != string.Empty &&
               NewTeam.MembersCount > 0;
    }

    private void AddTeam(object obj)
    {
        var dto = new TeamCreateDto
        {
            Name = NewTeam.Name,
            OriginCity = NewTeam.OriginCity,
            MembersCount = NewTeam.MembersCount
        };

        try
        {
            _teamService.Add(dto);
            _navigator.UpdateCurrentViewType(ViewType.Teams, null);
        }
        catch (ValidationException)
        {
            ErrorMessage = "Some validation error occured";
        }
    }
}
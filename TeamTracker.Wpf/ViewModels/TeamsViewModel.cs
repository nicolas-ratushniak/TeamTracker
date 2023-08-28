using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TeamTracker.Domain.Services;
using TeamTracker.Wpf.Commands;
using TeamTracker.Wpf.Navigation;

namespace TeamTracker.Wpf.ViewModels;

public class TeamsViewModel : ViewModelBase
{
    private readonly ITeamService _teamService;

    public TeamsListViewModel TeamsList { get; set; }
    public TeamDetailsViewModel SelectedTeamDetails { get; set; }
    public ICommand AddTeamCommand { get; }
    public ICommand EditTeamCommand { get; }
    public ICommand DeleteTeamCommand { get; }

    public TeamsViewModel(ITeamService teamService, INavigator navigator)
    {
        _teamService = teamService;
        var navigator1 = navigator;
        TeamsList = new TeamsListViewModel(teamService);
        SelectedTeamDetails = new TeamDetailsViewModel();
        
        TeamsList.PropertyChanged += TeamsList_OnPropertyChanged;

        AddTeamCommand = new RelayCommand<object>(o => navigator1.UpdateCurrentViewType(ViewType.TeamCreate));
        EditTeamCommand = new RelayCommand<object>(o => navigator1.UpdateCurrentViewType(ViewType.TeamUpdate, SelectedTeamDetails.Team!.Id),
            o => SelectedTeamDetails.Team is not null);
        DeleteTeamCommand = new RelayCommand<object>(DeleteTeam_CanExecute, o => SelectedTeamDetails.Team is not null);
    }

    private void DeleteTeam_CanExecute(object obj)
    {
        var messageBoxResult = MessageBox.Show("Are you sure, you want to delete this team?", "caption", MessageBoxButton.YesNo, 
            MessageBoxImage.Warning);

        if (messageBoxResult == MessageBoxResult.Yes)
        {
            _teamService.Delete(SelectedTeamDetails.Team!.Id);
            TeamsList.Refresh();
        }
    }

    private void TeamsList_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(TeamsList.SelectedTeam))
        {
            return;
        }
        
        var selectedTeam = ((TeamsListViewModel)sender!).SelectedTeam;

        if (selectedTeam is null)
        {
            SelectedTeamDetails.Team = null;
        }
        else
        {
            var team = _teamService.Get(selectedTeam.Id);

            SelectedTeamDetails.Team = new TeamDetailsItemViewModel
            {
                Id = team.Id,
                Name = team.Name,
                OriginCity = team.OriginCity,
                GamesWon = team.GamesWon,
                GamesLost = team.GamesLost,
                GamesDrawn = team.GamesDrawn,
                MembersCount = team.MembersCount,
                TotalGames = _teamService.GetTotalGames(team),
                Points = _teamService.CalculatePoints(team)
            };
        }
    }
}
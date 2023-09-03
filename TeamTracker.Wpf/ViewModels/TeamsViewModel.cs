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

    public TeamListViewModel TeamList { get; set; }
    public TeamDetailsViewModel SelectedTeamDetails { get; set; }
    public ICommand AddTeamCommand { get; }
    public ICommand EditTeamCommand { get; }
    public ICommand DeleteTeamCommand { get; }

    public TeamsViewModel(ITeamService teamService, INavigator navigator)
    {
        _teamService = teamService;
        TeamList = new TeamListViewModel(teamService);
        SelectedTeamDetails = new TeamDetailsViewModel();

        TeamList.PropertyChanged += TeamsList_OnPropertyChanged;

        AddTeamCommand = new RelayCommand<object>(
            _ => navigator.UpdateCurrentViewType(ViewType.TeamCreate, null));

        EditTeamCommand = new RelayCommand<object>(
            _ => navigator.UpdateCurrentViewType(ViewType.TeamUpdate, SelectedTeamDetails.Team!.Id),
            _ => SelectedTeamDetails.Team is not null);

        DeleteTeamCommand = new RelayCommand<object>(
            DeleteTeam_Execute,
            _ => SelectedTeamDetails.Team is not null);
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
            _teamService.Delete(SelectedTeamDetails.Team!.Id);
            TeamList.RefreshItemSource();
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
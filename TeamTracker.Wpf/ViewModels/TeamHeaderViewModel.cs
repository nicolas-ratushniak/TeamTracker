using TeamTracker.Domain.Models;

namespace TeamTracker.Wpf.ViewModels;

public class TeamHeaderViewModel : ViewModelBase
{
    public Guid Id { get; }
    public string FullName { get; }

    public TeamHeaderViewModel(Team team)
    {
        Id = team.Id;
        FullName = $"{team.Name}-{team.OriginCity}";
    }
}
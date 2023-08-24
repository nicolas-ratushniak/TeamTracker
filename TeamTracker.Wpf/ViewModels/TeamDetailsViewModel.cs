namespace TeamTracker.Wpf.ViewModels;

public class TeamDetailsViewModel : ViewModelBase
{
    private TeamDetailsItemViewModel? _team;

    public TeamDetailsItemViewModel? Team
    {
        get => _team;
        set
        {
            if (Equals(value, _team)) return;
            _team = value;
            OnPropertyChanged();
        }
    }
}
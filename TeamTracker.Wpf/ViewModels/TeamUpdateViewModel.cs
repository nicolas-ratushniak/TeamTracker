using System.Windows.Input;

namespace TeamTracker.Wpf.ViewModels;

public class TeamUpdateViewModel : ViewModelBase
{
    private string _name = string.Empty;
    private string _originCity = string.Empty;
    private int _membersCount;
    
    public Guid Id { get; init; }

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
}
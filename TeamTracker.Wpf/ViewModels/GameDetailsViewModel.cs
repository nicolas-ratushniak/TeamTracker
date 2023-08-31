namespace TeamTracker.Wpf.ViewModels;

public class GameDetailsViewModel : ViewModelBase
{
    private GameDetailsItemViewModel? _game;

    public GameDetailsItemViewModel? Game
    {
        get => _game;
        set
        {
            if (Equals(value, _game)) return;
            _game = value;
            OnPropertyChanged();
        }
    }
}
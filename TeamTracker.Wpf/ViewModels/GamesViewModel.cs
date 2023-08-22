using TeamTracker.Domain.Services;

namespace TeamTracker.Wpf.ViewModels;

public class GamesViewModel : ViewModelBase
{
    private readonly IGameInfoService _gameInfoService;

    public GamesViewModel(IGameInfoService gameInfoService)
    {
        _gameInfoService = gameInfoService;
    }
}
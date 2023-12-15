using TeamTracker.Wpf.Navigation;

namespace TeamTracker.Wpf.ViewModels.Factories;

public interface IViewModelFactory
{
    public BaseViewModel CreateViewModel(ViewType viewType, object? viewParameter = null);
}
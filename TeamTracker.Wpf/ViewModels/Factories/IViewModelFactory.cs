using TeamTracker.Wpf.Navigation;

namespace TeamTracker.Wpf.ViewModels.Factories;

public interface IViewModelFactory
{
    public ViewModelBase CreateViewModel(ViewType viewType);
}
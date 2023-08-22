using TeamTracker.Wpf.Navigation;
using TeamTracker.Wpf.ViewModels.Factories;

namespace TeamTracker.Wpf.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IViewModelFactory _viewModelFactory;
    private ViewModelBase _currentViewModel;

    public INavigator Navigator { get; }

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            if (Equals(value, _currentViewModel)) return;
            _currentViewModel = value;
            OnPropertyChanged();
        }
    }

    public MainViewModel(INavigator navigator, IViewModelFactory viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
        Navigator = navigator;
        
        Navigator.CurrentViewTypeChanged += Navigator_OnCurrentViewTypeChanged;
        Navigator.SetCurrentViewType(ViewType.Teams);
    }

    private void Navigator_OnCurrentViewTypeChanged(object? sender, EventArgs args)
    {
        var viewTypeChangedArgs = (ViewTypeChangedEventArgs)args;
        CurrentViewModel = _viewModelFactory.CreateViewModel(viewTypeChangedArgs.NewViewType);
    }
}
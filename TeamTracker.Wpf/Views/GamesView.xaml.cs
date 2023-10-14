using System.Windows;
using System.Windows.Controls;
using TeamTracker.Wpf.ViewModels;

namespace TeamTracker.Wpf.Views;

public partial class GamesView : UserControl
{
    public GamesView()
    {
        InitializeComponent();
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var viewModel = (BaseViewModel)DataContext;
        
        viewModel.LoadedCommand.Execute(null);
    }
}
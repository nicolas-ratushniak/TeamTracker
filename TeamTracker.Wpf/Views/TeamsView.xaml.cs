using System.Windows;
using System.Windows.Controls;
using TeamTracker.Wpf.ViewModels;

namespace TeamTracker.Wpf.Views;

public partial class TeamsView : UserControl
{
    public TeamsView()
    {
        InitializeComponent();
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var viewModel = (BaseViewModel)DataContext;
        
        viewModel.LoadedCommand.Execute(null);
    }
}
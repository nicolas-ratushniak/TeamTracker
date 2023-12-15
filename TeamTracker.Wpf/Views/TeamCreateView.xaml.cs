using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TeamTracker.Wpf.ViewModels;

namespace TeamTracker.Wpf.Views;

public partial class TeamCreateView : UserControl
{
    public TeamCreateView()
    {
        InitializeComponent();
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Keyboard.Focus(DefaultFocusedItem);
    }
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TeamTracker.Wpf.Views;

public partial class TeamUpdateView : UserControl
{
    public TeamUpdateView()
    {
        InitializeComponent();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Keyboard.Focus(DefaultFocusedItem);
    }
}
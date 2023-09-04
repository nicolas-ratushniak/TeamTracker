using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TeamTracker.Wpf.Views;

public partial class GameCreateView : UserControl
{
    public GameCreateView()
    {
        InitializeComponent();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Keyboard.Focus(DefaultFocusedItem);
    }
}
using System.Windows;
using TeamTracker.Wpf.ViewModels;

namespace TeamTracker.Wpf;

public partial class MainWindow : Window
{
    public MainWindow(ViewModelBase dataContextViewModel)
    {
        InitializeComponent();
        DataContext = dataContextViewModel;
    }
}
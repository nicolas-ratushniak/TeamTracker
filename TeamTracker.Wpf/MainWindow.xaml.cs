﻿using System.Windows;
using TeamTracker.Wpf.ViewModels;

namespace TeamTracker.Wpf;

public partial class MainWindow : Window
{
    public MainWindow(BaseViewModel dataContextViewModel)
    {
        InitializeComponent();
        DataContext = dataContextViewModel;
    }
}
﻿using DieLayoutDesigner.ViewModels;
using System.Windows;

namespace DieLayoutDesigner;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
    }
}
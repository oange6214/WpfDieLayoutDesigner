using System.Windows;
using DieLayoutDesigner.Managers;
using DieLayoutDesigner.ViewModels;

namespace DieLayoutDesigner;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
    }
}
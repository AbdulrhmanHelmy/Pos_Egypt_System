using System.Windows;
using HeCashier.ViewModels;

namespace HeCashier;

public partial class MainWindow : Window
{
    public MainWindow(ShellViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

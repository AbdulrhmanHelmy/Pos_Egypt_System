using System.Windows;
using System.Windows.Controls;
using HeCashier.ViewModels;

namespace HeCashier.Views;

public partial class LoginWindow : Window
{
    public LoginWindow(LoginViewModel viewModel) { InitializeComponent(); DataContext = viewModel; }
    private void PasswordChanged(object sender, RoutedEventArgs e) { if (DataContext is LoginViewModel vm && sender is PasswordBox box) vm.Password = box.Password; }
}

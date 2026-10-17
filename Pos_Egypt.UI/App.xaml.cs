using System.Windows;
using HeCashier.Data;
using HeCashier.Services;
using HeCashier.ViewModels;
using HeCashier.Views;

namespace HeCashier;

public partial class App : Application
{
    private readonly DatabaseInitializer _databaseInitializer = new();

    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        try
        {
            await _databaseInitializer.InitializeAsync();
            MainWindow = new LoginWindow(new LoginViewModel(
                new AuthenticationService(_databaseInitializer.Factory),
                OpenShell));
            MainWindow.Show();
        }
        catch (Exception exception)
        {
            MessageBox.Show("تعذر تهيئة قاعدة البيانات. يرجى مراجعة سجل الأخطاء أو التواصل مع مسؤول النظام.",
                "خطأ في بدء التشغيل", MessageBoxButton.OK, MessageBoxImage.Error);
            System.Diagnostics.Debug.WriteLine(exception);
            Shutdown(1);
        }
    }

    private void OpenShell(AuthenticatedUser user)
    {
        var shell = new MainWindow(new ShellViewModel(
            user,
            new ShiftService(_databaseInitializer.Factory)));
        MainWindow = shell;
        shell.Show();
    }
}

using System.Windows;
using System.Windows.Threading;
using System.IO;
using HeCashier.Data;
using HeCashier.Diagnostics;
using HeCashier.Services;
using HeCashier.ViewModels;
using HeCashier.Views;

namespace HeCashier;

public partial class App : Application
{
    private readonly DatabaseInitializer _databaseInitializer = new();
    private readonly FileLogger _logger;

    public App()
    {
        _logger = new FileLogger(() => _databaseInitializer.DataRoot);
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

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
            var logPath = _logger.Log(exception);
            ShowFatalError(logPath);
            Shutdown(1);
        }
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var logPath = _logger.Log(e.Exception);
        ShowFatalError(logPath);
        e.Handled = true;
        Shutdown(1);
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception ?? new Exception(e.ExceptionObject?.ToString());
        _logger.Log($"IsTerminating={e.IsTerminating}{Environment.NewLine}{exception}");
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        _logger.Log(e.Exception);
        e.SetObserved();
    }

    private static void ShowFatalError(string logPath)
    {
        MessageBox.Show($"تعذر تهيئة التطبيق. يرجى مراجعة سجل الأخطاء أو التواصل مع مسؤول النظام.\n{logPath}",
            "خطأ في بدء التشغيل", MessageBoxButton.OK, MessageBoxImage.Error);
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

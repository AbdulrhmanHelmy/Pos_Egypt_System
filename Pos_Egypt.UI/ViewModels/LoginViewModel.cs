using System.Windows;
using HeCashier.Core;
using HeCashier.Services;

namespace HeCashier.ViewModels;

public sealed class LoginViewModel : ViewModelBase
{
    private readonly AuthenticationService _authentication;
    private readonly Action<AuthenticatedUser> _authenticated;
    private string _userName = string.Empty;
    private string _password = string.Empty;
    private string _error = string.Empty;
    private bool _isBusy;
    public string UserName { get => _userName; set => SetProperty(ref _userName, value); }
    public string Password { get => _password; set => SetProperty(ref _password, value); }
    public string Error { get => _error; private set => SetProperty(ref _error, value); }
    public RelayCommand LoginCommand { get; }
    public RelayCommand ExitCommand { get; }

    public LoginViewModel(AuthenticationService authentication, Action<AuthenticatedUser> authenticated)
    {
        _authentication = authentication;
        _authenticated = authenticated;
        LoginCommand = new(async () => await LoginAsync());
        ExitCommand = new(() => Application.Current.Shutdown());
    }

    private async Task LoginAsync()
    {
        if (_isBusy) return;
        if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrEmpty(Password)) { Error = "يرجى إدخال اسم المستخدم وكلمة المرور."; return; }
        _isBusy = true; Error = string.Empty;
        try
        {
            var user = await _authentication.AuthenticateAsync(UserName.Trim(), Password);
            if (user is null) { Error = "بيانات الدخول غير صحيحة."; return; }
            _authenticated(user);
        }
        catch (Exception exception) { Error = "تعذر إتمام تسجيل الدخول. يرجى المحاولة مرة أخرى."; System.Diagnostics.Debug.WriteLine(exception); }
        finally { _isBusy = false; }
    }
}

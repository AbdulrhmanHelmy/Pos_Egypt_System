using System.Collections.ObjectModel;
using System.Windows.Threading;
using HeCashier.Core;
using HeCashier.Services;
using HeCashier.Views;

namespace HeCashier.ViewModels;

public sealed record NavigationItem(string Title, RelayCommand Command);

public sealed class ShellViewModel : ViewModelBase
{
    private readonly AuthenticatedUser _user;
    private readonly ShiftService _shiftService;
    private object _currentView = new PlaceholderView("الرئيسية", "مرحباً بك في HeCashier");
    private string _currentDateTime = string.Empty;
    private string _shiftStatus = "الوردية: غير نشطة";
    public string CurrentUserDisplay => $"المستخدم: {_user.UserName}";
    public string CurrentDateTime { get => _currentDateTime; private set => SetProperty(ref _currentDateTime, value); }
    public string ShiftStatus { get => _shiftStatus; private set => SetProperty(ref _shiftStatus, value); }
    public object CurrentView { get => _currentView; private set => SetProperty(ref _currentView, value); }
    public ObservableCollection<NavigationItem> NavigationItems { get; } = new();

    public ShellViewModel(AuthenticatedUser user, ShiftService shiftService)
    {
        _user = user; _shiftService = shiftService;
        Add("الرئيسية", "لوحة التحكم");
        Add("المبيعات", "المبيعات");
        Add("المشتريات", "المشتريات");
        Add("المنتجات والمخزون", "المنتجات والمخزون");
        Add("العملاء", "العملاء");
        Add("الموردين", "الموردين");
        Add("المرتجعات", "المرتجعات");
        Add("المصروفات والخزينة", "المصروفات والخزينة");
        Add("التقارير", "التقارير");
        Add("الموظفين", "الموظفين");
        Add("الإعدادات", "الإعدادات");
        Add("المستخدمون والصلاحيات", "المستخدمون والصلاحيات");
        NavigationItems.Add(new("بدء وردية", new RelayCommand(async () => await StartShiftAsync())));
        CurrentDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        timer.Tick += (_, _) => CurrentDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        timer.Start();
        _ = RefreshShiftAsync();
    }

    private void Add(string title, string viewTitle) => NavigationItems.Add(new(title, new RelayCommand(() => CurrentView = new PlaceholderView(viewTitle, "هذه الشاشة سيتم تنفيذها في مرحلة لاحقة"))));
    private async Task RefreshShiftAsync()
    {
        var shift = await _shiftService.GetActiveShiftAsync(_user.Id);
        ShiftStatus = shift is null ? "الوردية: غير نشطة" : $"الوردية: {shift.Number}";
    }
    private async Task StartShiftAsync()
    {
        try { var shift = await _shiftService.StartShiftAsync(_user.Id); ShiftStatus = $"الوردية: {shift.Number}"; CurrentView = new PlaceholderView("بدء الوردية", "تم بدء الوردية بنجاح."); }
        catch (Exception exception) { CurrentView = new PlaceholderView("خطأ", "تعذر بدء الوردية."); System.Diagnostics.Debug.WriteLine(exception); }
    }
}

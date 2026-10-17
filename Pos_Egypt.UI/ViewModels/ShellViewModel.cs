using System.Collections.ObjectModel;
using System.Windows.Threading;
using HeCashier.Core;
using HeCashier.Diagnostics;
using HeCashier.Services;
using HeCashier.Views;

namespace HeCashier.ViewModels;

public sealed record NavigationItem(string Title, RelayCommand Command);

public sealed class ShellViewModel : ViewModelBase
{
    private readonly AuthenticatedUser _user;
    private readonly IShiftService _shiftService;
    private readonly FileLogger _logger;
    private object _currentView = new PlaceholderView("الرئيسية", "مرحباً بك في HeCashier");
    private string _currentDateTime = string.Empty;
    private string _shiftStatus = "الوردية: غير نشطة";
    public string CurrentUserDisplay => $"المستخدم: {_user.UserName}";
    public string CurrentDateTime { get => _currentDateTime; private set => SetProperty(ref _currentDateTime, value); }
    public string ShiftStatus { get => _shiftStatus; private set => SetProperty(ref _shiftStatus, value); }
    public object CurrentView { get => _currentView; private set => SetProperty(ref _currentView, value); }
    public ObservableCollection<NavigationItem> NavigationItems { get; } = new();

    public ShellViewModel(
        AuthenticatedUser user,
        IShiftService shiftService,
        FileLogger logger,
        IProductService productService,
        IInventoryService inventoryService,
        ICategoryAndUnitService lookups,
        ISystemSettingsService settings)
    {
        _user = user; _shiftService = shiftService; _logger = logger;
        Add("الرئيسية", "لوحة التحكم");
        Add("المبيعات", "المبيعات");
        Add("المشتريات", "المشتريات");
        NavigationItems.Add(new("المنتجات والمخزون", new RelayCommand(() =>
            CurrentView = new InventoryView(new InventoryViewModel(productService, inventoryService, lookups, user.Id)))));
        NavigationItems.Add(new("التعديل السريع للأسعار والكمية", new RelayCommand(() =>
            CurrentView = new QuickProductEditView(new QuickProductEditViewModel(productService, user.Id)))));
        NavigationItems.Add(new("الأقسام والوحدات والإعدادات", new RelayCommand(() =>
            CurrentView = new SubSectionsView(new SubSectionsViewModel(lookups, settings)))));
        Add("العملاء", "العملاء");
        Add("الموردين", "الموردين");
        Add("المرتجعات", "المرتجعات");
        Add("المصروفات والخزينة", "المصروفات والخزينة");
        Add("التقارير", "التقارير");
        Add("الموظفين", "الموظفين");
        NavigationItems.Add(new("الإعدادات", new RelayCommand(() =>
            CurrentView = new SubSectionsView(new SubSectionsViewModel(lookups, settings)))));
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
        catch (Exception exception) { CurrentView = new PlaceholderView("خطأ", "تعذر بدء الوردية."); _logger.Log(exception); }
    }
}

using System.Collections.ObjectModel;
using HeCashier.Core;
using HeCashier.Models;
using HeCashier.Services;

namespace HeCashier.ViewModels;

public sealed class SubSectionsViewModel : ViewModelBase
{
    private readonly ICategoryAndUnitService _lookups;
    private readonly ISystemSettingsService _settings;
    private string _categoryName = string.Empty;
    private string _unitName = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _allowNegativeStock;
    private Category? _selectedCategory;
    private Unit? _selectedUnit;

    public ObservableCollection<Category> Categories { get; } = new();
    public ObservableCollection<Unit> Units { get; } = new();
    public string CategoryName { get => _categoryName; set => SetProperty(ref _categoryName, value); }
    public string UnitName { get => _unitName; set => SetProperty(ref _unitName, value); }
    public string StatusMessage { get => _statusMessage; private set => SetProperty(ref _statusMessage, value); }
    public bool AllowNegativeStock { get => _allowNegativeStock; set => SetProperty(ref _allowNegativeStock, value); }
    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value) && value is not null)
                CategoryName = value.Name;
        }
    }
    public Unit? SelectedUnit
    {
        get => _selectedUnit;
        set
        {
            if (SetProperty(ref _selectedUnit, value) && value is not null)
                UnitName = value.Name;
        }
    }

    public RelayCommand SaveCategoryCommand { get; }
    public RelayCommand DeleteCategoryCommand { get; }
    public RelayCommand SaveUnitCommand { get; }
    public RelayCommand DeleteUnitCommand { get; }
    public RelayCommand SaveSettingsCommand { get; }

    public SubSectionsViewModel(ICategoryAndUnitService lookups, ISystemSettingsService settings)
    {
        _lookups = lookups;
        _settings = settings;
        SaveCategoryCommand = new RelayCommand(async () => await SaveCategoryAsync());
        DeleteCategoryCommand = new RelayCommand(async () => await DeleteCategoryAsync());
        SaveUnitCommand = new RelayCommand(async () => await SaveUnitAsync());
        DeleteUnitCommand = new RelayCommand(async () => await DeleteUnitAsync());
        SaveSettingsCommand = new RelayCommand(async () => await SaveSettingsAsync());
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            await LoadLookupsAsync();
            AllowNegativeStock = await _settings.GetAllowNegativeStockAsync();
        }
        catch (Exception exception)
        {
            StatusMessage = exception.Message;
        }
    }

    private async Task LoadLookupsAsync()
    {
        Categories.Clear();
        foreach (var category in await _lookups.GetCategoriesAsync()) Categories.Add(category);
        Units.Clear();
        foreach (var unit in await _lookups.GetUnitsAsync()) Units.Add(unit);
    }

    private async Task SaveCategoryAsync()
    {
        try
        {
            if (SelectedCategory is null)
                await _lookups.AddCategoryAsync(CategoryName);
            else
                await _lookups.UpdateCategoryAsync(SelectedCategory.Id, CategoryName);
            SelectedCategory = null;
            CategoryName = string.Empty;
            await LoadLookupsAsync();
            StatusMessage = "تم حفظ القسم";
        }
        catch (Exception exception)
        {
            StatusMessage = exception.Message;
        }
    }

    private async Task DeleteCategoryAsync()
    {
        try
        {
            if (SelectedCategory is null)
                throw new InvalidOperationException("اختر القسم المراد حذفه.");
            await _lookups.DeleteCategoryAsync(SelectedCategory.Id);
            SelectedCategory = null;
            CategoryName = string.Empty;
            await LoadLookupsAsync();
            StatusMessage = "تم حذف القسم";
        }
        catch (Exception exception)
        {
            StatusMessage = exception.Message;
        }
    }

    private async Task SaveUnitAsync()
    {
        try
        {
            if (SelectedUnit is null)
                await _lookups.AddUnitAsync(UnitName);
            else
                await _lookups.UpdateUnitAsync(SelectedUnit.Id, UnitName);
            SelectedUnit = null;
            UnitName = string.Empty;
            await LoadLookupsAsync();
            StatusMessage = "تم حفظ الوحدة";
        }
        catch (Exception exception)
        {
            StatusMessage = exception.Message;
        }
    }

    private async Task DeleteUnitAsync()
    {
        try
        {
            if (SelectedUnit is null)
                throw new InvalidOperationException("اختر الوحدة المراد حذفها.");
            await _lookups.DeleteUnitAsync(SelectedUnit.Id);
            SelectedUnit = null;
            UnitName = string.Empty;
            await LoadLookupsAsync();
            StatusMessage = "تم حذف الوحدة";
        }
        catch (Exception exception)
        {
            StatusMessage = exception.Message;
        }
    }

    private async Task SaveSettingsAsync()
    {
        try
        {
            await _settings.SetAllowNegativeStockAsync(AllowNegativeStock);
            StatusMessage = "تم حفظ إعدادات المخزون";
        }
        catch (Exception exception)
        {
            StatusMessage = exception.Message;
        }
    }
}

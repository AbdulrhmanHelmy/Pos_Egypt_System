using System.Collections.ObjectModel;
using System.Globalization;
using HeCashier.Core;
using HeCashier.Models;
using HeCashier.Services;

namespace HeCashier.ViewModels;

public sealed class InventoryViewModel : ViewModelBase
{
    private readonly IProductService _productService;
    private readonly IInventoryService _inventoryService;
    private readonly ICategoryAndUnitService _lookups;
    private readonly int _userId;
    private string _statusMessage = string.Empty;
    private string _filterText = string.Empty;
    private string _productName = string.Empty;
    private string _productCode = string.Empty;
    private string _primaryBarcode = string.Empty;
    private string _secondaryBarcodes = string.Empty;
    private string _purchasePrice = "0";
    private string _costPrice = "0";
    private string _sellingPrice = "0";
    private string _quantity = "0";
    private string _minimumStock = "0";
    private string _adjustmentQuantity = string.Empty;
    private string _adjustmentNotes = string.Empty;
    private string _damagedQuantity = string.Empty;
    private string _damagedReason = string.Empty;
    private bool _isActive = true;
    private bool _lowStockOnly;
    private Product? _selectedProduct;
    private Category? _selectedCategory;
    private Category? _selectedCategoryFilter;
    private Unit? _selectedUnit;
    private Store? _selectedStore;

    public ObservableCollection<Product> Products { get; } = new();
    public ObservableCollection<Category> Categories { get; } = new();
    public ObservableCollection<Unit> Units { get; } = new();
    public ObservableCollection<Store> Stores { get; } = new();
    public RelayCommand SearchCommand { get; }
    public RelayCommand NewProductCommand { get; }
    public RelayCommand SaveProductCommand { get; }
    public RelayCommand GenerateBarcodeCommand { get; }
    public RelayCommand AdjustStockCommand { get; }
    public RelayCommand RemoveDamagedStockCommand { get; }

    public string StatusMessage { get => _statusMessage; private set => SetProperty(ref _statusMessage, value); }
    public string FilterText { get => _filterText; set => SetProperty(ref _filterText, value); }
    public string ProductName { get => _productName; set => SetProperty(ref _productName, value); }
    public string ProductCode { get => _productCode; set => SetProperty(ref _productCode, value); }
    public string PrimaryBarcode { get => _primaryBarcode; set => SetProperty(ref _primaryBarcode, value); }
    public string SecondaryBarcodes { get => _secondaryBarcodes; set => SetProperty(ref _secondaryBarcodes, value); }
    public string PurchasePrice { get => _purchasePrice; set => SetProperty(ref _purchasePrice, value); }
    public string CostPrice { get => _costPrice; set => SetProperty(ref _costPrice, value); }
    public string SellingPrice { get => _sellingPrice; set => SetProperty(ref _sellingPrice, value); }
    public string Quantity { get => _quantity; set => SetProperty(ref _quantity, value); }
    public string MinimumStock { get => _minimumStock; set => SetProperty(ref _minimumStock, value); }
    public string AdjustmentQuantity { get => _adjustmentQuantity; set => SetProperty(ref _adjustmentQuantity, value); }
    public string AdjustmentNotes { get => _adjustmentNotes; set => SetProperty(ref _adjustmentNotes, value); }
    public string DamagedQuantity { get => _damagedQuantity; set => SetProperty(ref _damagedQuantity, value); }
    public string DamagedReason { get => _damagedReason; set => SetProperty(ref _damagedReason, value); }
    public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }
    public bool LowStockOnly { get => _lowStockOnly; set => SetProperty(ref _lowStockOnly, value); }
    public Product? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            if (!SetProperty(ref _selectedProduct, value) || value is null)
                return;
            ProductName = value.Name;
            ProductCode = value.Code;
            PrimaryBarcode = value.Barcodes.FirstOrDefault(x => x.IsPrimary)?.Value ?? string.Empty;
            SecondaryBarcodes = string.Join(", ", value.Barcodes.Where(x => !x.IsPrimary).Select(x => x.Value));
            PurchasePrice = value.PurchasePrice.ToString(CultureInfo.CurrentCulture);
            CostPrice = value.CostPrice.ToString(CultureInfo.CurrentCulture);
            SellingPrice = value.SellingPrice.ToString(CultureInfo.CurrentCulture);
            Quantity = value.Quantity.ToString(CultureInfo.CurrentCulture);
            MinimumStock = value.ReorderPoint.ToString(CultureInfo.CurrentCulture);
            IsActive = value.IsActive;
            SelectedCategory = Categories.FirstOrDefault(x => x.Id == value.CategoryId);
            SelectedUnit = Units.FirstOrDefault(x => x.Id == value.UnitId);
        }
    }
    public Category? SelectedCategory { get => _selectedCategory; set => SetProperty(ref _selectedCategory, value); }
    public Category? SelectedCategoryFilter { get => _selectedCategoryFilter; set => SetProperty(ref _selectedCategoryFilter, value); }
    public Unit? SelectedUnit { get => _selectedUnit; set => SetProperty(ref _selectedUnit, value); }
    public Store? SelectedStore { get => _selectedStore; set => SetProperty(ref _selectedStore, value); }
    public int TotalProductCount => Products.Count;
    public decimal TotalCostValue => Products.Sum(x => x.CostPrice * x.Quantity);
    public decimal TotalSellingValue => Products.Sum(x => x.SellingPrice * x.Quantity);

    public InventoryViewModel(IProductService productService, IInventoryService inventoryService, ICategoryAndUnitService lookups, int userId)
    {
        _productService = productService;
        _inventoryService = inventoryService;
        _lookups = lookups;
        _userId = userId;
        SearchCommand = new RelayCommand(async () => await LoadProductsAsync());
        NewProductCommand = new RelayCommand(ClearForm);
        SaveProductCommand = new RelayCommand(async () => await SaveProductAsync());
        GenerateBarcodeCommand = new RelayCommand(async () => await GenerateBarcodeAsync());
        AdjustStockCommand = new RelayCommand(async () => await AdjustStockAsync());
        RemoveDamagedStockCommand = new RelayCommand(async () => await RemoveDamagedStockAsync());
        Products.CollectionChanged += (_, _) => RaiseSummaries();
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            Categories.Clear();
            foreach (var item in await _lookups.GetCategoriesAsync()) Categories.Add(item);
            Units.Clear();
            foreach (var item in await _lookups.GetUnitsAsync()) Units.Add(item);
            Stores.Clear();
            foreach (var item in await _inventoryService.GetStoresAsync()) Stores.Add(item);
            SelectedStore = Stores.FirstOrDefault();
            await LoadProductsAsync();
        }
        catch (Exception exception)
        {
            StatusMessage = exception.Message;
        }
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            var products = await _productService.SearchAsync(new ProductSearchFilter
            {
                SearchText = FilterText,
                CategoryId = SelectedCategoryFilter?.Id,
                LowStockOnly = LowStockOnly
            });
            Products.Clear();
            foreach (var product in products) Products.Add(product);
            RaiseSummaries();
            StatusMessage = $"عدد الأصناف: {Products.Count}";
        }
        catch (Exception exception)
        {
            StatusMessage = exception.Message;
        }
    }

    private async Task SaveProductAsync()
    {
        try
        {
            if (!TryParse(PurchasePrice, out var purchasePrice) || !TryParse(CostPrice, out var costPrice) || !TryParse(SellingPrice, out var sellingPrice) ||
                !TryParse(Quantity, out var quantity) || !TryParse(MinimumStock, out var minimumStock))
                throw new InvalidOperationException("تحقق من إدخال الأسعار والكميات بشكل صحيح.");
            var draft = new ProductDraft
            {
                Code = ProductCode,
                Name = ProductName,
                CategoryId = SelectedCategory?.Id,
                UnitId = SelectedUnit?.Id,
                PurchasePrice = purchasePrice,
                CostPrice = costPrice,
                SellingPrice = sellingPrice,
                Quantity = quantity,
                ReorderPoint = minimumStock,
                IsActive = IsActive,
                PrimaryBarcode = PrimaryBarcode,
                AdditionalBarcodes = SecondaryBarcodes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            };
            var saved = SelectedProduct is null
                ? await _productService.CreateAsync(draft, _userId)
                : await _productService.UpdateAsync(SelectedProduct.Id, draft, _userId);
            StatusMessage = $"تم حفظ الصنف {saved.Name}";
            ClearForm();
            await LoadProductsAsync();
        }
        catch (Exception exception)
        {
            StatusMessage = exception.Message;
        }
    }

    private async Task GenerateBarcodeAsync()
    {
        try
        {
            PrimaryBarcode = await _productService.GenerateBarcodeAsync();
            StatusMessage = "تم إنشاء باركود جديد";
        }
        catch (Exception exception)
        {
            StatusMessage = exception.Message;
        }
    }

    private async Task AdjustStockAsync()
    {
        try
        {
            if (SelectedProduct is null || SelectedStore is null || !TryParse(AdjustmentQuantity, out var delta) || delta == 0)
                throw new InvalidOperationException("اختر الصنف والمخزن وأدخل فرق كمية غير صفري.");
            await _inventoryService.AdjustStockAsync(SelectedProduct.Id, SelectedStore.Id, delta, InventoryMovementType.ManualAdjustment, AdjustmentNotes, _userId);
            StatusMessage = "تم تسجيل تسوية المخزون";
            AdjustmentQuantity = string.Empty;
            AdjustmentNotes = string.Empty;
            await LoadProductsAsync();
        }
        catch (Exception exception)
        {
            StatusMessage = exception.Message;
        }
    }

    private async Task RemoveDamagedStockAsync()
    {
        try
        {
            if (SelectedProduct is null || SelectedStore is null || !TryParse(DamagedQuantity, out var quantity))
                throw new InvalidOperationException("اختر الصنف والمخزن وأدخل كمية صحيحة.");
            await _inventoryService.RemoveDamagedStockAsync(SelectedProduct.Id, SelectedStore.Id, quantity, DamagedReason, _userId);
            StatusMessage = "تم تسجيل إخراج التالف";
            DamagedQuantity = string.Empty;
            DamagedReason = string.Empty;
            await LoadProductsAsync();
        }
        catch (Exception exception)
        {
            StatusMessage = exception.Message;
        }
    }

    private void ClearForm()
    {
        _selectedProduct = null;
        OnPropertyChanged(nameof(SelectedProduct));
        ProductName = string.Empty;
        ProductCode = string.Empty;
        PrimaryBarcode = string.Empty;
        SecondaryBarcodes = string.Empty;
        PurchasePrice = "0";
        CostPrice = "0";
        SellingPrice = "0";
        Quantity = "0";
        MinimumStock = "0";
        IsActive = true;
        SelectedCategory = null;
        SelectedUnit = null;
    }

    private void RaiseSummaries()
    {
        OnPropertyChanged(nameof(TotalProductCount));
        OnPropertyChanged(nameof(TotalCostValue));
        OnPropertyChanged(nameof(TotalSellingValue));
    }

    private static bool TryParse(string value, out decimal number) =>
        decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out number) ||
        decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out number);
}

using System.Globalization;
using HeCashier.Core;
using HeCashier.Models;
using HeCashier.Services;

namespace HeCashier.ViewModels;

public sealed class QuickProductEditViewModel : ViewModelBase
{
    private readonly IProductService _products;
    private readonly int _userId;
    private string _lookupText = string.Empty;
    private string _purchasePrice = string.Empty;
    private string _sellingPrice = string.Empty;
    private string _quantity = string.Empty;
    private string _barcode = string.Empty;
    private bool _isActive;
    private string _statusMessage = string.Empty;
    private Product? _product;

    public string LookupText { get => _lookupText; set => SetProperty(ref _lookupText, value); }
    public string PurchasePrice { get => _purchasePrice; set => SetProperty(ref _purchasePrice, value); }
    public string SellingPrice { get => _sellingPrice; set => SetProperty(ref _sellingPrice, value); }
    public string Quantity { get => _quantity; set => SetProperty(ref _quantity, value); }
    public string Barcode { get => _barcode; set => SetProperty(ref _barcode, value); }
    public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }
    public string StatusMessage { get => _statusMessage; private set => SetProperty(ref _statusMessage, value); }
    public Product? Product { get => _product; private set => SetProperty(ref _product, value); }
    public RelayCommand SearchCommand { get; }
    public RelayCommand SaveCommand { get; }

    public QuickProductEditViewModel(IProductService products, int userId)
    {
        _products = products;
        _userId = userId;
        SearchCommand = new RelayCommand(async () => await SearchAsync());
        SaveCommand = new RelayCommand(async () => await SaveAsync());
    }

    private async Task SearchAsync()
    {
        try
        {
            Product = await _products.FindByBarcodeOrNameAsync(LookupText);
            if (Product is null)
            {
                StatusMessage = "لم يتم العثور على الصنف";
                return;
            }
            PurchasePrice = Product.PurchasePrice.ToString(CultureInfo.CurrentCulture);
            SellingPrice = Product.SellingPrice.ToString(CultureInfo.CurrentCulture);
            Quantity = Product.Quantity.ToString(CultureInfo.CurrentCulture);
            Barcode = Product.Barcodes.FirstOrDefault(x => x.IsPrimary)?.Value ?? string.Empty;
            IsActive = Product.IsActive;
            StatusMessage = $"تم تحميل الصنف: {Product.Name}";
        }
        catch (Exception exception)
        {
            StatusMessage = exception.Message;
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            if (Product is null)
                throw new InvalidOperationException("ابحث عن صنف أولاً.");
            if (!TryParse(PurchasePrice, out var purchase) || !TryParse(SellingPrice, out var selling) || !TryParse(Quantity, out var quantity))
                throw new InvalidOperationException("تحقق من إدخال الأسعار والكمية بشكل صحيح.");
            Product = await _products.QuickUpdateAsync(new QuickProductUpdate
            {
                ProductId = Product.Id,
                PurchasePrice = purchase,
                SellingPrice = selling,
                Quantity = quantity,
                PrimaryBarcode = Barcode,
                IsActive = IsActive
            }, _userId);
            StatusMessage = "تم حفظ تعديلات الصنف فوراً";
        }
        catch (Exception exception)
        {
            StatusMessage = exception.Message;
        }
    }

    private static bool TryParse(string value, out decimal number) =>
        decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out number) ||
        decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out number);
}

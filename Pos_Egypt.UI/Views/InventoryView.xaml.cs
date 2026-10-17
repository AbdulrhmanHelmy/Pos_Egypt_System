using System.Windows.Controls;
using HeCashier.ViewModels;

namespace HeCashier.Views;

public partial class InventoryView : UserControl
{
    public InventoryView(InventoryViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

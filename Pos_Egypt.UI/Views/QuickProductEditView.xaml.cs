using System.Windows.Controls;
using HeCashier.ViewModels;

namespace HeCashier.Views;

public partial class QuickProductEditView : UserControl
{
    public QuickProductEditView(QuickProductEditViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

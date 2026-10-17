using System.Windows.Controls;
using HeCashier.ViewModels;

namespace HeCashier.Views;

public partial class SubSectionsView : UserControl
{
    public SubSectionsView(SubSectionsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

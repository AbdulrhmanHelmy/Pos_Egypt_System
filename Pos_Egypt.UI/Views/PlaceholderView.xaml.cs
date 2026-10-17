using System.Windows.Controls;

namespace HeCashier.Views;

public partial class PlaceholderView : UserControl
{
    public string Title { get; } public string Message { get; }
    public PlaceholderView(string title, string message) { Title = title; Message = message; InitializeComponent(); DataContext = this; }
}

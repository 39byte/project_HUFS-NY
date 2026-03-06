using Microsoft.UI.Xaml.Controls;
using NuriyeApp.ViewModels;

namespace NuriyeApp.Views
{
    public sealed partial class InventoryPage : Page
    {
        public InventoryViewModel ViewModel { get; } = new InventoryViewModel();

        public InventoryPage()
        {
            InitializeComponent();
        }
    }
}

using Microsoft.UI.Xaml.Controls;
using NuriyeApp.ViewModels;

namespace NuriyeApp.Views
{
    public sealed partial class RentalFormPage : Page
    {
        public RentalFormViewModel ViewModel { get; } = new RentalFormViewModel();

        public RentalFormPage()
        {
            InitializeComponent();
            _ = ViewModel.LoadInventoryAsync();
        }
    }
}

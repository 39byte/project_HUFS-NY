using Microsoft.UI.Xaml.Controls;
using NuriyeApp.ViewModels;

namespace NuriyeApp.Views
{
    public sealed partial class PendingPage : Page
    {
        public PendingViewModel ViewModel { get; } = new PendingViewModel();

        public PendingPage()
        {
            InitializeComponent();
        }
    }
}

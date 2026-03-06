using Microsoft.UI.Xaml.Controls;
using NuriyeApp.ViewModels;

namespace NuriyeApp.Views
{
    public sealed partial class HistoryPage : Page
    {
        public HistoryViewModel ViewModel { get; } = new HistoryViewModel();

        public HistoryPage()
        {
            InitializeComponent();
        }
    }
}

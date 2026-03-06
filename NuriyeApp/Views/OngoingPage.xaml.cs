using Microsoft.UI.Xaml.Controls;
using NuriyeApp.ViewModels;

namespace NuriyeApp.Views
{
    public sealed partial class OngoingPage : Page
    {
        public OngoingViewModel ViewModel { get; } = new OngoingViewModel();

        public OngoingPage()
        {
            InitializeComponent();
        }
    }
}

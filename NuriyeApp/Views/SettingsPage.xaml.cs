using Microsoft.UI.Xaml.Controls;
using NuriyeApp.ViewModels;

namespace NuriyeApp.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; } = new SettingsViewModel();

        public SettingsPage()
        {
            InitializeComponent();
        }
    }
}

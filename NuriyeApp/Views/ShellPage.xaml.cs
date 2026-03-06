using Microsoft.UI.Xaml.Controls;
using NuriyeApp.Services;

namespace NuriyeApp.Views
{
    public sealed partial class ShellPage : Page
    {
        public ShellPage()
        {
            InitializeComponent();
            ContentFrame.Navigate(typeof(CalendarPage));
            NavView.SelectedItem = NavView.MenuItems[0];
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is not NavigationViewItem item) return;

            switch (item.Tag?.ToString())
            {
                case "calendar":
                    ContentFrame.Navigate(typeof(CalendarPage));
                    break;
                case "form":
                    ContentFrame.Navigate(typeof(RentalFormPage));
                    break;
                case "admin":
                    ContentFrame.Navigate(typeof(MainPage));
                    break;
            }
        }

        private void Logout_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            AuthService.Instance.Logout();
            MainWindow.RootFrame?.Navigate(typeof(LoginPage));
        }
    }
}

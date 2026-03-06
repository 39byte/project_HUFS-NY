using Microsoft.UI.Xaml.Controls;
using NuriyeApp.Services;

namespace NuriyeApp.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            // Load first tab
            PendingPageControl.ViewModel.LoadCommand.Execute(null);

            // Subscribe to realtime new rentals
            _ = SupabaseService.Instance.SubscribeToNewRentalsAsync(rental =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    NotificationService.Instance.SendNewRentalNotification(
                        rental.Applicant, rental.Equipment, rental.StartDate);
                });
            });
        }

        private void LogoutButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            AuthService.Instance.Logout();
            MainWindow.RootFrame?.Navigate(typeof(LoginPage));
        }

        private void MainTabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (MainTabView.SelectedIndex)
            {
                case 0:
                    PendingPageControl.ViewModel.LoadCommand.Execute(null);
                    break;
                case 1:
                    OngoingPageControl.ViewModel.LoadCommand.Execute(null);
                    break;
                case 2:
                    HistoryPageControl.ViewModel.LoadCommand.Execute(null);
                    break;
                case 3:
                    InventoryPageControl.ViewModel.LoadCommand.Execute(null);
                    break;
            }
        }
    }
}

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NuriyeApp.Services;
using System.Collections.Specialized;

namespace NuriyeApp.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            // Load first tab
            PendingPageControl.ViewModel.LoadCommand.Execute(null);

            // Subscribe to badge updates
            PendingPageControl.ViewModel.Rentals.CollectionChanged += OnPendingRentalsChanged;

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

        private void OnPendingRentalsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePendingBadge();
        }

        private void UpdatePendingBadge()
        {
            var count = PendingPageControl.ViewModel.Rentals.Count;
            if (count > 0)
            {
                PendingBadge.Visibility = Visibility.Visible;
                PendingBadgeText.Text = count.ToString();
            }
            else
            {
                PendingBadge.Visibility = Visibility.Collapsed;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
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

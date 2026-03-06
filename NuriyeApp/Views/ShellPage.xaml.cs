using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NuriyeApp.Services;
using System;
using System.Threading.Tasks;

namespace NuriyeApp.Views
{
    public sealed partial class ShellPage : Page
    {
        private NavigationViewItem? _lastSelectedItem;
        private bool _isReverting = false;

        public ShellPage()
        {
            InitializeComponent();
            ContentFrame.Navigate(typeof(CalendarPage));
            NavView.SelectedItem = NavView.MenuItems[0];
            _lastSelectedItem = NavView.MenuItems[0] as NavigationViewItem;
        }

        private async void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (_isReverting) return;
            if (args.SelectedItem is not NavigationViewItem item) return;

            switch (item.Tag?.ToString())
            {
                case "calendar":
                    _lastSelectedItem = item;
                    ContentFrame.Navigate(typeof(CalendarPage));
                    break;
                case "form":
                    _lastSelectedItem = item;
                    ContentFrame.Navigate(typeof(RentalFormPage));
                    break;
                case "admin":
                    if (!AuthService.Instance.IsLoggedIn)
                    {
                        bool success = await ShowPasswordDialogAsync();
                        if (!success)
                        {
                            _isReverting = true;
                            NavView.SelectedItem = _lastSelectedItem;
                            _isReverting = false;
                            return;
                        }
                        LogoutButton.Visibility = Visibility.Visible;
                    }
                    _lastSelectedItem = item;
                    ContentFrame.Navigate(typeof(MainPage));
                    break;
            }
        }

        private async Task<bool> ShowPasswordDialogAsync()
        {
            var passwordBox = new PasswordBox { PlaceholderText = "비밀번호를 입력하세요" };
            var dialog = new ContentDialog
            {
                Title = "관리자 인증",
                Content = passwordBox,
                PrimaryButtonText = "확인",
                CloseButtonText = "취소",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                return await AuthService.Instance.LoginAsync(passwordBox.Password);

            return false;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            AuthService.Instance.Logout();
            LogoutButton.Visibility = Visibility.Collapsed;
            _lastSelectedItem = NavView.MenuItems[0] as NavigationViewItem;
            NavView.SelectedItem = _lastSelectedItem;
            ContentFrame.Navigate(typeof(CalendarPage));
        }
    }
}

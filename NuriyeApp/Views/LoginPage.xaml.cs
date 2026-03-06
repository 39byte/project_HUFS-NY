using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using NuriyeApp.ViewModels;
using Windows.System;

namespace NuriyeApp.Views
{
    public sealed partial class LoginPage : Page
    {
        public LoginViewModel ViewModel { get; } = new LoginViewModel();

        public LoginPage()
        {
            InitializeComponent();
            ViewModel.LoginSucceeded += () =>
            {
                MainWindow.RootFrame?.Navigate(typeof(ShellPage));
            };
        }

        private void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                ViewModel.LoginCommand.Execute(null);
        }
    }
}

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using NuriyeApp.Views;

namespace NuriyeApp
{
    public sealed partial class MainWindow : Window
    {
        public static Frame? RootFrame { get; private set; }

        public MainWindow()
        {
            SystemBackdrop = new MicaBackdrop();
            Title = "누리예 카메라 대여 관리";

            RootFrame = new Frame();
            Content = RootFrame;
            RootFrame.Navigate(typeof(LoginPage));
        }
    }
}

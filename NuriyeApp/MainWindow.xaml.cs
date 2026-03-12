using Microsoft.UI;
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
            Title = "\U0001F4F8 \uB204\uB9AC\uC608 \uCE74\uBA54\uB77C \uB300\uC5EC \uAD00\uB9AC";

            // Custom title bar
            ExtendsContentIntoTitleBar = true;

            var rootGrid = new Grid();
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) });
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Title bar
            var titleBar = new Grid();
            if (Application.Current.Resources.TryGetValue("NuriyeBrandBrush", out var brandBrush))
                titleBar.Background = (Brush)brandBrush;

            var titleText = new TextBlock
            {
                Text = "\U0001F4F8 \uB204\uB9AC\uC608 \uCE74\uBA54\uB77C \uB300\uC5EC \uAD00\uB9AC",
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(16, 0, 0, 0)
            };
            if (Application.Current.Resources.TryGetValue("NuriyeTitleBarTextBrush", out var titleBrush))
                titleText.Foreground = (Brush)titleBrush;

            titleBar.Children.Add(titleText);
            Grid.SetRow(titleBar, 0);
            rootGrid.Children.Add(titleBar);

            SetTitleBar(titleBar);

            // Main frame
            RootFrame = new Frame();
            Grid.SetRow(RootFrame, 1);
            rootGrid.Children.Add(RootFrame);

            Content = rootGrid;
            RootFrame.Navigate(typeof(ShellPage));
        }
    }
}

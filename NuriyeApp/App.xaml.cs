using Microsoft.UI.Xaml;
using NuriyeApp.Services;
using Windows.Globalization;

namespace NuriyeApp
{
    public partial class App : Application
    {
        private Window? _window;

        public App()
        {
            InitializeComponent();
            ApplicationLanguages.PrimaryLanguageOverride = "ko-KR";
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            NotificationService.Instance.Register();

            _window = new MainWindow();
            _window.Activate();
        }
    }
}

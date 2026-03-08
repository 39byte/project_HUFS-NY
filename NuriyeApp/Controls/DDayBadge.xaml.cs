using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace NuriyeApp.Controls
{
    public enum DDaySeverity
    {
        Normal,
        Warning,
        Critical
    }

    public sealed partial class DDayBadge : UserControl
    {
        public static readonly DependencyProperty DDayTextProperty =
            DependencyProperty.Register(nameof(DDayText), typeof(string), typeof(DDayBadge),
                new PropertyMetadata("", OnDDayTextChanged));

        public static readonly DependencyProperty SeverityProperty =
            DependencyProperty.Register(nameof(Severity), typeof(DDaySeverity), typeof(DDayBadge),
                new PropertyMetadata(DDaySeverity.Normal, OnSeverityChanged));

        public string DDayText
        {
            get => (string)GetValue(DDayTextProperty);
            set => SetValue(DDayTextProperty, value);
        }

        public DDaySeverity Severity
        {
            get => (DDaySeverity)GetValue(SeverityProperty);
            set => SetValue(SeverityProperty, value);
        }

        public DDayBadge()
        {
            InitializeComponent();
            ApplyColors();
        }

        private static void OnDDayTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var badge = (DDayBadge)d;
            var text = (string)e.NewValue;
            badge.BadgeText.Text = text;

            // Auto-determine severity from DDayText
            if (string.IsNullOrEmpty(text))
            {
                badge.Severity = DDaySeverity.Normal;
            }
            else if (text.Contains("\uc5f0\uccb4") || text == "D-Day")
            {
                badge.Severity = DDaySeverity.Critical;
            }
            else if (text == "D-1" || text == "D-2")
            {
                badge.Severity = DDaySeverity.Warning;
            }
            else
            {
                badge.Severity = DDaySeverity.Normal;
            }
        }

        private static void OnSeverityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DDayBadge)d).ApplyColors();
        }

        private void ApplyColors()
        {
            string bgKey, fgKey;
            switch (Severity)
            {
                case DDaySeverity.Warning:
                    bgKey = "NuriyeDDayWarningBgBrush";
                    fgKey = "NuriyeDDayWarningFgBrush";
                    break;
                case DDaySeverity.Critical:
                    bgKey = "NuriyeDDayCriticalBgBrush";
                    fgKey = "NuriyeDDayCriticalFgBrush";
                    break;
                default:
                    bgKey = "NuriyeDDayNormalBgBrush";
                    fgKey = "NuriyeDDayNormalFgBrush";
                    break;
            }

            if (Application.Current.Resources.TryGetValue(bgKey, out var bgObj) && bgObj is SolidColorBrush bgBrush)
                BadgeBorder.Background = bgBrush;

            if (Application.Current.Resources.TryGetValue(fgKey, out var fgObj) && fgObj is SolidColorBrush fgBrush)
                BadgeText.Foreground = fgBrush;
        }
    }
}

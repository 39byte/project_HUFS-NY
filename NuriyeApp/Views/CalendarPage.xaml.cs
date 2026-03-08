using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using NuriyeApp.Models;
using NuriyeApp.ViewModels;
using System;

namespace NuriyeApp.Views
{
    public sealed partial class CalendarPage : Page
    {
        public CalendarViewModel ViewModel { get; } = new CalendarViewModel();

        public CalendarPage()
        {
            InitializeComponent();
            BuildDayHeaders();
            ViewModel.CalendarRebuilt += () => DispatcherQueue.TryEnqueue(BuildCalendarGrid);
            _ = ViewModel.LoadAsync();
        }

        private void BuildDayHeaders()
        {
            string[] names = { "일", "월", "화", "수", "목", "금", "토" };
            for (int i = 0; i < 7; i++)
            {
                DayHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                var tb = new TextBlock
                {
                    Text = names[i],
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontWeight = FontWeights.SemiBold,
                    Padding = new Thickness(4)
                };
                if (i == 0) tb.Foreground = new SolidColorBrush(Colors.Crimson);
                else if (i == 6) tb.Foreground = new SolidColorBrush(Colors.RoyalBlue);
                Grid.SetColumn(tb, i);
                DayHeaderGrid.Children.Add(tb);
            }
        }

        private void BuildCalendarGrid()
        {
            CalendarBodyGrid.Children.Clear();
            CalendarBodyGrid.RowDefinitions.Clear();
            CalendarBodyGrid.ColumnDefinitions.Clear();

            for (int c = 0; c < 7; c++)
                CalendarBodyGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            for (int r = 0; r < 6; r++)
                CalendarBodyGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100) });

            foreach (var day in ViewModel.CalendarDays)
            {
                var cell = BuildDayCell(day);
                Grid.SetColumn(cell, day.Column);
                Grid.SetRow(cell, day.Row);
                CalendarBodyGrid.Children.Add(cell);
            }
        }

        private FrameworkElement BuildDayCell(CalendarDay day)
        {
            var todayBg = Application.Current.Resources.TryGetValue("NuriyeBrandGlowBrush", out var glowObj) && glowObj is Microsoft.UI.Xaml.Media.Brush glowBrush
                ? glowBrush
                : (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["SystemFillColorAttentionBackgroundBrush"];

            var border = new Border
            {
                BorderBrush = new SolidColorBrush(Colors.Gray) { Opacity = 0.25f },
                BorderThickness = new Thickness(0.5),
                Background = day.IsToday ? todayBg : null
            };

            var stack = new StackPanel { Padding = new Thickness(4, 4, 4, 2) };

            // 날짜 숫자
            var dayTb = new TextBlock
            {
                Text = day.IsCurrentMonth ? day.DayNumber.ToString() : "",
                FontSize = 12,
                Margin = new Thickness(0, 0, 0, 2)
            };
            if (day.IsToday) dayTb.FontWeight = FontWeights.Bold;
            if (day.Column == 0) dayTb.Foreground = new SolidColorBrush(Colors.Crimson);
            else if (day.Column == 6) dayTb.Foreground = new SolidColorBrush(Colors.RoyalBlue);
            stack.Children.Add(dayTb);

            // 대여 바 (최대 3개 표시)
            int shown = 0;
            foreach (var rental in day.Rentals)
            {
                if (shown >= 3)
                {
                    stack.Children.Add(new TextBlock
                    {
                        Text = $"+{day.Rentals.Count - shown}건",
                        FontSize = 9,
                        Foreground = new SolidColorBrush(Colors.Gray)
                    });
                    break;
                }

                var barBorder = new Border
                {
                    Background = new SolidColorBrush(HexToColor(rental.Color)),
                    CornerRadius = new CornerRadius(2),
                    Margin = new Thickness(0, 1, 0, 1),
                    Padding = new Thickness(3, 1, 3, 1)
                };
                barBorder.Child = new TextBlock
                {
                    Text = rental.Applicant,
                    FontSize = 10,
                    Foreground = new SolidColorBrush(Colors.White),
                    TextTrimming = TextTrimming.CharacterEllipsis
                };

                var tip = new ToolTip();
                var tipStack = new StackPanel { Spacing = 2 };
                tipStack.Children.Add(new TextBlock { Text = $"신청자: {rental.Applicant}", FontWeight = FontWeights.SemiBold });
                tipStack.Children.Add(new TextBlock { Text = $"장비: {rental.Equipment}" });
                tipStack.Children.Add(new TextBlock { Text = $"액세서리: {rental.Accessories}" });
                tip.Content = tipStack;
                ToolTipService.SetToolTip(barBorder, tip);

                stack.Children.Add(barBorder);
                shown++;
            }

            border.Child = stack;
            return border;
        }

        private static Windows.UI.Color HexToColor(string hex)
        {
            hex = hex.TrimStart('#');
            return Windows.UI.Color.FromArgb(
                255,
                Convert.ToByte(hex.Substring(0, 2), 16),
                Convert.ToByte(hex.Substring(2, 2), 16),
                Convert.ToByte(hex.Substring(4, 2), 16));
        }
    }
}

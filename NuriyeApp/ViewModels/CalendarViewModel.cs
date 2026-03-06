using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NuriyeApp.Models;
using NuriyeApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NuriyeApp.ViewModels
{
    public partial class CalendarViewModel : ObservableObject
    {
        private static readonly string[] ColorPalette =
        {
            "#4472C4", "#ED7D31", "#70AD47", "#FFC000",
            "#9966CC", "#FF3300", "#00B0F0"
        };

        [ObservableProperty]
        private string _monthTitle = "";

        [ObservableProperty]
        private bool _isLoading = false;

        private DateTime _currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        public List<CalendarDay> CalendarDays { get; private set; } = new();

        // code-behind가 구독해서 Grid를 재빌드
        public event Action? CalendarRebuilt;

        [RelayCommand]
        public async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                var rentals = await SupabaseService.Instance.GetActiveRentalsAsync();
                BuildCalendar(rentals);
            }
            catch { }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task PrevMonthAsync()
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            await LoadAsync();
        }

        [RelayCommand]
        public async Task NextMonthAsync()
        {
            _currentMonth = _currentMonth.AddMonths(1);
            await LoadAsync();
        }

        private void BuildCalendar(List<Rental> rentals)
        {
            MonthTitle = _currentMonth.ToString("yyyy년 M월");

            int startDow = (int)_currentMonth.DayOfWeek;

            rentals.Sort((a, b) =>
            {
                var daysA = (DateTime.Parse(a.EndDate) - DateTime.Parse(a.StartDate)).Days;
                var daysB = (DateTime.Parse(b.EndDate) - DateTime.Parse(b.StartDate)).Days;
                return daysB.CompareTo(daysA); // 내림차순
            });

            var colorMap = new Dictionary<int, string>();
            int ci = 0;
            foreach (var r in rentals)
                colorMap[r.Id] = ColorPalette[ci++ % ColorPalette.Length];

            var days = new List<CalendarDay>();
            for (int i = 0; i < 42; i++)
            {
                var date = _currentMonth.AddDays(i - startDow);
                var day = new CalendarDay
                {
                    Date = date,
                    DayNumber = date.Day,
                    IsCurrentMonth = date.Month == _currentMonth.Month,
                    IsToday = date.Date == DateTime.Today,
                    Column = i % 7,
                    Row = i / 7
                };

                foreach (var r in rentals)
                {
                    if (DateTime.TryParse(r.StartDate, out var rs) &&
                        DateTime.TryParse(r.EndDate, out var re) &&
                        date.Date >= rs.Date && date.Date <= re.Date)
                    {
                        day.Rentals.Add(new CalendarRental
                        {
                            Applicant = r.Applicant,
                            Equipment = r.Equipment,
                            Accessories = r.Accessories,
                            Color = colorMap.TryGetValue(r.Id, out var c) ? c : ColorPalette[0]
                        });
                    }
                }
                days.Add(day);
            }

            CalendarDays = days;
            CalendarRebuilt?.Invoke();
        }
    }
}

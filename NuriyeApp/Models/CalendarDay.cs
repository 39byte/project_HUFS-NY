using System;
using System.Collections.Generic;

namespace NuriyeApp.Models
{
    public class CalendarDay
    {
        public DateTime Date { get; set; }
        public int DayNumber { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public int Column { get; set; }  // 0=일, 6=토
        public int Row { get; set; }     // 0~5
        public List<CalendarRental> Rentals { get; set; } = new();
    }

    public class CalendarRental
    {
        public string Applicant { get; set; } = "";
        public string Equipment { get; set; } = "";
        public string Accessories { get; set; } = "";
        public string Color { get; set; } = "#4472C4";
    }
}

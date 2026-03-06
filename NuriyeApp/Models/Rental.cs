using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace NuriyeApp.Models
{
    [Table("Rentals")]
    public class Rental : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("신청자")]
        public string Applicant { get; set; } = "";

        [Column("연락처")]
        public string Contact { get; set; } = "";

        [Column("장비명")]
        public string Equipment { get; set; } = "";

        [Column("대여시작일")]
        public string StartDate { get; set; } = "";

        [Column("반납예정일")]
        public string EndDate { get; set; } = "";

        [Column("대면시간")]
        public string MeetingTime { get; set; } = "";

        [Column("담당자")]
        public string Staff { get; set; } = "미지정";

        [Column("상태")]
        public string Status { get; set; } = "대기";

        [Column("비고")]
        public string Remarks { get; set; } = "";

        [Column("실제반납일")]
        public string? ActualReturnDate { get; set; }

        [Column("액세서리")]
        public string Accessories { get; set; } = "";

        [Column("추가요청")]
        public string ExtraRequest { get; set; } = "";

        [Column("신청일시")]
        public string SubmittedAt { get; set; } = "";

        public string DDayLabel
        {
            get
            {
                if (DateTime.TryParse(EndDate, out var end))
                {
                    var diff = (end.Date - DateTime.Today).Days;
                    return diff switch
                    {
                        > 0 => $"D-{diff}",
                        0 => "D-Day",
                        _ => $"D+{-diff} (연체)"
                    };
                }
                return "";
            }
        }

        public bool IsOverdue
        {
            get
            {
                if (DateTime.TryParse(EndDate, out var end))
                    return (end.Date - DateTime.Today).Days <= -1;
                return false;
            }
        }
    }
}

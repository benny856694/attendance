using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace huaanClient.Report
{
    public class DailyAttendance
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public string PersonalNo { get; set; }
        public LocalDate Date { get; set; }
        public string Shift { get; set; }
        public LocalTime? CheckIn1 { get; set; }
        public LocalTime? CheckOut1 { get; set; }
        public LocalTime? CheckIn2 { get; set; }
        public LocalTime? CheckOut2 { get; set; }
        public float? Temperature { get; set; }
        public Period LateMinutes { get; set; }
        public Period EarlyMinutes { get; set; }
        public Period WorkHour { get; set; }
        public Remark Status { get; set; }
    }
}

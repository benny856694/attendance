using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace huaanClient.Report
{
    public class MonthlyAttendance
    {
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
        public YearMonth YearMonth { get; set; }
        public int PresentDaysCount { get; set; }
        public int AbsentDaysCount { get; set; }
        public int HolidaysCount { get; set; }
        public int LeaveDaysCount { get; set; }
        public Period TotalLateHours { get; set; }
        public int TotalLateDays { get; set; }
        public Period TotalEarlyHours { get; set; }
        public int TotalEarlyDays { get; set; }
    }
}

using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    public class DailyAttendanceData
    {
        public string EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public LocalDate Date { get; set; }
        public string ShiftName { get; set; }
        public LocalTime? ShiftStart { get; set; }
        public LocalTime? ShiftEnd { get; set; }
        public LocalTime? CheckIn { get; set; }
        public LocalTime? CheckOut { get; set; }
        public Period LateHour { get; set; } = Period.Zero;
        public Period EarlyHour { get; set; } = Period.Zero;
        public Period WorkHour { get; set; } = Period.Zero;
        public Period OverTime { get; set; } = Period.Zero;
        public Remark Remark { get; set; } = Remark.Absence;

        public static DailyAttendanceData Absense { get; } = new DailyAttendanceData { Remark = Remark.Absence };
        public static DailyAttendanceData OffDuty { get; } = new DailyAttendanceData { Remark = Remark.OffDuty };
        public static DailyAttendanceData Holiday { get; } = new DailyAttendanceData { Remark = Remark.Holiday };

    }
}

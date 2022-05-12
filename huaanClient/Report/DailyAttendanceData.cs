using huaanClient.Database;
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
        public string EmployeeDepartment { get; set; }
        public LocalDate Date { get; set; }
        public string ShiftName { get; set; }
        public LocalTime? ShiftStart1 { get; set; }
        public LocalTime? ShiftEnd1 { get; set; }
        public LocalTime? ShiftStart2 { get; set; }
        public LocalTime? ShiftEnd2 { get; set; }
        public LocalTime? CheckIn1 { get; set; }
        public LocalTime? CheckOut1 { get; set; }
        public LocalTime? CheckIn2 { get; set; }
        public LocalTime? CheckOut2 { get; set; }
        public Period LateHour { get; set; } = Period.Zero;
        public Period EarlyHour { get; set; } = Period.Zero;
        public Period WorkHour { get; set; } = Period.Zero;
        public Period OverTime { get; set; } = Period.Zero;
        public Remark Remark { get; set; } = Remark.Absence;
        public bool IsCrossMidnight { get; set; } //是否跨夜
        public Shift Shift { get; set; }

        public static DailyAttendanceData Absense { get; } = new DailyAttendanceData { Remark = Remark.Absence };
        public static DailyAttendanceData OffDuty { get; } = new DailyAttendanceData { Remark = Remark.OffDuty };
        public static DailyAttendanceData Holiday { get; } = new DailyAttendanceData { Remark = Remark.Holiday };
        public float? Temperature { get; internal set; }

        public Shift ToShift()
        {
            return new Shift
            {
                name = this.ShiftName,
                gotowork1 = this.ShiftStart1.HasValue && this.ShiftEnd1.HasValue ? $"{this.ShiftStart1.Value.ToDbTimeString()}-{this.ShiftEnd1.Value.ToDbTimeString()}" : "",
                gotowork2 = this.ShiftStart2.HasValue && this.ShiftEnd2.HasValue ? $"{this.ShiftStart2.Value.ToDbTimeString()}-{this.ShiftEnd2.Value.ToDbTimeString()}" : "",
                //gooffwork3 = this.ShiftStart3.HasValue && this.ShiftEnd3.HasValue ? $"{this.ShiftStart3.Value.ToDbTimeString()}-{this.ShiftEnd3.Value.ToDbTimeString()}" : ""

            };
        }

    }
}

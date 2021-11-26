using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    public class AttendanceDataForDay
    {
        public LocalDate Date { get; set; }
        public string ShiftName { get; set; }
        public LocalTime? ShiftStart { get; set; }
        public LocalTime? ShiftEnd { get; set; }
        public LocalTime? CheckIn { get; set; }
        public LocalTime? CheckOut { get; set; }
        public Period Late { get; set; } = Period.Zero;
        public Period Early { get; set; } = Period.Zero;
        public Period WorkHour { get; set; } = Period.Zero;
        public Remark Remark { get; set; }

    }
}

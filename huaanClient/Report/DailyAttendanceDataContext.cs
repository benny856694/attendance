using huaanClient.Database;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    public class DailyAttendanceDataContext
    {
        public Department Department { get; set; }
        public Employeetype Employeetype { get; set; }
        public Shift Shift { get; set; }
        public LocalDate Date { get; set; }
        public Staff Staff { get; set; }
        public DailyAttendanceData DailyAttendanceData { get; set; }
    }
}

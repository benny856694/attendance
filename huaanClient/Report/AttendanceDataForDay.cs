using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    class AttendanceDataForDay
    {
        LocalDate Day { get; set; }
        string ShiftName { get; set; }
        LocalTime ShiftStart { get; set; }
        LocalTime ShiftEnd { get; set; }
        LocalTime CheckIn { get; set; }
        LocalTime CheckOut { get; set; }
        Period Late { get; set; }
        Period Early { get; set; }
        Period WorkHour { get; set; }
        Remark Remark { get; set; }

    }
}

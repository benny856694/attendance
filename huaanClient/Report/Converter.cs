using huaanClient.Database;
using NodaTime.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    public static class Converter
    {
        public static AttendanceDataForDay ToAttendanceDataForDay(this AttendanceData data)
        {
            var shift = data.Shiftinformation.CalcShift();
            var result = new AttendanceDataForDay();
            result.EmployeeId = data.personId;
            result.EmployeeCode = data.Employee_code;
            result.EmployeeName = data.name;
            result.ShiftName = shift.Name;
            result.ShiftStart = shift.ShiftStart.ToLocalTime();
            result.ShiftEnd = shift.ShiftEnd.ToLocalTime();
            result.Date = data.Date.ToLocalDateTime().Date;
            result.CheckIn = data.Punchinformation.ToLocalTime();
            result.CheckOut = data.Punchinformation1.ToLocalTime();

            if (result.CheckIn.HasValue && result.CheckOut.HasValue)
            {
                if (result.CheckIn.Value > result.ShiftStart.Value)
                {
                    result.Late = (result.CheckIn.Value - result.ShiftStart.Value).Normalize();
                }

                if (result.CheckOut.Value < result.ShiftEnd.Value)
                {
                    result.Early = (result.ShiftEnd.Value - result.CheckOut.Value).Normalize();
                }

                var shiftHour = result.ShiftEnd.Value - result.ShiftStart.Value;
                var workHour = result.CheckOut.Value - result.CheckIn.Value;
                result.WorkHour = workHour.Normalize();
                var ot = (workHour - shiftHour).Normalize();
                if (!(ot.Hours < 0 || ot.Minutes < 0))
                {
                    result.OverTime = ot;
                }
                
            }
            result.Remark = data.CalcRemarks();
            return result;

        }

    }
}

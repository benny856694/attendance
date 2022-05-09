using huaanClient.Database;
using NodaTime;
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
        public static DailyAttendanceData ToAttendanceDataForDay(this AttendanceData data)
        {
            var shift = data.Shiftinformation.CalcShift();
            var result = new DailyAttendanceData();
            result.EmployeeId = data.personId;
            result.EmployeeCode = data.Employee_code;
            result.EmployeeName = data.name;
            result.EmployeeDepartment = data.department;
            result.ShiftName = shift.Name;
            result.ShiftStart1 = shift.ShiftStart1.ToLocalTime();
            result.ShiftEnd1 = shift.ShiftEnd1.ToLocalTime();
            result.ShiftStart2 = shift.ShiftStart2.ToLocalTime();
            result.ShiftEnd2 = shift.ShiftEnd2.ToLocalTime();
            result.Date = data.Date.ToLocalDateTime().Date;
            result.CheckIn1 = data.Punchinformation.ToLocalTime();
            result.CheckOut1 = data.Punchinformation1.ToLocalTime();
            result.CheckIn2 = data.Punchinformation2.ToLocalTime();
            result.CheckOut2 = data.Punchinformation22.ToLocalTime();
            result.IsCrossMidnight = data.IsAcrossNight == "True";
            result.Temperature = data.temperature.toFloat();
            CalcHours(result, result.ShiftStart1, result.ShiftEnd1, result.CheckIn1, result.CheckOut1);
            CalcHours(result, result.ShiftStart2, result.ShiftEnd2, result.CheckIn2, result.CheckOut2);
            result.Remark = result.CalcRemarks();
            if (result.Remark == Remark.Absence)
            {
                //请假
                if (string.Compare(data.Remarks, "3", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    result.Remark = Remark.Leave;
                }
            }
            return result;

        }
        
        private static void CalcHours(DailyAttendanceData result, LocalTime? start, LocalTime? end, LocalTime? checkIn, LocalTime? checkOut)
        {
            if (checkIn.HasValue && checkOut.HasValue)
            {
                if (checkIn.Value > start.Value)
                {
                    result.LateHour += (checkIn.Value - start.Value).Normalize();
                }

                if (checkOut.Value < end.Value)
                {
                    result.EarlyHour += (end.Value - checkOut.Value).Normalize();
                }
                
                var shiftHour = end.Value - start.Value;
                var workHour = checkOut.Value - checkIn.Value;
                if (result.IsCrossMidnight)
                {
                    shiftHour += Period.FromHours(24);
                    workHour += Period.FromHours(24);
                }
                result.WorkHour += workHour.Normalize();
                var ot = (workHour - shiftHour).Normalize();
                if (!(ot.Hours < 0 || ot.Minutes < 0))
                {
                    result.OverTime += ot;
                }

            }
        }


    }
}

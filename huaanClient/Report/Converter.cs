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
            CalcHours(result);
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

        private static void CalcOneCommuteHours()
        {
            
        }

        private static void CalcLateNEarlyHour(DailyAttendanceData result)
        {
            Period lateHours = Period.Zero;
            Period earlyHours = Period.Zero;
            if (result.CheckIn1.HasValue)
            {
                if (result.CheckIn1.Value > result.ShiftStart1.Value)
                {
                    lateHours += result.CheckIn1.Value - result.ShiftStart1.Value;
                }
            }

            if (result.CheckOut1.HasValue)
            {
                if (result.CheckOut1.Value < result.ShiftEnd1.Value)
                {
                    earlyHours += result.ShiftEnd1.Value - result.CheckOut1.Value;
                }
            }


            if (result.CheckIn2.HasValue)
            {
                if (result.CheckIn2.Value > result.ShiftStart2.Value)
                {
                    lateHours += result.CheckIn2.Value - result.ShiftStart2.Value;
                }
            }


            if (result.CheckOut2.HasValue)
            {
                if (result.CheckOut2.Value < result.ShiftEnd2.Value)
                {
                    earlyHours += result.ShiftEnd2.Value - result.CheckOut2.Value;
                }
            }

            result.LateHour = lateHours.Normalize();
            result.EarlyHour = earlyHours.Normalize();
        }

        private static void CalcWorkHourNOverTimeHour(DailyAttendanceData result, bool isFullPresent)
        {
            Period workHour = Period.Zero;
            Period overTimeHours = Period.Zero;
            LocalTime? checkInTime = null;

            if (isFullPresent)
            {
                var breakHour = result.ShiftStart2.Value - result.ShiftEnd1.Value;
                if (result.CheckOut1.HasValue && result.CheckIn2.HasValue)
                {
                    workHour += result.CheckOut1.Value - result.CheckIn1.Value;
                    workHour += result.CheckOut2.Value - result.CheckIn2.Value;
                }
                else
                {
                    workHour += result.CheckOut2.Value - result.CheckIn1.Value;
                    workHour -= breakHour;
                }
            }
            else
            {
                if (result.CheckIn1.HasValue)
                {
                    checkInTime = result.CheckIn1;
                }

                if (result.CheckOut1.HasValue)
                {
                    if (checkInTime.HasValue)
                    {
                        var endTime = result.CheckOut1.Value;
                        workHour += endTime - checkInTime.Value;
                    }
                    checkInTime = null;
                }
               

                if (result.CheckIn2.HasValue)
                {
                    checkInTime = result.CheckIn2;
                }
                

                if (result.CheckOut2.HasValue)
                {
                    if (checkInTime.HasValue)
                    {
                        var endTime = result.CheckOut2.Value;
                        workHour += endTime - checkInTime.Value;
                    }
                    checkInTime = null;
                }
            }
         

            var shiftHour = Period.Zero;
            if (result.ShiftStart1.HasValue && result.ShiftEnd1.HasValue)
            {
                shiftHour += result.ShiftEnd1.Value - result.ShiftStart1.Value;
            }
            if (result.ShiftStart2.HasValue && result.ShiftEnd2.HasValue)
            {
                shiftHour += result.ShiftEnd2.Value - result.ShiftStart2.Value;
            }
            var ot = (workHour - shiftHour).Normalize();
            result.OverTime = ot.Normalize();
            result.WorkHour = workHour.Normalize();
        }
        
      
        
        private static void CalcHours(DailyAttendanceData result)
        {
            var isFullPresent = result.CheckIn1.HasValue && result.CheckOut2.HasValue;
            CalcWorkHourNOverTimeHour(result, isFullPresent);
            CalcLateNEarlyHour(result);


            //if (checkIn.HasValue && checkOut.HasValue)
            //{
            //    if (checkIn.Value > start.Value)
            //    {
            //        result.LateHour += (checkIn.Value - start.Value).Normalize();
            //    }

            //    if (checkOut.Value < end.Value)
            //    {
            //        result.EarlyHour += (end.Value - checkOut.Value).Normalize();
            //    }

            //    var shiftHour = end.Value - start.Value;
            //    var workHour = checkOut.Value - checkIn.Value;
            //    if (result.IsCrossMidnight)
            //    {
            //        shiftHour += Period.FromHours(24);
            //        workHour += Period.FromHours(24);
            //    }
            //    result.WorkHour += workHour.Normalize();
            //    var ot = (workHour - shiftHour).Normalize();
            //    if (!(ot.Hours < 0 || ot.Minutes < 0))
            //    {
            //        result.OverTime += ot;
            //    }

            //}
        }


    }
}

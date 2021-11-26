using huaanClient.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using NodaTime.Extensions;

namespace huaanClient.Report
{
    internal static class Extensions
    {
        public static (string Name, string ShiftStart, string ShiftEnd) CalcShift(this string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return (null, null, null);
            }

            var sections = data.Split('-');
            if (sections.Length < 3)
            {
                return (null, null, null);
            }

            return (sections[0], sections[1], sections[2]);
        }

        public static LocalTime? ToLocalTime(this string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            var time = data.Split(':');
            return new LocalTime(Convert.ToInt32(time[0]), Convert.ToInt32(time[1]));
        }

        public static Remark CalcRemarks(this AttendanceData data)
        {
            if (string.IsNullOrEmpty(data.Punchinformation) && string.IsNullOrEmpty(data.Punchinformation1))
            {
                return Remark.Absent;
            }
            if (string.IsNullOrEmpty(data.Punchinformation) || string.IsNullOrEmpty(data.Punchinformation1) )
            {
                return Remark.SinglePunch;
            }
            
            //请假
            if (string.Compare(data.Remarks, "3", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return Remark.OffWork;
            }

            return Remark.Present;
        }

        public static string ToDisplayText(this Remark rmk)
        {
            var result = "";
            switch (rmk)
            {
                case Remark.Present:
                    result = "P";
                    break;
                case Remark.SinglePunch:
                    result = "PX";
                    break;
                case Remark.Absent:
                    result = "A";
                    break;
                default:
                    break;
            }

            return result;
        }

        public static AttendanceDataForDay ToAttendanceDataForDay(this AttendanceData data)
        {
            var shift = data.Shiftinformation.CalcShift();
            var result = new AttendanceDataForDay();
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
                result.WorkHour = (workHour - shiftHour).Normalize();
            }
            result.Remark = data.CalcRemarks();
            return result;

        }
        
    }
}

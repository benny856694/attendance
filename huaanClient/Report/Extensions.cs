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


        public static string ToMyString(this Period period)
        {
            if (period.Hours == 0 && period.Minutes == 0)
            {
                return null;
            }

            if (period.Hours < 0 || period.Minutes <0)
            {
                return null;
            }

            //var sb = new StringBuilder();
            //if (period.Days > 0)
            //{
            //    sb.AppendFormat("{0}:", period.Days);
            //}
            
            //sb.AppendFormat("{0:d2}:{1:d2}", period.Hours+period.Days*24, period.Minutes);
            
            

            return $"{period.Days*24+period.Hours:d2}:{period.Minutes:d2}";
        }

        
    }
        
  
}

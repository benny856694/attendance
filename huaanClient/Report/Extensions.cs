using huaanClient.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    internal static class Extensions
    {
        public static (string Shift, string ShiftStart, string ShiftEnd) CalcShift(this AttendanceData data)
        {
            if (string.IsNullOrEmpty(data.Shiftinformation))
            {
                return (null, null, null);
            }

            var sections = data.Shiftinformation.Split('-');
            if (sections.Length < 3)
            {
                return (null, null, null);
            }

            return (sections[0], sections[1], sections[2]);
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
                case Remark.Early:
                    break;
                case Remark.Late:
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}

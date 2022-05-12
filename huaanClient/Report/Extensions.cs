using huaanClient.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using NodaTime.Extensions;
using huaanClient.Properties;

namespace huaanClient.Report
{
    internal static class Extensions
    {
        public static (string Name, string ShiftStart1, string ShiftEnd1, string ShiftStart2, string ShiftEnd2) CalcShift(this string data)
        {
            string name = null, shiftStart1 = null, shiftEnd1 = null, shiftStart2 = null, shiftEnd2 = null;

            string commute1 = null, commute2 = null;
            if (data.Contains(";"))
            {
                var s = data.Split(';');
                commute1 = s[0];
                commute2 = s[1];
            }
            else
            {
                commute1 = data;
            }

            var sectionsCommute1 = commute1?.Split('-');
            if (sectionsCommute1?.Length == 3)
            {
                name = sectionsCommute1[0];
                shiftStart1 = sectionsCommute1[1];
                shiftEnd1 = sectionsCommute1[2];
            }

            var sectionsCommute2 = commute2?.Split('-');
            if (sectionsCommute2?.Length == 2)
            {
                shiftStart2 = sectionsCommute2[0];
                shiftEnd2 = sectionsCommute2[1];
            }

            return (name, shiftStart1, shiftEnd1, shiftStart2, shiftEnd2);
        }

        public static LocalTime? ToLocalTime(this string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            var time = data.Contains(";") ? data.Split(';')[0].Split(':') : data.Split(':');
            return new LocalTime(Convert.ToInt32(time[0]), Convert.ToInt32(time[1]));
        }

        public static LocalDate? ToLocalDate(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }



            var segments = s.Split('-');
            return new LocalDate(Convert.ToInt32(segments[0]), Convert.ToInt32(segments[1]), Convert.ToInt32(segments[2]));
        }

        public static Remark CalcRemarks(this DailyAttendanceData data)
        {
            if (data.Shift == null)
            {
                return Remark.OffDuty;
            }
            
            var expectedPunchCount = 0;
            if (data.ShiftStart1.HasValue) expectedPunchCount++;
            if (data.ShiftStart2.HasValue) expectedPunchCount++;
            if (data.ShiftEnd1.HasValue) expectedPunchCount++;
            if (data.ShiftEnd2.HasValue) expectedPunchCount++;

            var actualPunchCount = 0;
            if (data.CheckIn1.HasValue) actualPunchCount++;
            if (data.CheckIn2.HasValue) actualPunchCount++;
            if (data.CheckOut1.HasValue) actualPunchCount++;
            if (data.CheckOut2.HasValue) actualPunchCount++;


            if (actualPunchCount == 0)
            {
                return Remark.Absence;
            }

            if (actualPunchCount < expectedPunchCount)
            {
                return Remark.SinglePunch;
            }

            if (actualPunchCount == expectedPunchCount)
            {
                return Remark.Present;
            }

            return Remark.Absence;
        }

        public static string ToDisplayText(this Remark rmk)
        {
            var result = "";
            switch (rmk)
            {
                case Remark.Present:
                    result = Strings.ReportRemarkP;
                    break;
                case Remark.SinglePunch:
                    result = Strings.ReportRemarkPX;
                    break;
                case Remark.Absence:
                    result = Strings.ReportRemarkA;
                    break;
                case Remark.Holiday:
                case Remark.OffDuty:
                    result = Strings.ReportRemarkHO;
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

            if (period.Hours < 0 || period.Minutes < 0)
            {
                return null;
            }

            //var sb = new StringBuilder();
            //if (period.Days > 0)
            //{
            //    sb.AppendFormat("{0}:", period.Days);
            //}

            //sb.AppendFormat("{0:d2}:{1:d2}", period.Hours+period.Days*24, period.Minutes);



            return $"{period.Days * 24 + period.Hours:d2}:{period.Minutes:d2}";
        }

        public static string ToDbTimeString(this LocalTime time) => $"{time.Hour.ToString("d2")}:{time.Minute.ToString("d2")}";

        public static (LocalTime start, LocalTime end) ToLocalTimeFrame(this string data)
        {
            var time = data.Replace(" ","").Split('-');
            return (time[0].ToLocalTime().Value, time[1].ToLocalTime().Value);
        }
    }


}

﻿using huaanClient.Database;
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

        public static Remark CalcRemarks(this AttendanceData data)
        {
            if (string.IsNullOrEmpty(data.Punchinformation) && string.IsNullOrEmpty(data.Punchinformation1))
            {
                return Remark.Absence;
            }
            if (string.IsNullOrEmpty(data.Punchinformation) || string.IsNullOrEmpty(data.Punchinformation1) )
            {
                return Remark.SinglePunch;
            }
            
            //请假
            if (string.Compare(data.Remarks, "3", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return Remark.Leave;
            }

            return Remark.Present;
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

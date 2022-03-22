using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
    public static class Extensions
    {
        public static (LocalTime start, LocalTime end)? ToLocalTimeSlot(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }

            var segments = s.Split('-');
            var time1Segments = segments[0].Split(':');
            var time2Segments = segments[1].Split(':');

            var lt1 = new LocalTime(int.Parse(time1Segments[0]), int.Parse(time1Segments[1]));
            var lt2 = new LocalTime(int.Parse(time2Segments[0]), int.Parse(time2Segments[1]));
            return (lt1, lt2);
        }

        public static string ToIntString(this Enum v)
        {
            var i = Convert.ToInt32(v);
            return i.ToString();
        }

    }
}

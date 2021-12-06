using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
	[Table("AttendanceGroup")]
	public class AttendanceGroup
	{
		[Key]
		public long id { get; set; }
		public string attribute { get; set; }
		public string name { get; set; }
		public string publishtime { get; set; }
		public string isdefault { get; set; }

		public int GetShiftIdForDay(DayOfWeek day)
        {
            if (string.IsNullOrEmpty(attribute))
            {
				throw new InvalidOperationException($"Shift Configuration is empty for AttendanceGroup({id})");
            }

			var shiftIdForWeek = Newtonsoft.Json.JsonConvert.DeserializeObject<ShiftIdForWeek>(this.attribute);
            switch (day)
            {
                case DayOfWeek.Sunday:
                    return int.Parse(shiftIdForWeek.Sunday);
                case DayOfWeek.Monday:
                    return int.Parse(shiftIdForWeek.Monday);
                case DayOfWeek.Tuesday:
                    return int.Parse(shiftIdForWeek.Tuesday);
                case DayOfWeek.Wednesday:
                    return int.Parse(shiftIdForWeek.Wednesday);
                case DayOfWeek.Thursday:
                    return int.Parse(shiftIdForWeek.Thursday);
                case DayOfWeek.Friday:
                    return int.Parse(shiftIdForWeek.Friday);
                case DayOfWeek.Saturday:
                    return int.Parse(shiftIdForWeek.Saturday);
                default:
                    throw new ArgumentOutOfRangeException();
            }


        }
	}
}

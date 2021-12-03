using Dapper.Contrib.Extensions;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
	[Table("Shift")]
	public class Shift
	{
		[Key]
		public long id { get; set; }
		public string name { get; set; }
		public string Duration { get; set; }
		public string gotowork1 { get; set; }
		public string gotowork2 { get; set; }
		public string gooffwork3 { get; set; }
		public string rest_time { get; set; }
		public string EffectiveTime { get; set; }
		public string EffectiveTime2 { get; set; }
		public string EffectiveTime3 { get; set; }
		public string publish_time { get; set; }
		public string IsAcrossNight { get; set; }

		public (LocalTime ShiftStart, LocalTime ShiftEnd)? GetShift1() => this.gotowork1.ToLocalTimeSlot();
		public (LocalTime ShiftStart, LocalTime ShiftEnd)? GetShift2() => this.gotowork2.ToLocalTimeSlot();
		public (LocalTime ShiftStart, LocalTime ShiftEnd)? GetShift3() => this.gooffwork3.ToLocalTimeSlot();
	}
}

using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
	[Table("TimeSegment")]
	public class TimeSegment
	{
		[Key]
		public int Id { get; set; }
		public string Start { get; set; }
		public string End { get; set; }
		public int DayOfWeekId { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
	public class TimeSegment
	{
		public long Id { get; set; }
		public string Start { get; set; }
		public string End { get; set; }
		public long DayOfWeekId { get; set; }
	}
}

using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
	[Table("Day")]
	public class Day
	{
		[Key]
		public int Id { get; set; }
		public DayOfWeek DayOfWeek { get; set; }
		public int AccessRuleId { get; set; }

		[Computed]
		public List<TimeSegment> TimeSegments {  get; set; }
	}
}

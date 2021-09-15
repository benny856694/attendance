using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
	[Table("RuleDistributionItem")]
	public class RuleDistributionItem
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string StaffId { get; set; }
		public int GroupId { get; set; }
		public GroupIdType GroupType { get; set; }
		public int RuleDistributionId { get; set; }
	}
}

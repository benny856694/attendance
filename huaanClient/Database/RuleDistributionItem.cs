using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
	public class RuleDistributionItem
	{
		public long Id { get; set; }
		public string StaffId { get; set; }
		public long GroupId { get; set; }
		public long GroupType { get; set; }
		public long RuleDistributionId { get; set; }
	}
}

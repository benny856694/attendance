using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
	public class RuleDistribution
	{
		public long Id { get; set; }
		public string Name {  get; set; }
		public long AccessRuleId { get; set; }
		public long Priority { get; set; }
	}
}

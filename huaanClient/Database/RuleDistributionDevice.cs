using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
	[Table("RuleDistributionDevice")]
	public class RuleDistributionDevice
	{
		[Key]
		public int Id { get; set; }
		public string Name {  get; set; }
		public int DeviceId { get; set; }
		public int RuleDistributionId { get; set; }
	}
}

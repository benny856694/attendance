using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
	public class AccessRule
	{
		public long Id { get; set; }
		public long RuleNumber { get; set; }
		public string Name { get; set; }
		public long RepeatType { get; set; }
	}
}

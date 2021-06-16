using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
	public class AttendanceGroup
	{
		public long id { get; set; }
		public string attribute { get; set; }
		public string name { get; set; }
		public string publishtime { get; set; }
		public string isdefault { get; set; }
	}
}

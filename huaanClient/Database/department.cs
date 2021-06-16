using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
	public class department
	{
		public long id { get; set; }
		public string name { get; set; }
		public string phone { get; set; }
		public long no { get; set; }
		public string address { get; set; }
		public string explain { get; set; }
		public long code { get; set; }
		public string publish_time { get; set; }
		public long ParentId { get; set; }
	}
}

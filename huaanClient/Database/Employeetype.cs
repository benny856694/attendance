using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
	[Table("Employetype")]
	public class Employeetype
	{
		[Key]
		public long id { get; set; }
		public string Employetype_name { get; set; }
	}
}

using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Model
{
	[DelimitedRecord("|")]
	public class Pair
	{
		public string id;
		public string pair_id;
	}
}

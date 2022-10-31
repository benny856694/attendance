using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class DataSyn {

		[JsonProperty, Column(IsPrimary = true)]
		public int id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string name { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string imge { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string personid { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string publishtime { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string role { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string term_start { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string term { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string wg_card_id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string long_card_id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string addr_name { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string device_sn { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string model { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string stutas { get; set; }

	}

}

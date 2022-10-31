using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class LineFor_list {

		[JsonProperty, Column(IsPrimary = true)]
		public int id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string line_userid { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string message { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string type { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string name { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Date { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string time { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string temperature { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string late { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Leaveearly { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string status { get; set; }

	}

}

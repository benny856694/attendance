using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Visitor {

		[JsonProperty, Column(IsPrimary = true)]
		public string id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string name { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string phone { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string imge { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string staTime { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string endTime { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string isDown { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string downDetail { get; set; }

	}

}

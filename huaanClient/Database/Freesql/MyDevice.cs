using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class MyDevice {

		[JsonProperty, Column(IsPrimary = true)]
		public int id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string DeviceName { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string number { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string ipAddress { get; set; }

		[JsonProperty]
		public DateTime Last_query { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string time_syn { get; set; }

		[JsonProperty]
		public int? IsEnter { get; set; } = -1;

		[JsonProperty, Column(StringLength = -2)]
		public string username { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string password { get; set; }

	}

}

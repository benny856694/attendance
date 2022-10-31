using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Special_date {

		[JsonProperty, Column(IsPrimary = true)]
		public int id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string date { get; set; }

		[JsonProperty]
		public int? Shiftid { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string datetype { get; set; }

		[JsonProperty]
		public int? AttendanceGroupid { get; set; }

	}

}

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Shift {

		[JsonProperty, Column(IsPrimary = true)]
		public int id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string name { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Duration { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string gotowork1 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string gotowork2 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string gooffwork3 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string rest_time { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string EffectiveTime { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string EffectiveTime2 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string EffectiveTime3 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string publish_time { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string IsAcrossNight { get; set; }

	}

}

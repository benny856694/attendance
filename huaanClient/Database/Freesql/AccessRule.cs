using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class AccessRule {

		[JsonProperty, Column(IsPrimary = true)]
		public int Id { get; set; }

		[JsonProperty]
		public int? RuleNumber { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Name { get; set; }

		[JsonProperty]
		public int? RepeatType { get; set; }

	}

}

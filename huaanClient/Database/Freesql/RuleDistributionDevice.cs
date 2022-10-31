using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class RuleDistributionDevice {

		[JsonProperty, Column(IsPrimary = true)]
		public int Id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Name { get; set; }

		[JsonProperty]
		public int? DeviceId { get; set; } = -1;

		[JsonProperty]
		public int? RuleDistributionId { get; set; }

	}

}

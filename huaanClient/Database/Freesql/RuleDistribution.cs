using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class RuleDistribution {

		[JsonProperty, Column(IsPrimary = true)]
		public int Id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Name { get; set; }

		[JsonProperty]
		public int? DistributionItemType { get; set; }

		[JsonProperty]
		public int? AccessRuleId { get; set; }

		[JsonProperty]
		public int? Priority { get; set; }

	}

}

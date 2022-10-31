using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class TimeSegment {

		[JsonProperty, Column(IsPrimary = true)]
		public int Id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Start { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string End { get; set; }

		[JsonProperty]
		public int? DayOfWeekId { get; set; }

	}

}

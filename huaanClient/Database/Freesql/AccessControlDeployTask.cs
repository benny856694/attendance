using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class AccessControlDeployTask {

		[JsonProperty, Column(IsPrimary = true)]
		public int Id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Created { get; set; }

		[JsonProperty]
		public int? State { get; set; }

		[JsonProperty]
		public int? TotalCount { get; set; }

		[JsonProperty]
		public int? DeviceCount { get; set; }

		[JsonProperty]
		public int? Progress { get; set; }

		[JsonProperty]
		public int? FailCount { get; set; }

		[JsonProperty]
		public int? SuccessCount { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string RulesJson { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string ItemsFilePath { get; set; }

	}

}

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class user {

		[JsonProperty, Column(IsPrimary = true)]
		public int id { get; set; }

		[JsonProperty, Column(StringLength = -2, IsNullable = false)]
		public string username { get; set; } = "";

		[JsonProperty, Column(StringLength = -2, IsNullable = false)]
		public string password { get; set; } = "";

	}

}

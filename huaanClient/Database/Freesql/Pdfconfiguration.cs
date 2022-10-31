using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Pdfconfiguration {

		[JsonProperty, Column(IsPrimary = true)]
		public int id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string pdftitle { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string rows1 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string rows2 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string rows3 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string rows4 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string rows5 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string rows6 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string rows7 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string rows8 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string rows9 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string rows10 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string rows11 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string rows12 { get; set; }

	}

}

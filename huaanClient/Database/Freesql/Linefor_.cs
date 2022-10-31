using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Linefor_ {

		[JsonProperty, Column(IsPrimary = true)]
		public int id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string ftpserver { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string line_url { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string lineRQcodeEmail { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string lineRQcode { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Message12 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Message11 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Message10 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Message9 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string ftpusername { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Message8 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Message6 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Message5 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Message4 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Message3 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Message2 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Message { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string temperature { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string userid { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Message7 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string ftppassword { get; set; }

	}

}

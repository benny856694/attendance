using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Attendance_Data {

		[JsonProperty, Column(IsPrimary = true)]
		public int id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Todaylate { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string workOvertime { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Duration { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string temperature { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string isAbsenteeism3 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string isAbsenteeism2 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string isAbsenteeism { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Leaveearly { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string late { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Shiftinformation { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Punchinformation33 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Punchinformation3 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Punchinformation22 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Punchinformation2 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Punchinformation1 { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Punchinformation { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Date { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Employee_code { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string department { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string personId { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string name { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Remarks { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string IsAcrossNight { get; set; }

	}

}

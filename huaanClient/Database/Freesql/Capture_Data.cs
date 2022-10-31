using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Capture_Data {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string QRcodestatus { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string closeup { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string idcard_name { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string idcard_number { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string device_sn { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public float body_temp { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public bool exist_mask { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string match_failed_reson { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string QRcode { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string wg_card_id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string person_name { get; set; }

		[JsonProperty("person_id"), Column(Name = "person_id", StringLength = -2)]
		public string StaffId { get; set; }

        [JsonProperty]
		public Staff Staff { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public int match_type { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public int match_status { get; set; }

		[JsonProperty]
		public DateTime time { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string addr_name { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string device_id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public int sequnce { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public int hatColor { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string trip_infor { get; set; }

	}

}

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(Name = "staff", DisableSyncStructure = true)]
	public partial class Staff {

		[JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
		public string id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string customer_text { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string idcardtype { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string source { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string face_idcard { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string islineAdmin { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string line_codemail { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string line_type { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string line_code { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string line_userid { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string term_start { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string IDcardNo { get; set; }

		[JsonProperty]
		public int? Employetype_id { get; set; }

		[JsonProperty, Column(DbType = "TIMESTAMP", IsNullable = false)]
		public string publish_time { get; set; } = "";

		[JsonProperty, Column(StringLength = -2)]
		public string picture { get; set; } = "";

		[JsonProperty, Column(Name = "department_id", InsertValueSql = "")]
		public int? DepartmentId { get; set; }

		public Department Department { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string status { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Employee_code { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string phone { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Email { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string name { get; set; }

		[JsonProperty]
		public int? AttendanceGroup_id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string term { get; set; }

		public ICollection<Capture_Data> CaptureData { get; set; }

	}

}

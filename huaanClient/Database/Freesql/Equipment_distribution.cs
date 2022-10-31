using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Equipment_distribution {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int id { get; set; }

		[JsonProperty, Column(Name = "userid", StringLength = -2, IsNullable = false)]
		public string StaffId { get; set; } = "";

		public Staff Staff { get; set; }

		[JsonProperty, Column(Name = "deviceid", InsertValueSql = "")]
		public int DeviceId { get; set; }

		public MyDevice Device { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string status { get; set; }

		[JsonProperty]
		public string type { get; set; } = "0";

		[JsonProperty, Column()]
		public DateTime date { get; set; }

		[JsonProperty, Column()]
		public string code { get; set; }

		[JsonProperty]
		public bool? isDistributedByEmployeeCode { get; set; } = false;

		[JsonProperty, Column(StringLength = -2)]
		public string employeeCode { get; set; } = "";

		[JsonProperty, Column(StringLength = -2)]
		public string errMsg { get; set; } = "";

		[JsonProperty]
		public int? retryCount { get; set; } = 0;


		public void MarkForDistribution()
		{
			status = "inprogress";
			date = DateTime.Now;
			errMsg = "";
			code = "";
			type = "0";
		}

		public void MarkForDelete()
		{
			status = "deleting";
			date = DateTime.Now;
			errMsg = "";
			code = "";
			type = "1";
		}

	}

}

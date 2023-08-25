using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Attendance.Database.NJIS {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class CHECKINOUT {

		[JsonProperty, Column(IsPrimary = true, InsertValueSql = "getdate()")]
		public DateTime CHECKTIME { get; set; }

		[JsonProperty, Column(IsPrimary = true)]
		public int USERID { get; set; }

		[JsonProperty, Column(DbType = "varchar(1)")]
		public string CHECKTYPE { get; set; } = "I";

		[JsonProperty]
		public int? mask_flag { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(30)")]
		public string Memoinfo { get; set; }

		[JsonProperty, Column(DbType = "varchar(5)")]
		public string SENSORID { get; set; }

		[JsonProperty, Column(StringLength = 20)]
		public string sn { get; set; }

		[JsonProperty]
		public double? temperature { get; set; } = 0d;

		[JsonProperty]
		public short? UserExtFmt { get; set; } = 0;

		[JsonProperty]
		public int? VERIFYCODE { get; set; } = 0;

		[JsonProperty, Column(StringLength = 24)]
		public string WorkCode { get; set; } = "0";

	}

}

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace huaanClient.Database.Freesql {

	[JsonObject(MemberSerialization.OptIn), Table(Name = "department", DisableSyncStructure = true)]
	public partial class Department {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string name { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string phone { get; set; }

		[JsonProperty, Column(InsertValueSql = "")]
		public int? no { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string address { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string explain { get; set; }

        [JsonProperty]
        public int? code { get; set; }

        [JsonProperty, Column(StringLength = -2)]
		public DateTime? publish_time { get; set; }

		[JsonProperty]
		public int? ParentId { get; set; }

		public ICollection<Staff> Staffs { get; set; }

		public Department()
		{
			publish_time = DateTime.Now;
		}

	}

}

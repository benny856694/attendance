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
	public partial class USERINFO {

		[JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
		public int USERID { get; set; }

		[JsonProperty]
		public int? AccGroup { get; set; } = 1;

		[JsonProperty]
		public short ATT { get; set; } = 1;

		[JsonProperty]
		public short? AutoSchPlan { get; set; } = 1;

		[JsonProperty, Column(DbType = "varchar(24)", IsNullable = false)]
		public string BADGENUMBER { get; set; }

		[JsonProperty]
		public DateTime? BIRTHDAY { get; set; }

		[JsonProperty, Column(DbType = "varchar(20)")]
		public string CardNo { get; set; }

		[JsonProperty, Column(DbType = "varchar(2)")]
		public string CITY { get; set; }

		[JsonProperty]
		public short? DEFAULTDEPTID { get; set; } = 1;

		[JsonProperty, Column(StringLength = 100)]
		public string EMail { get; set; }

		[JsonProperty]
		public short? EMPRIVILEGE { get; set; } = 0;

		[JsonProperty]
		public int? Expires { get; set; } = 0;

		[JsonProperty]
		public int? FaceGroup { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(20)")]
		public string FPHONE { get; set; }

		[JsonProperty]
		public bool? FSelected { get; set; } = false;

		[JsonProperty, Column(DbType = "varchar(8)")]
		public string GENDER { get; set; }

		[JsonProperty]
		public DateTime? HIREDDAY { get; set; }

		[JsonProperty]
		public short HOLIDAY { get; set; } = 1;

		[JsonProperty, Column(StringLength = 24)]
		public string IDCard_MainCard { get; set; }

		[JsonProperty, Column(StringLength = 24)]
		public string IDCard_ViceCard { get; set; }

		[JsonProperty, Column(StringLength = 70)]
		public string IDCardAddr { get; set; }

		[JsonProperty, Column(StringLength = 16)]
		public string IDCardBirth { get; set; }

		[JsonProperty, Column(StringLength = 24)]
		public string IDCardDN { get; set; }

		[JsonProperty]
		public int? IDCardGender { get; set; }

		[JsonProperty, Column(StringLength = 32)]
		public string IDCardISSUER { get; set; }

		[JsonProperty, Column(StringLength = 30)]
		public string IDCardName { get; set; }

		[JsonProperty]
		public int? IDCardNation { get; set; }

		[JsonProperty]
		public string IDCardNewAddr { get; set; }

		[JsonProperty, Column(DbType = "varchar(18)")]
		public string IDCardNo { get; set; }

		[JsonProperty]
		public string IDCardNotice { get; set; }

		[JsonProperty, Column(StringLength = 36)]
		public string IDCardReserve { get; set; }

		[JsonProperty, Column(StringLength = 24)]
		public string IDCardSN { get; set; }

		[JsonProperty, Column(DbType = "varchar(32)")]
		public string IDCardValidTime { get; set; }

		[JsonProperty]
		public short? InheritDeptRule { get; set; } = 1;

		[JsonProperty]
		public short? InheritDeptSch { get; set; } = 1;

		[JsonProperty]
		public short? InheritDeptSchClass { get; set; } = 1;

		[JsonProperty]
		public short INLATE { get; set; } = 1;

		[JsonProperty]
		public short LUNCHDURATION { get; set; } = 1;

		[JsonProperty]
		public int? MinAutoSchInterval { get; set; } = 24;

		[JsonProperty, Column(DbType = "varchar(8)")]
		public string MINZU { get; set; }

		[JsonProperty, Column(DbType = "varchar(10)")]
		public string MVerifyPass { get; set; }

		[JsonProperty, Column(DbType = "varchar(40)")]
		public string NAME { get; set; }

		[JsonProperty, Column(DbType = "image")]
		public byte[] Notes { get; set; }

		[JsonProperty, Column(DbType = "varchar(20)")]
		public string OPHONE { get; set; }

		[JsonProperty]
		public short OUTEARLY { get; set; } = 1;

		[JsonProperty]
		public short OVERTIME { get; set; } = 1;

		[JsonProperty, Column(DbType = "varchar(20)")]
		public string PAGER { get; set; }

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string PASSWORD { get; set; }

		[JsonProperty, Column(DbType = "image")]
		public byte[] PHOTO { get; set; }

		[JsonProperty]
		public int? Pin1 { get; set; }

		[JsonProperty]
		public int? privilege { get; set; } = 0;

		[JsonProperty]
		public short? RegisterOT { get; set; } = 1;

		[JsonProperty]
		public short? SECURITYFLAGS { get; set; }

		[JsonProperty]
		public short SEP { get; set; } = 1;

		[JsonProperty, Column(DbType = "varchar(20)")]
		public string SSN { get; set; }

		[JsonProperty, Column(DbType = "varchar(2)")]
		public string STATE { get; set; }

		[JsonProperty, Column(DbType = "varchar(80)")]
		public string STREET { get; set; }

		[JsonProperty]
		public int? TimeZone1 { get; set; } = 1;

		[JsonProperty]
		public int? TimeZone2 { get; set; } = 0;

		[JsonProperty]
		public int? TimeZone3 { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(20)")]
		public string TITLE { get; set; }

		[JsonProperty]
		public int? UseAccGroupTZ { get; set; } = 1;

		[JsonProperty]
		public int? ValidCount { get; set; } = 0;

		[JsonProperty]
		public DateTime? ValidTimeBegin { get; set; }

		[JsonProperty]
		public DateTime? ValidTimeEnd { get; set; }

		[JsonProperty]
		public short? VERIFICATIONMETHOD { get; set; }

		[JsonProperty]
		public int? VerifyCode { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(12)")]
		public string ZIP { get; set; }

	}

}

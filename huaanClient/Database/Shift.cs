﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
	public class Shift
	{
		public long id { get; set; }
		public string name { get; set; }
		public string Duration { get; set; }
		public string gotowork1 { get; set; }
		public string gotowork2 { get; set; }
		public string gooffwork3 { get; set; }
		public string rest_time { get; set; }
		public string EffectiveTime { get; set; }
		public string EffectiveTime2 { get; set; }
		public string EffectiveTime3 { get; set; }
		public string publish_time { get; set; }
		public string IsAcrossNight { get; set; }
	}
}

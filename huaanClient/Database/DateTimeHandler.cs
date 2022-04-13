using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
    internal class DateTimeHandler : SqlMapper.TypeHandler<DateTime>
	{
		private static string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
		public override DateTime Parse(object value)
		{
			if (value is DateTime) return (DateTime)value;
			return DateTime.Parse(value as string, CultureInfo.InvariantCulture);
		}

		public override void SetValue(IDbDataParameter parameter, DateTime value)
		{
			parameter.Value = value.ToString(datetimeFormat);
		}
	}
}

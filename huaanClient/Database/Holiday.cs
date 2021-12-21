using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
    [Table("Special_date")]
    public class Holiday
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public int AttendanceGroupid { get; set; }
    }
}

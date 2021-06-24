//using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace huaanClient.Database
{
    //[Table("Equipment_distribution")]
    class EquipmentDistribution
    {
       // [Key]
        public int id { get; set; }
        public int  userid { get; set; }
        public int  deviceid { get; set; }
        public string status { get; set; }

        //0: 下发, 1: 删除, 2: 异常
        public string type { get; set; } = "0";
        public DateTime? date { get; set; }
        public string code { get; set; }

        public int isDistributedByEmployeeCode { get; set; }
        public string employeeCode { get; set; }

    }
}

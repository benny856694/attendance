using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace huaanClient.Database
{
    class EquipmentDistribution
    {
        public int id { get; set; }
        public int  userid { get; set; }
        public int  deviceid { get; set; }
        public string status { get; set; }

        //0: 下发, 1: 删除, 2: 异常
        public string type { get; set; }
        public DateTime? date { get; set; }
        public string code { get; set; }

        
    }
}

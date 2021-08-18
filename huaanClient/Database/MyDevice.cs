using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
    [Table("MyDevice")]
    class MyDevice
    {
        [Key]
        public int id { get; set; }
        public string DeviceName { get; set; }
        public string number { get; set; }
        public string ipAddress { get; set; }
        public string Last_query { get; set; }
        public string time_syn { get; set; }

        //-1: undefined, 1: enter, 0: exit
        public int IsEnter { get; set; }

    }
}

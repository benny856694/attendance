using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
    [Table("Visitor")]
    public class Visitor
    {
        [ExplicitKey]
        public string id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string image { get; set; }
        public string staTime { get; set; }
        public string endTime { get; set; }
        public string isDown { get; set; }
        public string idNumber { get; set; }
    }
}

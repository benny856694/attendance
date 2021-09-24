using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Api
{
    public class Response
    {
        public string reply { get; set; }
        public string cmd { get; set; }
        public int code { get; set; }
        public string device_sn { get; set; }
    }
}

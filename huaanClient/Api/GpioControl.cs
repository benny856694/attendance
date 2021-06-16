using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Api
{

    public class GpioControl
    {
        public string version { get; set; }
        public string cmd { get; set; }
        public int port { get; set; }
        public string ctrl_type { get; set; }
        public string ctrl_mode { get; set; }
        public string person_id { get; set; }
    }

}

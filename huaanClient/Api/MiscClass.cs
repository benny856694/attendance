using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Api
{

    public class Device_Info
    {
        public string addr_name { get; set; }
    }

    

    public class Record
    {
        public bool save_enable { get; set; }
        public string save_path { get; set; }
    }

    public class Name_List
    {
        public bool auto_clean { get; set; }
    }

}

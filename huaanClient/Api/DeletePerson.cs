using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Api
{

    public class DeletePerson
    {
        public string version { get; set; }
        public string cmd { get; set; }
        public int flag { get; set; }
        public string id { get; set; }
    }

}

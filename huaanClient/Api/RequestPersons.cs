using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Api
{

    public class RequestPersons
    {
        public string cmd { get; set; }
        public int role { get; set; }
        public int page_no { get; set; }
        public int page_size { get; set; }
        public int normal_image_flag { get; set; }
        public int image_flag { get; set; }
        public int query_mode { get; set; }
        public Condition condition { get; set; }
    }

    public class Condition
    {
        public string person_id { get; set; }
    }

}

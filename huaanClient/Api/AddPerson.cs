using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Api
{
    public class AddPerson
    {
        public string version { get; set; }
        public string cmd { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public int role { get; set; }
        public int feature_unit_size { get; set; }
        public int feature_num { get; set; }
        public object[] feature_data { get; set; }
        public int image_num { get; set; }
        public object[] reg_images { get; set; }
        public int norm_image_num { get; set; }
        public object[] norm_images { get; set; }
        public string term { get; set; }
        public string term_start { get; set; }
        public string long_card_id { get; set; }
    }

}

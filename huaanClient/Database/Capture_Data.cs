using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
    public class Capture_Data
    {
        public long id { get; set; }
        public string sequnce { get; set; }
        public string device_id { get; set; }
        public string addr_name { get; set; }
        public DateTime time { get; set; }
        public string match_status { get; set; }
        public string match_type { get; set; }
        public string person_id { get; set; }
        public string person_name { get; set; }
        public string hatColor { get; set; }
        public string wg_card_id { get; set; }
        public string match_failed_reson { get; set; }
        public string exist_mask { get; set; }
        public string body_temp { get; set; }
        public string device_sn { get; set; }
        public string idcard_number { get; set; }
        public string idcard_name { get; set; }
        public string closeup { get; set; }
        public string QRcodestatus { get; set; }
        public string QRcode { get; set; }
        public string trip_infor { get; set; }
    }
}

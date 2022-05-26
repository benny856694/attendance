using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Api
{

    public class ResponseCaptureRecord
    {
        public string cmd { get; set; }
        public int code { get; set; }
        public int count { get; set; }
        public string device_sn { get; set; }
        public int page_no { get; set; }
        public CaptureRecord[] records { get; set; }
        public string reply { get; set; }
        public int total { get; set; }
    }

    public class CaptureRecord
    {
        public float body_temp { get; set; }
        public string customer_id { get; set; }
        public string id { get; set; }
        public int match_failed_reson { get; set; }
        public int match_type { get; set; }
        public string name { get; set; }
        public string person_name_ext { get; set; }
        public int role { get; set; }
        public int score { get; set; }
        public int sequence { get; set; }
        public string time { get; set; }
        public int upload_state { get; set; }
    }

}

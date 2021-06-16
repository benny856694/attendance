using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Api
{

    public class UpdateAppParams
    {
        public string version { get; set; }
        public string cmd { get; set; }
        public Face face { get; set; }
        public Led_Control led_control { get; set; }
    }

    public class Face
    {
        public bool enable_same_face_reg { get; set; }
        public bool output_not_matched { get; set; }
        public bool enable_alive { get; set; }
        public Body_Temperature body_temperature { get; set; }
    }

    public class Body_Temperature
    {
        public bool enable { get; set; }
        public float limit { get; set; }
    }

    public class Led_Control
    {
        public string led_mode { get; set; }
        public string led_brightness { get; set; }
        public string led_sensitivity { get; set; }
    }

}

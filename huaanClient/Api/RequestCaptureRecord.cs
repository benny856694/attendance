﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Api
{
    public class RequestCaptureRecord
    {
        public string cmd { get; } = "request records";
        
        //从1开始
        public int page_no { get; set; } 
        public int page_size { get; set; }
        public int face_image_flag { get; set; }
        public int reg_image_flag { get; set; }
        public long time_start { get; set; }
        public long time_end { get; set; }
        
        public new string ToString()
        {
            return $"cmd={cmd}&page_no={page_no}&page_size={page_size}&face_image_flag={face_image_flag}&reg_image_flag={reg_image_flag}&time_start={time_start}&time_end={time_end}";
        }
           
            
    }

}

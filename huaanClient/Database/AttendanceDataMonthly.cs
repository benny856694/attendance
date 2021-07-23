using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
    public class AttendanceDataMonthly
    {
        public string name { get; set; }
        public string personId { get; set; }
        public string department { get; set; }
        public string Employee_code { get; set; }
        public string nowdate { get; set; }
        public int Attendance { get; set; }
        public int restcount { get; set; }
        public string latedata { get; set; }
        public string Leaveearlydata { get; set; }
        public int AbsenteeismCount { get; set; }
        public float LeaveCount { get; set; }
        public float LeaveCount1 { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
    public class AttendanceData
    {
        public long id { get; set; }
        public string name { get; set; }
        public string personId { get; set; }
        public string department { get; set; }
        public string Employee_code { get; set; }
        public DateTime Date { get; set; }
        public string Punchinformation { get; set; }
        public string Punchinformation1 { get; set; }
        public string Punchinformation2 { get; set; }
        public string Punchinformation22 { get; set; }
        public string Punchinformation3 { get; set; }
        public string Punchinformation33 { get; set; }
        public string Shiftinformation { get; set; }
        public string late { get; set; }
        public string Leaveearly { get; set; }
        public string isAbsenteeism { get; set; }
        public string isAbsenteeism2 { get; set; }
        public string isAbsenteeism3 { get; set; }
        public string temperature { get; set; }
        public string Duration { get; set; }
        public string workOvertime { get; set; }
        public string Todaylate { get; set; }
        public string Remarks { get; set; }
        public string IsAcrossNight { get; set; }
    }
}

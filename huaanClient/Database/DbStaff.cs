using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
    class DbStaff
    {
        public int id { get; set; }
        public string name { get; set; }
        public string Email { get; set; }
        public string phone { get; set; }
        public string Employee_code { get; set; }
        public string status { get; set; }
        public int department_id { get; set; }
        public string picture { get; set; }
        public DateTime publish_time { get; set; }
        public int Employetype_id { get; set; }
        public int AttendanceGroup_id { get; set; }
        public string IDcardNo { get; set; }
        public string line_userid { get; set; }
        public string line_code { get; set; }
        public string line_type { get; set; }
        public string line_codemail { get; set; }
        public string islineAdmin { get; set; }
        public string face_idcard { get; set; }
        public string idcardtype { get; set; }
        public string source { get; set; }

    }
}

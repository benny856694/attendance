using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Model
{
    class AddPersonRequest
    {
        public string version { get; set; } = "0.2";
        public string cmd { get; set; } = "upload person";
        public string id { get; set; }
        public int wg_card_id { get; set; }
        public string reg_image { get; set; }
    }
}

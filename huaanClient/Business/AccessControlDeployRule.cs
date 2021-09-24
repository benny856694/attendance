using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Business
{
    public class AccessControlDeployRule
    {
        public string name {  get; set; }
        public int kind {  get; set; }
        public string mode {  get; set; }
        public List<AccessControlDeplyDay> days {  get; set; } = new List<AccessControlDeplyDay>();
    }
}

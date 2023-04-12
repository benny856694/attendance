using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Attendance
{
    internal class ProgressValue
    {
        public bool busy { get; set; } = false;
        public bool done { get; set; } = false;
        public int Percent { get; set; } = 0;
        public string Message { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.Control;

namespace Dashboard.Controls
{
    public static class Extensions
    {
        public static Control[] ExtractAll(this ControlCollection cc)
        {
            return cc.OfType<Control>().ToArray();
        }
    }
}

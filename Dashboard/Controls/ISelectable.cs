using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dashboard.Controls
{
    public interface ISelectable
    {
        bool Selected { get; set; }
        event EventHandler<MouseEventArgs> MouseClicked;
    }
}

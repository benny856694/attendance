using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dashboard.Controls
{
    public class CreateNewControlEventArgs : EventArgs
    {
        public CellPosition Position { get; private set; }
        public Control Control { get; set; }

        public CreateNewControlEventArgs(CellPosition position)
        {
            Position = position;
        }
    }
}

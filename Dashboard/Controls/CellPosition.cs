using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Controls
{
    public class CellPosition : Tuple<int, int>
    {
        public CellPosition(int col, int row): base(col, row)
        {

        }
    }
}

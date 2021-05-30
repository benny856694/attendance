using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dashboard.Controls
{
    public partial class GridControl : UserControl
    {
        private (int Row, int Col) _rowColumns;
        private TableLayoutPanel _tlp;


        public event EventHandler RowColumnChanged;

        public (int Rows, int Cols) RowColumn
        {
            get => _rowColumns;
            set
            {
                if (value.Rows < 1 || value.Cols < 1)
                {
                    throw new ArgumentOutOfRangeException("Rows and Cols must be > 0");
                }

                if (_rowColumns == value)
                {
                    return;
                }

                _rowColumns = value;

                this.SuspendLayout();
                this.Controls.Clear();
                _tlp = CreateTableLayout(value);
                _tlp.Dock = DockStyle.Fill;
                this.Controls.Add(_tlp);
                this.ResumeLayout();

                RowColumnChanged?.Invoke(this, new EventArgs());
            }
        }


        public void Add(Control c, int row, int col) => _tlp.Controls.Add(c, col, row);

        public void Clear() => _tlp.Controls.Clear();


        public GridControl()
        {
            InitializeComponent();
        }

        private TableLayoutPanel CreateTableLayout((int rows, int columns) rowCols)
        {
            var tlp = new TableLayoutPanel();
            tlp.BackColor = Color.Black;
            tlp.ColumnCount = rowCols.columns;
            tlp.Dock = DockStyle.Fill;
            tlp.Location = new Point(0, 0);
            tlp.Name = "tableLayoutPanel1";
            tlp.RowCount = rowCols.rows;

            var columWidth = 100F / rowCols.columns;
            for (int i = 0; i < rowCols.columns; i++)
            {
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, columWidth));
            }

            var rowHeight = 100F / rowCols.rows;
            for (int i = 0; i < rowCols.rows; i++)
            {
                tlp.RowStyles.Add(new ColumnStyle(SizeType.Percent, rowHeight));
            }

            return tlp;
        }
    }
}

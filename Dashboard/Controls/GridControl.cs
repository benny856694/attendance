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
        private (int Row, int Col) _rowColumns = (0,0);
        private TableLayoutPanel _tlp;
        private List<Control> _visibleControls = new List<Control>();
        private List<Control> _invisibleControls = new List<Control>();


        public event EventHandler RowColumnChanged;

        public event EventHandler<CreateNewControlEventArgs> CreateNewControlForCell;


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

                var visibleControls = _tlp?.Controls.ExtractAll() ?? new Control[0];
                _tlp?.Controls.Clear();
                
                this.Controls.Clear();
                _tlp?.Dispose();
                _tlp = CreateTableLayout(value);
                _tlp.Dock = DockStyle.Fill;

                MoveOldControlToNewContainer(visibleControls, _tlp);
                

                this.Controls.Add(_tlp);
                this.ResumeLayout();

                RowColumnChanged?.Invoke(this, new EventArgs());
            }
        }

        private (Control[] Added, Control[] Hiden)  MoveOldControlToNewContainer(Control[] controls, TableLayoutPanel tlp)
        {
            var index = 0;
            var addedControls = new List<Control>();
            var hidenControls = new List<Control>();

            for (int i = 0; i < tlp.RowCount; i++)
            {
                for (int j = 0; j < tlp.ColumnCount; j++)
                {
                    if (index >= controls.Length)
                    {
                        if (CreateNewControlForCell == null)
                        {
                            throw new InvalidOperationException("CreateNewControlForCell event must be handled");
                        }

                        var e = new CreateNewControlEventArgs(new CellPosition(j, i));
                        CreateNewControlForCell.Invoke(this, e);
                        if (e.Control == null)
                        {
                            throw new InvalidOperationException(@"new control must be created by handling 'CreateNewControlForCell' event");
                        }

                        tlp.Controls.Add(e.Control, j, i);
                        addedControls.Add(e.Control);
                    }
                    else
                    {
                        tlp.Controls.Add(controls[index], j, i);
                    }

                    ++index;
                }
            }

            for (int i = index; i < controls.Length; i++)
            {
                hidenControls.Add(controls[i]);
            }

            return (addedControls.ToArray(), hidenControls.ToArray());

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

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
        private Control[] _invisibleControls = new Control[0];
        private Control _lastSelectedControl;



        public event EventHandler RowColumnChanged;

        public event EventHandler<CreateNewControlEventArgs> CreateNewControlForCell;

        public event EventHandler<ControlVisibleChangedEventArgs> ControlVisibleChanged;

        public Control SelectedControl => _lastSelectedControl;

        public Control[] HidenControls => _invisibleControls;

        public ControlCollection ChildControls => _tlp?.Controls;

        public int Rows
        {
            get => _rowColumns.Row;
            set
            {
                RowColumn = (value, Cols);
            }
        }

        public int Cols
        {
            get => _rowColumns.Col;
            set
            {
                RowColumn = (Rows, value);
            }
        }

        public (int Rows, int Cols) RowColumn
        {
            get => _rowColumns;
            set
            {
                if (value.Rows < 0 || value.Cols < 0)
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
                
                this.Controls?.Clear();
                _tlp?.Dispose();
                _tlp = CreateTableLayout(value);
                _tlp.Dock = DockStyle.Fill;

                var result = MoveOldControlToNewContainer(visibleControls, _tlp);
                _invisibleControls = result.Hiden;

                if (_invisibleControls.Contains(_lastSelectedControl))
                {
                    var selectable = _lastSelectedControl as ISelectable;
                    if (selectable != null)
                    {
                        selectable.Selected = false;
                    }
                    _lastSelectedControl = null;
                }
                

                this.Controls.Add(_tlp);
                this.ResumeLayout();

                foreach (var item in result.VisibleToHidden)
                {
                    var args = new ControlVisibleChangedEventArgs(item, VisibleState.Show, VisibleState.Hide);
                    ControlVisibleChanged?.Invoke(this, args);
                }

                foreach (var item in result.HidenToVisible)
                {
                    var args = new ControlVisibleChangedEventArgs(item, VisibleState.Hide, VisibleState.Show);
                    ControlVisibleChanged?.Invoke(this, args);
                }

                RowColumnChanged?.Invoke(this, new EventArgs());
            }
        }

        private (Control[] Added, Control[] Hiden, Control[] VisibleToHidden, Control[] HidenToVisible)  MoveOldControlToNewContainer(Control[] controls, TableLayoutPanel tlp)
        {
            var index = 0;
            var addedControls = new List<Control>();
            var hidenControls = new List<Control>(_invisibleControls ?? new Control[0]);
            var hidenToVisibleControls = new List<Control>();
            var visibleToHidenControls = new List<Control>();

            for (int i = 0; i < tlp.RowCount; i++)
            {
                for (int j = 0; j < tlp.ColumnCount; j++)
                {
                    Control c = null;
                    
                    if (index >= controls.Length)
                    {
                        if (hidenControls.Count > 0)
                        {
                            c = hidenControls[0];
                            hidenControls.RemoveAt(0);
                            hidenToVisibleControls.Add(c);
                        }
                        else
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

                            var cameraControl = (e.Control as ISelectable);
                            if (cameraControl != null)
                            {
                                cameraControl.MouseClicked += CameraControl_OnClicked;

                            }

                            addedControls.Add(e.Control);
                            c = e.Control;
                        }
                    }
                    else
                    {
                        c = controls[index];
                    }

                    tlp.Controls.Add(c, j, i);
                    ++index;
                }
            }

            for (int i = index; i < controls.Length; i++)
            {
                hidenControls.Insert(0, controls[i]);
                visibleToHidenControls.Add(controls[i]);
            }


            return (addedControls.ToArray(), hidenControls.ToArray(), visibleToHidenControls.ToArray(), hidenToVisibleControls.ToArray());

        }

        private void CameraControl_OnClicked(object sender, MouseEventArgs e)
        {
            var cc = sender as Control;
            if (cc == _lastSelectedControl)
            {
                return;
            }

            var lastSelectable = _lastSelectedControl as ISelectable;
            if (lastSelectable != null)
            {
                lastSelectable.Selected = false;
            }

            var newSelectable = cc as ISelectable;
            if (newSelectable != null)
            {
                newSelectable.Selected = true;
            }

            _lastSelectedControl = cc;
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
                tlp.RowStyles.Add(new RowStyle(SizeType.Percent, rowHeight));
            }

            return tlp;
        }
    }
}

using Dashboard.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dashboard
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();

            gridControl1.RowColumnChanged += GridControl1_RowColumnChanged;
            gridControl1.CreateNewControlForCell += GridControl1_CreateNewControlForCell;
            gridControl1.ControlVisibleChanged += GridControl1_ControlVisibleChanged;
        }

        private void GridControl1_ControlVisibleChanged(object sender, ControlVisibleChangedEventArgs e)
        {
            Debug.WriteLine($"{e.OldVisibleState} -> {e.NewVisibleState}");
        }

        private void GridControl1_CreateNewControlForCell(object sender, Controls.CreateNewControlEventArgs e)
        {
            var c = new CameraUserControl();
            c.Dock = DockStyle.Fill;


            e.Control = c;
        }

        private void GridControl1_RowColumnChanged(object sender, EventArgs e)
        {
            
        }

       
        private void TestForm_Load(object sender, EventArgs e)
        {
            gridControl1.SetRowCol(1, 1);
        }

   
        private void numericUpDownCol_ValueChanged(object sender, EventArgs e)
        {
            var col = Convert.ToInt32(numericUpDownCol.Value);
            gridControl1.Cols = col;
        }

        private void numericUpDownRow_ValueChanged(object sender, EventArgs e)
        {
            var row = Convert.ToInt32(numericUpDownRow.Value);
            gridControl1.Rows = row;
        }
    }
}

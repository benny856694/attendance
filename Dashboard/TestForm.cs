using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        }

        private void GridControl1_CreateNewControlForCell(object sender, Controls.CreateNewControlEventArgs e)
        {
            var c = new Panel() 
            { 
                BackColor = Color.Gray,
                Dock = DockStyle.Fill
            };

            c.Controls.Add(new Label() { Text = $"{e.Position} @ {DateTime.Now}", ForeColor = Color.White, AutoSize = true });

            e.Control = c;
        }

        private void GridControl1_RowColumnChanged(object sender, EventArgs e)
        {
            
        }

       
        private void TestForm_Load(object sender, EventArgs e)
        {
            gridControl1.RowColumn = (1, 1);
        }

   
        private void numericUpDownCol_ValueChanged(object sender, EventArgs e)
        {
            var rc = (gridControl1.RowColumn.Rows, Convert.ToInt32(numericUpDownCol.Value));
            gridControl1.RowColumn = rc;
        }

        private void numericUpDownRow_ValueChanged(object sender, EventArgs e)
        {
            var rc = (Convert.ToInt32(numericUpDownRow.Value), gridControl1.RowColumn.Cols);
            gridControl1.RowColumn = rc;
        }
    }
}

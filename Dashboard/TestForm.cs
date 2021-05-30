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
        }

        private void GridControl1_RowColumnChanged(object sender, EventArgs e)
        {
            var rowcol = gridControl1.RowColumn;
            for (int i = 0; i < rowcol.Rows; i++)
            {
                for (int j = 0; j < rowcol.Cols; j++)
                {
                    var panel = new Panel();
                    panel.Dock = DockStyle.Fill;
                    panel.ForeColor = Color.White;
                    panel.BorderStyle = BorderStyle.FixedSingle;
                    panel.BackColor = Color.Gray;

                    gridControl1.Add(panel, i, j);
                }
            }
        }

        private void buttonIncColAndRow_Click(object sender, EventArgs e)
        {
            var rc = gridControl1.RowColumn;
            rc.Cols++;
            rc.Rows++;
            gridControl1.RowColumn = rc;
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            gridControl1.RowColumn = (1, 1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var rc = gridControl1.RowColumn;
            rc.Cols--;
            rc.Rows--;
            gridControl1.RowColumn = rc;
        }
    }
}

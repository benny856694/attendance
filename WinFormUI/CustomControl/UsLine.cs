using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZXCL.WinFormUI.CustomControl
{
    public partial class UsLine : UserControl
    {
        Font textFont = new Font("宋体", 11.5f, FontStyle.Regular, GraphicsUnit.Pixel);
        Color lineColor = Color.Gray;
        //Color textColor = Color.Black;
        //Color backColor = Color.Transparent;

        public UsLine()
        {
            InitializeComponent();
        }

        string _Text = "";
        public string Label
        {
            get { return _Text; }
            set { _Text = value; }
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            using (Pen pen = new Pen(lineColor))
            {
                e.Graphics.DrawLine(pen, new Point(0, this.Height - 1), new Point(this.Width, this.Height - 1));
            }

            TextRenderer.DrawText(
                         e.Graphics,
                         this.Label,
                        textFont,
                         this.ClientRectangle,
                         Color.Gray,
                         TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }
}

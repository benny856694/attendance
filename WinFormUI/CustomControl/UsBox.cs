using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ZXCL.WinFormUI.CustomControl
{
    public class UsBox : Panel
    {
        

        Color borderColor = Color.FromArgb(184, 184, 184);
        Color backColor = Color.White;
#pragma warning disable CS0414 // 字段“UsBox.labelWidth”已被赋值，但从未使用过它的值
        int labelWidth = 80;
#pragma warning restore CS0414 // 字段“UsBox.labelWidth”已被赋值，但从未使用过它的值

        ZCheckBox innerRadioButton1 = new ZCheckBox();
        ZCheckBox innerRadioButton2 = new ZCheckBox();
        [Browsable(false)]
        public ZCheckBox InnerRadioButton1 { get { return innerRadioButton1; } }
        [Browsable(false)]
        public ZCheckBox InnerRadioButton2 { get { return innerRadioButton2; } }
        

        /// <summary>
        /// 圆角半径
        /// </summary>
        [Description("圆角半径")]
        public int Radius { get; set; }


        //protected override void OnLoad(EventArgs e)
        //{
            
        //}

        protected override void OnPaint(PaintEventArgs e)
        {
            //绘制边框和背景
            if (Radius > 0)
            {
                SmoothingMode smoothingMode = e.Graphics.SmoothingMode;
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

                GraphicsPath path = CreateRoundedRect(this.ClientRectangle, Radius);

                using (SolidBrush brush = new SolidBrush(backColor))
                {
                    e.Graphics.FillPath(brush, path);
                }

                using (Pen pen = new Pen(borderColor))
                {
                    e.Graphics.DrawPath(pen, path);
                }
                e.Graphics.SmoothingMode = smoothingMode;
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
                using (Pen pen = new Pen(borderColor))
                {
                    e.Graphics.DrawRectangle(pen, new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1));
                }
            }

        }


        private GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            rect.Width--;
            rect.Height--;

            Rectangle rectTopLeft = new Rectangle(rect.X, rect.Y, radius, radius);
            Rectangle rectTopRight = new Rectangle(rect.Right - radius, rect.Y, radius, radius);
            Rectangle rectBottomLeft = new Rectangle(rect.X, rect.Bottom - radius, radius, radius);
            Rectangle rectBottomRight = new Rectangle(rect.Right - radius, rect.Bottom - radius, radius, radius);
            Point p1 = new Point(rect.X, rect.Y);
            Point p2 = new Point(rect.Right, rect.Y);
            Point p3 = new Point(rect.Right, rect.Bottom);
            Point p4 = new Point(rect.X, rect.Bottom);
            path.AddArc(rectTopLeft, 180, 90);
            path.AddArc(rectTopRight, 270, 90);
            path.AddArc(rectBottomRight, 0, 90);
            path.AddArc(rectBottomLeft, 90, 90);
            path.CloseFigure();
            return path;
        }


    }
}

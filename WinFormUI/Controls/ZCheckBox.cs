using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZXCL.WinFormUI
{
    public class ZCheckBox: CheckBox
    {

        Color boxColor = Color.DimGray;
        Color enableFalseColor = Color.FromArgb(79,79,79);

        public ZCheckBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        }


        Rectangle rectangleBox
        {
            get
            {
                int y = (this.Height - 12) / 2;
                Rectangle rectangle = new Rectangle(1, y, 12, 12);
                return rectangle;
            }
        }

        Rectangle innerBox
        {
            get
            {
                Rectangle rectangle = new Rectangle(rectangleBox.X+3, rectangleBox.Top+3, 7, 7);
                return rectangle;
            }
        }

        GraphicsPath checkPath {
            get
            {
                Rectangle rectangle = rectangleBox;
                Point a = new Point { X = rectangleBox.Left+2, Y = rectangleBox.Top+5 };
                Point b = new Point { X = rectangleBox.Left+5, Y = rectangleBox.Top+9 };
                Point c = new Point { X = rectangleBox.Left+10, Y = rectangleBox.Top+3 };
                GraphicsPath path = new GraphicsPath();
                path.AddLines(new Point[] { a, b, c });
                return path;
            }
        }


        Rectangle rectangleText
        {
            get
            {
                Rectangle rectangle = this.ClientRectangle;
                rectangle.X = 15;
                rectangle.Width -= 15;
                return rectangle;
            }
        }

        Color _boxColor = Color.DimGray;
        [Description("方框颜色")]
        [DefaultValue(typeof(Color), "DimGray")]
        public Color BoxColor { get => _boxColor; set{ _boxColor = value; } }

        Color _boxColorFocus = Color.FromArgb(71, 154, 255);
        [Description("方框聚焦或鼠标悬浮时颜色")]
        [DefaultValue(typeof(Color),"71,154,255")]
        public Color BoxColorFocus { get => _boxColorFocus; set => _boxColorFocus = value; }

        Color _boxColorMouseDown = Color.FromArgb(0, 84, 153);
        [Description("鼠标按下时方框颜色")]
        [DefaultValue(typeof(Color), "0,84,153")]
        public Color BoxColorMouseDown { get => _boxColorMouseDown; set => _boxColorMouseDown = value; }

        bool _ShowInnerBox;
        [Browsable(false)]
        public bool ShowInnerBox
        {
            get { return _ShowInnerBox; }
            set
            {
                _ShowInnerBox = value;
                Invalidate();
            }
        }
        //public void ShowInnerBox(bool _ShowInnerBox)
        //{
        //    this._ShowInnerBox = _ShowInnerBox;
        //    Invalidate();
        //}


        /// <summary>
        /// 设置此无效
        /// </summary>
        [Browsable(false)]
        public override Color BackColor
        {
            get { return Color.Transparent; }
            set { base.BackColor = value; }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (GetStyle(ControlStyles.AllPaintingInWmPaint))
                OnPaintBackground(pevent);


            Color boxColor = this.boxColor;
            Color foreColor = this.ForeColor;


            if (!Enabled)
            {
                boxColor = Color.FromArgb(209, 209, 209);
                foreColor = Color.FromArgb(79, 79, 79);
            }

            //pevent.Graphics.Clear(this.Parent.BackColor);

            //if(this.Checked)

            Rectangle rec = rectangleBox;
            using (Pen pen = new Pen(boxColor))
            {
                pevent.Graphics.DrawRectangle(pen, rec);
            }


            if(ShowInnerBox)
            {
                using (SolidBrush brush = new SolidBrush(boxColor))
                {
                    pevent.Graphics.FillRectangle(brush, innerBox);
                }
            }
            else if (this.Checked)
            {
                SmoothingMode smoothingMode = pevent.Graphics.SmoothingMode;
                pevent.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                
                using (Pen p = new Pen(boxColor))
                {
                    pevent.Graphics.DrawPath(p, checkPath);
                }
                pevent.Graphics.SmoothingMode = smoothingMode;

            }

            TextRenderer.DrawText(
                pevent.Graphics,
                this.Text,
                this.Font,
                rectangleText,
                foreColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            boxColor = BoxColor;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            boxColor = BoxColorFocus;
        }


        protected override void OnLostFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            boxColor = BoxColor;
        }


        protected override void OnMouseEnter(EventArgs eventargs)
        {
            base.OnMouseEnter(eventargs);
            boxColor = BoxColorFocus;
        }


        protected override void OnMouseLeave(EventArgs eventargs)
        {
            base.OnMouseLeave(eventargs);
            if (!this.Focused)
                boxColor = BoxColor;
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            boxColor = BoxColorMouseDown;
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            boxColor = BoxColorFocus;
            Invalidate();
        }

    }
}

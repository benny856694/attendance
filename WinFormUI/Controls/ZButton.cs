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
    public class ZButton: Button
    {
        Color borderColor = Color.FromArgb(173, 173, 173);
        Color backColor = Color.FromArgb(225, 225, 225);

        public ZButton()
        {
            //SetStyle(
            //      ControlStyles.AllPaintingInWmPaint |
            //      ControlStyles.OptimizedDoubleBuffer |
            //      ControlStyles.ResizeRedraw |
            //      ControlStyles.UserPaint|
            //       ControlStyles.SupportsTransparentBackColor,
            //       true);

            this.SetStyle(
                ControlStyles.UserPaint |  //控件自行绘制，而不使用操作系统的绘制
                ControlStyles.AllPaintingInWmPaint | //忽略擦出的消息，减少闪烁。
                ControlStyles.OptimizedDoubleBuffer |//在缓冲区上绘制，不直接绘制到屏幕上，减少闪烁。
                ControlStyles.ResizeRedraw | //控件大小发生变化时，重绘。                  
                ControlStyles.SupportsTransparentBackColor, true);//支持透明背景颜色

        }

        Color _borderColorNormal = Color.FromArgb(173, 173, 173);
        [Description("边框颜色")]
        [DefaultValue(typeof(Color), "173, 173, 173")]
        public Color BorderColorNormal
        {
            get { return _borderColorNormal; }
            set
            {
                _borderColorNormal = borderColor = value;
                Invalidate();
            }
        }

        Color _borderColorFocus = Color.FromArgb(71, 154, 255);
        [Description("边框获得焦点时颜色")]
        [DefaultValue(typeof(Color), "71, 154, 255")]
        public Color BorderColorFocus
        {
            get { return _borderColorFocus; }
            set { _borderColorFocus = value; Invalidate(); }
        }

        //Color _borderColorHover = Color.FromArgb(153, 153, 153);
        //[Description("鼠标经过时边框颜色")]
        //public Color BorderColorHover
        //{
        //    get { return _borderColorHover; }
        //    set { _borderColorHover = value; Invalidate(); }
        //}


        Color _backColorNormal = Color.FromArgb(225, 225, 225);
        [DefaultValue(typeof(Color),"225, 225, 225")]
        [Description("背景颜色")]
        public Color BackColorNormal
        {
            get { return _backColorNormal; }
            set
            {
                _backColorNormal = backColor = value;
                Invalidate();
            }
        }

        Color _backColorMouseDown = Color.FromArgb(204, 228, 247);
        [Description("鼠标按下时背景颜色")]
        [DefaultValue(typeof(Color), "204, 228, 247")]
        public Color BackColorMouseDown
        {
            get { return _backColorMouseDown; }
            set { _backColorMouseDown = value; Invalidate(); }
        }

        Color _backColorHover = Color.FromArgb(229, 241, 251);
        [Description("鼠标经过时背景颜色")]
        [DefaultValue(typeof(Color), "229, 241, 251")]
        public Color BackColorHover
        {
            get { return _backColorHover; }
            set { _backColorHover = value; Invalidate(); }
        }

        [Description("圆角半径")]
        [DefaultValue(0)]
        public int Radius { get; set; }

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
            if (Radius > 0)
            {
                //pevent.Graphics.Clear(this.Parent.BackColor);

                if (GetStyle(ControlStyles.AllPaintingInWmPaint))
                    OnPaintBackground(pevent);

                SmoothingMode smoothingMode = pevent.Graphics.SmoothingMode;
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                GraphicsPath path = CreateRoundedRect(this.ClientRectangle, Radius);

                using (SolidBrush brush = new SolidBrush(backColor))
                {
                    pevent.Graphics.FillPath(brush, path);
                }
                if(borderColor.A > 0)
                {
                    using (Pen pen = new Pen(borderColor))
                    {
                        pevent.Graphics.DrawPath(pen, path);
                    }
                }

                pevent.Graphics.SmoothingMode = smoothingMode;
            }
            else
            {
                pevent.Graphics.Clear(backColor);
                if (borderColor.A > 0)
                {
                    using (Pen pen = new Pen(borderColor))
                    {
                        pevent.Graphics.DrawRectangle(pen, new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1));
                    }
                }
                
            }

           
            TextRenderer.DrawText(
                      pevent.Graphics,
                      this.Text,
                      this.Font,
                      this.ClientRectangle,
                      Enabled ? this.ForeColor : Color.FromArgb(79, 79, 79),
                      TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

    


        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            borderColor = BorderColorFocus;
            //Invalidate();
        }


        protected override void OnLostFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            borderColor = BorderColorNormal;
           // Invalidate();
        }

        
        protected override void OnMouseEnter(EventArgs eventargs)
        {
            base.OnMouseEnter(eventargs);
            backColor = BackColorHover;
            borderColor = BorderColorFocus;
        }


        protected override void OnMouseLeave(EventArgs eventargs)
        {
            base.OnMouseLeave(eventargs);
            
                backColor = BackColorNormal;
            if (!this.Focused)
                borderColor = BorderColorNormal;
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            backColor = BackColorMouseDown;
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            if(!this.IsDisposed)
            if (this.ClientRectangle.Contains(this.PointToClient(MousePosition)))
            {
                backColor = BackColorHover;
                Invalidate();
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
            path.AddArc(rectTopLeft, 180, 90);
            path.AddArc(rectTopRight, 270, 90);
            path.AddArc(rectBottomRight, 0, 90);
            path.AddArc(rectBottomLeft, 90, 90);
            path.CloseFigure();
            return path;
        }
    }



}

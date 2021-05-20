using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZXCL.WinFormUI.Properties;

namespace ZXCL.WinFormUI
{
    public class TabHeader : IDisposable, IComparable
    {
        public event Action<TabHeader, Rectangle> OnPaintRequest;
        public bool IsDisposed;
        #region 变量

        public string Title = String.Empty;
        public Rectangle Rect;

        public ZFormChild ChildForm;
        public Font Font;
        public string Url;
        public Control Parent;
        public Image HeaderIcon = Properties.Resources.icon_normal;         // 当PageState 为 Loading 时，不会显示该Image
        private WebPageState webPageState;
        private int tabIndex;
        public PaintType paintType;
        public bool Selected = true;
        public bool IsMouseLeave;
        //public Image HeaderIcon
        //{
        //    get { return headerIcon; }
        //    set {
        //        headerIcon = value;
        //        paintType = PaintType.PaintHeaerIcon;
        //        paintRequest();
        //    }
        //}

        // 先设置状态，再修改绘制的TabIcon
        public WebPageState PageState
        {
            get { return webPageState; }
            set
            {
                webPageState = value;
                if (webPageState == WebPageState.Loading)
                {
                    tmAnimation.Start();
                }
                else
                {
                    tmAnimation.Stop();
                    iCurFrame = 0;
                }
            }
        }

        public int Width
        {
            get { return Rect.Width; }
            set
            {
                Rect = GenRect(this.tabIndex, value);
            }
        }

        public int TabIndex
        {
            get { return tabIndex; }
            set
            {
                this.tabIndex = value;
                Rect = GenRect(value, Rect.Width);
            }
        }

        //public bool Selected
        //{
        //    get { return selected; }
        //    set
        //    {
        //        selected = value;
        //        paintType = PaintType.All;
        //        paintRequest();
        //    }
        //}

        // GIF
        int iCurFrame = 0;
        int iFrameCount = 60;
        System.Windows.Forms.Timer tmAnimation = new System.Windows.Forms.Timer();

        //Region region = null;
        private Rectangle rectClose;
        private Rectangle rectIcon;
        private Rectangle rectFont;
        private Rectangle rectFontLinearBrush;
        Color noSelectedColor = Color.FromArgb(218, 218, 218);

        public static readonly int Left_Offset = 0;
        //public static readonly Color BottomLineColor = Color.FromArgb(188, 188, 188);

        


        SolidBrush brushFont = new SolidBrush(Color.DimGray);
        #endregion

        /// <summary>
        /// 创建Tab
        /// </summary>
        public TabHeader(int index, string title, Font font, int tabWidth, Control parent, string url = "")
        {
            tmAnimation.Interval = 20;
            tmAnimation.Tick += tmAnimation_Tick;

            // 构建当前Tab Rect
            Rect = GenRect(index, tabWidth);

            this.tabIndex = index;
            Title = title;
            Font = font;
            Url = url;
            Parent = parent;

            if (string.IsNullOrEmpty(url))
            {
                this.PageState = WebPageState.Normal;
            }
            else
            {
                this.PageState = WebPageState.Loading;
            }
        }

        void tmAnimation_Tick(object sender, EventArgs e)
        {
            iCurFrame = (iCurFrame) % iFrameCount + 1;
            this.paintType = PaintType.PaintHeaerIcon;
            paintRequest();
        }

        public void DrawAll(Graphics g, Rectangle rect, bool drawLine = false)
        {
            try
            {
                rectFont = new Rectangle(rect.X + 25, 0, rect.Width - 40, rect.Height);
                rectFontLinearBrush = new Rectangle(rectFont.X + rectFont.Width - 35, rect.Y + 6, 35, rect.Height - 10);
                rectIcon = new Rectangle(rect.X + 8, rect.Height / 2 - 8, 16, 16);
                rectClose = new Rectangle(rect.X + rect.Width - 20, rect.Height / 2 - 6, 12, 12);
                //g.FillRectangle(new SolidBrush(Color.Red), rectClose);

                //Rectangle rectN = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
                //bool mouseOn = rect.Contains(Parent.PointToClient(Cursor.Position));
                
                drawRect(g, rect,drawLine);
                drawString(g, rectFont, rectFontLinearBrush, Title, Font);
                drawTabIcon(g, rectIcon);

                //drawClose(g, rectClose, rectClose.Contains(Parent.PointToClient(Cursor.Position)));
            }
            catch { }
        }

        private void drawRect(Graphics g, Rectangle rect,  bool drawLine)
        {
            /*
            GraphicsPath path = new GraphicsPath();

            path = new GraphicsPath();
            path.AddBezier(
                new Point(rect.X, rect.Bottom),
                new Point(rect.X + 3, rect.Bottom - 2),
                new Point(rect.X + 3, rect.Bottom - 2),
                new Point(rect.X + 4, rect.Bottom - 4));
            //path.AddLine(rect.X + 4, rect.Bottom - 4, rect.Left + 15 - 4, rect.Y + 4);
            path.AddBezier(
                new Point(rect.Left + 15 - 4, rect.Y + 4),
                new Point(rect.Left + 15 - 3, rect.Y + 2),
                new Point(rect.Left + 15 - 3, rect.Y + 2),
                new Point(rect.Left + 15, rect.Y));
            //path.AddLine(rect.Left + 15, rect.Y, rect.Right - 15, rect.Y);
            path.AddBezier(
                new Point(rect.Right - 15, rect.Y),
                new Point(rect.Right - 15 + 3, rect.Y + 2),
                new Point(rect.Right - 15 + 3, rect.Y + 2),
                new Point(rect.Right - 15 + 4, rect.Y + 4));
            //path.AddLine(rect.Right - 15 + 4, rect.Y + 4, rect.Right - 4, rect.Bottom - 4);
            path.AddBezier(
                new Point(rect.Right - 4, rect.Bottom - 4),
                new Point(rect.Right - 3, rect.Bottom - 3),
                new Point(rect.Right - 3, rect.Bottom - 3),
                new Point(rect.Right, rect.Bottom));

            region = new System.Drawing.Region(path);

            using (Pen p = new Pen(Color.Black))
            {
                g.DrawPath(p, path);
            }

            using (SolidBrush brush = new SolidBrush(Selected ? Color.White : noSelectedColor))
            {
                g.FillPath(brush, path);
            }
            using (Pen p = new Pen(Selected ? Color.White : BottomLineColor, 1))
            {
                g.DrawLine(p, rect.X + 3, 27, rect.Right - 3, 27);
            }
            */


                Color color = new Color();
            if (Selected)
                color = Color.White;
            //else if (mouseOn)
            //    color = Color.FromArgb(207,212,220);
            else
                color = Color.FromArgb(225,228,233);
            
            using (SolidBrush brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, rect);
            }

            if(drawLine)
            using (Pen pen = new Pen(Color.FromArgb(183,187,191)))
            {
                g.DrawLine(pen, rect.Right-1, rect.Top + 6, rect.Right-1, rect.Bottom-6);
            }

        }

        private void drawString(Graphics g, Rectangle rect, Rectangle rectFontLinearBrush, string title, Font font)
        {
            //g.DrawString(title, font, brushFont, rect);

            //using (LinearGradientBrush brush = new LinearGradientBrush(rectFontLinearBrush, Color.Transparent, Selected ? Color.White : noSelectedColor, 0, false))
            //{
            //    g.FillRectangle(brush, rectFontLinearBrush);
            //}
            Color textColor = Color.FromArgb(55, 55, 55);
            //if (Selected)
               // textColor = Color.White;
            TextRenderer.DrawText(
                   g,
                   title,
                   this.Font/*SystemFonts.CaptionFont*/,
                   rect,
                   textColor,
                   TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private void drawTabIcon(Graphics g, Rectangle rect)
        {
            if (webPageState == WebPageState.Loading)
            {
                if (iCurFrame == 0)
                    g.DrawImage((Image)Resources.ResourceManager.GetObject("Marty_000" + (0).ToString("00")), rect);
                else
                    g.DrawImage((Image)Resources.ResourceManager.GetObject("Marty_000" + (iCurFrame - 1).ToString("00")), rect);
            }
            else
                g.DrawIcon(ChildForm.Icon, rect);
        }

        private void drawClose(Graphics g, Rectangle rect, bool mouseOn)
        {
            //SmoothingMode oldMode = g.SmoothingMode;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //Color color = Color.White;
            //if (mouseOn)
            //    color = Color.FromArgb(219, 68, 55);

            //Pen pen = new Pen(color, 1);
            //g.DrawPath(pen, ClosePath);
            //pen.Dispose();
            //g.SmoothingMode = oldMode;

            drawClose_(g, mouseOn);
        }

        GraphicsPath closePath = null;
        GraphicsPath ClosePath
        {
            get
            {
                if (closePath == null)
                    closePath = getClosePath();
                return closePath;
            }
        }

        private GraphicsPath getClosePath()
        {
            float l = 8;  //边长
            //MessageBox.Show((CloseRect.X + l / 2f).ToString());
            PointF leftTop = new PointF(rectClose.X + 2, rectClose.Top + 2/*rectClose.X + l / 2f, (28 - l) / 2f*/);  //左顶点坐标顶点

            //MessageBox.Show((leftTop.X + l).ToString());
            PointF rightTop = new PointF(leftTop.X + l, leftTop.Y);
            PointF leftBottom = new PointF(leftTop.X, leftTop.Y + l);
            PointF rightBottom = new PointF(rightTop.X, leftTop.Y + l);
            GraphicsPath path = new GraphicsPath();
            path.AddLine(leftTop, rightBottom);
            path.CloseFigure();
            //往右一个单位
            path.AddLine(new PointF(leftTop.X + 1, leftTop.Y), new PointF(rightBottom.X + 1, rightBottom.Y));
            path.CloseFigure();
            path.AddLine(rightTop, leftBottom);
            path.CloseFigure();
            path.AddLine(new PointF(rightTop.X + 1, rightTop.Y), new PointF(leftBottom.X + 1, leftBottom.Y));
            return path;
        }

        private void drawClose_(Graphics g, bool mouseOn)
        {

            SmoothingMode oldMode = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //如果要修改大小和位置
            float l = 8;  //边长
            //MessageBox.Show((CloseRect.X + l / 2f).ToString());
            PointF leftTop = new PointF(rectClose.X+2,rectClose.Top+2/*rectClose.X + l / 2f, (28 - l) / 2f*/);  //左顶点坐标顶点

            //MessageBox.Show((leftTop.X + l).ToString());
            PointF rightTop = new PointF(leftTop.X + l, leftTop.Y);
            PointF leftBottom = new PointF(leftTop.X, leftTop.Y + l);
            PointF rightBottom = new PointF(rightTop.X, leftTop.Y + l);
            GraphicsPath path = new GraphicsPath();
            path.AddLine(leftTop, rightBottom);
            path.CloseFigure();
            //往右一个单位
            path.AddLine(new PointF(leftTop.X + 1, leftTop.Y), new PointF(rightBottom.X + 1, rightBottom.Y));
            path.CloseFigure();
            path.AddLine(rightTop, leftBottom);
            path.CloseFigure();
            path.AddLine(new PointF(rightTop.X + 1, rightTop.Y), new PointF(leftBottom.X + 1, leftBottom.Y));
            //往右一个单位
            Color color = Color.Gray;
            if (mouseOn)
                color = Color.FromArgb(254, 28, 28);
            //else if (Selected)
                //color = Color.FromArgb(232,234,238);

            Pen pen = new Pen(color, 1);
            //MessageBox.Show(CloseRect.X.ToString());
            //g.FillRectangle(new SolidBrush(Color.Beige), CloseRect);
            g.DrawPath(pen, path);
            //drawsk(g);
            pen.Dispose();
            g.SmoothingMode = oldMode;
        }

        void drawCircle(Graphics g)
        {
            SmoothingMode oldMode = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            GraphicsPath path = new GraphicsPath();

            //path.AddLine(new PointF((float)(16 / Math.Sqrt(2)), (float)(16 / Math.Sqrt(2))), new PointF((float)(23/Math.Sqrt(2)), (float)(23 / Math.Sqrt(2))));


            float d = 15;            //直径

            float leftTopX = rectClose.X;    //左上角x坐标
            float leftTopY = 30 / 2f - d / 2f - 0;


            path.AddEllipse(leftTopX, leftTopY, d, d);


            float r = d * 0.5f;  //直径
            float a = leftTopX + r;  //圆心x
            float b = leftTopY + r;   //圆心y

            float k = (float)Math.Cos(45.0);
            PointF p1 = new PointF(a - k * r, b + k * r);
            PointF p2 = new PointF(a - 4 * k * r, b + 4 * k * r);

            //path.AddLine(p1, p2);

            Color color = Color.PaleVioletRed;
            

            
            SolidBrush brush = new SolidBrush(Color.FromArgb(219, 68, 55));
            g.FillPath(brush, path);
            brush.Dispose();
            g.SmoothingMode = oldMode;
        }

        public bool HitTest(Point cltPosition)
        {
            return Rect.Contains(cltPosition);
        }

        public bool CloseHitTest(Point cltPosition)
        {
            return rectClose.Contains(cltPosition);
        }

        Rectangle GenRect(int index, int tabWidth)
        {
            if (index == 0)
                return new Rectangle(1, 1, tabWidth, 30);
            else
            {
                Rectangle re = new Rectangle(index * tabWidth+1, 1, tabWidth, 30);
                //re.Offset(-index * Left_Offset, 0);
                return re;
            }
        }



        public void Dispose()
        {
            tmAnimation.Stop();
            IsDisposed = true;
        }

        public int CompareTo(object o)
        {
            TabHeader th = o as TabHeader;
            return this.tabIndex.CompareTo(th.TabIndex);
        }



        void paintRequest()
        {
            if (OnPaintRequest != null)
            {
                switch (paintType)
                {
                    case PaintType.PaintHeaerIcon:
                        OnPaintRequest(this, rectIcon);
                        break;
                }

            }
        }

        public enum WebPageState
        {
            Loading, Normal
        }

        public enum PaintType
        {
            PaintTabRect, PaintText, PaintHeaerIcon, PaintClose, All
        }
    }
}

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
    public class ZTextBoxEx : Control
    {

        Color borderColor = Color.FromArgb(184, 184, 184);
        ZTextBoxBase innerTextBox;
        bool closeShowed;

        public ZTextBoxEx()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, false);
            createInnerTextBox();
        }


        private void createInnerTextBox()
        {
            innerTextBox = new ZTextBoxBase();
            innerTextBox.Text = Text;
            innerTextBox.BackColor = BackColor;
            //innerTextBox.BackColor = Color.Red;
            innerTextBox.BorderStyle = BorderStyle.None;
            innerTextBox.MouseEnter += (s, e) =>
            {
                if (!innerTextBox.Focused)
                {
                    this.borderColor = BorderColorHover;
                    Invalidate();
                }
            };
            innerTextBox.GotFocus += (s, e) => { this.borderColor = BorderColorFocus; /*innerTextBox.SelectionStart = 0;*/ Invalidate(); };
            innerTextBox.LostFocus += (s, e) => { this.borderColor = BorderColorNormal; Invalidate(); };
            innerTextBox.TextAlignChanged += (s, e) => { base.Text = innerTextBox.Text; };
            innerTextBox.TextChanged += (s, e) =>
            {
                base.Text = innerTextBox.Text;
                if (innerTextBox.Text.Length > 0)
                {
                    if (!closeShowed)
                        Invalidate();
                }
                else
                {
                    if (closeShowed)
                        Invalidate();
                }

                //if (this.TextChanged != null)
                //    this.TextChanged(s, e);
            };


            Controls.Add(innerTextBox);

            AddEventHandler();
        }

        //string _Text = "";
        [Browsable(true)]
        public new string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                if (innerTextBox != null)
                    innerTextBox.Text = value;
                Invalidate();
            }
        }

        //string _WaterText = "";
        /// <summary>
        /// 水印文本
        /// </summary>
        /// 
        [Description("水印文本")]
        [DefaultValue("")]
        public string WaterText
        {
            get { return innerTextBox.WaterText; }
            set { innerTextBox.WaterText = value; }
        }

        [DefaultValue(false)]
        public bool UseSystemPasswordChar
        {
            get { return innerTextBox.UseSystemPasswordChar; }
            set { innerTextBox.UseSystemPasswordChar = value; }

        }

        [Description("是否多行显示")]
        [DefaultValue(false)]
        public bool Multiline { get; set; }

        Color _borderColorNormal = Color.FromArgb(184, 184, 184);
        [Description("边框颜色")]
        [DefaultValue(typeof(Color), "184,184,184")]
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
        [DefaultValue(typeof(Color), "71,154,255")]
        public Color BorderColorFocus
        {
            get { return _borderColorFocus; }
            set { _borderColorFocus = value; Invalidate(); }
        }

        Color _borderColorHover = Color.FromArgb(153, 153, 153);
        [Description("鼠标经过时边框颜色")]
        [DefaultValue(typeof(Color), "153,153,153")]
        public Color BorderColorHover
        {
            get { return _borderColorHover; }
            set { _borderColorHover = value; Invalidate(); }
        }

        Color _backColor = Color.White;
        bool isChaged = false;
        public override Color BackColor
        {
            get
            {
                if (isChaged)
                    return base.BackColor;
                else
                    return Color.White;

            }
            set
            {
                isChaged = true;
                base.BackColor = value;
                //_backColor = value;

                if (InnerTextBox != null)
                    InnerTextBox.BackColor = value;
                Invalidate();
            }
        }

        bool _showSearch = false;
        /// <summary>
        /// 是否显示搜索图标
        /// </summary>
        [Description("是否显示搜索图标")]
        [DefaultValue(false)]
        public bool ShowSearch
        {
            get { return _showSearch; }
            set { _showSearch = value; Invalidate(); }
        }

        bool _showClose = false;
        /// <summary>
        /// 是否显示关闭图标图标
        /// </summary>
        [Description("是否显示关闭图标图标")]
        [DefaultValue(false)]
        public bool ShowClose
        {
            get { return _showClose; }
            set { _showClose = value; Invalidate(); }
        }

        private Bitmap _Image = null;
        [Description("显示图片")]
        [DefaultValue(null)]
        public Bitmap Image
        {
            get { return _Image; }
            set { _Image = value; Invalidate(); }
        }

        private ImageStyle _ImageStyle = ImageStyle.Left;
        [Description("显示图片位置")]
        [DefaultValue(ImageStyle.Left)]
        public ImageStyle ImageStyle
        {
            get { return _ImageStyle; }
            set { _ImageStyle = value; Invalidate(); }
        }
        [DefaultValue('\0')]
        public char PasswordChar
        {
            get { return innerTextBox.PasswordChar; }
            set { innerTextBox.PasswordChar = value; }
        }

        //int _radius = 0;
        //[Description("圆角度")]
        //public int Radius
        //{
        //    get { return _radius; }
        //    set { _radius = value; Invalidate(); }
        //}

        bool _ShowBorder = true;
        [DefaultValue(true)]
        public bool ShowBorder { get => _ShowBorder; set => _ShowBorder = value; }


        /// <summary>
        /// 获取控件内部文本框
        /// </summary>
        [Browsable(false)]
        public TextBox InnerTextBox { get { return this.innerTextBox; } }

        [Browsable(false)]
        public override Image BackgroundImage { get; set; }
        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout { get; set; }


        private Rectangle CloseRect
        {
            get
            {
                if (!ShowClose)
                    return new Rectangle();
                Rectangle rec = new Rectangle(this.Width - 40, 0, 20, this.Height);
                if (!ShowSearch)
                    rec.X += 20;
                if (Image != null && ImageStyle == ImageStyle.Right)
                    rec.X -= ImageRect.Width;
                return rec;
            }
        }



        private Rectangle SearchRect
        {
            get
            {
                if (!ShowSearch)
                    return new Rectangle();
                Rectangle rec = new Rectangle(this.Width - 20, 0, 20, this.Height);
                if (Image != null && ImageStyle == ImageStyle.Right)
                    rec.X -= ImageRect.Width;
                return rec;
            }
        }

        private Rectangle ImageRect
        {
            get
            {
                if (Image == null)
                    return new Rectangle();

                Rectangle rec = new Rectangle(5, this.Height / 2 - 8, 16, 16);
                if (ImageStyle == ImageStyle.Right)
                    rec = new Rectangle(this.Width - 20, this.Height / 2 - 8, 16, 16);
                return rec;
            }
        }


        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            int diameter = radius;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));
            GraphicsPath path = new GraphicsPath();
            // 左上角  
            path.AddArc(arcRect, 180, 90);
            // 右上角  
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);
            // 右下角  
            arcRect.Y += rect.Height - diameter;
            path.AddArc(arcRect, 0, 90);
            // 左下角  
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);
            path.CloseFigure();
            return path;
        }



        //暂未使用
        //void drawBack(Graphics g)
        //{
        //    //g.SmoothingMode = SmoothingMode.HighQuality;
        //    Rectangle rec = new Rectangle(0, 0, ClientRectangle.Width-1, ClientRectangle.Height-1);

        //    SolidBrush brush = new SolidBrush(Enabled ? Color.Red : Color.FromArgb(240, 240, 240));
        //    innerTextBox.Visible = false;

        //    if (Radius > 0)
        //    {
        //        SmoothingMode oldMode = g.SmoothingMode;
        //        g.SmoothingMode = SmoothingMode.AntiAlias;
        //        GraphicsPath path = GetRoundedRectPath(rec, 10);
        //        g.FillPath(brush, path);
        //        g.SmoothingMode = oldMode;
        //        path.Dispose();
        //    }
        //    else
        //        g.FillRectangle(brush, rec);
        //    //this.Region
        //    brush.Dispose();
        //}

        protected override void OnPaint(PaintEventArgs e)
        {
            SmoothingMode oldMode = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //drawBack(e.Graphics);
            drawBorder(e.Graphics);
            drawSearch(e.Graphics);
            drawClose(e.Graphics);
            drawImage(e.Graphics);
            //SmoothingMode oldMode = e.Graphics.SmoothingMode;
            //e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //Rectangle rec = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height);
            //GraphicsPath path = DrawPath.CreateRoundedRect(rec,10);
            //this.Region = new Region(path);
            e.Graphics.SmoothingMode = oldMode;
            //path.Dispose();
            setLocation();
        }

        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //    base.OnPaintBackground(e);
        //    drawBack(e.Graphics);
        //}


        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Rectangle rec = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height);

            SolidBrush brush = new SolidBrush(Enabled ? BackColor : Color.FromArgb(240, 240, 240));
            e.Graphics.FillRectangle(brush, rec);
            brush.Dispose();
        }

        private void drawBorder(Graphics g)
        {
            if (!ShowBorder)
                return;

            SmoothingMode oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Pen pen = new Pen(borderColor, 1);
            Rectangle rec = new Rectangle(0, 0, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);

            //if (Radius > 0)
            //{
            //    GraphicsPath path = GetRoundedRectPath(rec, 10);

            //    CompositingMode old = g.CompositingMode;
            //    //g.CompositingMode = CompositingMode.SourceCopy;
            //    //SolidBrush solidBrush = new SolidBrush(Color.Transparent);
            //    //g.DrawPath(new Pen(Color.Transparent), path);



            //    g.DrawPath(pen, path);
            //    path.Dispose();
            //    g.CompositingMode = old;
            //}
            //else
            g.DrawRectangle(pen, rec);

            pen.Dispose();

            g.SmoothingMode = oldMode;
        }

        private void drawClose(Graphics g)
        {
            closeShowed = false;

            if (!ShowClose)
                return;
            if (Text.Length == 0)
                return;

            if (!innerTextBox.Focused)
                return;

            closeShowed = true;
            SmoothingMode oldMode = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //如果要修改大小和位置
            float l = 7;  //边长
            //MessageBox.Show((CloseRect.X + l / 2f).ToString());
            PointF leftTop = new PointF(CloseRect.X + l / 2f, (this.Height - l) / 2f);  //左顶点坐标顶点

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
            Color color = Color.CornflowerBlue;
            if (isMouseOnClose)
                color = Color.Red;

            Pen pen = new Pen(color, 1);
            //MessageBox.Show(CloseRect.X.ToString());
            //g.FillRectangle(new SolidBrush(Color.Beige), CloseRect);
            g.DrawPath(pen, path);
            g.SmoothingMode = oldMode;
            pen.Dispose();
        }

        private void drawSearch(Graphics g)
        {
            //return;
            if (!ShowSearch)
                return;

            SmoothingMode oldMode = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            GraphicsPath path = new GraphicsPath();

            //path.AddLine(new PointF((float)(16 / Math.Sqrt(2)), (float)(16 / Math.Sqrt(2))), new PointF((float)(23/Math.Sqrt(2)), (float)(23 / Math.Sqrt(2))));


            float d = 8;            //直径

            float leftTopX = this.Width - 15;    //左上角x坐标
            float leftTopY = this.Height / 2f - d / 2f - 2;


            path.AddEllipse(leftTopX, leftTopY, d, d);


            float r = d * 0.5f;  //直径
            float a = leftTopX + r;  //圆心x
            float b = leftTopY + r;   //圆心y

            float k = (float)Math.Cos(45.0);
            PointF p1 = new PointF(a - k * r, b + k * r);
            PointF p2 = new PointF(a - 4 * k * r, b + 4 * k * r);

            path.AddLine(p1, p2);

            Color color = Color.Gray;
            if (isMouseOnSearch)
                color = Color.Black;

            Pen pen = new Pen(color, 2);
            g.DrawPath(pen, path);
            pen.Dispose();
            g.SmoothingMode = oldMode;
        }

        private void drawImage(Graphics g)
        {
            //g.FillRectangle(new SolidBrush(Color.Red), ImageRect);
            if (Image != null)
                g.DrawImage(Image, ImageRect);
        }




        private void setLocation()
        {
            if (Image == null)
            {
                innerTextBox.Location = new Point(2, (this.Height - innerTextBox.Height) / 2 + 1);
                innerTextBox.Width = this.Width - CloseRect.Width - SearchRect.Width - 5;
            }
            else
            {
                if (ImageStyle == ImageStyle.Left)
                {
                    innerTextBox.Location = new Point(ImageRect.Right + 2, (this.Height - innerTextBox.Height) / 2 + 1);
                    innerTextBox.Width = this.Width - CloseRect.Width - SearchRect.Width - ImageRect.Width - 10;

                }
                else
                {
                    innerTextBox.Location = new Point(2, (this.Height - innerTextBox.Height) / 2 + 1);
                    innerTextBox.Width = this.Width - CloseRect.Width - SearchRect.Width - ImageRect.Width - 5;
                }
            }

            if (!innerTextBox.Focused)
            {
                innerTextBox.Width += CloseRect.Width;
                //innerTextBox.Select(innerTextBox.Text.Length, 0);
                innerTextBox.SelectionStart = innerTextBox.Text.Length;
            }
            //innerTextBox.Width = this.Width - ImageRect.Width - CloseRect.Width - SearchRect.Width - 4;

        }

        bool isMouseOnSearch = false;
        bool isMouseOnClose = false;

        private void isMouseOnSearchOnClose(Point mousePoint)
        {
            //Point point = this.PointToClient(MousePosition);

            bool isMouseOnSearchBefore = isMouseOnSearch;
            bool isMouseOnCloseBefore = isMouseOnClose;

            isMouseOnSearch = SearchRect.Contains(mousePoint);

            isMouseOnClose = CloseRect.Contains(mousePoint);

            if (isMouseOnSearchBefore != isMouseOnSearch)
                Invalidate();

            if (isMouseOnCloseBefore != isMouseOnClose)
                Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //innerTextBox.Text = this.PointToClient(MousePosition).ToString();
            isMouseOnSearchOnClose(e.Location);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            innerTextBox.Focus();
            innerTextBox.SelectionStart = innerTextBox.TextLength;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (!innerTextBox.Focused)
            {
                this.borderColor = BorderColorHover;
                //Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!innerTextBox.Focused)
            {
                this.borderColor = BorderColorNormal;
                Invalidate();
            }

            isMouseOnSearchOnClose(this.PointToClient(MousePosition));
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (isMouseOnSearch)
                isMouseOnSearchOnClose(this.PointToClient(MousePosition));
        }


        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (!innerTextBox.Focused)
            {
                this.borderColor = BorderColorNormal;
                //Invalidate();
            }

            if (ShowClose && CloseRect.Contains(e.Location))
                innerTextBox.Clear();

            if (ShowSearch && SearchButtonClick != null && SearchRect.Contains(e.Location))
                SearchButtonClick(this, e);

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Multiline)
                innerTextBox.Height = this.ClientRectangle.Height - 4;

        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            innerTextBox.Multiline = Multiline;
        }

        public event EventHandler SearchButtonClick;
        //public new event EventHandler TextChanged;

        //需要添加事件,请在这里添加
        private void AddEventHandler()
        {

            innerTextBox.Click += (s, e) => base.OnClick(e);
            innerTextBox.DoubleClick += (s, e) => base.OnDoubleClick(e);
            innerTextBox.MouseClick += (s, e) => base.OnMouseClick(e);
            innerTextBox.MouseDoubleClick += (s, e) => base.OnMouseDoubleClick(e);

            innerTextBox.KeyDown += (s, e) => base.OnKeyDown(e);
            innerTextBox.KeyPress += (s, e) => base.OnKeyPress(e);
            innerTextBox.KeyUp += (s, e) => base.OnKeyUp(e);
            //innerTextBox.TextChanged += (s, e) => base.OnTextChanged(e);

        }

    }

    public enum ImageStyle
    {
        Left = 0,
        Right = 1
    }
}

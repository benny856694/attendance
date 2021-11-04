using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ZXCL.WinFormUI
{
    public partial class ZForm : Form
    {
        [Browsable(false)]
        public ZFormMdi MdiForm { get; set; }
        int controlBoxHeight = 25;
        int controlBoxWidth = 30;

        public ZForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            //Screen currentScreen = Screen.FromControl(this);
            //this.MaximizedBounds = currentScreen.WorkingArea;
        }


        #region 工具栏暴露隐藏的属性

        int captionHeight = 30;
        [Description("标题栏高度")]
        [DefaultValue(30)]
        public int CaptionHeight
        {
            get { return captionHeight; }
            set
            {
                captionHeight = value;
                Padding = new Padding(base.Padding.Left, captionHeight + 1, base.Padding.Right, base.Padding.Bottom);
                //MessageBox.Show("");
                //Refresh();
                Invalidate();
            }
        }

        Font _CaptionFont = new Font("宋体", 9f);
        public Font CaptionFont
        {
            get { return _CaptionFont; }
            set { _CaptionFont = value; this.Invalidate(); }
        }

        [Description("标题文字左间距")]
        [DefaultValue(0)]
        public int CaptionLeftSpacing { get; set; }

        public new Padding Padding
        {
            get { return new Padding(base.Padding.Left, captionHeight + 1, base.Padding.Right, base.Padding.Bottom); }
            set { base.Padding = new Padding(value.Left, captionHeight + 1, value.Right, value.Bottom); }
        }


        Color captionColorStart = Color.Transparent;
        [Description("标题颜色开始(渐变)")]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color CaptionColorStart
        {
            get { return captionColorStart; }
            set { captionColorStart = value; Invalidate(); }
        }

        Color captionColorEnd = Color.Transparent;
        [Description("标题颜色结束(渐变)")]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color CaptionColorEnd
        {
            get { return captionColorEnd; }
            set { captionColorEnd = value; Invalidate(); }
        }

        LinearGradientMode captionColorMode = LinearGradientMode.Vertical;
        [Description("标题颜色渐变方式")]
        [DefaultValue(LinearGradientMode.Vertical)]
        public LinearGradientMode CaptionColorMode
        {
            get { return captionColorMode; }
            set { captionColorMode = value; Invalidate(); }
        }

        Color borderColor = Color.FromArgb(0, 144, 198);
        [Description("边框颜色")]
        [DefaultValue(typeof(Color), "0,144,198")]
        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; Invalidate(); }
        }

        bool showIconOnCaptionon = true;
        [Description("是否在标题栏显示图标")]
        [DefaultValue(true)]
        public bool ShowIconOnCaptionon
        {
            get { return showIconOnCaptionon; }
            set { showIconOnCaptionon = value; Invalidate(); }
        }

        bool showCaptiononText = true;
        [Description("是否在标题栏显示文字")]
        [DefaultValue(true)]
        public bool ShowCaptiononText
        {
            get { return showCaptiononText; }
            set { showCaptiononText = value; Invalidate(); }
        }

        Size iconSize = new Size(20, 20);
        [Description("标题栏图标尺寸")]
        [DefaultValue(typeof(Size), "20,20")]
        public Size IconSize
        {
            get { return iconSize; }
            set { iconSize = value; Invalidate(); }
        }

        Color captionTextColor = Color.Black;
        [Description("标题栏文字颜色")]
        [DefaultValue(typeof(Color), "Black")]
        public Color CaptionTextColor
        {
            get { return captionTextColor; }
            set { captionTextColor = value; Invalidate(); }
        }

        [Description("标题栏文字")]
        [Browsable(true)]
        [DefaultValue("")]
        public new string Text
        {
            get { return base.Text; }
            set { base.Text = value; Invalidate(); }
        }

        CloseBoxColor closeBoxColor = new CloseBoxColor();
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("关闭按钮颜色")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CloseBoxColor CloseBoxColor
        {
            get { closeBoxColor.OnColorChange = refreshControlBox; return closeBoxColor; }
        }

        ControlBoxColor minimizeBoxColor = new ControlBoxColor();
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("最小化按钮颜色")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ControlBoxColor MinimizeBoxColor
        {
            get { minimizeBoxColor.OnColorChange = refreshControlBox; return minimizeBoxColor; }
        }


        ControlBoxColor maximizeBoxColor = new ControlBoxColor();
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("按钮颜色")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ControlBoxColor MaximizeBoxColor
        {
            get { maximizeBoxColor.OnColorChange = refreshControlBox; return maximizeBoxColor; }
        }

        [Browsable(false)]
        public bool IsDialog { get; set; }

        bool resizable = true;
        [DefaultValue(true)]
        [Description("是否可改变窗口大小")]
        public bool Resizable
        {
            get { return resizable; }
            set { resizable = value; }
        }
        [Description("在该窗体的标题栏中是否显示控件框")]
        [Browsable(true)]
        public new bool ControlBox
        {
            get { return base.ControlBox; }
            set { base.ControlBox = value; controlBoxVisibleChange(); }
        }


        [Description("是否在窗体的标题栏中显示“最大化”按钮")]
        [Browsable(true)]
        public new bool MaximizeBox
        {
            get { return base.MaximizeBox; }
            set { base.MaximizeBox = value; controlBoxVisibleChange(); }
        }

        [Description("是否在窗体的标题栏中显示“最小化”按钮")]
        [Browsable(true)]
        public new bool MinimizeBox
        {
            get { return base.MinimizeBox; }
            set { base.MinimizeBox = value; controlBoxVisibleChange(); }
        }
        bool _DropDwon = false;
        [Description("是否在窗体的标题栏中显示“下拉”按钮")]
        [Browsable(true)]
        [DefaultValue(false)]
        public bool DropDwon
        {
            get { return _DropDwon; }
            set { _DropDwon = value; controlBoxVisibleChange(); }
        }

        ////在属性工具栏隐藏此属性
        /// <summary>
        /// 此属性设置无效
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new FormBorderStyle FormBorderStyle
        {
            get { return base.FormBorderStyle; }
            set { base.FormBorderStyle = value; }
        }

        public event Action OnCloseBoxClick;

        public event Action<object> OnDropDownClick;

        #region ControlBox图片相关属性

        //私有属性, 如果需要自定义图片可设为公共

        Bitmap imageCloseNormal;
        Bitmap ImageCloseNormal
        {
            get
            {
                if (imageCloseNormal == null)
                    imageCloseNormal = ControlBoxDrawImage.DrawImage(CloseBoxColor.ForeColorNormal, CloseBoxColor.BackColorNormal, ControlBoxDrawImage.ImgeType.Close);
                return imageCloseNormal;
            }
            set { imageCloseNormal = value; }
        }

        Bitmap imageCloseHover;
        /// <summary>
        /// 关闭按钮鼠标经过显示图片
        /// </summary>
        [Browsable(false)]
        Bitmap ImageCloseHover
        {
            get
            {
                if (imageCloseHover == null)
                    imageCloseHover = ControlBoxDrawImage.DrawImage(CloseBoxColor.ForeColorHover, CloseBoxColor.BackColorHover, ControlBoxDrawImage.ImgeType.Close);
                return imageCloseHover;
            }
            set { imageCloseHover = value; }
        }

        Bitmap imageClosePress;
        /// <summary>
        /// 关闭按钮鼠标按下时图片
        /// </summary>
        [Browsable(false)]
        Bitmap ImageClosePress
        {
            get
            {
                if (imageClosePress == null)
                    imageClosePress = ControlBoxDrawImage.DrawImage(CloseBoxColor.ForeColorPress, CloseBoxColor.BackColorPress, ControlBoxDrawImage.ImgeType.Close);
                return imageClosePress;
            }
            set { imageClosePress = value; }
        }

        Bitmap imageMaximizeNormal;
        /// <summary>
        /// 最大化按钮图片
        /// </summary>
        [Browsable(false)]
        Bitmap ImageMaximizeNormal
        {
            get
            {
                if (imageMaximizeNormal == null)
                    imageMaximizeNormal = ControlBoxDrawImage.DrawImage(MaximizeBoxColor.ForeColorNormal, MaximizeBoxColor.BackColorNormal, ControlBoxDrawImage.ImgeType.Maximize);
                return imageMaximizeNormal;
            }
            set { imageMaximizeNormal = value; }
        }

        Bitmap imageMaximizeHover;
        /// <summary>
        /// 最大化按钮鼠标经过是图片
        /// </summary>
        [Browsable(false)]
        Bitmap ImageMaximizeHover
        {
            get
            {
                if (imageMaximizeHover == null)
                    imageMaximizeHover = ControlBoxDrawImage.DrawImage(MaximizeBoxColor.ForeColorHover, MaximizeBoxColor.BackColorHover, ControlBoxDrawImage.ImgeType.Maximize);
                return imageMaximizeHover;
            }
            set { imageMaximizeHover = value; }
        }

        Bitmap imageMaximizePress;
        /// <summary>
        /// 最大化按钮鼠标按下时图片
        /// </summary>
        [Browsable(false)]
        Bitmap ImageMaximizePress
        {
            get
            {
                if (imageMaximizePress == null)
                    imageMaximizePress = ControlBoxDrawImage.DrawImage(MaximizeBoxColor.ForeColorPress, MaximizeBoxColor.BackColorPress, ControlBoxDrawImage.ImgeType.Maximize);
                return imageMaximizePress;
            }
            set { imageMaximizePress = value; }
        }

        Bitmap imageMinimizeNormal;
        /// <summary>
        /// 最小化按钮图片
        /// </summary>
        [Browsable(false)]
        Bitmap ImageMinimizeNormal
        {
            get
            {
                if (imageMinimizeNormal == null)
                    imageMinimizeNormal = ControlBoxDrawImage.DrawImage(MinimizeBoxColor.ForeColorNormal, MinimizeBoxColor.BackColorNormal, ControlBoxDrawImage.ImgeType.Minimize);
                return imageMinimizeNormal;
            }
            set { imageMinimizeNormal = value; }
        }

        Bitmap imageMinimizeHover;
        /// <summary>
        /// 最小化按钮鼠标经过时图片
        /// </summary>
        [Browsable(false)]
        Bitmap ImageMinimizeHover
        {
            get
            {
                if (imageMinimizeHover == null)
                    imageMinimizeHover = ControlBoxDrawImage.DrawImage(MinimizeBoxColor.ForeColorHover, MinimizeBoxColor.BackColorHover, ControlBoxDrawImage.ImgeType.Minimize);
                return imageMinimizeHover;
            }
            set { imageMinimizeHover = value; }
        }

        Bitmap imageMinimizePress;
        /// <summary>
        /// 最小化按钮鼠标按下时图片
        /// </summary>
        [Browsable(false)]
        Bitmap ImageMinimizePress
        {
            get
            {
                if (imageMinimizePress == null)
                    imageMinimizePress = ControlBoxDrawImage.DrawImage(MinimizeBoxColor.ForeColorPress, MinimizeBoxColor.BackColorPress, ControlBoxDrawImage.ImgeType.Minimize);
                return imageMinimizePress;
            }
            set { imageMinimizePress = value; }
        }


        Bitmap imageRestoreNormal;
        /// <summary>
        /// 窗口还原图片
        /// </summary>
        [Browsable(false)]
        Bitmap ImageRestoreNormal
        {
            get
            {
                if (imageRestoreNormal == null)
                    imageRestoreNormal = ControlBoxDrawImage.DrawImage(MaximizeBoxColor.ForeColorNormal, MaximizeBoxColor.BackColorNormal, ControlBoxDrawImage.ImgeType.Restore);
                return imageRestoreNormal;
            }
            set { imageRestoreNormal = value; }
        }

        Bitmap imageRestoreHover;
        /// <summary>
        /// 窗口还原鼠标经过时图片
        /// </summary>
        [Browsable(false)]
        Bitmap ImageRestoreHover
        {
            get
            {
                if (imageRestoreHover == null)
                    imageRestoreHover = ControlBoxDrawImage.DrawImage(MaximizeBoxColor.ForeColorHover, MaximizeBoxColor.BackColorHover, ControlBoxDrawImage.ImgeType.Restore);
                return imageRestoreHover;
            }
            set { imageRestoreHover = value; }
        }

        Bitmap imageRestorePress;
        /// <summary>
        /// 窗口还原鼠标按下时图片
        /// </summary>
        [Browsable(false)]
        Bitmap ImageRestorePress
        {
            get
            {
                if (imageRestorePress == null)
                    imageRestorePress = ControlBoxDrawImage.DrawImage(MaximizeBoxColor.ForeColorPress, MaximizeBoxColor.BackColorPress, ControlBoxDrawImage.ImgeType.Restore);
                return imageRestorePress;
            }
            set { imageRestorePress = value; }
        }

        Bitmap _imageDrop;
        [Browsable(false)]
        Bitmap ImageDrop
        {
            get
            {
                if (_imageDrop == null)
                {
                    int width = 30;
                    int height = 25;
                    Bitmap img = new Bitmap(width, height);

                    Graphics g = Graphics.FromImage(img);
                    SmoothingMode smoothingMode = g.SmoothingMode;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    //g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0,0, width, height));
                    //drawDropDowmBtn(g, new Rectangle(0,0,20,this.Height));

                    Color color = Color.Black;//.FromArgb(49, 49, 49);
                    //SolidBrush brush = new SolidBrush(color);
                    PointF o = new PointF(width / 2, (height + 3) / 2);

                    //画一个倒三角形 
                    int d = 10;  // 底边长度
                    int h = 5;  //高度
                    PointF a = new PointF(o.X - d / 2f, o.Y - h / 2f);
                    PointF b = new PointF(o.X + d / 2f, o.Y - h / 2f);
                    PointF c = new PointF(o.X, o.Y + h / 2f);
                    //g.FillPolygon(brush, new PointF[] { a, b, c });   //实心
                    //brush.Dispose();
                    using (Pen p = new Pen(color, 1.5f))
                    {
                        // g.DrawLine(p, a.X+1, a.Y - 2, b.X-1, b.Y - 2);
                        GraphicsPath path = new GraphicsPath();
                        path.AddLines(new PointF[] { a, c, b });
                        g.DrawPath(p, path);
                    }
                    g.SmoothingMode = smoothingMode;
                    g.Dispose();
                    return img;
                }

                return _imageDrop;
            }
            set { _imageDrop = value; }
        }


        #endregion

        #endregion

        #region 重写
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (base.Region != null)
                base.Region.Dispose();
            Rectangle rect = new Rectangle(Point.Empty, base.Size);
            if (this.WindowState == FormWindowState.Maximized)
                rect.Height -= SystemInformation.FrameBorderSize.Width;
            //int a = SystemInformation.FrameBorderSize.Width;
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(rect);
            this.Region = new Region(path);

            //if (this.Top < 0)
            //    this.Top = 10;


        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //窗口边框样式,下面这些样式并不会修改窗体样式, 因为截取的绘制消息. 但这些样式会影响窗体的一些行为.
            if (DesignMode)
                FormBorderStyle = FormBorderStyle.None;
            else if (this.Resizable)
                FormBorderStyle = FormBorderStyle.Sizable;
            else
                FormBorderStyle = FormBorderStyle.None;
            base.Padding = this.Padding;

            //创建ControlBox
            createControlBox();

        }



        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawFormBackground(e.Graphics);
            DrawFormBorder(e.Graphics);
            DrawCaption(e.Graphics);
            DrawFormIconAndText(e.Graphics);
            if (DesignMode)
                setControlBoxLocation();
        }




        #endregion

        #region 重绘相关方法

        private void DrawFormBackground(Graphics g)
        {

            using (SolidBrush sb = new SolidBrush(BackColor))
            {

                g.FillRectangle(sb, ClientRectangle);
            }
        }

        bool isWindowActive = true;
        private void DrawFormBorder(Graphics g)
        {
            Color color = BorderColor;
            if (!isWindowActive && !isFlashing)
            {
                if (IsDialog)
                    return;
                color = Color.Gray;
            }


            using (Pen p = new Pen(color))
            {

                Point p0 = new Point(ClientRectangle.X, ClientRectangle.Y);
                Point p1 = new Point(ClientRectangle.Width - 1, ClientRectangle.Y);
                Point p2 = new Point(ClientRectangle.Width - 1, ClientRectangle.Height - 1);
                Point p3 = new Point(ClientRectangle.X, ClientRectangle.Height - 1);

                g.DrawLines(p, new Point[] { p0, p1, p2, p3, p0 });

            }
        }

        private void DrawCaption(Graphics g)
        {
            Rectangle rec = new Rectangle(ClientRectangle.X + 1, ClientRectangle.Y + 1, ClientRectangle.Width - 2, CaptionHeight);
            using (LinearGradientBrush lb = new LinearGradientBrush(
                 rec,
                 captionColorStart,
                 captionColorEnd,
                 CaptionColorMode))
            {
                g.FillRectangle(lb, rec);
            }
        }

        private void DrawFormIconAndText(Graphics g)
        {
            Rectangle iconRect = new Rectangle();
            Rectangle textRect = new Rectangle();

            if (this.ShowIconOnCaptionon && this.Icon != null)
            {
                int spacing = (int)Math.Floor((CaptionHeight - IconSize.Height) * 0.5);
                iconRect.Location = new Point(spacing, spacing);
                iconRect.Size = IconSize;
                g.DrawIcon(this.Icon, iconRect);

            }

            if (ShowCaptiononText && !string.IsNullOrEmpty(Text))
            {
                textRect.X = iconRect.Right + CaptionLeftSpacing;
                textRect.Width = ClientRectangle.Width - 100;
                textRect.Height = CaptionHeight;
                TextRenderer.DrawText(
                    g,
                    Text,
                   _CaptionFont,
                    textRect,
                    CaptionTextColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
        }

        #endregion

        #region ControlBox相关方法

        private void setControlBoxLocation()
        {
            Control c = Controls["ControlBox_Close"];
            if (c != null && ControlBox)
                c.Location = new Point(ClientRectangle.Width - controlBoxWidth - 1, 1);
            Control c1 = Controls["ControlBox_Max"];
            if (c1 != null && MaximizeBox)
                c1.Location = new Point(ClientRectangle.Width - controlBoxWidth * 2, 1);
            Control c2 = Controls["ControlBox_Min"];
            if (c2 != null && MinimizeBox)
                c2.Location = new Point(ClientRectangle.Width - controlBoxWidth * (c1 == null ? 2 : 3), 1);

            Control c3 = Controls["ControlBox_Drop"];
            if (c3 != null)
            {
                c3.Visible = DropDwon;
                c3.Location = new Point(ClientRectangle.Width - controlBoxWidth * (c2 == null ? 3 : 4), 1);
            }

        }

        private void refreshControlBox()
        {
            Control c = Controls["ControlBox_Close"];
            if (c != null)
                c.BackgroundImage = ControlBoxDrawImage.DrawImage(CloseBoxColor.ForeColorNormal, CloseBoxColor.BackColorNormal, ControlBoxDrawImage.ImgeType.Close);
            Control c1 = Controls["ControlBox_Max"];
            if (c1 != null)
                c1.BackgroundImage = ControlBoxDrawImage.DrawImage(MaximizeBoxColor.ForeColorNormal, MaximizeBoxColor.BackColorNormal, ControlBoxDrawImage.ImgeType.Maximize);
            Control c2 = Controls["ControlBox_Min"];
            if (c2 != null)
                c2.BackgroundImage = ControlBoxDrawImage.DrawImage(MinimizeBoxColor.ForeColorNormal, MinimizeBoxColor.BackColorNormal, ControlBoxDrawImage.ImgeType.Minimize);
        }

        private void controlBoxVisibleChange()
        {
            if (!IsHandleCreated)
                return;

            if (!ControlBox)
            {
                Controls.RemoveByKey("ControlBox_Close");
                Controls.RemoveByKey("ControlBox_Max");
                Controls.RemoveByKey("ControlBox_Min");
                Controls.RemoveByKey("ControlBox_Drop");
            }

            if (!MaximizeBox)
                Controls.RemoveByKey("ControlBox_Max");

            if (!MinimizeBox)
                Controls.RemoveByKey("ControlBox_Min");

            if (!DropDwon)
                Controls.RemoveByKey("ControlBox_Drop");

            createControlBox();

            //refreshControlBox();
        }

        private void createControlBox()
        {
            if (ControlBox)
            {
                if (Controls["ControlBox_Close"] == null)
                {
                    PictureBox close = new PictureBox();
                    close.Name = "ControlBox_Close";
                    close.Size = new Size(controlBoxWidth, controlBoxHeight);
                    close.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    close.BackColor = Color.Transparent;
                    close.BackgroundImageLayout = ImageLayout.Stretch;
                    close.BackgroundImage = ImageCloseNormal;
                    close.Click += (a, b) => {
                        if (OnCloseBoxClick != null)
                            OnCloseBoxClick();
                        this.Close();
                    };
                    close.MouseEnter += (a, b) => close.BackgroundImage = ImageCloseHover;
                    close.MouseLeave += (a, b) => close.BackgroundImage = ImageCloseNormal;
                    close.MouseDown += (a, b) => close.BackgroundImage = ImageClosePress;
                    this.Controls.Add(close);
                    this.Tip.SetToolTip(close, Strings.Close);
                }

                if (MaximizeBox && Controls["ControlBox_Max"] == null)
                {
                    PictureBox max = new PictureBox();
                    max.Name = "ControlBox_Max";
                    max.Size = new Size(controlBoxWidth, controlBoxHeight);
                    max.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    max.BackColor = Color.Transparent;
                    max.BackgroundImageLayout = ImageLayout.Stretch;
                    max.BackgroundImage = ImageMaximizeNormal;
                    if (Resizable)
                    {
                        max.MouseEnter += (s, e) => max.BackgroundImage = ImageMaximizeHover;
                        max.MouseLeave += (s, e) => max.BackgroundImage = ImageMaximizeNormal;
                        max.MouseDown += (s, e) => max.BackgroundImage = ImageMaximizePress;
                        max.Click += (s, e) =>
                        {
                            this.Tip.Active = false;
                            if (this.WindowState == FormWindowState.Maximized)
                                this.WindowState = FormWindowState.Normal;
                            else if (this.WindowState == FormWindowState.Normal)
                                this.WindowState = FormWindowState.Maximized;
                        };
                        this.Resize += (s, e) =>
                        {
                            this.Tip.Active = true;
                            if (!max.IsDisposed)
                            {
                                if (this.WindowState == FormWindowState.Maximized)
                                {
                                    max.BackgroundImage = ImageRestoreNormal;
                                    max.MouseEnter += (a, b) => max.BackgroundImage = ImageRestoreHover;
                                    max.MouseLeave += (a, b) => max.BackgroundImage = ImageRestoreNormal;
                                    max.MouseDown += (a, b) => max.BackgroundImage = ImageRestorePress;
                                    this.Tip.SetToolTip(max, Strings.Restore);
                                }
                                else if (this.WindowState == FormWindowState.Normal)
                                {
                                    max.BackgroundImage = ImageMaximizeNormal;
                                    max.MouseEnter += (a, b) => max.BackgroundImage = ImageMaximizeHover;
                                    max.MouseLeave += (a, b) => max.BackgroundImage = ImageMaximizeNormal;
                                    max.MouseDown += (a, b) => max.BackgroundImage = ImageMaximizePress;
                                    this.Tip.SetToolTip(max, Strings.Maximize);
                                }
                            }
                        };
                        this.Tip.SetToolTip(max, Strings.Maximize);
                    }
                    this.Controls.Add(max);
                }

                if (MinimizeBox && Controls["ControlBox_Min"] == null)
                {
                    PictureBox min = new PictureBox();
                    min.Name = "ControlBox_Min";
                    min.Size = new Size(controlBoxWidth, controlBoxHeight);
                    min.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    min.BackColor = Color.Transparent;
                    min.BackgroundImageLayout = ImageLayout.Stretch;
                    min.BackgroundImage = ImageMinimizeNormal;
                    min.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
                    min.MouseEnter += (s, e) => min.BackgroundImage = ImageMinimizeHover;
                    min.MouseLeave += (s, e) => min.BackgroundImage = ImageMinimizeNormal;
                    min.MouseDown += (s, e) => min.BackgroundImage = ImageMinimizePress;
                    this.Controls.Add(min);
                    this.Tip.SetToolTip(min, Strings.Minimize);
                }

                if (DropDwon && Controls["ControlBox_Drop"] == null)
                {
                    PictureBox drop = new PictureBox();
                    drop.Name = "ControlBox_Drop";
                    drop.Size = new Size(controlBoxWidth, controlBoxHeight);
                    drop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    drop.BackColor = Color.Transparent;
                    //drop.ImageLayout = ImageLayout.Center;
                    drop.Image = ImageDrop;

                    drop.Click += (s, e) =>
                    {
                        if (OnDropDownClick != null)
                            OnDropDownClick(drop);
                    };
                    drop.MouseEnter += (s, e) => drop.BackColor = Color.FromArgb(211, 214, 218);
                    drop.MouseLeave += (s, e) => drop.BackColor = Color.Transparent;
                    drop.MouseDown += (s, e) => drop.BackColor = Color.FromArgb(204, 204, 204);
                    this.Controls.Add(drop);
                    this.Tip.SetToolTip(drop, Strings.Settings);
                }

                setControlBoxLocation();

            }
        }


        #endregion

        #region Windows窗口消息控制
        //修改窗口消息
        protected override void WndProc(ref Message m)
        {
            if (!DesignMode)  //在VS设计模式下不启用
            {
                bool alreadyHandled = false;
                //if (m.Msg == 0x46 && !this.CanFocus && MyHandler != null)
                //{
                //    MyHandler();
                //}
                switch (m.Msg)
                {
                    case (int)WinAPI.WindowMessages.WM_NCCALCSIZE:
                        alreadyHandled = WmNcCalcSize(ref m);
                        break;

                    case (int)WinAPI.WindowMessages.WM_NCHITTEST:
                        alreadyHandled = WmNcHitTest(ref m);
                        break;

                    case (int)WinAPI.WindowMessages.WM_NCACTIVATE:
                        alreadyHandled = WmNcActivate(ref m);
                        if (m.WParam == IntPtr.Zero)
                        {
                            isWindowActive = false;
                            this.Invalidate();
                        }
                        else
                        {
                            isWindowActive = true;
                            this.Invalidate();

                        }
                        break;

                    case (int)WinAPI.WindowMessages.WM_NCPAINT:
                        alreadyHandled = true;
                        break;
                    case (int)WinAPI.WindowMessages.WM_NCLBUTTONDBLCLK:
                        if (!this.Resizable)
                            return;
                        break;
                    case 0x46:
                        break;

                    default:
                        break;
                }

                if (!alreadyHandled)
                    base.WndProc(ref m);
            }
            else
                base.WndProc(ref m);

        }


        private bool isConvert = true;
        private bool isAboutToMaximize(WinAPI.RECT rect)
        {
            /*
             * 判断的方法是，只要窗体的左右、上下都延伸到了屏幕工作区之外，
             * 并且左和右、上和下都延伸相同的量，就认为窗体是要进行最大化
             */

            int left = rect.Left;
            int top = rect.Top;
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            if (left < 0 && top < 0)
            {
                Rectangle workingArea = Screen.GetWorkingArea(this);
                //this.MaximumSize = workingArea.Size;
                if (width == (workingArea.Width + (-left) * 2)
                    && height == (workingArea.Height + (-top) * 2))
                    return true;
            }
            return false;
        }

        //调整最大化尺寸(最大化后尺寸会偏大一点, 所以对尺寸做调整)
        private bool WmNcCalcSize(ref Message m)
        {
            if (m.WParam == new IntPtr(1))
            {
                WinAPI.NCCALCSIZE_PARAMS info = (WinAPI.NCCALCSIZE_PARAMS)
                    Marshal.PtrToStructure(m.LParam, typeof(WinAPI.NCCALCSIZE_PARAMS));
                if (isAboutToMaximize(info.rectNewForm))
                {
                    Rectangle workingRect = Screen.GetWorkingArea(this);
                    info.rectNewForm.Left = workingRect.Left - 1;
                    info.rectNewForm.Top = workingRect.Top - 1;
                    info.rectNewForm.Right = workingRect.Right + 1;
                    info.rectNewForm.Bottom = workingRect.Bottom + 1;
                    Marshal.StructureToPtr(info, m.LParam, false);
                }
            }

            if (isConvert)
            {
                Size = ClientSize;
                isConvert = false;
            }

            return true;
        }

        //窗口拖拽, 窗口尺寸拖拉
        protected virtual bool WmNcHitTest(ref Message m)
        {
            int para = m.LParam.ToInt32();
            int x0 = WinAPI.LOWORD(para);
            int y0 = WinAPI.HIWORD(para);
            Point p = PointToClient(new Point(x0, y0));

            if (this.Resizable)
            {
                if (new Rectangle(0, 0, 5, 5).Contains(p))  //左上角
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTTOPLEFT);
                    return true;
                }

                if (new Rectangle(5, 0, ClientRectangle.Width - 10, 3).Contains(p))  //顶部
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTTOP);
                    return true;
                }

                if (new Rectangle(ClientRectangle.Width - 5, 0, 5, 5).Contains(p))  //右上角
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTTOPRIGHT);
                    return true;
                }

                if (new Rectangle(0, 5, 3, ClientRectangle.Height - 10).Contains(p))   //左侧
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTLEFT);
                    return true;
                }

                if (new Rectangle(ClientRectangle.Width - 3, 5, 3, ClientRectangle.Height - 10).Contains(p))   //右侧
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTRIGHT);
                    return true;
                }

                if (new Rectangle(0, ClientRectangle.Height - 5, 5, 5).Contains(p))   //左下
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTBOTTOMLEFT);
                    return true;
                }

                if (new Rectangle(5, ClientRectangle.Height - 3, ClientRectangle.Width - 10, 3).Contains(p))//底部
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTBOTTOM);
                    return true;
                }

                if (new Rectangle(ClientRectangle.Width - 5, ClientRectangle.Height - 5, 5, 5).Contains(p))  //右下
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTBOTTOMRIGHT);
                    return true;
                }
            }

            if (new Rectangle(0, 0, ClientRectangle.Width, captionHeight).Contains(p))
            {
                m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTCAPTION);
                return true;
            }

            m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTCLIENT);
            return true;
        }

        private bool WmNcActivate(ref Message m)
        {
            // something here
            m.Result = WinAPI.TRUE;
            return true;
        }


        //重写该方法解决窗体每次还原都会变大的问题    
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (base.WindowState == FormWindowState.Normal)
            {
                if (this.Size == this.ClientSize)
                {
                    if (width == (this.Size.Width + SystemInformation.FrameBorderSize.Width * 2))
                    //if (width == (this.Size.Width + 4 * 2) || width == (this.Size.Width + 8 * 2))
                    {
                        width = this.Size.Width;
                        height = this.Size.Height;
                    }
                }
            }
            base.SetBoundsCore(x, y, width, height, specified);
        }

        #region Dialog
        public new DialogResult ShowDialog(IWin32Window owner)
        {
            IsDialog = true;
        	return base.ShowDialog(owner);
        }

        public new DialogResult ShowDialog()
        {
            IsDialog = true;
            return base.ShowDialog();
          
        }

        #endregion


        #endregion

        bool isFlashing = false;
        public void BorderFlash()
        {
            if (!isFlashing)
            {

                isFlashing = true;
                Thread td = new Thread(() =>
                {
                    Color old = this.BorderColor;

                    for (int i = 0; i < 10; i++)
                    {
                        if (i % 2 == 0)
                            this.BorderColor = Color.Transparent;
                        else
                            this.BorderColor = old == Color.Transparent ? Color.Crimson : old;
                        Thread.Sleep(100);
                    }
                    this.BorderColor = old;
                    isFlashing = false;
                });
                td.IsBackground = true;
                td.Start();

            }
        }

        #region 通知窗口闪烁  暂未使用
        /*
        protected override void DefWndProc(ref Message m)
        {
            Console.WriteLine(m);
            base.DefWndProc(ref m);

            Form ownerForm = this.Owner;
            
            if (ownerForm == null || ownerForm.CanFocus)
                return;

            if (m.Msg != 0x46)  //WM_WINDOWPOSCHANGING
                return;

            bool mouseOnOwnerForm = ownerForm.ClientRectangle.Contains(ownerForm.PointToClient(MousePosition));
            bool mouseOnThisForm = this.ClientRectangle.Contains(this.PointToClient(MousePosition));

            if (mouseOnOwnerForm && !mouseOnThisForm)
                borderFlash();
        }

        bool isFlashing = false;
        private void borderFlash()
        {
            if (!isFlashing)
            {

                isFlashing = true;
                Thread td = new Thread(() =>
                {
                    Color old = this.BorderColor;

                    for (int i = 0; i < 10; i++)
                    {
                        if (i % 2 == 0)
                            this.BorderColor = Color.Transparent;
                        else
                            this.BorderColor = old == Color.Transparent ? Color.Crimson : old;
                        Thread.Sleep(100);
                    }
                    this.BorderColor = old;
                    isFlashing = false;
                });
                td.IsBackground = true;
                td.Start();

            }
        }
        */
        #endregion


    }

}

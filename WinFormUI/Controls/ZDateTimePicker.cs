using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZXCL.WinFormUI
{
    public class ZDateTimePicker : Control
    {

        bool isDowned = false;
        PictureBox dropDownButton;
        Color borderColor = Color.FromArgb(184, 184, 184);

        public ZDateTimePicker()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, false);
            createDropDownButton();
        }

        string _Text = "";
        [Browsable(true)]
        [DefaultValue("")]
        public override string Text
        {
            get { return _Text; }
            set
            {
                //MessageBox.Show(value);
                _Text = value;
                if (InnerTextBox != null)
                    InnerTextBox.Text = value;
                Invalidate();
            }
        }



        string _Format = "yyyy-MM-dd";   //yyyy-MM-dd HH:mm:ss
        [Description("日期格式")]
        [DefaultValue("yyyy-MM-dd")]
        public string Format
        {
            get { return _Format; }
            set { _Format = value; }
        }


        [Browsable(false)]
        public DateTime Value
        {
            get { return InnerMonthCalendar.SelectionStart; }
        }

        //[Description("是否显示时间(时分秒)修改框")]
        //public bool ShowTimerBox { get; set; }



        string _WaterText = "";
        /// <summary>
        /// 水印文本
        /// </summary>
        [Description("水印文本")]
        [DefaultValue("")]
        public string WaterText
        {
            get
            {
                if (InnerTextBox != null)
                    return InnerTextBox.WaterText;
                else
                    return _WaterText;

            }
            set
            {
                if (InnerTextBox != null)
                    InnerTextBox.WaterText = value;
                else
                    _WaterText = value;
            }
        }


        bool _editable = false;
        /// <summary>
        /// 是否可编辑文本
        /// </summary>
        [Description("是否可编辑文本")]
        [DefaultValue(false)]
        public bool Editable
        {
            get { return _editable; }
            set { _editable = value; Invalidate(); }
        }

        /// <summary>
        /// 指示是否启用该控件
        /// </summary>
        [Description("指示是否启用该控件")]
        public new bool Enabled
        {
            get { return base.Enabled; }
            set { base.Enabled = value; Invalidate(); }
        }

    



        Color _borderColorNormal = Color.FromArgb(184, 184, 184);
        [Description("边框颜色")]
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
        [Description("边框获得焦点时颜色(在不可编辑状态,此颜色不可用)")]
        public Color BorderColorFocus
        {
            get { return _borderColorFocus; }
            set { _borderColorFocus = value; Invalidate(); }
        }

        Color _borderColorHover = Color.FromArgb(153, 153, 153);
        [Description("鼠标经过时边框颜色")]
        public Color BorderColorHover
        {
            get { return _borderColorHover; }
            set { _borderColorHover = value; Invalidate(); }
        }

        bool _ShowBorder = true;
        public bool ShowBorder { get => _ShowBorder; set => _ShowBorder = value; }

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



        /// <summary>
        /// 展开下拉框
        /// </summary>
        [Browsable(false)]
        public void ShowDropDown()
        {
           // InnerDropDown.Size = new Size(InnerMonthCalendar.Width + 20, InnerMonthCalendar.Height + 20);

            if (canShowBelowControl(InnerDropDown.Height))
            {
                InnerDropDown.Show(this, 0, this.Height + 1);
            }
            else
            {
                InnerDropDown.Show(this, 0, -InnerDropDown.Height - 1);
            }
        }

        //如果需要自定义图片可以将属性设为公共
        Bitmap _imageDrop;
        Bitmap ImageDrop
        {
            get
            {
                if (_imageDrop == null)
                {
                    int width = 18;
                    int height = 20;
                    Bitmap img = new Bitmap(width, height);

                    Graphics g = Graphics.FromImage(img);

                    //g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0,0, width, height));
                    //drawDropDowmBtn(g, new Rectangle(0,0,20,this.Height));

                    SolidBrush brush = new SolidBrush(Color.Gray);
                    PointF o = new PointF(width / 2, height / 2);

                    //画一个倒三角形 
                    int d = 10;  // 底边长度
                    int h = 5;  //高度
                    PointF a = new PointF(o.X - d / 2f, o.Y - h / 2f);
                    PointF b = new PointF(o.X + d / 2f, o.Y - h / 2f);
                    PointF c = new PointF(o.X, o.Y + h / 2f);
                    g.FillPolygon(brush, new PointF[] { a, b, c });   //实心
                    brush.Dispose();


                    g.Dispose();
                    return img;
                }

                return _imageDrop;
            }
            set { _imageDrop = value; }
        }


        Bitmap _imageSearch;
        Bitmap ImageSearch
        {
            get
            {
                if (_imageSearch == null)
                {
                    int width = 18;
                    int height = 20;
                    Bitmap img = new Bitmap(width, height);

                    Graphics g = Graphics.FromImage(img);

                    //g.FillRectangle(new SolidBrush(Color.Red), new Rectangle(0, 0, 10, 10));

                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    GraphicsPath path = new GraphicsPath();

                    //path.AddLine(new PointF((float)(16 / Math.Sqrt(2)), (float)(16 / Math.Sqrt(2))), new PointF((float)(23/Math.Sqrt(2)), (float)(23 / Math.Sqrt(2))));
                    int leftTopX = 6;    //左上角x坐标
                    int leftTopY = 4;     //左上角y坐标
                    int d = 8;
                    path.AddEllipse(leftTopX, leftTopY, d, d);


                    float r = d * 0.5f;  //直径
                    float a = leftTopX + r;  //圆心x
                    float b = leftTopY + r;   //圆心y

                    float k = (float)Math.Cos(45.0);
                    PointF p1 = new PointF(a - k * r, b + k * r);
                    PointF p2 = new PointF(a - 4 * k * r, b + 4 * k * r);

                    path.AddLine(p1, p2);

                    Pen pen = new Pen(Color.Gray, 2);
                    g.DrawPath(pen, path);
                    pen.Dispose();





                    g.Dispose();
                    return img;
                }

                return _imageSearch;
            }
            set { _imageSearch = value; }
        }

        ZTextBoxBase _innerTextBox;
        /// <summary>
        /// 内部文本框
        /// </summary>
        [Browsable(false)]
        public ZTextBoxBase InnerTextBox
        {
            get
            {
                if (!Editable)
                {
                    if (_innerTextBox != null)
                    {
                        RemoveEventHandler();
                        Controls.RemoveByKey("_innerTextBox");
                        _innerTextBox.Dispose();
                        _innerTextBox = null;
                    }
                    return null;
                }
                   
                if (Controls["_innerTextBox"] == null)
                {
                    _innerTextBox = new ZTextBoxBase();
                    _innerTextBox.Name = "_innerTextBox";
                    _innerTextBox.Text = Text;
                    _innerTextBox.BorderStyle = BorderStyle.None;
                    _innerTextBox.BackColor = BackColor;
                    _innerTextBox.TextChanged += (s, e) => { _Text = _innerTextBox.Text; };
                    _innerTextBox.GotFocus += (s, e) => { this.borderColor = BorderColorFocus; Invalidate(); };
                    _innerTextBox.LostFocus += (s, e) => { this.borderColor = _borderColorNormal; Invalidate(); };
                    _innerTextBox.MouseEnter += (s, e) =>
                    {
                        if (!_innerTextBox.Focused)
                        {
                            this.borderColor = BorderColorHover;
                            Invalidate();
                        }
                    };
                    base.Controls.Add(_innerTextBox);
                    AddEventHandler();
                }
                return _innerTextBox;
            }
        }

        MonthCalendar _innerMonthCalendar;
        [Browsable(false)]
        public MonthCalendar InnerMonthCalendar
        {
            get
            {
                if (_innerMonthCalendar == null)
                {
                    _innerMonthCalendar = new MonthCalendar();
                    //_innerMonthCalendar.Location = new Point(
                    _innerMonthCalendar.MaxSelectionCount = 1;
                    _innerMonthCalendar.ShowTodayCircle = false;
                    _innerMonthCalendar.DateSelected += (s, e) =>
                    {
                        if (string.IsNullOrEmpty(Format))
                            Text = _innerMonthCalendar.SelectionStart.ToString();
                        else
                            Text = _innerMonthCalendar.SelectionStart.ToString(Format);

                        InnerDropDown.Close();
                    };
                    //_innerMonthCalendar.SelectedIndexChanged += (s, e) =>
                    //{
                    //    if (SelectedIndexChanged != null)
                    //        SelectedIndexChanged(this, e);
                    //};
                }
                return _innerMonthCalendar;
            }
        }

        //UsDateTimerBox _innerTimerBox;
        //UsDateTimerBox InnerTimerBox
        //{
        //    get
        //    {
        //        if(_innerTimerBox==null)
        //        {
        //            _innerTimerBox = new UsDateTimerBox();
        //        }
        //        return _innerTimerBox;
        //    }
        //}


        ToolStripDropDown _innerDropDown;
        private ToolStripDropDown InnerDropDown
        {
            get
            {
                if (_innerDropDown == null)
                {
                    _innerDropDown = new ToolStripDropDown();
                    _innerDropDown.Padding = Padding.Empty;
                    _innerDropDown.AutoSize = false;
                    ToolStripControlHost toolStripControlHost = new ToolStripControlHost(InnerMonthCalendar, "ListBoxForDateTimePicker");
                    toolStripControlHost.Margin = Padding.Empty;
                    toolStripControlHost.Padding = Padding.Empty;
                    _innerDropDown.Items.Add(toolStripControlHost);
                    InnerMonthCalendar.SizeChanged += (s, e) => 
                    {
                        _innerDropDown.Width = InnerMonthCalendar.Width-1;
                        _innerDropDown.Height = InnerMonthCalendar.Height-1;
                        //if (ShowTimerBox)
                        //{
                        //    _innerDropDown.Height += 25;
                        //    //InnerTimerBox.Width = 100;
                        //}
                    };
                    _innerDropDown.Closed += (s, e) => { isDowned = false; };
                    _innerDropDown.Closing += (s, e) =>
                    {
                        if (Editable && new Rectangle(this.Width - 18, 0, 18, this.Height).Contains(this.PointToClient(MousePosition)))
                        {
                            if (e.CloseReason != ToolStripDropDownCloseReason.CloseCalled)
                                e.Cancel = true;
                        }
                        if (!Editable)
                        {
                            if (this.ClientRectangle.Contains(this.PointToClient(MousePosition)))
                            {
                                if (e.CloseReason != ToolStripDropDownCloseReason.CloseCalled)
                                    e.Cancel = true;
                            }

                        }
                    };
                    _innerDropDown.Opened += (s, e) => { isDowned = true; };


                    //if (ShowTimerBox)
                    //{
                    //    ToolStripControlHost toolStripControlHost_ = new ToolStripControlHost(InnerTimerBox, "ListBoxForTimer");
                    //    toolStripControlHost_.Margin = Padding.Empty;
                    //    toolStripControlHost_.Padding = Padding.Empty;
                    //    _innerDropDown.Items.Add(toolStripControlHost_);
                    //    toolStripControlHost_.AutoSize = false;
                       

                    //}

                }
                return _innerDropDown;
            }
        }


        private void createDropDownButton()
        {
            if (dropDownButton == null)
            {
                dropDownButton = new PictureBox();
                dropDownButton.BackColor = Color.Transparent;
                dropDownButton.BackgroundImageLayout = ImageLayout.Center;
                dropDownButton.BackgroundImage = ImageDrop;
                dropDownButton.MouseEnter += (s, e) => dropDownButton.BackColor = Color.FromArgb(230, 245, 253);
                dropDownButton.MouseLeave += (s, e) => dropDownButton.BackColor = Color.Transparent;
                dropDownButton.MouseDown += (s, e) =>
                {
                    if (isDowned)
                    {
                        InnerDropDown.Close(ToolStripDropDownCloseReason.CloseCalled);
                    }

                    else
                    {
                        ShowDropDown();
                    }
                    this.Focus();
                };
                this.Controls.Add(dropDownButton);
            }

           // dropDownButton.MouseDown += 

        }


        private bool canShowBelowControl(int height)
        {
            Rectangle workingRect = Screen.GetWorkingArea(this);
            Point screenPoint = PointToScreen(Point.Empty);
            int belowHeight = workingRect.Bottom - (screenPoint.Y + base.Height);
            int aboveHieght = screenPoint.Y - workingRect.Top;

            if (belowHeight >= height)
                return true;
            else if (belowHeight >= aboveHieght)
                return true;
            return false;
        }



        private void setLocation()
        {
            dropDownButton.Location = new Point(this.Width - 18 - 1, 1);
            dropDownButton.Size = new Size(18, this.Height - 2);
           

            if (InnerTextBox != null)
            {

                InnerTextBox.Location = new Point(2, (this.Height - InnerTextBox.Height) / 2+1);
                InnerTextBox.Width = this.Width - 18 - 2;
            }

        }

        private void drawComboText(Graphics g)
        {
            if (InnerTextBox != null)
            {
                InnerTextBox.Visible = Enabled;
            }
            else
            {
                string txt = Text;
                Color color = Color.Black;
                if (string.IsNullOrEmpty(Text))
                {
                    txt = _WaterText;
                    color = Color.DarkGray;
                }

                Rectangle rec = new Rectangle(0, 0, this.Width - 18, this.Height);
                TextRenderer.DrawText(
                    g,
                    txt,
                    this.Font,
                    rec,
                    color,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.PreserveGraphicsClipping);
            }
        }


        private void drawBorder(Graphics g)
        {
            if (!ShowBorder)
                return;

            Pen pen = new Pen(borderColor, 1);
            g.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            pen.Dispose();
        }



        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Rectangle rec = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height);

            SolidBrush brush = new SolidBrush(Enabled ? BackColor : Color.FromArgb(240, 240, 240));
            e.Graphics.FillRectangle(brush, rec);
            brush.Dispose();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            drawBorder(e.Graphics);
            drawComboText(e.Graphics);
            setLocation();
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (InnerTextBox == null)
            {
                this.Focus();
                if (isDowned)
                    InnerDropDown.Close(ToolStripDropDownCloseReason.CloseCalled);
                else
                    ShowDropDown();
            }
            else
            {
                InnerTextBox.Focus();
                InnerTextBox.SelectionStart = InnerTextBox.TextLength;
            }
        }


        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (InnerTextBox == null)
            {
                this.borderColor = BorderColorHover;
                //Invalidate();
            }
            //else if (!InnerTextBox.Focused)
            //    InnerTextBox.Focus();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (InnerTextBox == null)
            {

                this.borderColor = BorderColorNormal;
                //Invalidate();
            }
        }


        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (InnerTextBox != null && !InnerTextBox.Focused)
            {
                this.borderColor = BorderColorHover;
                //Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (InnerTextBox != null && !InnerTextBox.Focused)
            {
                this.borderColor = BorderColorNormal;
                Invalidate();
            }
        }



        //[Description("搜索按钮点击事件")]
        //public event EventHandler SearchClick;

        //[Description("选项索引改变事件")]
        //public event EventHandler SelectedIndexChanged;


        //需要添加事件,请在这里添加(只在可编辑状态下)
        private void AddEventHandler()
        {
            _innerTextBox.Click += _innerTextBox_Click;
            _innerTextBox.DoubleClick += _innerTextBox_DoubleClick;
            _innerTextBox.MouseClick += _innerTextBox_MouseClick;
            _innerTextBox.MouseDoubleClick += _innerTextBox_MouseDoubleClick;

            _innerTextBox.KeyDown += _innerTextBox_KeyDown;
            _innerTextBox.KeyPress += _innerTextBox_KeyPress;
            _innerTextBox.KeyUp += _innerTextBox_KeyUp;
            _innerTextBox.TextChanged += _innerTextBox_TextChanged;
        }

        private void RemoveEventHandler()
        {
            _innerTextBox.Click -= _innerTextBox_Click;
            _innerTextBox.DoubleClick -= _innerTextBox_DoubleClick;
            _innerTextBox.MouseClick -= _innerTextBox_MouseClick;
            _innerTextBox.MouseDoubleClick -= _innerTextBox_MouseDoubleClick;

            _innerTextBox.KeyDown -= _innerTextBox_KeyDown;
            _innerTextBox.KeyPress -= _innerTextBox_KeyPress;
            _innerTextBox.KeyUp -= _innerTextBox_KeyUp;
            _innerTextBox.TextChanged -= _innerTextBox_TextChanged;
        }


        void _innerTextBox_Click(object sender, EventArgs e)
        {
            base.OnClick(e);
        }
        void _innerTextBox_DoubleClick(object sender, EventArgs e)
        {
            base.OnDoubleClick(e);
        }
        void _innerTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }
        void _innerTextBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
        }
        void _innerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }
        void _innerTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }
        void _innerTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }
        void _innerTextBox_TextChanged(object sender, EventArgs e)
        {
            base.OnTextChanged(e);
        }
       
    }



}

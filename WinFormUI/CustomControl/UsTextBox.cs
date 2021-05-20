using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZXCL.WinFormUI;
using System.Drawing.Drawing2D;
using System.Threading;

namespace ZXCL.WinFormUI.CustomControl
{
    public partial class UsTextBox : UsControl
    {

        Font textFont = new Font("宋体", 11.5f, FontStyle.Regular, GraphicsUnit.Pixel);

        public UsTextBox()
        {
            InitializeComponent();
        }

        public UsTextBox(string label)
        {
            InitializeComponent();
            Label = label;
        }

        public UsTextBox(string label, string water)
        {
            InitializeComponent();
            Label = label;
            this.WaterText = water;
        }

        public UsTextBox(string label, string water1, string water2)
        {
            InitializeComponent();
            Label = label;
            this.WaterText = water1;
            this.WaterText2 = water2;
        }


        Color borderColor = Color.FromArgb(184, 184, 184);
        Color backColor = Color.White;
        int labelWidth = 80;

        ZTextBoxEx innerTextBox = new ZTextBoxEx();
        ZTextBoxEx innerTextBox2 = new ZTextBoxEx();
        [Browsable(false)]
        public ZTextBoxEx InnerTextBox { get { return innerTextBox; } }
        [Browsable(false)]
        public ZTextBoxEx InnerTextBox2 { get { return innerTextBox2; } }


        bool _ShowTwoTextBox = false;
        /// <summary>
        /// 显示两个文本框
        /// </summary>
        [Description("显示两个文本框")]
        [DefaultValue(false)]
        public override bool ShowTwoBox
        {
            get { return _ShowTwoTextBox; }
            set
            {
                _ShowTwoTextBox =value;
                if (!value)
                    if (this.Controls.Contains(innerTextBox2))
                        this.Controls.Remove(innerTextBox2);
                setLocation();
                this.Invalidate();
            }
        }

        /// <summary>
        /// 圆角半径
        /// </summary>
        [Description("圆角半径")]
        [DefaultValue(0)]
        public int Radius { get; set; }

        [Browsable(true)]
        [DefaultValue("")]
        public new string Text
        {
            get { return innerTextBox.Text; }
            set
            {
                innerTextBox.Text = value;
            }
        }

        [Browsable(true)]
        [DefaultValue("")]
        public  string Text2
        {
            get { return innerTextBox2.Text; }
            set
            {
                innerTextBox2.Text = value;
            }
        }

        [Description("水印文本")]
        [DefaultValue("")]
        public string WaterText
        {
            get { return innerTextBox.WaterText; }
            set { innerTextBox.WaterText = value; }
        }

        [Description("水印文本")]
        [DefaultValue("")]
        public string WaterText2
        {
            get { return innerTextBox2.WaterText; }
            set { innerTextBox2.WaterText = value; }
        }

      

        string _Label = "";
        [DefaultValue("")]
        public string Label { get { return _Label; } set { _Label = value; } }

        [DefaultValue(false)]
        public bool ShowSearch
        {
            get { return innerTextBox.ShowSearch; }
            set { innerTextBox.ShowSearch = value; innerTextBox2.ShowSearch = value; }
        }

        [DefaultValue(false)]
        public bool ShowClose
        {
            get { return innerTextBox.ShowClose; }
            set { innerTextBox.ShowClose = value; innerTextBox2.ShowClose = value; }
        }

        public event Action<string> OnTextBoxSearch;
        public event Action<string> OnTextBox2Search;

        protected override void OnLoad(EventArgs e)
        {
            //this.Font = textFont;
            innerTextBox.Height = this.Height - 3;
            innerTextBox.InnerTextBox.Font = textFont;
            innerTextBox.ShowBorder = false;
            innerTextBox.SearchButtonClick += InnerTextBox_SearchButtonClick;
            this.Controls.Add(innerTextBox);
            setLocation();
        }

        private void InnerTextBox_SearchButtonClick(object sender, EventArgs e)
        {
            if (OnTextBoxSearch != null)
                OnTextBoxSearch(innerTextBox.Text);
        }

        protected override void OnResize(EventArgs e)
        {
            setLocation();
        }

        void setLocation()
        {
            int top = (this.Height - innerTextBox.Height) / 2 + 1;
            

            if(ShowTwoBox)
            {
                if (!this.Controls.Contains(innerTextBox2))
                {
                    innerTextBox2.Height = this.Height - 3;
                    //innerTextBox2.InnerTextBox.Font = textFont;
                    innerTextBox2.ShowBorder = false;
                    innerTextBox2.SearchButtonClick += InnerTextBox2_SearchButtonClick;
                    this.Controls.Add(innerTextBox2);
                }

                Size size = TextRenderer.MeasureText("商", textFont);

                int width = (this.Width - labelWidth - 2 - size.Width * 2) / 2 - 1;
                innerTextBox.Width = width;
                innerTextBox2.Width = width;
                innerTextBox.Location = new Point(labelWidth + size.Width+1, top);
                innerTextBox2.Location = new Point(innerTextBox.Right + size.Width+1, top);
            }
            else
            {
                //innerTextBox.Location = new Point(labelWidth + 5, top);
                //innerTextBox.Width = this.Width - labelWidth - 10;
                innerTextBox.Location = new Point(labelWidth + 1, top);
                innerTextBox.Width = this.Width - labelWidth - 2;
            }
        }

        private void InnerTextBox2_SearchButtonClick(object sender, EventArgs e)
        {
            if (OnTextBox2Search != null)
                OnTextBox2Search(innerTextBox2.Text);
        }

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
                    e.Graphics.FillRectangle(brush,this.ClientRectangle);
                }
                using (Pen pen = new Pen(borderColor))
                {
                    e.Graphics.DrawRectangle(pen, new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1));
                }
            }

            //绘制label
            TextRenderer.DrawText(e.Graphics, this._Label, textFont, new Rectangle(1, 0, labelWidth, this.Height), this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            //绘制分隔线
            int top = this.ClientRectangle.Top + 5;
            int bottom = this.ClientRectangle.Bottom - 5;
            using (Pen pen = new Pen(Color.Gray))
            {
                e.Graphics.DrawLine(pen, new Point(labelWidth, top), new Point(labelWidth, bottom));
            }

            if (ShowTwoBox)
            {
                using (Pen pen = new Pen(Color.LightGray))
                {
                    e.Graphics.DrawLine(pen, new Point(innerTextBox.Right, top), new Point(innerTextBox.Right, bottom));
                }
                Size size = TextRenderer.MeasureText("商", textFont);
                TextRenderer.DrawText(e.Graphics,"商", textFont, new Rectangle(labelWidth+3, 0, size.Width, this.Height), Color.Maroon, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                TextRenderer.DrawText(e.Graphics, "交", textFont, new Rectangle(innerTextBox.Right+3, 0, size.Width, this.Height), Color.Maroon, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
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

        bool isFlashing = false;
        public override void BorderFlash()
        {
            if (!isFlashing)
            {

                isFlashing = true;
                Thread td = new Thread(() =>
                {
                    Color old = this.borderColor;

                    for (int i = 0; i < 15; i++)
                    {
                        if (i % 2 == 0)
                            this.borderColor = Color.Transparent;
                        else
                            this.borderColor = Color.Crimson;
                        Invalidate(false);
                        Thread.Sleep(100);
                    }
                    this.borderColor = old;
                    Invalidate(false);
                    isFlashing = false;
                });
                td.IsBackground = true;
                td.Start();

            }
        }

        //验证输入
        public override bool ValidateInput()
        {
            if (OnValidateInput != null)
                return OnValidateInput(this);

            if (MustInput)
            {
                if (innerTextBox.Text=="")
                    return false;

                if (ShowTwoBox && innerTextBox2.Text == "")
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 自定义验证方式
        /// </summary>
        public event Func<object, bool> OnValidateInput;

        /// <summary>
        /// 自定义返回value
        /// </summary>
        public event Func<object, string> OnGetValue;

        public override string Value
        {
            get
            {
                if (OnGetValue != null)
                    return OnGetValue(this);


                string _value = Text;
                if(ShowTwoBox)
                    _value += Separator + Text2;
                return _value;
            }
        }

       
    }
}

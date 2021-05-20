using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;

namespace ZXCL.WinFormUI.CustomControl
{
    public partial class UsDateTimePicker : UsControl
    {
        Font textFont = new Font("宋体", 11.5f, FontStyle.Regular, GraphicsUnit.Pixel);

        public UsDateTimePicker()
        {
            InitializeComponent();
        }

        public UsDateTimePicker(string label)
        {
            InitializeComponent();
            Label = label;
        }

        public UsDateTimePicker(string label, string water, string text)
        {
            InitializeComponent();
            Label = label;
            this.WaterText = water;
            this.Text = text;
        }
        

        public UsDateTimePicker(string label, string water1, string water2, bool showTwo)
        {
            InitializeComponent();
            Label = label;
            this.WaterText = water1;
            this.WaterText2 = water2;
            this.ShowTwoBox = showTwo;
        }


        Color borderColor = Color.FromArgb(184, 184, 184);
        Color backColor = Color.White;
        int labelWidth = 80;

        ZDateTimePicker innerDateTimePicker = new ZDateTimePicker();
        ZDateTimePicker innerDateTimePicker2 = new ZDateTimePicker();
        [Browsable(false)]
        public ZDateTimePicker InnerZDateTimePicker { get { return innerDateTimePicker; } }
        [Browsable(false)]
        public ZDateTimePicker InnerZDateTimePicker2 { get { return innerDateTimePicker2; } }

        ZCheckBox innerZCheckBox;
        public ZCheckBox InnerZCheckBox { get { return innerZCheckBox; } }

        bool _ShowTwoTextBox = false;
        /// <summary>
        /// 显示两个下拉框
        /// </summary>
        [Description("显示两个下拉框")]
        [DefaultValue(false)]
        public override bool ShowTwoBox
        {
            get { return _ShowTwoTextBox; }
            set
            {
                _ShowTwoTextBox = value;
                if (!value)
                    if (this.Controls.Contains(innerDateTimePicker2))
                        this.Controls.Remove(innerDateTimePicker2);
                setLocation();
                this.Invalidate();
            }
        }

        bool _ShowCheckBox;
        [DefaultValue(false)]
        public bool ShowCheckBox
        {
            get { return _ShowCheckBox; }
            set
            {
                _ShowCheckBox = value;
                if (value)
                {
                    if (innerZCheckBox == null)
                        addCheckBox();
                }
                else if (innerZCheckBox != null)
                {
                    this.Controls.Remove(innerZCheckBox);
                    innerZCheckBox.Dispose();
                    innerZCheckBox = null;
                }
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
            get { return innerDateTimePicker.Text; }
            set
            {
                innerDateTimePicker.Text = value;
            }
        }

        [Browsable(true)]
        [DefaultValue("")]
        public string Text2
        {
            get { return innerDateTimePicker2.Text; }
            set
            {
                innerDateTimePicker2.Text = value;
            }
        }

        [Description("水印文本")]
        [DefaultValue("")]
        public string WaterText
        {
            get { return innerDateTimePicker.WaterText; }
            set { innerDateTimePicker.WaterText = value; }
        }

        [Description("水印文本")]
        [DefaultValue("")]
        public string WaterText2
        {
            get { return innerDateTimePicker2.WaterText; }
            set { innerDateTimePicker2.WaterText = value; }
        }

       

        string _Label = "";
        [DefaultValue("")]
        public string Label { get { return _Label; } set { _Label = value; } }

        public event Action<bool> OnCheckedChanged;

        protected override void OnLoad(EventArgs e)
        {
            innerDateTimePicker.Height = this.Height - 3;
            innerDateTimePicker.ShowBorder = false;
            //innerDateTimePicker.InnerTextBox.Font = textFont;
            //innerDateTimePicker.InnerMonthCalendar.Font = textFont;
            this.Controls.Add(innerDateTimePicker);

            //if (ShowCheckBox)
                //addCheckBox();
            setLocation();
        }

        protected override void OnResize(EventArgs e)
        {
            setLocation();
        }

        void setLocation()
        {
            int top = (this.Height - innerDateTimePicker.Height) / 2 + 1;


            if (ShowTwoBox)
            {
                if (!this.Controls.Contains(innerDateTimePicker2))
                {
                    innerDateTimePicker2.Height = this.Height - 3;
                    innerDateTimePicker2.ShowBorder = false;
                    //innerDateTimePicker2.InnerTextBox.Font = textFont;
                    //innerDateTimePicker2.InnerMonthCalendar.Font = textFont;
                    this.Controls.Add(innerDateTimePicker2);
                }

                //int width = (this.Width - labelWidth - 20) / 2;
                //innerDateTimePicker.Width = width;
                //innerDateTimePicker2.Width = width;
                //innerDateTimePicker.Location = new Point(labelWidth + 5, top);
                //innerDateTimePicker2.Location = new Point(innerDateTimePicker.Right + 10, top);

                Size size = TextRenderer.MeasureText("商", textFont);

                int width = (this.Width - labelWidth - 2 - size.Width * 2) / 2 - 1;
                innerDateTimePicker.Width = width;
                innerDateTimePicker2.Width = width;
                innerDateTimePicker.Location = new Point(labelWidth + size.Width + 1, top);
                innerDateTimePicker2.Location = new Point(innerDateTimePicker.Right + size.Width + 1, top);
            }
            else
            {
                //if(ShowCheckBox)
                //{
                //    innerDateTimePicker.Width = this.Width - labelWidth - innerZCheckBox.Width - 12;
                //    innerDateTimePicker.Location = new Point(labelWidth + 5, top);
                //    innerZCheckBox.Location = new Point(innerDateTimePicker.Right + 5, (this.Height - innerZCheckBox.Height) / 2+1);
                //}
                //else
                //{
                //    innerDateTimePicker.Location = new Point(labelWidth + 5, top);
                //    innerDateTimePicker.Width = this.Width - labelWidth - 10;
                //}

                if (ShowCheckBox)
                {
                    //innerDateTimePicker.Width = this.Width - labelWidth - innerZCheckBox.Width - 12;
                    //innerDateTimePicker.Location = new Point(labelWidth + 5, top);
                    //innerZCheckBox.Location = new Point(innerDateTimePicker.Right + 5, (this.Height - innerZCheckBox.Height) / 2 + 1);

                    innerDateTimePicker.Width = this.Width - labelWidth - innerZCheckBox.Width;
                    innerDateTimePicker.Location = new Point(labelWidth + 1, top);
                    innerZCheckBox.Location = new Point(innerDateTimePicker.Right+3, (this.Height - innerZCheckBox.Height) / 2 + 1);
                }
                else
                {
                    innerDateTimePicker.Location = new Point(labelWidth + 1, top);
                    innerDateTimePicker.Width = this.Width - labelWidth - 2;
                }

            }
        }

        private void addCheckBox()
        {
            innerZCheckBox = new ZCheckBox();

            innerZCheckBox.AutoSize = true;
            innerZCheckBox.Text = "记住";
            innerZCheckBox.Font = textFont;
            innerZCheckBox.ForeColor = Color.DarkGray;
            innerZCheckBox.BoxColor = Color.DarkGray;
            innerZCheckBox.CheckedChanged += InnerZCheckBox_CheckedChanged;
            this.Controls.Add(innerZCheckBox);
        }

        private void InnerZCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (OnCheckedChanged != null)
                OnCheckedChanged(innerZCheckBox.Checked);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
                setLocation();
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

            //绘制label
            TextRenderer.DrawText(
                          e.Graphics,
                          this._Label,
                          textFont,
                          new Rectangle(1, 0, labelWidth, this.Height),
                          this.ForeColor,
                          TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            //绘制分隔线
            int top = this.ClientRectangle.Top + 5;
            int bottom = this.ClientRectangle.Bottom - 5;
            using (Pen pen = new Pen(Color.Gray))
            {
                e.Graphics.DrawLine(pen, new Point(labelWidth, top), new Point(labelWidth, bottom));
            }

            if (ShowTwoBox)
            {
                //using (Pen pen = new Pen(Color.LightGray))
                //{
                //    e.Graphics.DrawLine(pen, new Point(innerDateTimePicker.Right + 5, top), new Point(innerDateTimePicker.Right + 5, bottom));
                //}

                using (Pen pen = new Pen(Color.LightGray))
                {
                    e.Graphics.DrawLine(pen, new Point(innerDateTimePicker.Right, top), new Point(innerDateTimePicker.Right, bottom));
                }

                Size size = TextRenderer.MeasureText("商", textFont);
                TextRenderer.DrawText(e.Graphics, "商", textFont, new Rectangle(labelWidth + 3, 0, size.Width, this.Height), Color.Maroon, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                TextRenderer.DrawText(e.Graphics, "交", textFont, new Rectangle(innerDateTimePicker.Right + 3, 0, size.Width, this.Height), Color.Maroon, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
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

        public override bool ValidateInput()
        {
            if (MustInput)
            {
                if (innerDateTimePicker.Text == "")
                    return false;

                if (ShowTwoBox && innerDateTimePicker2.Text == "")
                    return false;
            }

            try
            {
                if (innerDateTimePicker.Text.Length > 0)
                {
                    DateTime dateTime = Convert.ToDateTime(innerDateTimePicker.Text);
                    innerDateTimePicker.Text = dateTime.ToString(innerDateTimePicker.Format);
                }
                if (innerDateTimePicker2.Text.Length > 0)
                {
                    DateTime dateTime = Convert.ToDateTime(innerDateTimePicker2.Text);
                    innerDateTimePicker2.Text = dateTime.ToString(innerDateTimePicker2.Format);
                }

            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
            {
                ErrorText = "日期格式不正确";
                return false;
            }

            return true;
        }

        public override string Value
        {
            get
            {
                string _value = innerDateTimePicker.Text;
                if (ShowTwoBox)
                    _value += Separator + innerDateTimePicker2.Text;
                return _value;
            }
        }
    }
}

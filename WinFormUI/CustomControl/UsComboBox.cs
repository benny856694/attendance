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
    public partial class UsComboBox : UsControl
    {
        Font textFont = new Font("宋体", 11.5f, FontStyle.Regular, GraphicsUnit.Pixel);

        public UsComboBox()
        {
            InitializeComponent();
        }

        public UsComboBox(string label)
        {
            InitializeComponent();
            Label = label;
        }

        public UsComboBox(string label, string water)
        {
            InitializeComponent();
            Label = label;
            this.WaterText = water;
        }

        public UsComboBox(string label, string water1, string water2, bool showTwo)
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

        ZComboBox innerComboBox = new ZComboBox();
        ZComboBox innerComboBox2 = new ZComboBox();
        [Browsable(false)]
        public ZComboBox InnerZComboBox { get { return innerComboBox; } }
        [Browsable(false)]
        public ZComboBox InnerZComboBox2 { get { return innerComboBox2; } }


        bool _ShowTwoTextBox = false;
        /// <summary>
        /// 显示两个下拉框
        /// </summary>
        [Description("显示两个下拉框")]
        public override bool ShowTwoBox
        {
            get { return _ShowTwoTextBox; }
            set
            {
                _ShowTwoTextBox = value;
                if (!value)
                    if (this.Controls.Contains(innerComboBox2))
                        this.Controls.Remove(innerComboBox2);
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
            get { return innerComboBox.Text; }
            set
            {
                innerComboBox.Text = value;
            }
        }

        [Browsable(true)]
        [DefaultValue("")]
        public string Text2
        {
            get { return innerComboBox2.Text; }
            set
            {
                innerComboBox2.Text = value;
            }
        }

        [DefaultValue(false)]
        public bool Editable
        {
            get { return innerComboBox.Editable; }
            set { innerComboBox.Editable = value; }
        }
        [DefaultValue(false)]
        public bool Editable2
        {
            get { return innerComboBox2.Editable; }
            set { innerComboBox2.Editable = value; }
        }

        [Description("水印文本")]
        [DefaultValue("")]
        public string WaterText
        {
            get { return innerComboBox.WaterText; }
            set { innerComboBox.WaterText = value; }
        }

        [Description("水印文本")]
        [DefaultValue("")]
        public string WaterText2
        {
            get { return innerComboBox2.WaterText; }
            set { innerComboBox2.WaterText = value; }
        }


        string _Label = "";
        [DefaultValue("")]
        public string Label { get { return _Label; } set { _Label = value; } }

        [DefaultValue(false)]
        public bool ShowSearch
        {
            get { return innerComboBox.ShowSearchButton; }
            set { innerComboBox.ShowSearchButton = value; innerComboBox2.ShowSearchButton = value; }
        }
        public event Action<string> OnTextBoxSearch;
        public event Action<string> OnTextBox2Search;

#pragma warning disable CS0108 // “UsComboBox.OnKeyPress”隐藏继承的成员“Control.OnKeyPress(KeyPressEventArgs)”。如果是有意隐藏，请使用关键字 new。
        public event Action<string, char> OnKeyPress;  //键盘按下并释放
#pragma warning restore CS0108 // “UsComboBox.OnKeyPress”隐藏继承的成员“Control.OnKeyPress(KeyPressEventArgs)”。如果是有意隐藏，请使用关键字 new。
        public event Action<string, char> OnKeyPress2;  //键盘按下并释放

        public event Action OnSelectedIndexChanged;

        public event Action<bool> OnCheckedChanged;

        ZCheckBox innerZCheckBox;
        public ZCheckBox InnerZCheckBox {get { return innerZCheckBox; } }

        protected override void OnLoad(EventArgs e)
        {
            //this.Font = textFont;
            innerComboBox.Height = this.Height - 3;
            innerComboBox.ShowBorder = false;
            //innerComboBox.InnerTextBox.Font = textFont;
            innerComboBox.Font = textFont;
            innerComboBox.InnerListbox.TextFont = textFont;
            innerComboBox.SearchClick += InnerComboBox_SearchClick;
            innerComboBox.KeyPress += InnerComboBox_KeyPress;
            innerComboBox.SelectedIndexChanged += InnerComboBox_SelectedIndexChanged;
            this.Controls.Add(innerComboBox);

            //if (ShowCheckBox)
                //addCheckBox();

            setLocation();
        }

        private void InnerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OnSelectedIndexChanged != null)
                OnSelectedIndexChanged();
        }

        private void InnerComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (OnKeyPress != null)
                OnKeyPress(innerComboBox.Text, e.KeyChar);
        }

        private void InnerComboBox_SearchClick(object sender, EventArgs e)
        {
            if (OnTextBoxSearch != null)
                OnTextBoxSearch(innerComboBox.Text);
        }

        protected override void OnResize(EventArgs e)
        {
            setLocation();
        }

        void setLocation()
        {
            int top = (this.Height - innerComboBox.Height) / 2 + 1;


            if (ShowTwoBox)
            {
                if (!this.Controls.Contains(innerComboBox2))
                {
                    innerComboBox2.Height = this.Height - 3;
                    innerComboBox2.ShowBorder = false;
                    //innerComboBox.InnerTextBox.Font = textFont;
                    //innerComboBox.InnerListbox.TextFont = textFont;
                    innerComboBox2.SearchClick += InnerComboBox2_SearchClick;
                    innerComboBox2.KeyPress += InnerComboBox2_KeyPress;
                    this.Controls.Add(innerComboBox2);
                }

                //int width = (this.Width - labelWidth - 20) / 2;
                //innerComboBox.Width = width;
                //innerComboBox2.Width = width;
                //innerComboBox.Location = new Point(labelWidth + 5, top);
                //innerComboBox2.Location = new Point(innerComboBox.Right + 10, top);

                Size size = TextRenderer.MeasureText("商", textFont);

                int width = (this.Width - labelWidth - 2 - size.Width * 2) / 2 - 1;
                innerComboBox.Width = width;
                innerComboBox2.Width = width;
                innerComboBox.Location = new Point(labelWidth + size.Width + 1, top);
                innerComboBox2.Location = new Point(innerComboBox.Right + size.Width + 1, top);

            }
            else
            {
                //innerComboBox.Location = new Point(labelWidth + 5, top);
                //innerComboBox.Width = this.Width - labelWidth - 10;
                //innerComboBox.Location = new Point(labelWidth + 1, top);
                //innerComboBox.Width = this.Width - labelWidth - 2;

                if (ShowCheckBox)
                {
                    //innerDateTimePicker.Width = this.Width - labelWidth - innerZCheckBox.Width - 12;
                    //innerDateTimePicker.Location = new Point(labelWidth + 5, top);
                    //innerZCheckBox.Location = new Point(innerDateTimePicker.Right + 5, (this.Height - innerZCheckBox.Height) / 2 + 1);

                    innerComboBox.Width = this.Width - labelWidth - innerZCheckBox.Width;
                    innerComboBox.Location = new Point(labelWidth + 1, top);
                    innerZCheckBox.Location = new Point(innerComboBox.Right+3, (this.Height - innerZCheckBox.Height) / 2 + 1);
                }
                else
                {
                    innerComboBox.Location = new Point(labelWidth + 1, top);
                    innerComboBox.Width = this.Width - labelWidth - 2;
                }

            }
        }

        private void InnerComboBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (OnKeyPress2 != null)
                OnKeyPress2(innerComboBox2.Text, e.KeyChar);
        }

        private void InnerComboBox2_SearchClick(object sender, EventArgs e)
        {
            if (OnTextBox2Search != null)
                OnTextBox2Search(innerComboBox2.Text);
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
                //    e.Graphics.DrawLine(pen, new Point(innerComboBox.Right + 5, top), new Point(innerComboBox.Right + 5, bottom));
                //}

                using (Pen pen = new Pen(Color.LightGray))
                {
                    e.Graphics.DrawLine(pen, new Point(innerComboBox.Right, top), new Point(innerComboBox.Right, bottom));
                }
                Size size = TextRenderer.MeasureText("商", textFont);
                TextRenderer.DrawText(e.Graphics, "商", textFont, new Rectangle(labelWidth + 3, 0, size.Width, this.Height), Color.Maroon, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                TextRenderer.DrawText(e.Graphics, "交", textFont, new Rectangle(innerComboBox.Right + 3, 0, size.Width, this.Height), Color.Maroon, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            if(DesignMode)
                setLocation();
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
            if(MustInput)
            {
                if (innerComboBox.SelectedIndex < 0)
                    return false;

                if(ShowTwoBox && innerComboBox2.SelectedIndex < 0)
                    return false;
            }

            return true;
        }



        [Browsable(false)]
        [DefaultValue(null)]
        public string ValuePath { get; set; }


        public override string Value
        {
            get
            {
                if(ValuePath==null)
                {
                    string _value = Text;
                    if (ShowTwoBox)
                        _value += Separator + Text2;
                    return _value;
                }
                else
                {
                    string _value = "";
                    object selectValue = innerComboBox.SelectedItem;
                    if(selectValue != null)
                        _value = getValue(selectValue);

                    if (ShowTwoBox)
                    {
                        object selectValue2 = innerComboBox2.SelectedItem;
                        if (selectValue2 != null)
                            _value += Separator+ getValue(selectValue2);
                    }
                    return _value;
                }
               
            }
        }

        string getValue(object obj)
        {
            Type t = obj.GetType();
            object _value = t.GetProperty(ValuePath).GetValue(obj, null);
            if (_value != null)
                return _value.ToString();
            else
                return null;
        }
    }
}

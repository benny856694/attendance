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
    public partial class UsRadioButton : UsControl
    {
        Font textFont = new Font("宋体", 11.5f, FontStyle.Regular, GraphicsUnit.Pixel);

        public UsRadioButton()
        {
            InitializeComponent();
        }

        public UsRadioButton(string label, string text1, string text2)
        {
            InitializeComponent();
            Label = label;
            Text1 = text1;
            Text2 = text2;
        }

        public UsRadioButton(string label, string text1, string text2, bool check1, bool check2)
        {
            InitializeComponent();
            Label = label;
            Text1 = text1;
            Text2 = text2;
            Radio1Checked = check1;
            Radio2Checked = check2;
        }

        Color borderColor = Color.FromArgb(184, 184, 184);
        Color backColor = Color.White;
        int labelWidth = 80;

        ZRadioButton innerRadioButton1 = new ZRadioButton();
        ZRadioButton innerRadioButton2 = new ZRadioButton();
        [Browsable(false)]
        public RadioButton InnerRadioButton1 { get { return innerRadioButton1; } }
        [Browsable(false)]
        public RadioButton InnerRadioButton2 { get { return innerRadioButton2; } }
        

        /// <summary>
        /// 圆角半径
        /// </summary>
        [Description("圆角半径")]
        [DefaultValue(0)]
        public int Radius { get; set; }

        [Browsable(true)]
        [DefaultValue("")]
        public  string Text1
        {
            get { return innerRadioButton1.Text; }
            set
            {
                innerRadioButton1.Text = value;
            }
        }

        [Browsable(true)]
        [DefaultValue("")]
        public string Text2
        {
            get { return innerRadioButton2.Text; }
            set
            {
                innerRadioButton2.Text = value;
            }
        }

        string _Value1 = "";
        [Browsable(true)]
        [DefaultValue("")]
        public string Value1
        {
            get { return _Value1; }
            set { _Value1 = value; }
        }

        string _Value2 = "";
        [Browsable(true)]
        [DefaultValue("")]
        public string Value2
        {
            get { return _Value2; }
            set { _Value2 = value; }
        }

        [DefaultValue(false)]
        public bool Radio1Checked
        {
            get { return innerRadioButton1.Checked; }
            set { innerRadioButton1.Checked = value; }
        }
        [DefaultValue(false)]
        public bool Radio2Checked
        {
            get { return innerRadioButton2.Checked; }
            set { innerRadioButton2.Checked = value; }
        }

        bool _HasChecked;
        [Browsable(false)]
        [DefaultValue(false)]
        public bool HasChecked
        {
            get { return _HasChecked; }
        }

        string _Label = "";
        [DefaultValue("")]
        public string Label { get { return _Label; } set { _Label = value; } }

        public event Action OnRadio1Checked;
        public event Action OnRadio2Checked;

        protected override void OnLoad(EventArgs e)
        {
            innerRadioButton1.AutoSize = true;
            innerRadioButton2.AutoSize = true;
            innerRadioButton1.Text = Text1;
            innerRadioButton2.Text = Text2;
            innerRadioButton1.Font = textFont;
            innerRadioButton2.Font = textFont;
            innerRadioButton1.CheckedChanged += InnerComboBox1_CheckedChanged;
            innerRadioButton2.CheckedChanged += InnerComboBox2_CheckedChanged;

            this.Controls.Add(innerRadioButton1);
            this.Controls.Add(innerRadioButton2);
            setLocation();
        }

        private void InnerComboBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (innerRadioButton2.Checked)
            {
                _HasChecked = true;
                if (OnRadio2Checked != null)
                    OnRadio2Checked();
            }
        }

        private void InnerComboBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (innerRadioButton1.Checked)
            {
                _HasChecked = true;
                if (OnRadio1Checked != null)
                    OnRadio1Checked();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            setLocation();
        }

        void setLocation()
        {
            int top = (this.Height - innerRadioButton1.Height) / 2 + 1;

            int width = innerRadioButton1.Width;
            int width2 = innerRadioButton2.Width;

            int space = (this.Width - width - width2- labelWidth) / 3;

            innerRadioButton1.Location = new Point(labelWidth + space, top);
            innerRadioButton2.Location = new Point(innerRadioButton1.Right + space, top);
           
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
                return HasChecked;
            }

            return true;
        }

        public override string Value
        {
            get
            {
                if (Radio1Checked)
                    return Value1;
                else if (Radio2Checked)
                    return Value2;
                else
                    return null;
            }
        }
    }
}

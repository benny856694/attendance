using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace ZXCL.WinFormUI
{
    public class ZListBoxItem
    {
        public string Text { get; set; }    //文本
        public string Remarks { get; set; } //备注
        public Bitmap Img { get; set; }     //图像

        public ZListBoxItem(string text, string remarks, Bitmap img)
        {
            Text = text;
            Remarks = remarks;
            Img = img;
        }

        public ZListBoxItem(string text, Bitmap img)
        {
            Text = text;
            Img = img; ;
        }

        public ZListBoxItem(string text, string remarks)
        {
            Text = text;
            Remarks = remarks; ;
        }

        public ZListBoxItem(string text)
        {
            Text = text;
        }

        public ZListBoxItem()
        {
        }
    }


    public class ZListBox : ListBox
    {
        public ZListBox()
        {
            this.DrawMode = DrawMode.OwnerDrawVariable;
            //SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            UpdateStyles();
        }
        /*
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
           if (this.Parent != null)
            innerZScrollBar.Height = this.ClientRectangle.Height;
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            if (this.Parent != null)
            {
                int c = (this.Height - this.ClientRectangle.Height) / 2;
                innerZScrollBar.Location = new Point(this.Location.X + this.Width - innerZScrollBar.Width - c, this.Location.Y + c);
                innerZScrollBar.Height = this.ClientRectangle.Height;
            }
        }
        */


        protected override void OnMeasureItem(MeasureItemEventArgs e) //设置各项高度
        {
            try
            {
                if (e.Index >= 0)
                {
                    base.OnMeasureItem(e);
                    ZListBoxItem i = Items[e.Index] as ZListBoxItem;
                    if (i != null)
                    {
                        e.ItemHeight = this.ItemHeight;
                    }
                }
            }
            catch { }
        }

        protected override void OnPaint(PaintEventArgs e) //重绘
        {
            base.OnPaint(e);
            //Console.WriteLine(this.ClientRectangle);
            //SetScrollVisible();
            //ShowScrollBar(Handle, ScrollBar.Both, 0);
            // e.Graphics.DrawImage(Properties.Resources._12321, new Point(0, 0)); //背景
            // e.Graphics.DrawImage(Resources.bk, new Rectangle(0, 0, Width, Height)); //半透明

            for (int i = 0; i < Items.Count; ++i)
            {
                ZListBoxItem item = Items[i] as ZListBoxItem;

                Rectangle bound = GetItemRectangle(i);
                Color rectangleColor = Color.Transparent;
                Color textColor = Color.Transparent;
                Color remarksColor = Color.Transparent;
                Point mousePoint = PointToClient(MousePosition);

                if (i == this.SelectedIndex)  //选中样式
                {
                    rectangleColor = ItemColorSelected;
                    textColor = TextColorSelected;
                    remarksColor = RemarksColorSelected;

                    //if()

                }
                else if (bound.Contains(mousePoint))  //鼠标经过样式
                {
                    //mouseOnRect = new Rectangle(bound.Location,bound.Size);
                    rectangleColor = ItemColorHover;
                    textColor = TextColorHover;
                    remarksColor = RemarksColorHover;
                }
                else  //未选中样式
                {
                    rectangleColor = ItemColorNormal;
                    textColor = TextColorNormal;
                    remarksColor = RemarksColorNormal;
                }

                //画方块
                using (SolidBrush brush = new SolidBrush(rectangleColor))
                {
                    e.Graphics.FillRectangle(brush, bound);
                }

                //绘制分割线
                if (ShowLine)
                    e.Graphics.DrawLine(new Pen(Color.Gainsboro, 0.01f), new Point(bound.Left, bound.Top), new Point(bound.Right, bound.Top));


                //绘制文字与图片
                Rectangle imgRec = new Rectangle(bound.X, bound.Y, 0, 0);
                Rectangle textRec = new Rectangle();
                Rectangle remarkRec = new Rectangle();
                string text = "";
                if (item != null && item.Img != null)
                {
                    imgRec = new Rectangle(new Point(bound.X + 5, bound.Y + (bound.Height - ImageSize.Height) / 2), ImageSize);
                    e.Graphics.DrawImage(item.Img, imgRec);
                }
                if (item != null && item.Remarks == null && item.Text != null)
                {
                    textRec = new Rectangle(imgRec.Right + 5, bound.Y, bound.Width - imgRec.Width - 5, bound.Height);
                    text = item.Text;
                }
                else if (item != null && item.Remarks != null)
                {
                    int heightText = TextRenderer.MeasureText("A", TextFont).Height;
                    int heightRemarks = TextRenderer.MeasureText("A", RemarksFont).Height;

                    int y = (bound.Height - heightText - heightRemarks) / 2;

                    textRec = new Rectangle(imgRec.Right + 5, bound.Y + y, bound.Width - imgRec.Width - 5, heightText);
                    remarkRec = new Rectangle(imgRec.Right + 8, textRec.Bottom, bound.Width - imgRec.Width - 5, heightRemarks);
                    text = item.Text;
                    TextRenderer.DrawText(
                       e.Graphics,
                       item.Remarks,
                       RemarksFont,
                       remarkRec,
                       remarksColor,
                       TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }
                else if (item == null)
                {
                    textRec = new Rectangle(imgRec.Right + 5, bound.Y, bound.Width - imgRec.Width - 5, bound.Height);
                    text = Items[i].ToString();
                }

                TextRenderer.DrawText(
                   e.Graphics,
                   text,
                   TextFont,
                   textRec,
                   textColor,
                   TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);



                //鼠标选中的区块
                if (bound.Contains(mousePoint))
                {
                    mouseOnRect = new Rectangle(bound.Location, bound.Size);
                    if(ShowTip)
                    {
                        int width = TextRenderer.MeasureText(text, TextFont).Width;
                        if (width > bound.Width - 5)
                        {
                            toolTip.SetToolTip(this, text);
                        }
                        else
                        {
                            toolTip.SetToolTip(this, "");
                            toolTip.Hide(this);
                        }
                    }
                   
                }

            }
        }

        Color _ItemColorNormal = Color.Transparent;
        [DefaultValue(typeof(Color), "Transparent")]
        public Color ItemColorNormal
        {
            get { return _ItemColorNormal; }
            set { _ItemColorNormal = value; }
        }

        Color _ItemColorSelected = Color.FromArgb(177, 182, 188);
        [DefaultValue(typeof(Color), "177,182,188")]
        public Color ItemColorSelected
        {
            get { return _ItemColorSelected; }
            set { _ItemColorSelected = value; }
        }

        Color _ItemColorHover = Color.FromArgb(218, 218, 218);
        [DefaultValue(typeof(Color), "218,218,218")]
        public Color ItemColorHover
        {
            get { return _ItemColorHover; }
            set { _ItemColorHover = value; }
        }



        Color _TextColorNormal = Color.Black;
        [DefaultValue(typeof(Color), "Black")]
        public Color TextColorNormal
        {
            get { return _TextColorNormal; }
            set { _TextColorNormal = value; }
        }

        Color _TextColorSelected = Color.Black;
        [DefaultValue(typeof(Color), "Black")]
        public Color TextColorSelected
        {
            get { return _TextColorSelected; }
            set { _TextColorSelected = value; }
        }

        Color _TextColorHover = Color.Black;
        [DefaultValue(typeof(Color), "Black")]
        public Color TextColorHover
        {
            get { return _TextColorHover; }
            set { _TextColorHover = value; }
        }


        Color _RemarksColorNormal = Color.Gray;
        [DefaultValue(typeof(Color), "Gray")]
        public Color RemarksColorNormal
        {
            get { return _RemarksColorNormal; }
            set { _RemarksColorNormal = value; }
        }

        Color _RemarksColorSelected = Color.Gray;
        [DefaultValue(typeof(Color), "Gray")]
        public Color RemarksColorSelected
        {
            get { return _RemarksColorSelected; }
            set { _RemarksColorSelected = value; }
        }

        Color _RemarksColorHover = Color.Gray;
        [DefaultValue(typeof(Color), "Gray")]
        public Color RemarksColorHover
        {
            get { return _RemarksColorHover; }
            set { _RemarksColorHover = value; }
        }

        Font _TextFont = new Font("宋体", 9);
        public Font TextFont
        {
            get { return _TextFont; }
            set { _TextFont = value; }
        }

        Font _RemarksFont = new Font("宋体", 7.5f);
        public Font RemarksFont
        {
            get { return _RemarksFont; }
            set { _RemarksFont = value; }
        }

        int _ItemHeight = 23;
        public override int ItemHeight
        {
            get { return _ItemHeight; }
            set { _ItemHeight = value; }
        }

        Size _ImageSize = new Size(15, 15);
        public Size ImageSize
        {
            get { return _ImageSize; }
            set { _ImageSize = value; }
        }

        [DefaultValue(false)]
        public bool ShowLine { get; set; }

        [DefaultValue(false)]
        public bool ShowTip { get; set; }

        ToolTip _toolTip;
        ToolTip toolTip
        {
            get
            {
                if (_toolTip == null)
                    _toolTip = new ToolTip();
                return _toolTip;
            }
        }
        /// <summary>
        /// 此属性无效, 请使用TextFont
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }

        /*
        ZScrollBar _innerZScrollBar;
        ZScrollBar innerZScrollBar
        {
            get {
                if(_innerZScrollBar==null)
                {
                    VScrollBar vScrollBar = new VScrollBar();
                    int width = vScrollBar.Width;
                    vScrollBar.Dispose();
                    _innerZScrollBar = new ZScrollBar(ZScrollOrientation.Vertical);
                    _innerZScrollBar.Width = width;
                    int c = (this.Height - this.ClientRectangle.Height) / 2;
                    _innerZScrollBar.Location = new Point(this.Location.X+this.Width-width-c,this.Location.Y+c);
                    _innerZScrollBar.Height = this.ClientRectangle.Height;
                    _innerZScrollBar.Visible = false;
                    _innerZScrollBar.Scroll += _innerZScrollBar_Scroll;
                    this.Parent.Controls.Add(_innerZScrollBar);
                    _innerZScrollBar.BringToFront();
                }
                return _innerZScrollBar;
            }
        }

        private void _innerZScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            //当滚动条滚动时，通知控件也跟着滚动吧。。。

            SCROLLINFO info = scrollInfo;

            info.nPos = innerZScrollBar.Value;

            SetScrollInfo(this.Handle, (int)ScrollBarDirection.SB_VERT, ref info, true);

            PostMessage(this.Handle, WM_VSCROLL, MakeLong((short)SB_THUMBTRACK, (short)(info.nPos)), 0);
        }
        */
        Rectangle mouseOnRect = new Rectangle();
        //Point mouseOnPoint = new Point(-1,-1);


        protected override void OnMouseMove(MouseEventArgs e) //鼠标滑过事件
        {
            base.OnMouseMove(e);
            if (!mouseOnRect.Contains(e.Location))
            {
                //mouseOnPoint = e.Location;
                Invalidate();
            }

        }

        protected override void OnMouseDown(MouseEventArgs e) //鼠标点击事件
        {
            base.OnMouseDown(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e) //鼠标移开事件
        {
            base.OnMouseLeave(e);

            mouseOnRect = new Rectangle();
            //mouseOnPoint = new Point(-1, -1);

            Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }
       
        /*
        [DllImport("user32.dll")]
        public static extern bool ShowScrollBar(IntPtr hWnd, ScrollBar bar, int cmd);

        public enum ScrollBar
        {
            Horizontal = 0,
            Vertical = 1,
            Control = 2,
            Both = 3
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if(m.Msg== 0x198)
            {
                SCROLLINFO info = scrollInfo;
                if (info.nMax > 0)
                {
                    int pos = info.nPos;

                    if (pos >= 0)
                    {
                        innerZScrollBar.ValueSet = pos;
                    }
                }
            }
            //Console.WriteLine(m);
            
        }


        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
           
            //ScrollWindow(this.Handle, 0, -25, IntPtr.Zero, IntPtr.Zero);
        }

        [DllImport("User32.dll", EntryPoint = "ScrollWindow")]
        public static extern int ScrollWindow(IntPtr hWnd, int XAmount, int YAmount, IntPtr IpRect, IntPtr lpClipRect);

        private void button1_Click(object sender, EventArgs e)
        {
            IntPtr handle = this.Handle;
            ScrollWindow(handle, 0, -5, IntPtr.Zero, IntPtr.Zero);
        }



        [StructLayout(LayoutKind.Sequential)]
        public struct tagSCROLLINFO
        {
            public uint cbSize;
            public uint fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;
        }
        public enum fnBar
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2
        }
        public enum fMask
        {
            SIF_ALL,
            SIF_DISABLENOSCROLL = 0X0010,
            SIF_PAGE = 0X0002,
            SIF_POS = 0X0004,
            SIF_RANGE = 0X0001,
            SIF_TRACKPOS = 0X0008
        }

        public static int MakeLong(short lowPart, short highPart)
        {
            return (int)(((ushort)lowPart) | (uint)(highPart << 16));
        }
        public const int SB_THUMBTRACK = 5;
        public const int WM_HSCROLL = 0x114;
        public const int WM_VSCROLL = 0x115;
        [DllImport("user32.dll", EntryPoint = "GetScrollInfo")]
        public static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);
        [DllImport("user32.dll", EntryPoint = "SetScrollInfo")]
        public static extern int SetScrollInfo(IntPtr hwnd, int fnBar, [In] ref SCROLLINFO lpsi, bool fRedraw);

        [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, long wParam, int lParam);

        private void SetScrollVisible()
        {

            if (scrollInfo.nMax > 0)
                innerZScrollBar.Visible = true;
            else
            {
                innerZScrollBar.Visible = false;
                return;
            }
               

            innerZScrollBar.Minimum = scrollInfo.nMin;

            innerZScrollBar.Maximum = Convert.ToInt32(scrollInfo.nMax + 1);

            innerZScrollBar.LargeChange = (int)scrollInfo.nPage;

            innerZScrollBar.SmallChange = this.ItemHeight; 

           
        }


        
        private SCROLLINFO scrollInfo
        {
            get
            {
                SCROLLINFO si = new SCROLLINFO();

                si.cbSize = (uint)Marshal.SizeOf(si);

                si.fMask = (int)(ScrollInfoMask.SIF_DISABLENOSCROLL | ScrollInfoMask.SIF_ALL);

                GetScrollInfo(this.Handle, (int)ScrollBarDirection.SB_VERT, ref si);

                return si;

            }

        }
        */
    }


    /*
    public struct SCROLLINFO
    {
        public uint cbSize;
        public uint fMask;
        public int nMin;
        public int nMax;
        public uint nPage;
        public int nPos;
        public int nTrackPos;
    }
    enum ScrollInfoMask
    {
        SIF_RANGE = 0x1,
        SIF_PAGE = 0x2,
        SIF_POS = 0x4,
        SIF_DISABLENOSCROLL = 0x8,
        SIF_TRACKPOS = 0x10,
        SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS
    }
    enum ScrollBarDirection
    {
        SB_HORZ = 0,
        SB_VERT = 1,
        SB_CTL = 2,
        SB_BOTH = 3
    }
    */


}

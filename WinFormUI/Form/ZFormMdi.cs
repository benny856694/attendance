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
    public partial class ZFormMdi : ZForm
    {
        List<TabHeader> lstTabHeader = new List<TabHeader>();
        int defaultTabWidth = 190;
        int tabWidth = 0;

       

        // 滑动
        TabHeader thMouseDown = null;
        Point pMouseDown = new Point();
        bool slided = false;

        //public Color BottomLineColor = Color.FromArgb(188, 188, 188);
        [Browsable(false)]
        public ZFormChild SelectedChild { get; set; }

        public ZFormMdi()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.ResizeRedraw = true;

            tabWidth = defaultTabWidth;
        }


        public void Select(TabHeader tab)
        {
            tab.Selected = true;
            tab.ChildForm.FormActive();
            if (SelectedChild != null && !SelectedChild.IsDisposed)
                SelectedChild.FormDeactivate();
            SelectedChild = tab.ChildForm;
           

            if (tab != null)
            {
                foreach (TabHeader th in lstTabHeader)
                {
                    if (th != tab)
                    {
                        th.Selected = false;
                    }
                }
                this.Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            thMouseDown = null;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                pMouseDown = e.Location;

                foreach (TabHeader th in lstTabHeader)
                {
                    if (th.HitTest(pMouseDown) && !th.CloseHitTest(pMouseDown))
                    {
                        thMouseDown = th;
                        if (SelectedChild == th.ChildForm)
                            return;
                        thMouseDown.Selected = true;
                        th.ChildForm.FormActive();
                        if (SelectedChild != null && !SelectedChild.IsDisposed)
                            SelectedChild.FormDeactivate();
                        SelectedChild = th.ChildForm;
                        break;
                    }
                }

                if (thMouseDown != null)
                {
                    foreach (TabHeader th in lstTabHeader)
                    {
                        if (th != thMouseDown)
                        {
                            th.Selected = false;
                        }
                    }
                    this.Invalidate();
                }
            }

        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left && thMouseDown != null)
            {
                if (!slided)
                {
                    if (Math.Abs(e.X - pMouseDown.X) > 15)
                    {
                        slided = true;
                    }
                }
                else
                {
                    //btnAddNew.Visible = false;

                    Point newPos = thMouseDown.Rect.Location;
                    newPos.X += e.Location.X - pMouseDown.X;

                    // 是否在父窗体范围内移动
                    if (newPos.X < 0)
                        newPos.X = 0;
                    if (newPos.X > this.Width - thMouseDown.Rect.Width)
                        newPos.X = this.Width - thMouseDown.Rect.Width;



                    // 判断移动方向，向左或向右
                    if (e.Location.X - pMouseDown.X > 0)
                    {
                        // 判断是否已经是最后一个Tab
                        if (thMouseDown.TabIndex != lstTabHeader.Count - 1)
                        {
                            TabHeader thRight = lstTabHeader[thMouseDown.TabIndex + 1];

                            // 向右移动时，判断是否与后一Tab 交换位置：当前Tab的 Right ,超过后一Tab 位置的一半
                            if (newPos.X + tabWidth > thRight.Rect.X + tabWidth / 2)
                            {
                                thRight.TabIndex--;
                                thMouseDown.TabIndex++;
                                lstTabHeader.Sort();
                            }
                        }
                    }
                    else
                    {
                        // 判断是否已经是第0个Tab
                        if (thMouseDown.TabIndex != 0)
                        {
                            TabHeader thLeft = lstTabHeader[thMouseDown.TabIndex - 1];

                            // 向右移动时，判断是否与后一Tab 交换位置：当前Tab的 Right ,超过后一Tab 位置的一半
                            if (newPos.X < thLeft.Rect.X + tabWidth / 2)
                            {
                                thLeft.TabIndex++;
                                thMouseDown.TabIndex--;
                                lstTabHeader.Sort();
                            }
                        }
                    }

                    thMouseDown.Rect.X = newPos.X;
                    pMouseDown = e.Location;
                    this.Invalidate();
                }
            }
            else
            {
                this.Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            thMouseDown = null;
            pMouseDown = new Point();

            if (slided)
            {
                slided = false;

                for (int i = 0; i < lstTabHeader.Count; i++)
                {
                    lstTabHeader[i].TabIndex = i;
                }

                //btnAddNew.Visible = true;
                this.Invalidate();
            }
            else
            {
                // 判断是否是Close 区域被点击
                TabHeader thDelete = null;
                foreach (TabHeader th in lstTabHeader)
                {
                    if (th.CloseHitTest(e.Location))
                    {
                        thDelete = th;
                        break;
                    }
                }

                if (thDelete != null)
                {
                    // 移除关闭的Tab, 并重新调整Tab 的Index ,以及Tab 的宽度
                    //lstTabHeader.Remove(thDelete);
                    //thDelete.Dispose();

                    //widthCalculate(false);

                    //for (int i = 0; i < lstTabHeader.Count; i++)
                    //{
                    //    lstTabHeader[i].TabIndex = i;
                    //}

                    //lstTabHeader.Sort();
                    //this.Invalidate();

                    thDelete.ChildForm.Close();
                    //removeTabHeader(thDelete);
                }
            }
        }

        //protected override void OnMouseLeave(EventArgs e)
        //{
        //    base.OnMouseLeave(e);
        //    this.Invalidate();
        //}

        protected override void OnPaint(PaintEventArgs e)
        {

            base.OnPaint(e);

            if (DesignMode)
                return;
            SmoothingMode oldMode = e.Graphics.SmoothingMode;
            CompositingQuality oldCompositingQuality = e.Graphics.CompositingQuality;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            // 判断重绘区域大小，解决由最小化还原后，无法绘制Tab的问题
            if (currPaintTh == null || e.ClipRectangle.Size.Width > TabHeader.Left_Offset)
            {
                // 被选中的Tab 需要处于顶层，因此最后绘制
                TabHeader thSelected = null;
                foreach (TabHeader th in lstTabHeader)
                {
                    bool drawLine = true;
                    if (th.TabIndex + 1 < lstTabHeader.Count)
                        if (lstTabHeader[th.TabIndex + 1].Selected)
                            drawLine = false;
                    if(th.TabIndex+1== lstTabHeader.Count)
                        drawLine = false;

                    if (th.Selected)
                        thSelected = th;
                    else
                        th.DrawAll(e.Graphics, th.Rect, drawLine);
                }
                // 最后绘制
                if (thSelected != null)
                {
                    //thSelected.DrawAll(e.Graphics, thSelected.Rect);
                }

            }
            else
            {
                // 绘制完整的TabHeader，如果仅绘制指定区域，可能会出现白色背景
                currPaintTh.DrawAll(e.Graphics, currPaintTh.Rect);
                currPaintTh = null;
            }

            e.Graphics.SmoothingMode = oldMode;
            e.Graphics.CompositingQuality = oldCompositingQuality;
        }

        protected override void OnResize(EventArgs e)
        {

            widthCalculate(false);

            foreach (TabHeader th in lstTabHeader)
            {
                th.Width = tabWidth;
            }

            base.OnResize(e);
        }

        private TabHeader addNewTab(string title, Font font, string url = "")
        {
            widthCalculate();
            
            TabHeader newTh = new TabHeader(lstTabHeader.Count, title, font, tabWidth, this, url);
            newTh.Selected = true;

            foreach (TabHeader th in lstTabHeader)
            {
                th.Selected = false;
            }

            lstTabHeader.Add(newTh);
            newTh.OnPaintRequest += newTh_OnPaintRequest;

            this.Invalidate();
            return newTh;
        }


        void widthCalculate(bool forAdd = true)
        {
            if (forAdd)
            {
                if (this.Width < (lstTabHeader.Count + 1) * (tabWidth) + 150 || tabWidth < defaultTabWidth)
                {
                    tabWidth = (this.Width - 150) / (lstTabHeader.Count + 1);
                }
            }
            else
            {
                if (this.Width < (lstTabHeader.Count) * (tabWidth) + 150 || tabWidth < defaultTabWidth)
                {
                    if (lstTabHeader.Count > 0)
                        tabWidth = (this.Width - 150) / (lstTabHeader.Count);
                }
            }

            if (tabWidth > defaultTabWidth)
                tabWidth = defaultTabWidth;

            foreach (TabHeader th in lstTabHeader)
            {
                th.Width = tabWidth;
            }

            // 设置btnAddNew 的位置
            //if (forAdd)
            //{
            //    btnAddNew.Left = (tabWidth - TabHeader.Left_Offset) * (lstTabHeader.Count + 1) + 18;
            //}
            //else
            //    btnAddNew.Left = (tabWidth - TabHeader.Left_Offset) * (lstTabHeader.Count) + 18;

        }

        TabHeader currPaintTh;
        //Rectangle currRect;
        void newTh_OnPaintRequest(TabHeader th, Rectangle rect)
        {
            currPaintTh = th;
            //currRect = rect;
            //this.Invalidate(rect);
            this.Invalidate(false);
        }

        public void HideForm(Form form)
        {
            if(form is ZFormChild)
            {
                RemoveTabHeader(((ZFormChild)form).TabHeader);
            }
           
        }

        public void RemoveTabHeader(TabHeader thDelete)
        {
            //thDelete.ChildForm
            if(thDelete.IsDisposed)
            {
                return;
            }


            if (this.MdiChildren.Count() == 1)  //if (lstTabHeader.Count == 1)
            {
                this.Close();
                return;
            }

            lstTabHeader.Remove(thDelete);
            thDelete.Dispose();

            widthCalculate(false);


            if (thDelete.ChildForm == SelectedChild)
            {
                //MessageBox.Show(thDelete.TabIndex.ToString());
                if (thDelete.TabIndex < lstTabHeader.Count)
                {
                    lstTabHeader[thDelete.TabIndex].Selected = true;
                    if (SelectedChild != null && !SelectedChild.IsDisposed)
                        SelectedChild.FormDeactivate();
                    SelectedChild = lstTabHeader[thDelete.TabIndex].ChildForm;
                    SelectedChild.FormActive();
                }
                else if (thDelete.TabIndex > 0)
                {
                    lstTabHeader[thDelete.TabIndex - 1].Selected = true;
                    if (SelectedChild != null && !SelectedChild.IsDisposed)
                        SelectedChild.FormDeactivate();
                    SelectedChild = lstTabHeader[thDelete.TabIndex - 1].ChildForm;
                    SelectedChild.FormActive();
                }
            }


            for (int i = 0; i < lstTabHeader.Count; i++)
            {
                lstTabHeader[i].TabIndex = i;
            }

            lstTabHeader.Sort();
            this.Invalidate();


        }

        //public event Action<TabHeader> OnTabChange;

        

        //bool isfirst = true;
        public void AddTableForm(ZFormChild zFormChild)
        {
            zFormChild.FormBorderStyle = FormBorderStyle.None;
            Font font = new System.Drawing.Font("微软雅黑", 9F);
            zFormChild.MdiParent = this;
            zFormChild.MdiForm = this;
            //zFormChild.Parent = DockPanel;
            zFormChild.Dock = DockStyle.Fill;
            
            //zFormChild.Text = "你好" + i;
            if (SelectedChild != null && !SelectedChild.IsDisposed)
                SelectedChild.FormDeactivate();
            SelectedChild = zFormChild;
            TabHeader th = addNewTab(zFormChild.Text, font);
            th.ChildForm = zFormChild;
            zFormChild.TabHeader = th;
            zFormChild.HandleDestroyed += (a, b) =>
            {
                RemoveTabHeader(th);
            };
            zFormChild.TextChanged += (a, b) =>
            {
                th.Title = zFormChild.Text;
                this.Invalidate(false);
            };
           
            

            //var wnd = win32.FindWindowA(null, "窗口标题");



            //MoveWindow(zFormChild.Handle, this.DockPanel.Left, this.DockPanel.Top, this.DockPanel.Width, this.DockPanel.Height, true);
            zFormChild.BringToFront();
            zFormChild.Show();


            //SetParent(zFormChild.Handle, DockPanel.Handle);
        }

       

        ////修改窗口消息
        //protected override void WndProc(ref Message m)
        //{
        //    if (!DesignMode)  //在VS设计模式下不启用
        //    {
        //        bool alreadyHandled = false;
        //        switch (m.Msg)
        //        {
        //            case (int)WinAPI.WindowMessages.WM_NCHITTEST:
        //                alreadyHandled = WmNcHitTest(ref m);
        //                break;
        //            default:
        //                break;
        //        }
        //        if (!alreadyHandled)
        //            base.WndProc(ref m);
        //    }
        //    else
        //        base.WndProc(ref m);

        //}

        private Rectangle TabRectangle
        {
            get
            {
                if (lstTabHeader == null || lstTabHeader.Count == 0)
                    return new Rectangle();
                return new Rectangle(1, 1, lstTabHeader[lstTabHeader.Count-1].Rect.Right, 28);
            }
        }

        //[DllImport("user32.dll", SetLastError = true)]
        //public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);
        //窗口拖拽, 窗口尺寸拖拉
        protected override bool WmNcHitTest(ref Message m)
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

            //这段代码和基类ZFrom大致相同, 除了这句代码
            if (TabRectangle.Contains(p))
            {
                m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTCLIENT);
                return true;
            }

            if (new Rectangle(0, 0, ClientRectangle.Width, this.CaptionHeight).Contains(p))
            {
                m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTCAPTION);
                return true;
            }

            m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTCLIENT);
            return true;
        }

       

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetMdiClient();
        }
        
        #region 设置MDI样式
        private MdiClient GetMdiClient()
        {
            MdiClient mdiclient = null;
            foreach (Control ctl in Controls)
            {
                if ((mdiclient = ctl as MdiClient) != null)
                    break;
            }
            return mdiclient;
        }

        private void SetMdiClient()
        {
            if (!IsMdiContainer)
                return;

            MdiClient mdi = GetMdiClient();
            if (mdi != null)
            {
                SetMdiStyles(mdi);
                UpdateMdiStyles(mdi);
                SetMdiClientLocation(mdi);
            }
        }

        private void SetMdiClientLocation(MdiClient mdi)
        {
            mdi.BackColor = Color.White;
        }

        private void SetMdiStyles(MdiClient mdi)
        {
            // remove the border

            int style = WinAPI.GetWindowLong(mdi.Handle, (int)WinAPI.GWLPara.GWL_STYLE);
            int exStyle = WinAPI.GetWindowLong(mdi.Handle, (int)WinAPI.GWLPara.GWL_EXSTYLE);

            style &= ~(int)WinAPI.WindowStyle.WS_BORDER;
            exStyle &= ~(int)WinAPI.WindowStyleEx.WS_EX_CLIENTEDGE;

            WinAPI.SetWindowLong(mdi.Handle, (int)WinAPI.GWLPara.GWL_STYLE, style);
            WinAPI.SetWindowLong(mdi.Handle, (int)WinAPI.GWLPara.GWL_EXSTYLE, exStyle);

            WinAPI.ShowScrollBar(mdi.Handle, (int)WinAPI.ScrollBar.SB_BOTH, 0 /*false*/);
        }

        private void UpdateMdiStyles(MdiClient mdi)
        {
            WinAPI.SetWindowPos(mdi.Handle, IntPtr.Zero, 0, 0, 0, 0,
                (uint)WinAPI.SWPPara.SWP_NOACTIVATE |
                (uint)WinAPI.SWPPara.SWP_NOMOVE |
                (uint)WinAPI.SWPPara.SWP_NOSIZE |
                (uint)WinAPI.SWPPara.SWP_NOZORDER |
                (uint)WinAPI.SWPPara.SWP_NOOWNERZORDER |
                (uint)WinAPI.SWPPara.SWP_FRAMECHANGED);
        }
        #endregion



    }
}

using MetroFramework.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace ZXCL.WinFormUI
{
    

    public class ZFlowLayoutPanel : FlowLayoutPanel
    {
        //private Gdu.WinFormUI.GMHScrollBar gmhScrollBar;
       // private Gdu.WinFormUI.GMVScrollBar gmvScrollBar;
        bool added = true;
        public ZScrollBar VerticalMetroScrollbar = new ZScrollBar(ZScrollOrientation.Vertical);
        public ZFlowLayoutPanel()
        {
            VerticalMetroScrollbar.Scroll += VerticalScrollbarScroll;
        }

       
        private void VerticalScrollbarScroll(object sender, ScrollEventArgs e)
        {
            AutoScrollPosition = new Point(0, e.NewValue);
            setScrollBarLocation();
        }

       

        private void setScrollBarLocation()
        {
            VerticalMetroScrollbar.Location = new Point(this.Width + this.Location.X - VerticalMetroScrollbar.Width, this.Location.Y);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (DesignMode)
                return;

            if (VerticalScroll.Visible)
            {
                if (this.Parent != null && added)
                {
                    this.Parent.Controls.Add(VerticalMetroScrollbar);
                    //this.Parent.Controls.Add(lb);
                    setScrollBarLocation();
                    VerticalMetroScrollbar.BringToFront();
                    added = false;
                }
                VerticalMetroScrollbar.Height = this.Height;
                VerticalMetroScrollbar.Minimum = VerticalScroll.Minimum;
                VerticalMetroScrollbar.Maximum = VerticalScroll.Maximum;
                VerticalMetroScrollbar.LargeChange = VerticalScroll.LargeChange;
                VerticalMetroScrollbar.SmallChange = VerticalScroll.SmallChange;
                VerticalMetroScrollbar.Value = VerticalScroll.Value;


            }
            VerticalMetroScrollbar.Visible = VerticalScroll.Visible;
        }


        [SecuritySafeCritical]
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (!DesignMode)
            {
                WinApi.ShowScrollBar(Handle, (int)WinApi.ScrollBar.SB_BOTH, 0);
            }
        }

    }
}

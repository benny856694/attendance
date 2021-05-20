using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace ZXCL.WinFormUI
{
    public class ZTextBoxBase : TextBox
    {
        
       // private bool waterDrawed = false;

        public ZTextBoxBase()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            base.BorderStyle = BorderStyle.None;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0xf)
            {
                //MessageBox.Show("");
                //拦截系统消息，获得当前控件进程以便重绘。  
                //一些控件（如TextBox、Button等）是由系统进程绘制，重载OnPaint方法将不起作用.  
                //所有这里并没有使用重载OnPaint方法绘制TextBox边框。  
                //  
                //MSDN:重写 OnPaint 将禁止修改所有控件的外观。  
                //那些由 Windows 完成其所有绘图的控件（例如 Textbox）从不调用它们的 OnPaint 方法，  
                //因此将永远不会使用自定义代码。请参见您要修改的特定控件的文档，  
                //查看 OnPaint 方法是否可用。如果某个控件未将 OnPaint 作为成员方法列出，  
                //则您无法通过重写此方法改变其外观。  
                //  
                //MSDN:要了解可用的 Message.Msg、Message.LParam 和 Message.WParam 值，  
                //请参考位于 MSDN Library 中的 Platform SDK 文档参考。可在 Platform SDK（“Core SDK”一节）  
                //下载中包含的 windows.h 头文件中找到实际常数值，该文件也可在 MSDN 上找到。  

                //返回结果  
                using (Graphics g = Graphics.FromHwnd(base.Handle))
                {
                    _Paint(g);
                }

                m.Result = IntPtr.Zero;
            }
        }


        private void _Paint(Graphics g)
        {
            drawWater(g);
        }

       

        private void drawWater(Graphics g)
        {
            //获取控件绑定到的窗口句柄

            if (string.IsNullOrEmpty(this.Text) && !String.IsNullOrEmpty(this.WaterText) && !this.Focused)
            {
                TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter;
                TextRenderer.DrawText(g, WaterText, this.Font, this.ClientRectangle, WaterColor, flags);
            }
        }


        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            //using (Graphics g = Graphics.FromHwnd(base.Handle))
            //{
            //    _Paint(g);
            //}
            this.Invalidate();

        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            //using (Graphics g = Graphics.FromHwnd(base.Handle))
            //{
            //    _Paint(g);
            //}
            this.Invalidate();
        }

        

        private string _waterText = String.Empty;
        /// <summary>
        /// 水印文字
        /// </summary>
        [Description("水印文字")]
        public string WaterText
        {
            get { return _waterText; }
            set { _waterText = value; }
        }
        private Color _waterColor = Color.DarkGray;
        /// <summary>
        /// 水印颜色
        /// </summary>
        [Description("水印颜色")]
        public Color WaterColor
        {
            get { return _waterColor; }
            set { _waterColor = value; }
        }

       
    }
}
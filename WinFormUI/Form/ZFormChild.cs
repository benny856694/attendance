using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZXCL.WinFormUI
{
    public partial class ZFormChild : Form
    {
        [Browsable(false)]
        public ZFormMdi MdiForm { get; set; }
        [Browsable(false)]
        public TabHeader TabHeader { get; set; }

        public ZFormChild()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (OnFormActive != null)
                OnFormActive();
        }


        /// <summary>
        /// 窗口选中
        /// </summary>
        public void FormActive()
        {
            this.BringToFront();
            if(OnFormActive!=null)
            OnFormActive();
        }

        /// <summary>
        /// 选中当前窗口
        /// </summary>
        public new void Select()
        {
            MdiForm.Select(TabHeader);
        }

        /// <summary>
        /// 窗口失去选中状态
        /// </summary>
        public void FormDeactivate()
        {
            if(OnFormDeactivate!=null)
            OnFormDeactivate();
        }

        /// <summary>
        /// 选中当前窗口时发生
        /// </summary>
        public event Action OnFormActive;
        /// <summary>
        /// 窗口失去选中状态时发生
        /// </summary>
        public event Action OnFormDeactivate;
    }
}

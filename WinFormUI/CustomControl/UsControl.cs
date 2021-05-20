using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZXCL.WinFormUI.CustomControl
{
    public partial class UsControl : UserControl
    {
        public UsControl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            //this.Font = new Font("宋体", 11.5f, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        [Description("错误提示文本")]
        [Browsable(true)]
        [DefaultValue(null)]
        public string ErrorText { get; set; }

        [Description("是否必录")]
        [Browsable(true)]
        [DefaultValue(false)]
        public bool MustInput { get; set; }

        [Description("忽略标志")]
        [Browsable(true)]
        [DefaultValue(false)]
        public bool Ignore { get; set; }


        //纯数据一般不需要校验,但是某些特殊的项需要
        [Description("是否用于纯数据校验")]
        [Browsable(true)]
        [DefaultValue(false)]
        public bool IsPureData { get; set; }

        /// <summary>
        /// 验证输入
        /// </summary>
        /// <returns></returns>
        public virtual bool ValidateInput()
        {
            return true;
        }

        /// <summary>
        /// 控件值 默认第一个文本值, 有两个值则分隔符分开
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        public virtual string Value { get; }

        string _Separator = "#";  //默认#号
        [Description("分隔符")]
        [Browsable(true)]
        [DefaultValue("#")]
        public string Separator { get { return _Separator; } set { _Separator = value; } }

        [DefaultValue(false)]
        public bool CanShowTwoBox { get; set; }

        [DefaultValue(false)]
        public virtual bool ShowTwoBox { get; set; }

        public virtual void BorderFlash()
        {

        }
    }
}

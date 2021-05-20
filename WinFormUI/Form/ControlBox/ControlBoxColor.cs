using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ZXCL.WinFormUI
{
    public class ControlBoxColor
    {
        public Action OnColorChange;

        private void change()
        {
            if (OnColorChange != null)
                OnColorChange();
        }
        protected Color foreColorNormal = Color.Black;
        protected Color backColorNormal = Color.Transparent;
        protected Color foreColorHover = Color.White;
        protected Color backColorHover = Color.CornflowerBlue;
        protected Color foreColorPress = Color.White;
        protected Color backColorPress = Color.RoyalBlue;

        [Description("前景色")]
        [DefaultValue(typeof(Color), "Black")]
        public Color ForeColorNormal
        {
            get { return foreColorNormal; }
            set { foreColorNormal = value; change(); }
        }
        [Description("背景色")]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color BackColorNormal
        {
            get { return backColorNormal; }
            set { backColorNormal = value; change(); }
        }

        [Description("鼠标经过时前景色")]
        [DefaultValue(typeof(Color), "White")]
        public Color ForeColorHover
        {
            get { return foreColorHover; }
            set { foreColorHover = value; }
        }

        [Description("鼠标经过时背景色")]
        [DefaultValue(typeof(Color), "CornflowerBlue")]
        public Color BackColorHover
        {
            get { return backColorHover; }
            set { backColorHover = value; }
        }

        [Description("鼠标按下时前景色")]
        [DefaultValue(typeof(Color), "White")]
        public Color ForeColorPress
        {
            get { return foreColorPress; }
            set { foreColorPress = value; }
        }

        [Description("鼠标按下时前景色")]
        [DefaultValue(typeof(Color), "RoyalBlue")]
        public Color BackColorPress
        {
            get { return backColorPress; }
            set { backColorPress = value; }
        }

        public override string ToString()
        {
            return "";
        }
    }

    public class CloseBoxColor : ControlBoxColor
    {

        public CloseBoxColor()
        {
            backColorHover = Color.Red;
            backColorPress = Color.OrangeRed;
        }

        
        [Description("鼠标经过时背景色")]
        [DefaultValue(typeof(Color), "Red")]
        public new Color BackColorHover
        {
            get { return backColorHover; }
            set { backColorHover = value; }
        }

        [Description("鼠标按下时前景色")]
        [DefaultValue(typeof(Color), "OrangeRed")]
        public new Color BackColorPress
        {
            get { return backColorPress; }
            set { backColorPress = value; }
        }
    }

}

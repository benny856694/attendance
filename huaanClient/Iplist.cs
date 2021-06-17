using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace huaanClient
{
    public partial class Iplist : UserControl
    {
        public Iplist()
        {
            InitializeComponent();
            this.btn.Text = Properties.Strings.Switch;
        }
    }
}

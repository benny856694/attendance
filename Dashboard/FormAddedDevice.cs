using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dashboard
{
    public partial class FormAddedDevice : Form
    {
        public FormAddedDevice()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            var valid = this.ValidateChildren();
            if (!valid) return;
            this.DialogResult = DialogResult.OK;
        }

        private void textBoxName_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(textBoxName, string.IsNullOrEmpty(textBoxName.Text)
                ? Properties.Strings.CantBeEmpty : string.Empty);
        }
    }
}

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
            var valid = this.Validate();
            if (!valid) return;
            this.DialogResult = DialogResult.OK;
        }


        private void textBox_Validating(object sender, CancelEventArgs e)
        {
            bool valid = ValidateNotEmpty(((TextBox) sender));
            e.Cancel = !valid;
        }

        private  bool ValidateNotEmpty(TextBox tb)
        {
            var valid = !string.IsNullOrEmpty(tb.Text);
            errorProvider1.SetError(tb, valid ? string.Empty : Properties.Strings.CantBeEmpty);
            return valid;
        }
    }
}

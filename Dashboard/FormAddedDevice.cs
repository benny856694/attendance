using Dashboard.Model;
using FluentValidation;
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

        public Device Device =>  new Model.Device
                    {
                        Name = bunifuTextBoxName.Text,
                        IP = bunifuTextBoxIp.Text,
                        Port = int.Parse(bunifuTextBoxPort.Text),
                        UserName = bunifuTextBoxUsername.Text,
                        Password = bunifuTextBoxPassword.Text,
                    };

    public FormAddedDevice()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            var vr = new FormAddDeviceValidator().Validate(this);
            if (!vr.IsValid)
            {
                MessageBox.Show(this, vr.ToString());
                return;
            }
            this.DialogResult = DialogResult.OK;
        }


        private void textBox_Validating(object sender, CancelEventArgs e)
        {
           // bool valid = ValidateNotEmpty(((TextBox) sender));
           // e.Cancel = !valid;
        }

        private  bool ValidateNotEmpty(TextBox tb)
        {
            var valid = !string.IsNullOrEmpty(tb.Text);
            errorProvider1.SetError(tb, valid ? string.Empty : Properties.Strings.CantBeEmpty);
            return valid;
        }



        class FormAddDeviceValidator : AbstractValidator<FormAddedDevice>
        {
            public FormAddDeviceValidator()
            {
                CascadeMode = CascadeMode.Stop;
                RuleFor(d => d.bunifuTextBoxName.Text)
                    .NotEmpty()
                    .WithName(Properties.Strings.Name);
                RuleFor(d => d.bunifuTextBoxIp.Text)
                    .NotEmpty()
                    .Matches(@"^(?:(?:2(?:[0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9])\.){3}(?:(?:2([0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9]))$")
                    .WithName(Properties.Strings.IP);
                RuleFor(d => d.bunifuTextBoxPort.Text)
                    .NotEmpty()
                    .Matches(@"^\d{1,5}$")
                    .WithName(Properties.Strings.Port);
            }
        }

        private void FormAddedDevice_Load(object sender, EventArgs e)
        {

        }
    }


    
}

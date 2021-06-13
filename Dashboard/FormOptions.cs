using Dashboard.Model;
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
    public partial class FormOptions : Form
    {
        public Settings Settings { get; set; } = new Settings();
        public FormOptions()
        {
            InitializeComponent();
        }

        private void FormOptions_Load(object sender, EventArgs e)
        {
            Services.Tracker.Track(Settings);

            checkBoxShowTemplateImage.Checked = Settings.ShowTemplateImage;
            checkBoxShowRealtimeImage.Checked = Settings.ShowRealtimeImage;
        }

        private void FormOptions_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!checkBoxShowRealtimeImage.Checked && !checkBoxShowTemplateImage.Checked)
            {
                MessageBox.Show(Properties.Strings.AtLeastOneImageShouldBeDisplayed);
                return;
            }

            Settings.ShowTemplateImage = checkBoxShowTemplateImage.Checked;
            Settings.ShowRealtimeImage = checkBoxShowRealtimeImage.Checked;
            Services.Tracker.Persist(Settings);


            DialogResult = DialogResult.OK;
        }
    }
}

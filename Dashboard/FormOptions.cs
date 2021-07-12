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
            bunifuTextBoxPairingFolder.Text = Settings.PairingFolder;
            checkBoxPlaySound.Checked = Settings.PlayAudio;
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
            Settings.PairingFolder = bunifuTextBoxPairingFolder.Text;
            Settings.PlayAudio = checkBoxPlaySound.Checked;
            Services.Tracker.Persist(Settings);


            DialogResult = DialogResult.OK;
        }

        private void buttonSelPairingFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = Settings.PairingFolder;
            var dr = folderBrowserDialog1.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                bunifuTextBoxPairingFolder.Text = folderBrowserDialog1.SelectedPath;
            }

        }
    }
}

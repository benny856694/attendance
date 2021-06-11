using Dashboard.Model;
using HaSdkWrapper;
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
    public partial class FormSearchCamera : Form
    {
        DateTime _lastIpFoundTime = DateTime.Now;


        public IList<Model.Device> AddedDevices { get; set; }

        public FormSearchCamera()
        {
            InitializeComponent();
            AddedDevices = new List<Model.Device>(0);
        }

        private void FormSearchCamera_Load(object sender, EventArgs e)
        {
            foreach (var item in AddedDevices)
            {
                listBoxAdded.Items.Add(item);
            }

            HaCamera.DeviceDiscovered += HaCamera_DeviceDiscovered;
            HaCamera.DiscoverDevice();

        }

        private void HaCamera_DeviceDiscovered(object sender, DeviceDiscoverdEventArgs e)
        {
            _lastIpFoundTime = DateTime.Now;
            this.BeginInvoke((Func<object, int>)listBoxNew.Items.Add, e.IP);

        }

        private void FormSearchCamera_FormClosed(object sender, FormClosedEventArgs e)
        {
            HaCamera.DeviceDiscovered -= HaCamera_DeviceDiscovered;
            timer1.Dispose();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            AddedDevices = this.listBoxAdded.Items.OfType<Device>().ToList();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((DateTime.Now - _lastIpFoundTime) > TimeSpan.FromSeconds(10))
            {
                timer1.Dispose();
                pictureBox1.Visible = false;
            }
        }

        private void listBoxNew_SelectedValueChanged(object sender, EventArgs e)
        {
            var selectedIp = (string)listBoxNew.SelectedItem;
            buttonAdd.Enabled = listBoxNew.SelectedItem != null 
                && AddedDevices.FirstOrDefault(x=>x.IP == selectedIp) == null;
        }

        private void listBoxAdded_SelectedValueChanged(object sender, EventArgs e)
        {
            this.buttonDelete.Enabled = listBoxAdded.SelectedItem != null;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            using (var form = new FormAddedDevice())
            {
                form.textBoxIP.Text = (string)listBoxNew.SelectedItem;
                var dr = form.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    listBoxAdded.Items.Add(form.Device);
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            listBoxAdded.Items.Remove(listBoxAdded.SelectedItem);
        }
    }
}

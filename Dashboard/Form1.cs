using Dashboard.Controls;
using HaSdkWrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dashboard
{
    public partial class Form1 : Form
    {
        CameraUserControl lastCameraUserControl = null;
        Dictionary<HaCamera, CameraUserControl> _cameraToControlMap
            = new Dictionary<HaCamera, CameraUserControl>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HaCamera.InitEnvironment();
            HaCamera.DeviceDiscovered += HaCamera_DeviceDiscovered;
            HaCamera.DiscoverDevice();


        }

        private void CameraUserControl1_OnClicked(object sender, EventArgs e)
        {
            var cam = sender as CameraUserControl;
            if (cam != null)
            {
                if (cam == lastCameraUserControl)
                {
                    return;
                }

                if (lastCameraUserControl != null)
                {
                    lastCameraUserControl.Selected = false;
                }
                lastCameraUserControl = cam;
                cam.Selected = true;
            }

            
        }

        private void HaCamera_DeviceDiscovered(object sender, DeviceDiscoverdEventArgs e)
        {
            this.BeginInvoke(new Action(()=>listBox1.Items.Add(e.IP)));
            
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            var selectedCameraIp = (string)listBox1.SelectedItem;
            var oldCam = lastCameraUserControl?.Tag as HaCamera;
            if (oldCam?.Ip == selectedCameraIp)
            {
                return;
            }

            if (oldCam != null)
            {
                oldCam.FaceCaptured -= Cam_FaceCaptured;
                oldCam.DisConnect();

            }


            var cam = new HaCamera() { Ip = listBox1.SelectedItem as string };
            cam.Tag = lastCameraUserControl;
            lastCameraUserControl.Tag = cam;
            lastCameraUserControl.TopRightText = selectedCameraIp;
            cam.Port = 9527;
            cam.Username = "admin";
            cam.Password = "admin";
            cam.FaceCaptured += Cam_FaceCaptured;
            cam.Connectnovideo();

        }

        private void Cam_FaceCaptured(object sender, FaceCapturedEventArgs e)
        {
            var cam = (HaCamera)sender;
            var control = cam.Tag as CameraUserControl;
            if (control != null)
            {
                this.BeginInvoke(new Action(() =>
                {
                    var oldImage = control.Image;
                    control.Image = Image.FromStream(new MemoryStream(e.FeatureImageData));
                    oldImage?.Dispose();

                    var bgc = e.IsPersonMatched ? Color.Green : Color.Red;
                    var name = e.PersonName ?? "Unidentified";
                    control.BottomText = name;
                    control.BackColor = bgc;
                }));
            }
           
            
        }
    }
}

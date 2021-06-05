using Dashboard.Controls;
using HaSdkWrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        Dictionary<HaCamera, CameraUserControl> _cameraToControlMap
            = new Dictionary<HaCamera, CameraUserControl>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadIcon();

            HaCamera.InitEnvironment();
            HaCamera.DeviceDiscovered += HaCamera_DeviceDiscovered;
            HaCamera.DiscoverDevice();

            gridControl1.CreateNewControlForCell += GridControl1_CreateNewControlForCell;
            gridControl1.ControlVisibleChanged += GridControl1_ControlVisibleChanged;

            gridControl1.RowColumn = (2, 2);
        }

        private void LoadIcon()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"branding\logo.ico");
            if (File.Exists(path))
            {
                this.Icon = new Icon(path);
            }
        }

        private void GridControl1_ControlVisibleChanged(object sender, ControlVisibleChangedEventArgs e)
        {
            var c = e.Control as CameraUserControl;
            if (c != null)
            {
                ReleaseControlCamera(c);
            }
        }

        private void GridControl1_CreateNewControlForCell(object sender, CreateNewControlEventArgs e)
        {
            var c = new CameraUserControl();
            c.Dock = DockStyle.Fill;
            e.Control = c;
        }

        private void CameraUserControl1_OnClicked(object sender, EventArgs e)
        {

            
        }

        private void HaCamera_DeviceDiscovered(object sender, DeviceDiscoverdEventArgs e)
        {
            this.BeginInvoke(new Action(()=>listBox1.Items.Add(e.IP)));
            
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            var targetControl = gridControl1.SelectedControl as CameraUserControl;
            if (targetControl == null) return;

            var selectedCameraIp = (string)listBox1.SelectedItem;
            HaCamera cam = GetRunningCameraByIp(selectedCameraIp);
            if (ReferenceEquals(cam?.Tag, targetControl)) return;

            if (cam != null)
            {
                //change camera target control
                ChangeCamTargetControl(cam, targetControl);
            }
            else
            {
                ConnectNewCamToControl(selectedCameraIp, targetControl);
            }

        }

        private void ConnectNewCamToControl(string ip, CameraUserControl targetControl)
        {
            ReleaseControlCamera(targetControl);

            var newCam = new HaCamera() { Ip = ip };
            newCam.Tag = targetControl;
            targetControl.Tag = newCam;
            targetControl.TopRightText = ip;
            newCam.Port = 9527;
            newCam.Username = "admin";
            newCam.Password = "admin";
            newCam.FaceCaptured += Cam_FaceCaptured;
            newCam.Connectnovideo();

            _cameraToControlMap.Add(newCam, targetControl);

        }

        private void ChangeCamTargetControl(HaCamera cam, CameraUserControl targetControl)
        {
            ReleaseControlCamera(targetControl);

            var oldControl = cam.Tag as CameraUserControl;
            if (oldControl != null)
            {
                oldControl.Tag = null;
                oldControl.Clear();
            }

            cam.Tag = targetControl;
            targetControl.Tag = cam;
            targetControl.TopRightText = cam.Ip;
            _cameraToControlMap[cam] = targetControl;
            return;

        }

        private void ReleaseControlCamera(CameraUserControl targetControl)
        {
            var cam = targetControl.Tag as HaCamera;
            if (cam != null)
            {
                cam.FaceCaptured -= Cam_FaceCaptured;
                cam.Tag = null;
                targetControl.Tag = null;
                cam.DisConnect();
                _cameraToControlMap.Remove(cam);
            }
            targetControl.Clear();
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

        private HaCamera GetRunningCameraByIp(string ip)
        {
            var controls = gridControl1
                .ChildControls
                .OfType<CameraUserControl>();

            Debug.WriteLine($"total {controls.Count()} controls");
            foreach (var c in controls)
            {
                Debug.WriteLine((c.Tag as HaCamera)?.Ip);
            }

            var cam = controls
                .Select(x => x.Tag as HaCamera)
                .FirstOrDefault(x=>x?.Ip == ip);
            return cam;
                
        }

        private void gridControl1_MouseEnter(object sender, EventArgs e)
        {

        }

        
        private void gridControl1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void gridControl1_MouseLeave(object sender, EventArgs e)
        {
        }

        private void comboBox1Col_SelectedValueChanged(object sender, EventArgs e)
        {
            gridControl1.Cols = Convert.ToInt32(comboBox1Col.SelectedItem);
        }

        private void comboBoxRow_SelectedValueChanged(object sender, EventArgs e)
        {
            gridControl1.Rows = Convert.ToInt32(comboBoxRow.SelectedItem);
        }
    }
}

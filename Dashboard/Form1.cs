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
using Dashboard.Model;
using Jot.Configuration;

namespace Dashboard
{
    public partial class Form1 : Form, ITrackingAware
    {
        Dictionary<HaCamera, CameraUserControl> _cameraToControlMap
            = new Dictionary<HaCamera, CameraUserControl>();


        IList<Device> _connectedDevices
            = new List<Device>();

        public Device[] ConnectedDevices
        {
            get => _connectedDevices?.ToArray() ?? new Device[0];
            set
            {
                var l = new List<Device>(value);
                _connectedDevices = l;
            }
        }

        public IEnumerable<CameraUserControl> AllCameraUserControls => gridControl1.ChildControls.OfType<CameraUserControl>();


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gridControl1.CreateNewControlForCell += GridControl1_CreateNewControlForCell;
            gridControl1.ControlVisibleChanged += GridControl1_ControlVisibleChanged;


            LoadIcon();
            InitUi();

            HaCamera.InitEnvironment();

            Services.Tracker.Track(this);

            ConnectCameras();
            //gridControl1.SetRowCol(2, 2);
        }

        private void ConnectCameras()
        {
            if (_connectedDevices?.Count == 0) return;

            var rows = gridControl1.Rows;
            var cols = gridControl1.Cols;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (_connectedDevices.Count == 0) return;
                    var d = _connectedDevices[0];
                    _connectedDevices.RemoveAt(0);
                    var c = gridControl1.CellAtPosition(i, j) as CameraUserControl;

                    var hc = new HaCamera()
                    {
                        Name = d.Name,
                        Ip = d.IP,
                        Port = d.Port,
                        Username = d.UserName,
                        Password = d.Password
                    };
                    hc.Tag = c;
                    c.Tag = hc;
                    c.TopRightText = hc.Name ?? hc.Ip;
                    hc.FaceCaptured += Cam_FaceCaptured;
                    hc.Connectnovideo();
                    _cameraToControlMap.Add(hc, c);

                }
            }
        }


        private void InitUi()
        {
            this.comboBoxRow.DataSource = new[] { 1, 2, 3, 4, 5};

            this.comboBoxCol.DataSource = new[] { 1, 2, 3, 4, 5};
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
            if (targetControl == null)
            {
                MessageBox.Show(Properties.Strings.PromptSelectedCellIsNull);
                return;
            }

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
            if (targetControl.Tag is HaCamera cam)
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
                    control.BottomText = $"{name} | {e.CaptureTime:T}";
                    control.BackColor = bgc;
                }));
            }
           
            
        }

        private HaCamera GetRunningCameraByIp(string ip)
        {
            var controls = AllCameraUserControls;

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
            gridControl1.Cols = (int)comboBoxCol.SelectedValue;
        }

        private void comboBoxRow_SelectedValueChanged(object sender, EventArgs e)
        {
            gridControl1.Rows = (int)comboBoxRow.SelectedValue;
        }

        public void ConfigureTracking(TrackingConfiguration configuration)
        {
            var cfg = configuration.AsGeneric<Form1>();
            cfg.Property(f => f.comboBoxCol.SelectedIndex, "GridCol");
            cfg.Property(f => f.comboBoxRow.SelectedIndex, "GridRow");
            cfg.Properties(f => new { f.ConnectedDevices });
        }




        private IEnumerable<HaCamera> GetConnectedDevices()
        {
            return AllCameraUserControls
                .Select(x => x.Tag as HaCamera)
                .Where(x => x != null);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //to save the state
            _connectedDevices = GetConnectedDevices().Select(x => new Device
            {
                Name = x.Name,
                IP = x.Ip,
                Port = x.Port,
                Password = x.Password,
                UserName = x.Username
            }).ToList();
        }

        private void buttonSearchDevice_Click(object sender, EventArgs e)
        {
            using (var form = new FormSearchCamera())
            {
                form.ShowDialog(this);
            }
        }
    }
}

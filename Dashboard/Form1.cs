﻿using Dashboard.Controls;
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
using FileHelpers.ExcelNPOIStorage;
using System.Media;

namespace Dashboard
{
    public partial class Form1 : Form, ITrackingAware
    {
        Dictionary<HaCamera, CameraUserControl> _cameraToControlMap
            = new Dictionary<HaCamera, CameraUserControl>();


        IList<string> _connectedDeviceIps
            = new List<string>();

        Settings _setting = new Settings();

        FaceRegitration[] _faces = new FaceRegitration[0];
        Dictionary<string, string[]> _pairing = new Dictionary<string, string[]>();
        private FormWindowState PreviousWindowState;
        private SoundPlayer _identified;
        private SoundPlayer _unIdentified;

        public string[] ConnectedDeviceIps
        {
            get => _connectedDeviceIps?.ToArray() ?? new string[0];
            set
            {
                var l = new List<string>(value);
                _connectedDeviceIps = l;
            }
        }

        public Device[] AddedDevices { get; set; } = new Device[0];

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
            Services.Tracker.Track(_setting);

            if (Directory.Exists(_setting.PairingFolder))
            {
                LoadPairing(_setting.PairingFolder);
            }
            
            ShowAddedCameras();
            LoadAudios();
            ConnectCameras();
            //gridControl1.SetRowCol(2, 2);
        }

        private void LoadAudios()
        {
            var identifiedPath = Path.Combine(Directory.GetCurrentDirectory(), "Sound/identified.wav");
            var unidentifiedPath = Path.Combine(Directory.GetCurrentDirectory(), "sound/unidentified.wav");

            _identified = new SoundPlayer();
            _identified.SoundLocation = identifiedPath;
            _identified.Load();
            _unIdentified = new SoundPlayer();
            _unIdentified.SoundLocation = unidentifiedPath;
            _unIdentified.Load();
        }

        private void ShowAddedCameras()
        {
            foreach (var item in this.AddedDevices)
            {
                listBox1.Items.Add(item);
            }
        }

        private void ConnectCameras()
        {
            if (_connectedDeviceIps?.Count == 0) return;
            var existsDeviceIps = _connectedDeviceIps.Where(x => AddedDevices.Any(y => y.IP == x)).ToList();

            var rows = gridControl1.Rows;
            var cols = gridControl1.Cols;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (existsDeviceIps.Count() == 0) return;
                    var d = AddedDevices.FirstOrDefault(x=>x.IP == existsDeviceIps[0]);
                    existsDeviceIps.RemoveAt(0);
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
                    c.TopRightText = d.ToString();
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

            var device = (Device) listBox1.SelectedItem ;
            HaCamera cam = GetRunningCameraByIp(device.IP);
            if (ReferenceEquals(cam?.Tag, targetControl)) return;

            if (cam != null)
            {
                //change camera target control
                ChangeCamTargetControl(cam, targetControl);
            }
            else
            {
                ConnectNewCamToControl(device, targetControl);
            }

        }

        private void ConnectNewCamToControl(Device d, CameraUserControl targetControl)
        {
            ReleaseControlCamera(targetControl);

            var newCam = new HaCamera() { Ip = d.IP };
            newCam.Tag = targetControl;
            targetControl.Tag = newCam;
            targetControl.TopRightText = d.ToString();
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
                    var mode = _setting.ShowRealtimeImage != _setting.ShowTemplateImage
                     ? DisplayMode.Single : DisplayMode.Double;
                    if (_setting.PlayAudio)
                    {
                        var p = e.IsPersonMatched ? _identified : _unIdentified;
                        p.Play();
                    }

                    ShowImages(e, control, mode);

                    var bgc = e.IsPersonMatched ? Color.Green : Color.Red;
                    var name = e.PersonName ?? Properties.Strings.Unidentified;
                    control.BottomText = $"{name} | {e.CaptureTime.ToLocalTime():T}";
                    control.BackColor = bgc;
                }));
            }
           
            
        }

        private void ShowImages(FaceCapturedEventArgs e, CameraUserControl control, DisplayMode mode = DisplayMode.Single)
        {
            Image realtimeFace = null;
            Image templateFace = null;
            if (e.FeatureImageData != null)
            {
                realtimeFace = Image.FromStream(new MemoryStream(e.FeatureImageData));
            }
            if (e.ModelFaceImageData != null)
            {
                templateFace = Image.FromStream(new MemoryStream(e.ModelFaceImageData));
            }

            

            if (mode == DisplayMode.Single || templateFace == null)
            {
                var img = _setting.ShowRealtimeImage ? realtimeFace : templateFace;

                var oldImage = control.Image;
                control.Image = img;
                oldImage?.Dispose();

            }
            else if (mode == DisplayMode.Double)
            {
                var oldImageLeft = control.ImageLeft;
                control.ImageLeft = templateFace;
                oldImageLeft?.Dispose();

                var oldImageRight = control.ImageRight;
                control.ImageRight = realtimeFace;
                oldImageRight?.Dispose();
            }


            ShowPairing(e, control);
        }

        private void ShowPairing(FaceCapturedEventArgs e, CameraUserControl control)
        {

            var pairings = FindPairings(e.PersonID);
            var show = pairings?.Length > 0;
            if (show == true)
            {
                control.ClearPairImages();
            }
            control.ShowPairingImages = show;
            if (pairings == null) return;

            for (int i = 0; i < control.PairingPictureBoxs.Length; i++)
            {
                if (i > pairings.Length - 1) return;
                var pair = pairings[i];
                if (File.Exists(pair.FullPathToImage))
                {
                    control.PairingPictureBoxs[i].Image = Image.FromFile(pair.FullPathToImage).ExifRotate();
                }

            }

        }

        private FaceRegitration[] FindPairings(string personID)
        {
            if (personID == null) return null;

            var found = _pairing.TryGetValue(personID, out var pairing);
            if (found)
            {
                return  _faces.Where(x => pairing.Contains(x.Id)).ToArray();
            }

            return null;
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
            cfg.Properties(f => new { f.ConnectedDeviceIps, f.AddedDevices });
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

            AddedDevices = this.listBox1.Items.OfType<Device>().ToArray();

            _connectedDeviceIps = GetConnectedDevices().Select(x => x.Ip).ToList();
        }

        private void buttonSearchDevice_Click(object sender, EventArgs e)
        {
            using (var form = new FormSearchCamera())
            {
                form.AddedDevices = AddedDevices;
               var dr = form.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    listBox1.Items.Clear();
                    AddedDevices = form.AddedDevices.ToArray();
                    ShowAddedCameras();
                }
            }
        }

        private void bunifuImageButtonOptions_Click(object sender, EventArgs e)
        {
            using (var form = new FormOptions())
            {
                var dr = form.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    _setting = form.Settings;
                }
            }
        }

        private void bunifuImageSyncFace_Click(object sender, EventArgs e)
        {
            using (var form = new FormSyncFace())
            {
                form.AddedDevice = this.AddedDevices;
                 form.ShowDialog(this);
            }
        }


        private void LoadPairing(string fullPathToDirectory)
        {
            var allFiles = Utils.EnumerateAllFiles(fullPathToDirectory);
            var excel = allFiles.FirstOrDefault(x=>string.Compare(Path.GetExtension(x), ".xlsx", StringComparison.OrdinalIgnoreCase) == 0);
            if (excel == null) throw new InvalidOperationException(Properties.Strings.PairingExcelFileMissing);
            _pairing = LoadPairingFile(excel)
                .GroupBy(x => x.id)
                .ToDictionary(g => g.Key, g => g.Select(p => p.pair_id).ToArray());

            _faces = Utils.LoadFiles(allFiles).validFiles;

        }

        private Pair[] LoadPairingFile(string fullPathToExcelFile)
        {
            var provider = new ExcelNPOIStorage(typeof(Pair))
            {
                FileName = fullPathToExcelFile,
                StartColumn = 0,
                StartRow = 1
            };
            return (Pair[])provider.ExtractRecords();
        }

        private void bunifuImageButtonFullScreen_Click(object sender, EventArgs e)
        {
            FullScreen();
        }

        private void FullScreen(bool enable = true)
        {
            this.SuspendLayout();

            if (enable)
            {

                PreviousWindowState = this.WindowState;
                this.splitContainer1.Panel1Collapsed = true;
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.TopMost = true;
            }
            else
            {
                this.splitContainer1.Panel1Collapsed = false;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.TopMost = false;
                this.WindowState = PreviousWindowState;

            }
            this.ResumeLayout();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.FullScreen(false);
            }
        }
    }
}

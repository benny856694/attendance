using Dashboard.Model;
using HaSdkWrapper;
using ImageMagick;
using Jot.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dashboard
{
    public partial class FormSyncFace : Form, ITrackingAware
    {
        public Device[] AddedDevice { private get; set; } 
        public FaceRegitration[] Registrations { get; private set; } = new FaceRegitration[0];

        private bool showFileFormatPrompt = true;
        private bool _ipSelected;

        public FormSyncFace()
        {
            InitializeComponent();
        }

        private void buttonSelDirectory_Click(object sender, EventArgs e)
        {
            if (showFileFormatPrompt)
            {
                using (var form = new FormImageTypePrompt())
                {
                    form.checkBoxDontPromptAgain.Checked = !this.showFileFormatPrompt;
                    form.ShowDialog(this);
                    this.showFileFormatPrompt = !form.checkBoxDontPromptAgain.Checked;
                }

            }

            folderBrowserDialog1.SelectedPath = textBoxDirectory.Text;
            var dr = folderBrowserDialog1.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                textBoxDirectory.Text = folderBrowserDialog1.SelectedPath;
                LoadAndShowFiles();
            }
        }

        private void LoadAndShowFiles()
        {
            var allFiles = Utils.EnumerateAllFiles(textBoxDirectory.Text);
            var (_, validFiles) = Utils.LoadFiles(allFiles);
            if (validFiles.Length == 0) return;
            Registrations = validFiles;
            ShowFiles(allFiles, validFiles);
            _ipSelected = false;
        }

        private void ShowFiles(string[] allFiles, FaceRegitration[] validFiles)
        {
            toolStripStatusLabelFilesCount.Text = string.Format(Properties.Strings.FilesCount, allFiles.Length);
            toolStripStatusLabelValidFileCount.Text = string.Format(Properties.Strings.FilesCount, validFiles.Length);


            CreateColumns();
            foreach (var item in validFiles)
            {
                var idx = bunifuDataGridView1.Rows.Add(item.Id, item.Name, Path.GetFileName(item.FullPathToImage));
                bunifuDataGridView1.Rows[idx].Tag = item;
            }

        }

        

        private void CreateColumns()
        {
            bunifuDataGridView1.Columns.Clear();
            bunifuDataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ID", HeaderText = "ID", AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
            bunifuDataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Name", HeaderText = "Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
            bunifuDataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Path", HeaderText = "Path", AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
        }

       

        

        private HttpClient CreateHttpClient(string ip)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri($"http://{ip}:8000");
            return client;
        }

        private async Task<Response> PostFaceRegAsync(HttpClient client, FaceRegitration reg)
        {
            var imgData = GetRotatedImageData(reg.FullPathToImage);

            dynamic addPerson = new ExpandoObject();
            addPerson.version = "0.2";
            addPerson.cmd = "upload person";
            addPerson.id = reg.Id;
            addPerson.name = reg.Name;
            addPerson.reg_image = imgData;

            var json =  (string) JsonConvert.SerializeObject(addPerson);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("", content);
            response.EnsureSuccessStatusCode();
            var resObj = await response.Content.ReadAsAsync<Response>();
            return resObj;
           
        }

        private object GetRotatedImageData(string fullPathToImage)
        {
            using (var img = new MagickImage(fullPathToImage))
            {
                img.AutoOrient();   // Fix orientation
                img.Strip();        // remove all EXIF information
                img.Resize(800, 0);
                return img.ToBase64();
            }
        }

        private async void buttonDeploy_ClickAsync(object sender, EventArgs e)
        {
            if (Registrations?.Length == 0 || AddedDevice?.Length == 0 || !_ipSelected) return;

            buttonSync.Enabled = false;
            toolStripProgressBar1.Visible = true;
            buttonSelDirectory.Enabled = false;
            buttonRefresh.Enabled = false;
            buttonChooseDevice.Enabled = false;

            foreach (var ip in AddedDevice)
            {
                var client = CreateHttpClient(ip.IP);
                foreach (DataGridViewRow row in  bunifuDataGridView1.Rows)
                {
                    if (row.Tag is FaceRegitration reg)
                    {
                        var result = Properties.Strings.Success;
                        var success = true;
                        try
                        {
                            var res = await PostFaceRegAsync(client, reg);
                            result = res.code == 0 ? 
                                Properties.Strings.Success : $"{Properties.Strings.Fail}-{res.code}:{Api.HttpApiErrorCodes.GetErrorDesc(res.code)}";
                            success = res.code == 0;
                        }
                        catch (Exception ex)
                        {
                            result = $"{Properties.Strings.Fail}: {ex.Message}";
                            success = false;
                        }

                        row.Cells[ip.IP].Value = result;
                        row.Cells[ip.IP].Style.ForeColor = success ? Color.DarkGreen : Color.DarkRed;
                    }
                    
                }
            }

            buttonSync.Enabled = true;
            toolStripProgressBar1.Visible  = false;
            buttonSelDirectory.Enabled = true;
            buttonRefresh.Enabled = true;
            buttonChooseDevice.Enabled = true;

        }

        private void ShowDeployResult(FaceRegitration reg, string targetIp, bool success, string errorMessage)
        {
            var row = bunifuDataGridView1.Rows.OfType<DataGridViewRow>().FirstOrDefault(x => x.Tag.Equals(reg));
        }

        private void buttonChooseDevice_Click(object sender, EventArgs e)
        {
            using (var form = new FormSearchCamera())
            {
                form.AddedDevices = this.AddedDevice;
                var dr = form.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    this.AddedDevice = form.AddedDevices.ToArray();
                    AddIpColumns(this.AddedDevice);
                    _ipSelected = true;
                }
            }
        }

        private void AddIpColumns(IList<Device> addedDevices)
        {
            foreach (var ip in addedDevices)
            {
                if (!bunifuDataGridView1.Columns.Contains(ip.IP))
                {
                    bunifuDataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { Name = ip.IP, HeaderText = ip.IP });
                }
            }
        }

        public void ConfigureTracking(TrackingConfiguration configuration)
        {
            var cfg = configuration.AsGeneric<FormSyncFace>();
            cfg.Property(f => f.textBoxDirectory.Text, "choosenDirectory");
            cfg.Property(f => f.showFileFormatPrompt);
        }

        private void FormSyncFace_Load(object sender, EventArgs e)
        {
            Services.Tracker.Track(this);
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxDirectory.Text)) return;
            buttonSelDirectory.Enabled = false;

            LoadAndShowFiles();

            buttonSelDirectory.Enabled = true;
        }
    }
}

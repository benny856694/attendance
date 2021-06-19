using Dashboard.Model;
using HaSdkWrapper;
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
            var (allFiles, validFiles) = LoadFiles(textBoxDirectory.Text);
            if (validFiles.Length == 0) return;
            Registrations = validFiles;
            ShowFiles(allFiles, validFiles);
        }

        private void ShowFiles(string[] allFiles, FaceRegitration[] validFiles)
        {
            toolStripStatusLabelFilesCount.Text = string.Format(Properties.Strings.FilesCount, allFiles.Length);
            toolStripStatusLabelValidFileCount.Text = string.Format(Properties.Strings.FilesCount, validFiles.Length);


            CreateColumns();
            foreach (var item in validFiles)
            {
                var idx = bunifuDataGridView1.Rows.Add(item.Id, item.Name, item.FullPathToImage);
                bunifuDataGridView1.Rows[idx].Tag = item;
            }

        }

        private (string[] allFiles, FaceRegitration[] validFiles) LoadFiles(string folder)
        {
            var allFiles = EnumerateAllFiles(folder);
            var valid = ParseFileNames(allFiles);
            return (allFiles, valid);

           
        }

        private void CreateColumns()
        {
            bunifuDataGridView1.Columns.Clear();
            bunifuDataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { Name = "id", HeaderText = "id" });
            bunifuDataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { Name = "name", HeaderText = "name" });
            bunifuDataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { Name = "path", HeaderText = "path" });
        }

        public FaceRegitration[] ParseFileNames(string[] files)
        {
            var result = new List<FaceRegitration>();
            var jpgFiles = files.Where(x => x.EndsWith(".jpg"));   
            foreach (var file in jpgFiles)
            {
                var nameOnly = Path.GetFileNameWithoutExtension(file);
                if (TryParseName(nameOnly, out var id, out var name))
                {
                    var reg = new FaceRegitration { Id = id, Name = name, FullPathToImage = file };
                    result.Add(reg);
                } 
            }

            return result.ToArray();

        }

        public string[] EnumerateAllFiles(string folder) => Directory.EnumerateFiles(folder).ToArray();

        private bool TryParseName(string fileName, out string id, out string name)
        {
            id = null;
            name = null;
            var match = Regex.Match(fileName, @"^\s*(\S*)\s+(.*?)\s*$");
            if (match.Success)
            {
                id = match.Groups[1].Value;
                name = match.Groups[2].Value;
                return true;
            }

            return false;
        }

        private HttpClient CreateHttpClient(string ip)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri($"http://{ip}:8000");
            return client;
        }

        private async Task<Response> PostFaceRegAsync(HttpClient client, FaceRegitration reg)
        {
            dynamic addPerson = new ExpandoObject();
            addPerson.version = "0.2";
            addPerson.cmd = "upload person";
            addPerson.id = reg.Id;
            addPerson.name = reg.Name;
            addPerson.reg_image = Convert.ToBase64String(File.ReadAllBytes(reg.FullPathToImage));

            var json =  (string) JsonConvert.SerializeObject(addPerson);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("", content);
            response.EnsureSuccessStatusCode();
            var resObj = await response.Content.ReadAsAsync<Response>();
            return resObj;
           
        }

        private async void buttonDeploy_ClickAsync(object sender, EventArgs e)
        {
            if (Registrations?.Length == 0 || AddedDevice?.Length == 0) return;

            buttonSync.Enabled = false;
            toolStripProgressBar1.Visible = true;
            buttonSelDirectory.Enabled = false;
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
                }
            }
        }

        private void AddIpColumns(IList<Device> addedDevices)
        {
            foreach (var ip in addedDevices)
            {
                bunifuDataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { Name = ip.IP, HeaderText = ip.IP });

            }
        }

        public void ConfigureTracking(TrackingConfiguration configuration)
        {
            var cfg = configuration.AsGeneric<FormSyncFace>();
            cfg.Property(f => f.textBoxDirectory.Text);
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

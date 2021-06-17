using Dashboard.Model;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dashboard
{
    public partial class FormSyncFace : Form
    {
        public Device[] AddedDevice { private get; set; }
        public FaceRegitration[] Regs { get; private set; }

        private string directory;

        public FormSyncFace()
        {
            InitializeComponent();
        }

        private void buttonSelDirectory_Click(object sender, EventArgs e)
        {
            var dr = folderBrowserDialog1.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                directory = folderBrowserDialog1.SelectedPath;
                textBoxDirectory.Text = directory;
                Regs = ParseFolder(folderBrowserDialog1.SelectedPath);
                CreateColumns();
                foreach (var item in Regs)
                {
                    var idx = bunifuDataGridView1.Rows.Add(item.Id, item.Name, item.FullPathToImage);
                    var row = bunifuDataGridView1.Rows[idx].Tag = item;
                }
            }
        }

        private void CreateColumns()
        {
            bunifuDataGridView1.Columns.Clear();
            bunifuDataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { Name = "id", HeaderText = "id" });
            bunifuDataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { Name = "name", HeaderText = "name" });
            bunifuDataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { Name = "path", HeaderText = "path" });
        }

        public FaceRegitration[] ParseFolder(string path)
        {
            var result = new List<FaceRegitration>();
            var files = Directory.EnumerateFiles(path, "*.jpg");
            foreach (var file in files)
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

        private bool TryParseName(string fileName, out string id, out string name)
        {
            id = null;
            name = null;
            var sections = fileName.Split('-', ' ', '#');
            if(sections.Length == 2)
            {
                id = sections[0];
                if (!int.TryParse(id, out var _)) return false;
                name = sections[1];
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
            var addPerson = new AddPersonRequest
            {
                id = reg.Id,
                wg_card_id = int.Parse(reg.Id),
                reg_image = Convert.ToBase64String(File.ReadAllBytes(reg.FullPathToImage))
            };

            var response = await client.PostAsJsonAsync("", addPerson);
            response.EnsureSuccessStatusCode();
            var resObj = await response.Content.ReadAsAsync<Response>();
            return resObj;
           
        }

        private async void buttonSync_ClickAsync(object sender, EventArgs e)
        {
            if (directory == null || AddedDevice.Length == 0) return;

            var regs = ParseFolder(directory);
            buttonSync.Enabled = false;
            bunifuCircleProgress1.Animated = true;
            bunifuCircleProgress1.Visible = true;
            foreach (var ip in AddedDevice)
            {
                var client = CreateHttpClient(ip.IP);
                foreach (DataGridViewRow row in  bunifuDataGridView1.Rows)
                {
                    if (row.Tag is FaceRegitration reg)
                    {
                        var result = Properties.Strings.Success;
                        try
                        {
                            var res = await PostFaceRegAsync(client, reg);
                            result = res.code == 0 ? Properties.Strings.Success : $"{Properties.Strings.Fail}: {res.reply}";
                        }
                        catch (Exception ex)
                        {
                            result = $"{Properties.Strings.Fail}: {ex.Message}";
                        }

                        row.Cells[ip.IP].Value = result;
                    }
                    
                }
            }

            buttonSync.Enabled = true;
            bunifuCircleProgress1.Animated = false;
            bunifuCircleProgress1.Visible = false;


        }

        private void buttonChooseDevice_Click(object sender, EventArgs e)
        {
            using (var form = new FormSearchCamera())
            {
                form.AddedDevices = this.AddedDevice;
                var dr = form.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    AddIpColumns(form.AddedDevices);
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
    }
}

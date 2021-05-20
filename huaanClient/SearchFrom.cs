using CCWin;
using Ewin.Client.Frame.UcGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZXCL.WinFormUI;

namespace huaanClient
{
    public partial class SearchFrom : ZForm
    {
        public SearchFrom()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            
        }
        
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

       
        public DataTable Fill<T>(List<T> objlist)
        {
            if (objlist == null || objlist.Count <= 0)
            {
                return null;
            }
            DataTable dt = new DataTable(typeof(T).Name);
            DataColumn column;
            DataRow row;
            PropertyInfo[] myPropertyInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (T t in objlist)
            {
                if (t == null)
                {
                    continue;
                }
                row = dt.NewRow();
                for (int i = 0, j = myPropertyInfo.Length; i < j; i++)
                {
                    System.Reflection.PropertyInfo pi = myPropertyInfo[i];
                    string name = pi.Name;
                    if (dt.Columns[name] == null)
                    {
                        column = new DataColumn(name, pi.PropertyType);
                        dt.Columns.Add(column);
                    }
                    row[name] = pi.GetValue(t, null);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
        

        
        
        private void skinLabel3_Click(object sender, EventArgs e)
        {

        }

        private void DataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int index = DataGridView.CurrentRow.Index;
                this.DataGridView.Rows[e.RowIndex].Selected = true;
                if (Convert.ToBoolean(DataGridView.Rows[index].Cells[0].Value))
                {
                    DataGridView.Rows[index].Cells[0].Value = false;
                }
                else
                {
                    DataGridView.Rows[index].Cells[0].Value = true;
                    ////其他的都是false
                    //for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    //{
                    //    if (i != index)
                    //    {
                    //        dataGridView1.Rows[i].Cells[0].Value = false;
                    //    }
                    //}
                }
            }
            catch
            {

            }
        }
        int nuber = 1;//记录第几次点击
        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)//如果单击列表头，全选．
            {
                if(nuber==1){
                    nuber = 2;
                    DataGridView.EndEdit();
                    for (int i = 0; i < this.DataGridView.RowCount; i++)
                    {
                        this.DataGridView.Rows[i].Cells[0].Value = true;//如果为true则为选中,false未选中
                    }
                }
                else if (nuber == 2)
                {
                    nuber = 1;
                    DataGridView.EndEdit();
                    for (int i = 0; i < this.DataGridView.RowCount; i++)
                    {
                        this.DataGridView.Rows[i].Cells[0].Value = false;//如果为true则为选中,false未选中
                    }
                }
            }
        }

        private void DataGridView_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void SearchFrom_Shown(object sender, EventArgs e)
        {
            DataGridView.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ShowLayer();
            Thread thread = new Thread(() =>
            {
                List<(string mac, string ip, string mask, string platform, string system)> ds = null;
                try
                {
                    ds = DeviceDiscover.Search(5);
                    ds.ForEach(d => Console.WriteLine(d.ip));

                    if (ds.Count() > 0)
                    {
                        for (int i = 0; i < ds.Count(); i++)
                        {
                            object[] values = { false, ds[i].mac, ds[i].ip, ds[i].mask, ds[i].platform, ds[i].system };
                            this.BeginInvoke(new Action(() =>
                            {
                                DataGridView.Rows.Add(values);
                            }));
                        }
                        this.BeginInvoke(new Action(()=>
                        {
                            DataGridView.ClearSelection();
                            DataGridView.CurrentCell = null;
                        }));
                        
                    }
                    else
                    {
                        MessageBox.Show("未查询到信息。", "提示");
                    }
                    HideLayer();
                }
                catch (Exception ex)
                {
                    HideLayer();
                    MessageBox.Show("查询异常: \r\n\r\n1. 请联系客服人员", "提示");
                }
            });
            thread.IsBackground = true;
            thread.Start();
            Application.DoEvents();
        }
        private OpaqueLayer m_OpaqueLayer = null;//半透明蒙板层
        bool isLoading = false;
        /// <summary>
        /// 显示遮罩层
        /// </summary>
        /// <param name="control"></param>
        /// <param name="alpha"></param>
        /// <param name="showLoadingImage"></param>
        protected void ShowLayer()
        {
            isLoading = true;
            Control control = this;
            int alpha = 125;
            bool showLoadingImage = true;
            this.Invoke(new Action(() =>
            {
                if (this.m_OpaqueLayer == null)
                {
                    this.m_OpaqueLayer = new OpaqueLayer(alpha, showLoadingImage);
                    control.Controls.Add(this.m_OpaqueLayer);
                    this.m_OpaqueLayer.Dock = DockStyle.Fill;
                    this.m_OpaqueLayer.BringToFront();
                }
                this.m_OpaqueLayer.Enabled = true;
                this.m_OpaqueLayer.Visible = true;
            }));
        }

        /// <summary>
        /// 隐藏遮罩层
        /// </summary>
        protected void HideLayer()
        {
            isLoading = false;
            this.Invoke(new Action(() =>
            {
                if (this.m_OpaqueLayer != null)
                {
                    this.m_OpaqueLayer.Enabled = false;
                    this.m_OpaqueLayer.Visible = false;
                }
            }));

        }

        private void zButton3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();  //显示选择文件对话框
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"; //所有的文件格式
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.FilePath.Text = openFileDialog1.FileName;   //显示文件路径
            }
        }

        private void searchbtn_Click(object sender, EventArgs e)
        {

        }
    }
}

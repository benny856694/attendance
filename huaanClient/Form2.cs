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
using System.Windows.Forms;

namespace huaanClient
{
    public partial class SearchFrom : CCSkinMain
    {
        public SearchFrom()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }
        MasterControl masterDetail;
        public void clearFields()
        {
            panel1.Controls.Clear();
            masterDetail = null;
            Refresh();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            String[] values = { "111", "111", "111", "111", "111", "111", "111" }; 
            DataGridView.Rows.Add(values);



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
            System.Reflection.PropertyInfo[] myPropertyInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
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
    }
}

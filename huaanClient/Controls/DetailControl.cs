// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
// End of VB project level imports


namespace Ewin.Client.Frame.UcGrid
{
    /// <summary>
    /// 此类用来定义盛放子DataGridview的容器
    /// </summary>
    [ToolboxItem(false)]
    public class detailControl : Panel
    {
        #region 字段
        public List<DataGridView> childGrid = new List<DataGridView>();
        public DataSet _cDataset;
        #endregion

        #region 方法
        public void Add(string tableName, string strPrimaryKey, string strForeignKey)
        {
            //TabPage tPage = new TabPage() { Text = pageCaption };
            //this.Controls.Add(tPage);
            var newGrid = new MasterControl(_cDataset, controlType.middle) { Dock = DockStyle.Fill, DataSource = new DataView(_cDataset.Tables[tableName]) };
            newGrid.setParentSource(tableName, strPrimaryKey, strForeignKey);//设置主外键
            //newGrid.Name = "ChildrenMaster";
            //tPage.Controls.Add(newGrid);
            this.Controls.Add(newGrid);
            //this.BorderStyle = BorderStyle.FixedSingle;
            cModule.applyGridTheme(newGrid);
            cModule.setGridRowHeader(newGrid);
            newGrid.RowPostPaint += cModule.rowPostPaint_HeaderCount;
            childGrid.Add(newGrid);
        }

        public void Add2(string tableName)
        {
            //TabPage tPage = new TabPage() { Text = pageCaption };
            //this.Controls.Add(tPage);
            DataGridView newGrid = new DataGridView() { Dock = DockStyle.Fill, DataSource = new DataView(_cDataset.Tables[tableName]) };
            newGrid.AllowUserToAddRows = false;
            //tPage.Controls.Add(newGrid);
            this.Controls.Add(newGrid);
            cModule.applyGridTheme(newGrid);
            cModule.setGridRowHeader(newGrid);
            newGrid.RowPostPaint += cModule.rowPostPaint_HeaderCount;
            childGrid.Add(newGrid);
        }

        public void RemoveControl()
        {
            this.Controls.Remove(childGrid[0]);
            childGrid.Clear();
        }
        #endregion

    }

}

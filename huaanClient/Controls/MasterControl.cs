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
using System.Reflection;
using System.ComponentModel;

// End of VB project level imports


namespace Ewin.Client.Frame.UcGrid
{
    /// <summary>
    /// 此类用来定义折叠的DataGridView
    /// </summary>
    [ToolboxItem(false)]
    public class MasterControl : DataGridView
    {
        #region 字段
        private List<int> rowCurrent = new List<int>();
        internal static int rowDefaultHeight = 22;
        internal static int rowExpandedHeight = 300;
        internal static int rowDefaultDivider = 0;
        internal static int rowExpandedDivider = 300 - 22;
        internal static int rowDividerMargin = 5;
        internal static bool collapseRow;
        public detailControl childView = new detailControl() { Visible = false }; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
        //
        internal System.Windows.Forms.ImageList RowHeaderIconList;
        private System.ComponentModel.Container components = null;
        //
        DataSet _cDataset;
        string _foreignKey;
        string _primaryKey;
        string _filterFormat;
        private controlType EControlType;
        public int ExpandRowIndex = 0;
        

        #endregion

        #region 构造函数
        /// <summary>
        /// 通过传递过来的枚举判断是两级还是三级展开，表的对应关系通过Relations来读取
        /// 所以调用此构造函数的时候必须要讲Relations设置正确，才能正确显示层级关系。
        ///  oDataSet.Relations.Add("1", oDataSet.Tables["T1"].Columns["Menu_ID"], oDataSet.Tables["T2"].Columns["Menu_ID"]);
        ///  oDataSet.Relations.Add("2", oDataSet.Tables["T2"].Columns["Menu_Name2"], oDataSet.Tables["T3"].Columns["Menu_Name2"]);
        ///  这两次Add的顺序不能颠倒，必须先添加一、二级的表关联，再添加二、三级的表关联
        /// </summary>
        /// <param name="cDataset">数据源DataSet，里面还有各个表的对应关系</param>
        /// <param name="eControlType">枚举类型</param>
        public MasterControl(DataSet cDataset, controlType eControlType)
        {
            SetMasterControl(cDataset, eControlType);   
        }

        /// <summary>
        /// 第二种使用方法
        /// </summary>
        /// <param name="lstData1">折叠控件第一层的集合</param>
        /// <param name="lstData2">折叠控件第二层的集合</param>
        /// <param name="lstData3">折叠控件第三层的集合</param>
        /// <param name="dicRelateKey1">第一二层之间对应主外键</param>
        /// <param name="dicRelateKey2">第二三层之间对应主外键</param>
        /// <param name="eControlType">枚举类型</param>
        public MasterControl(object lstData1, object lstData2, 
                             object lstData3, Dictionary<string, string> dicRelateKey1, 
                             Dictionary<string ,string>dicRelateKey2, controlType eControlType)
        {
            var oDataSet = new DataSet();
            try
            {
                var oTable1 = new DataTable();
                oTable1 = Fill(lstData1);
                oTable1.TableName = "T1";

                var oTable2 = Fill(lstData2);
                oTable2.TableName = "T2";

                if (lstData3 == null || dicRelateKey2 == null || dicRelateKey2.Keys.Count <= 0)
                {
                    oDataSet.Tables.AddRange(new DataTable[] { oTable1, oTable2 });
                    oDataSet.Relations.Add("1", oDataSet.Tables["T1"].Columns[dicRelateKey1.Keys.FirstOrDefault()], oDataSet.Tables["T2"].Columns[dicRelateKey1.Values.FirstOrDefault()]);
                }
                else
                {
                    var oTable3 = Fill(lstData3);
                    oTable3.TableName = "T3";

                    oDataSet.Tables.AddRange(new DataTable[] { oTable1, oTable2, oTable3 });
                    //这是对应关系的时候主键必须唯一
                    oDataSet.Relations.Add("1", oDataSet.Tables["T1"].Columns[dicRelateKey1.Keys.FirstOrDefault()], oDataSet.Tables["T2"].Columns[dicRelateKey1.Values.FirstOrDefault()]);
                    oDataSet.Relations.Add("2", oDataSet.Tables["T2"].Columns[dicRelateKey2.Keys.FirstOrDefault()], oDataSet.Tables["T3"].Columns[dicRelateKey2.Values.FirstOrDefault()]);
                }
            }
            catch
            {
                oDataSet = new DataSet();
            }
            SetMasterControl(oDataSet, eControlType);
        }

        /// <summary>
        /// 控件初始化
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            base.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(MasterControl_RowHeaderMouseClick);
            base.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(MasterControl_RowPostPaint);
            base.Scroll += new System.Windows.Forms.ScrollEventHandler(MasterControl_Scroll);
            base.SelectionChanged += new System.EventHandler(MasterControl_SelectionChanged);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MasterControl));
            this.RowHeaderIconList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            this.SuspendLayout();
            //
            //RowHeaderIconList
            //
            this.RowHeaderIconList.ImageStream = (System.Windows.Forms.ImageListStreamer)(resources.GetObject("RowHeaderIconList.ImageStream"));
            this.RowHeaderIconList.TransparentColor = System.Drawing.Color.Transparent;
            this.RowHeaderIconList.Images.SetKeyName(0, "expand.png");
            this.RowHeaderIconList.Images.SetKeyName(1, "collapse.png");
            //
            //MasterControl
            //
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        #region 数据绑定
        /// <summary>
        /// 设置表之间的主外键关联
        /// </summary>
        /// <param name="tableName">DataTable的表名称</param>
        /// <param name="foreignKey">外键</param>
        public void setParentSource(string tableName, string primarykey, string foreignKey)
        {
            this.DataSource = new DataView(_cDataset.Tables[tableName]);
            cModule.setGridRowHeader(this);
            _foreignKey = foreignKey;
            _primaryKey = primarykey;
            if (_cDataset.Tables[tableName].Columns[primarykey].GetType().ToString() == typeof(int).ToString()
                || _cDataset.Tables[tableName].Columns[primarykey].GetType().ToString() == typeof(double).ToString()
                || _cDataset.Tables[tableName].Columns[primarykey].GetType().ToString() == typeof(decimal).ToString())
            {
                _filterFormat = foreignKey + "={0}";
            }
            else
            {
                _filterFormat = foreignKey + "=\'{0}\'";
            }
        }
        #endregion

        #region 事件
        //控件的行头点击事件
        private void MasterControl_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                Rectangle rect = new Rectangle(System.Convert.ToInt32((double)(rowDefaultHeight - 16) / 2), System.Convert.ToInt32((double)(rowDefaultHeight - 16) / 2), 16, 16);
                if (rect.Contains(e.Location))
                {
                    //缩起
                    if (rowCurrent.Contains(e.RowIndex))
                    {
                        rowCurrent.Clear();
                        this.Rows[e.RowIndex].Height = rowDefaultHeight;
                        this.Rows[e.RowIndex].DividerHeight = rowDefaultDivider;

                        this.ClearSelection();
                        collapseRow = true;
                        this.Rows[e.RowIndex].Selected = true;
                        if (EControlType == controlType.middle)
                        {
                            var oParent = ((MasterControl)this.Parent.Parent);
                            oParent.Rows[oParent.ExpandRowIndex].Height = rowDefaultHeight * (this.Rows.Count + 4);
                            oParent.Rows[oParent.ExpandRowIndex].DividerHeight = rowDefaultHeight * (this.Rows.Count + 3);
                            if (oParent.Rows[oParent.ExpandRowIndex].Height > 500)
                            {
                                oParent.Rows[oParent.ExpandRowIndex].Height = 500;
                                oParent.Rows[oParent.ExpandRowIndex].Height = 480;
                            }
                        }
                    }
                    //展开
                    else
                    {
                        if (!(rowCurrent.Count == 0))
                        {
                            var eRow = rowCurrent[0];
                            rowCurrent.Clear();
                            this.Rows[eRow].Height = rowDefaultHeight;
                            this.Rows[eRow].DividerHeight = rowDefaultDivider;
                            this.ClearSelection();
                            collapseRow = true;
                            this.Rows[eRow].Selected = true;
                        }
                        rowCurrent.Add(e.RowIndex);
                        this.ClearSelection();
                        collapseRow = true;
                        this.Rows[e.RowIndex].Selected = true;
                        this.ExpandRowIndex = e.RowIndex;
                        
                        this.Rows[e.RowIndex].Height = 66 + rowDefaultHeight * (((DataView)(childView.childGrid[0].DataSource)).Count + 1);
                        this.Rows[e.RowIndex].DividerHeight = 66 + rowDefaultHeight * (((DataView)(childView.childGrid[0].DataSource)).Count);
                        //设置一个最大高度
                        if (this.Rows[e.RowIndex].Height > 500)
                        {
                            this.Rows[e.RowIndex].Height = 500;
                            this.Rows[e.RowIndex].DividerHeight = 480;
                        }
                        if (EControlType == controlType.middle)
                        {
                            if (this.Parent.Parent.GetType() != typeof(MasterControl))
                                return;
                            var oParent = ((MasterControl)this.Parent.Parent);
                            oParent.Rows[oParent.ExpandRowIndex].Height = this.Rows[e.RowIndex].Height + rowDefaultHeight * (this.Rows.Count + 3);
                            oParent.Rows[oParent.ExpandRowIndex].DividerHeight = this.Rows[e.RowIndex].DividerHeight + rowDefaultHeight * (this.Rows.Count + 3);
                            if (oParent.Rows[oParent.ExpandRowIndex].Height > 500)
                            {
                                oParent.Rows[oParent.ExpandRowIndex].Height = 500;
                                oParent.Rows[oParent.ExpandRowIndex].Height = 480;
                            }
                        }
                        //if (EControlType == controlType.outside)
                        //{
                        //    //SetControl(this);
                        //}
                        //this.Rows[e.RowIndex].Height = rowExpandedHeight;
                        //this.Rows[e.RowIndex].DividerHeight = rowExpandedDivider;
                    }
                    //this.ClearSelection();
                    //collapseRow = true;
                    //this.Rows[e.RowIndex].Selected = true;
                }
                else
                {
                    collapseRow = false;
                }
            }
            catch (Exception ex)
            {

            }
        }

        //控件的行重绘事件
        private void MasterControl_RowPostPaint(object obj_sender, DataGridViewRowPostPaintEventArgs e)
        {
            try
            {
                var sender = (DataGridView)obj_sender;
                //set childview control
                var rect = new Rectangle((int)(e.RowBounds.X + ((double)(rowDefaultHeight - 16) / 2)), (int)(e.RowBounds.Y + ((double)(rowDefaultHeight - 16) / 2)), 16, 16);
                if (collapseRow)
                {
                    if (this.rowCurrent.Contains(e.RowIndex))
                    {
                        #region 更改点开后背景色 刘金龙
                        var rect1 = new Rectangle(e.RowBounds.X, e.RowBounds.Y + rowDefaultHeight, e.RowBounds.Width, e.RowBounds.Height - rowDefaultHeight);
                        using (Brush b = new SolidBrush(Color.FromArgb(164, 169, 143)))
                        {
                            e.Graphics.FillRectangle(b, rect1);
                        }
                        using (Pen p = new Pen(Color.GhostWhite))
                        {
                            var iHalfWidth = (e.RowBounds.Left + sender.RowHeadersWidth) / 2;
                            var oPointHLineStart = new Point(rect1.X + iHalfWidth, rect1.Y);
                            var oPointHLineEnd = new Point(rect1.X + iHalfWidth, rect1.Y + rect1.Height / 2);
                            e.Graphics.DrawLine(p, oPointHLineStart, oPointHLineEnd);
                            //折叠线
                            e.Graphics.DrawLine(p, oPointHLineEnd, new Point(oPointHLineEnd.X + iHalfWidth, oPointHLineEnd.Y));
                        } 
                        #endregion
                        sender.Rows[e.RowIndex].DividerHeight = sender.Rows[e.RowIndex].Height - rowDefaultHeight;
                        e.Graphics.DrawImage(RowHeaderIconList.Images[(int)rowHeaderIcons.collapse], rect);
                        childView.Location = new Point(e.RowBounds.Left + sender.RowHeadersWidth, e.RowBounds.Top + rowDefaultHeight + 5);
                        childView.Width = e.RowBounds.Right - sender.RowHeadersWidth;
                        childView.Height = System.Convert.ToInt32(sender.Rows[e.RowIndex].DividerHeight - 10);
                        childView.Visible = true;
                    }
                    else
                    {
                        childView.Visible = false;
                        e.Graphics.DrawImage(RowHeaderIconList.Images[(int)rowHeaderIcons.expand], rect);
                    }
                    collapseRow = false;
                }
                else
                {
                    if (this.rowCurrent.Contains(e.RowIndex))
                    {
                        #region 更改点开后背景色 刘金龙
                        var rect1 = new Rectangle(e.RowBounds.X, e.RowBounds.Y + rowDefaultHeight, e.RowBounds.Width, e.RowBounds.Height - rowDefaultHeight);
                        using (Brush b = new SolidBrush(Color.FromArgb(164,169,143)))
                        {
                            e.Graphics.FillRectangle(b, rect1);
                        }
                        using (Pen p = new Pen(Color.GhostWhite))
                        {
                            var iHalfWidth = (e.RowBounds.Left + sender.RowHeadersWidth) / 2;
                            var oPointHLineStart = new Point(rect1.X + iHalfWidth, rect1.Y);
                            var oPointHLineEnd = new Point(rect1.X + iHalfWidth, rect1.Y + rect1.Height / 2);
                            e.Graphics.DrawLine(p, oPointHLineStart, oPointHLineEnd);
                            //折叠线
                            e.Graphics.DrawLine(p, oPointHLineEnd, new Point(oPointHLineEnd.X + iHalfWidth, oPointHLineEnd.Y));
                        } 
                        #endregion
                        sender.Rows[e.RowIndex].DividerHeight = sender.Rows[e.RowIndex].Height - rowDefaultHeight;
                        e.Graphics.DrawImage(RowHeaderIconList.Images[(int)rowHeaderIcons.collapse], rect);
                        childView.Location = new Point(e.RowBounds.Left + sender.RowHeadersWidth, e.RowBounds.Top + rowDefaultHeight + 5);
                        childView.Width = e.RowBounds.Right - sender.RowHeadersWidth;
                        childView.Height = System.Convert.ToInt32(sender.Rows[e.RowIndex].DividerHeight - 10);
                        childView.Visible = true;
                    }
                    else
                    {
                        childView.Visible = false;
                        e.Graphics.DrawImage(RowHeaderIconList.Images[(int)rowHeaderIcons.expand], rect);
                    }
                }
                cModule.rowPostPaint_HeaderCount(sender, e);
            }
            catch
            {

            }
        }

        //控件的滚动条滚动事件
        private void MasterControl_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (!(rowCurrent.Count == 0))
                {
                    collapseRow = true;
                    this.ClearSelection();
                    this.Rows[rowCurrent[0]].Selected = true;
                }
            }
            catch
            {

            }
        }

        //控件的单元格选择事件
        private void MasterControl_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (!(this.RowCount == 0))
                {
                    if (rowCurrent.Contains(this.CurrentRow.Index))
                    {
                        foreach (DataGridView cGrid in childView.childGrid)
                        {
                            ((DataView)cGrid.DataSource).RowFilter = string.Format(_filterFormat, this[_primaryKey, this.CurrentRow.Index].Value);
                        }
                    }
                }
            }
            catch
            {

            }
        }
        #endregion

        #region Private
        //设置构造函数的参数
        private void SetMasterControl(DataSet cDataset, controlType eControlType)
        {
            //1.控件初始化赋值
            this.Controls.Add(childView);
            InitializeComponent();
            _cDataset = cDataset;
            childView._cDataset = cDataset;
            cModule.applyGridTheme(this);
            Dock = DockStyle.Fill;
            EControlType = eControlType;
            this.AllowUserToAddRows = false;

            //2.通过读取DataSet里面的Relations得到表的关联关系
            if (cDataset.Relations.Count <= 0)
            {
                return;
            }
            DataRelation oRelates;
            if (eControlType == controlType.outside)
            {
                oRelates = cDataset.Relations[1];
                childView.Add(oRelates.ParentTable.TableName, oRelates.ParentColumns[0].ColumnName, oRelates.ChildColumns[0].ColumnName);
            }
            else if (eControlType == controlType.middle)
            {
                oRelates = cDataset.Relations[cDataset.Relations.Count - 1];
                childView.Add2(oRelates.ChildTable.TableName);
            }

            //3.设置主外键对应关系
            oRelates = cDataset.Relations[0];
            //主表里面的值，副表里面的过滤字段
            setParentSource(oRelates.ParentTable.TableName,oRelates.ParentColumns[0].ColumnName, oRelates.ChildColumns[0].ColumnName);
        }

        private void SetControl(MasterControl oGrid)
        {
            oGrid.childView.RemoveControl();
            //oGrid.childView.Controls.RemoveByKey("ChildrenMaster");
            //
            //var oRelates = _cDataset.Relations[1];
            //oGrid.childView.Add(oRelates.ParentTable.TableName, oRelates.ChildColumns[0].ColumnName);
            

            //foreach (var oGridControl in oGrid.Controls)
            //{
            //    if (oGridControl.GetType() != typeof(detailControl))
            //    {
            //        continue;
            //    }
            //    var DetailControl =(detailControl)oGridControl;
            //    foreach (var odetailControl in DetailControl.Controls)
            //    {
            //        if (odetailControl.GetType() != typeof(MasterControl))
            //        {
            //            continue;
            //        }
            //        var OMasterControl = (MasterControl)odetailControl;
            //        foreach (var oMasterControl in OMasterControl.Controls)
            //        {
            //            if (oMasterControl.GetType() == typeof(detailControl))
            //            {
            //                ((detailControl)oMasterControl).Visible = false;
            //                return;
            //            }
            //        }
            //    }
            //}
        }

        //将List集合转换成DataTable
        private DataTable Fill(object obj)
        {
            if(!(obj is IList))
            {
                return null;
            }
            var objlist = obj as IList;
            if (objlist == null || objlist.Count <= 0)
            {
                return null;
            }
            var tType = objlist[0];
            DataTable dt = new DataTable(tType.GetType().Name);
            DataColumn column;
            DataRow row;
            System.Reflection.PropertyInfo[] myPropertyInfo = tType.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var t in objlist)
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
        #endregion
    }
}

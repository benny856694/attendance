using Dapper.Contrib.Extensions;
using DBUtility.SQLite;
using huaanClient.Database;
using huaanClient.Properties;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
namespace huaanClient
{
    class DataToCsv
    {
        public DataToCsv()
        {
        }

        /// <summary>
        /// 将DataTable导出CSV表格
        /// </summary>
        /// <param name="dataTable">DataTable数据源</param>
        /// <param name="ColumnName">标题列(英文逗号"，"分割)</param>
        /// <param name="ColumnValue">内容列参数名称</param>
        /// <param name="CsvName">导出的CSV表格名称</param>
        /// <returns></returns>
        public static StringBuilder Data_To_Csv(DataTable dataTable, string ColumnName, string[] ColumnValue, string CsvName)
        {
            DataTable dt = dataTable;
            try
            {
                StringWriter swCSV = new StringWriter();
                //列名
                swCSV.WriteLine(ColumnName);//"工单编号,工单标题,工单类型,创建时间,当前状态,当前节点名称,当前处理人"
                                            //遍历datatable导出数据
                foreach (DataRow drTemp in dt.Rows)
                {
                    StringBuilder sbText = new StringBuilder();

                    for (int i = 0; i < ColumnValue.Length; i++)  // ---------- 字段循环  
                    {
                        sbText = AppendCSVFields(sbText, drTemp[ColumnValue[i].ToString()].ToString());
                    }
                    //去掉尾部的逗号
                    sbText.Remove(sbText.Length - 1, 1);
                    //写datatable的一行
                    swCSV.WriteLine(sbText.ToString());
                }
                swCSV.Close();
                return swCSV.GetStringBuilder();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// csv添加逗号 用来区分列
        /// </summary>
        /// <param name="argFields">字段</param>
        /// <returns>添加后内容</returns>
        public static StringBuilder AppendCSVFields(StringBuilder argSource, string argFields)
        {
            return argSource.Append(argFields.Replace(",", " ").Trim()).Append(",");
        }

        /// <summary>
        /// 导出数据到CSV文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="table"></param>
        public static void ExportDataToCSV(string fileName, DataTable table, string date)
        {
            //Thread.SetApartmentState(ApartmentState.STA);
            if (table == null || table.Rows.Count == 0)
            {
                return;
            }

            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "Excel文件(*.xlsx)|*.xlsx";
            saveDlg.FileName = fileName + "-" + date;

            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                var workBook = new XSSFWorkbook();
                var sheet = workBook.CreateSheet();
                var titleRow = sheet.CreateRow(0);
                try
                {
                    //标题行
                    string name = "";
                    string department = "";
                    //string personId = table.Columns["UserKey"].ColumnName = "用户编码";
                    string Employee_code = "";
                    string nowdate = "";
                    string Attendance = "";
                    //string restcount ="";
                    string latedata = "";
                    string Leaveearlydata = "";
                    string AbsenteeismCount = "";
                    string LeaveCount = "";
                    if (ApplicationData.LanguageSign.Contains("English"))
                    {
                        name = table.Columns["name"].ColumnName = "Name";
                        department = table.Columns["department"].ColumnName = "Department";
                        Employee_code = table.Columns["Employee_code"].ColumnName = "Employee number";
                        nowdate = table.Columns["nowdate"].ColumnName = "Attendance date";
                        Attendance = table.Columns["Attendance"].ColumnName = "Attendance (days)";
                        //restcount = table.Columns["restcount"].ColumnName = "Rest (days)";
                        latedata = table.Columns["latedata"].ColumnName = "Number of lateness / total time (minutes)";
                        Leaveearlydata = table.Columns["Leaveearlydata"].ColumnName = "Number of early leave / total time (minutes)";
                        AbsenteeismCount = table.Columns["AbsenteeismCount"].ColumnName = "Absenteeism days";
                        LeaveCount = table.Columns["LeaveCount"].ColumnName = "LeaveCount";
                    }
                    else if (ApplicationData.LanguageSign.Contains("日本語"))
                    {
                        name = table.Columns["name"].ColumnName = "名前";
                        department = table.Columns["department"].ColumnName = "ポジション";
                        Employee_code = table.Columns["Employee_code"].ColumnName = " ユーザー番号";
                        nowdate = table.Columns["nowdate"].ColumnName = "勤務評定日";
                        Attendance = table.Columns["Attendance"].ColumnName = "出勤する";
                        //restcount = table.Columns["restcount"].ColumnName = "休み(日)";
                        latedata = table.Columns["latedata"].ColumnName = "遅刻回数/総時間(分)";
                        Leaveearlydata = table.Columns["Leaveearlydata"].ColumnName = "早退の回数/総時間の長さ(分)";
                        AbsenteeismCount = table.Columns["AbsenteeismCount"].ColumnName = "欠勤日数";
                        LeaveCount = table.Columns["LeaveCount"].ColumnName = "休暇日数";
                    }
                    else
                    {
                        name = table.Columns["name"].ColumnName = "姓名";
                        department = table.Columns["department"].ColumnName = "部门";
                        //string personId = table.Columns["UserKey"].ColumnName = "用户编码";
                        Employee_code = table.Columns["Employee_code"].ColumnName = "员工编号";
                        nowdate = table.Columns["nowdate"].ColumnName = "考勤日期";
                        Attendance = table.Columns["Attendance"].ColumnName = "出勤(天)";
                        //restcount = table.Columns["restcount"].ColumnName = "休息(天)";
                        latedata = table.Columns["latedata"].ColumnName = "迟到次数/总时长(分钟)";
                        Leaveearlydata = table.Columns["Leaveearlydata"].ColumnName = "早退次数/总时长(分钟)";
                        AbsenteeismCount = table.Columns["AbsenteeismCount"].ColumnName = "旷工天数";
                        LeaveCount = table.Columns["LeaveCount"].ColumnName = "请假天数";
                    }

                    var col = 0;
                    titleRow.CreateCell(col++).SetCellValue(name);
                    titleRow.CreateCell(col++).SetCellValue(department);
                    titleRow.CreateCell(col++).SetCellValue(Employee_code);
                    titleRow.CreateCell(col++).SetCellValue(nowdate);
                    titleRow.CreateCell(col++).SetCellValue(Attendance);
                    //write.Write(restcount + ",");
                    titleRow.CreateCell(col++).SetCellValue(latedata);
                    titleRow.CreateCell(col++).SetCellValue(Leaveearlydata);
                    titleRow.CreateCell(col++).SetCellValue(AbsenteeismCount);
                    titleRow.CreateCell(col++).SetCellValue(LeaveCount);

                    //明细行
                    for (int row = 0; row < table.Rows.Count; row++)
                    {
                        var sheetRow = sheet.CreateRow(row+1);
                        col = 0;
                        for (int column = 0; column < table.Columns.Count - 1; column++)
                        {
                            if (column == 1 || column == 6)
                                continue;
                            if (table.Rows[row][column] != DBNull.Value)
                            {
                                //加入请假  请假天数=实际请假全天数量和（上下班都没有打卡信息）+请假半天的数量（只有一条打卡信息）
                                if (column == 10)
                                {
                                    string LeaveCountforint = string.Empty;
                                    try
                                    {
                                        //int sss = int.Parse(table.Rows[row][column].ToString().Trim());
                                        //float ssss = float.Parse(table.Rows[row][table.Columns.Count - 1].ToString().Trim()) / 2;
                                        LeaveCountforint = (float.Parse(table.Rows[row][column].ToString().Trim()) + float.Parse(table.Rows[row][table.Columns.Count - 1].ToString().Trim()) / 2).ToString();
                                    }
                                    catch { }
                                    sheetRow.CreateCell(col++).SetCellValue(LeaveCountforint);
                                }
                                else
                                {
                                    string TemString = table.Rows[row][column].ToString().Trim();
                                    
                                    sheetRow.CreateCell(col++).SetCellValue(TemString);
                                }

                            }
                            else
                            {
                                string TemString = "";
                                sheetRow.CreateCell(col++).SetCellValue(TemString);
                            }
                        }
                        
                    }

                    var fs = saveDlg.OpenFile();
                    workBook.Write(fs);
                    workBook.Close();
                    
                    string msg = "导出成功：";
                    if (ApplicationData.LanguageSign.Contains("English"))
                        msg = "Export succeeded：";
                    else if (ApplicationData.LanguageSign.Contains("日本語"))
                        msg = "エクスポート成功：";
                    MessageBox.Show(msg + saveDlg.FileName.ToString().Trim());
                }
                catch (Exception ex)
                {
                    string msg = $"导出失败:{ex.Message}";
                    if (ApplicationData.LanguageSign.Contains("English"))
                        msg = $"Export failed:{ex.Message}";
                    else if (ApplicationData.LanguageSign.Contains("日本語"))
                        msg = $"エクスポート失敗:{ex.Message}";
                    MessageBox.Show(msg);
                    
                }
            }
        }


        /// <summary>
        /// 导出数据到CSV文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="table"></param>
        public static void ExportDataToCSVforDay1(string fileName, DataTable dt, string values)
        {
            //Thread.SetApartmentState(ApartmentState.STA);
            if (dt == null || dt.Rows.Count == 0)
            {
                return;
            }

            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "CSV文件(*.csv)|*.csv";
            saveDlg.FileName = fileName;

            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(saveDlg.FileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                try
                {
                    string[] value = values.Split(',');
                    //标题行
                    var data = string.Empty;
                    //写出列名称
                    for (var i = 0; i < dt.Columns.Count - 1; i++)
                    {
                        dt.Columns[i].ColumnName = value[i].Trim();


                        data += dt.Columns[i].ColumnName;
                        if (i < dt.Columns.Count - 1)
                        {
                            data += ",";
                        }
                    }
                    sw.WriteLine(data);

                    //写出各行数据
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        data = string.Empty;
                        for (var j = 0; j < dt.Columns.Count - 1; j++)
                        {
                            if (dt.Rows[i][j] != DBNull.Value)
                            {
                                if (dt.Columns[j].ColumnName.Trim().Contains("是否旷工") ||
                                    dt.Columns[j].ColumnName.Trim().Contains("Absenteeism") ||
                                    dt.Columns[j].ColumnName.Trim().Contains("会社を休むかどうか"))
                                {
                                    string TemString = dt.Rows[i][j].ToString().Trim();
                                    if (TemString.Trim().Equals("0"))
                                    {
                                        TemString = "旷工";
                                        if (ApplicationData.LanguageSign.Contains("English"))
                                            TemString = "YES";
                                        else if (ApplicationData.LanguageSign.Contains("日本語"))
                                            TemString = "無断欠勤";
                                    }

                                    else
                                        TemString = "";
                                    data += TemString + "\t";
                                    data += ",";
                                }
                                //判断请假
                                else if (dt.Columns[j].ColumnName.Trim().Contains("出勤カード") || dt.Columns[j].ColumnName.Trim().Contains("退勤してカードを打つ")
                                || dt.Columns[j].ColumnName.Trim().Contains("班打卡")
                                || dt.Columns[j].ColumnName.Trim().Contains("Clock"))
                                {
                                    string TemString = "";
                                    if (string.IsNullOrEmpty(dt.Rows[i][j].ToString().Trim()) && dt.Rows[i][dt.Columns.Count - 1].ToString().Trim() == "3")
                                    {
                                        TemString = "请假";
                                        if (ApplicationData.LanguageSign.Contains("English"))
                                            TemString = "Leave";
                                        else if (ApplicationData.LanguageSign.Contains("日本語"))
                                            TemString = "休暇をとる";
                                    }
                                    else
                                        TemString = dt.Rows[i][j].ToString().Trim();
                                    data += TemString + "\t";
                                    data += ",";
                                }
                                else
                                {
                                    string TemString = dt.Rows[i][j].ToString().Trim();
                                    data += TemString + "\t";
                                    data += ",";
                                }
                            }
                            else
                            {
                                string TemString = "";
                                data += TemString;
                                data += ",";
                            }
                        }
                        sw.WriteLine(data);
                    }
                    sw.Flush();
                    if (sw != null) sw.Close();
                    if (fs != null) fs.Close();
                    string msg = "导出成功";
                    if (ApplicationData.LanguageSign.Contains("English"))
                        msg = "Export succeeded";
                    else if (ApplicationData.LanguageSign.Contains("日本語"))
                        msg = "エクスポート失敗";
                    MessageBox.Show(msg + saveDlg.FileName.ToString().Trim());
                }
                catch (IOException ex)
                {
                    if (sw != null) sw.Close();
                    if (fs != null) fs.Close();
                    string msg = "导出失败";
                    if (ApplicationData.LanguageSign.Contains("English"))
                        msg = "Export failed";
                    else if (ApplicationData.LanguageSign.Contains("日本語"))
                        msg = "エクスポート失敗";
                    MessageBox.Show(msg);
                }

            }
        }


       



        public static void ExportDataToXlsx<T>(
            string fileName, 
            T[] data, 
            Dictionary<string, string> propertyNames,
            Func<T, string, object, string> convertValueToString,
            string[] selectedPropertyNames = null
        )
        {
            if (data.Length == 0)
            {
                return;
            }

            var cultureInfo = CultureInfo.CurrentCulture.TextInfo;
            selectedPropertyNames = selectedPropertyNames ?? propertyNames.Keys.ToArray();

            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = Strings.ExcelFile;
            saveDlg.FileName = fileName;

            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var workbook = new XSSFWorkbook();
                    var sheet = workbook.CreateSheet();

                    var title = sheet.CreateRow(0);
                    for (var i = 0; i < selectedPropertyNames.Length; ++i)
                    {
                        title.CreateCell(i).SetCellValue(cultureInfo.ToTitleCase(propertyNames[selectedPropertyNames[i]]));
                    }

                    

                    for (var i = 0; i < data.Length; ++i)
                    {
                        var row = sheet.CreateRow(i + 1);
                        var d = data[i];
                        for (var j = 0; j < selectedPropertyNames.Length; ++j)
                        {
                            var propertyName = selectedPropertyNames[j];
                            var v = d.GetType().GetProperty(propertyName).GetValue(d);
                            var str = convertValueToString(d, propertyName, v);
                            row.CreateCell(j).SetCellValue(str);
                        }
                        Application.DoEvents();
                    }


                    FileStream fs = new FileStream(saveDlg.FileName, FileMode.Create);
                    workbook.Write(fs);
                    fs.Close();
    
                    
                    MessageBox.Show($"{Strings.ExportFileSucceed}: {saveDlg.FileName}");
                }
                catch (Exception ex)
                {
                    var msg = string.Format(Strings.ExportFileFailedWithError, ex.Message);
                    MessageBox.Show(msg);
                }
            }
        }

        public static void ExportDataToCSVforDay_deprecated(string fileName, DataTable table)
        {
            //Thread.SetApartmentState(ApartmentState.STA);
            if (table == null || table.Rows.Count == 0)
            {
                return;
            }

            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "CSV文件(*.csv)|*.csv";
            saveDlg.FileName = fileName;

            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(saveDlg.FileName, FileMode.Create);
                StreamWriter write = new StreamWriter(fs, Encoding.Default);
                try
                {
                    //标题行
                    string name = "";
                    string department = "";
                    //string personId = table.Columns["UserKey"].ColumnName = "用户编码";
                    string Employee_code = "";
                    string Date = "";
                    string Punchinformation = "";
                    string Punchinformation1 = "";
                    string Shiftinformation = "";
                    string Duration = "";
                    string late = "";
                    string Leaveearly = "";
                    string workOvertime = "";
                    string isAbsenteeism = "";
                    string temperature = "";

                    if (ApplicationData.LanguageSign.Contains("English"))
                    {
                        name = table.Columns["name"].ColumnName = "Name";
                        department = table.Columns["department"].ColumnName = "department";
                        //string personId = table.Columns["UserKey"].ColumnName = "用户编码";
                        Employee_code = table.Columns["Employee_code"].ColumnName = "Employee number";
                        Date = table.Columns["Date"].ColumnName = "Attendance date";
                        Punchinformation = table.Columns["Punchinformation"].ColumnName = "Clock in";
                        Punchinformation1 = table.Columns["Punchinformation1"].ColumnName = "Clock out";
                        Shiftinformation = table.Columns["Shiftinformation"].ColumnName = "Shift information";
                        Duration = table.Columns["Duration"].ColumnName = "Due attendance time (hours)";
                        late = table.Columns["late"].ColumnName = "Late (minutes)";
                        Leaveearly = table.Columns["Leaveearly"].ColumnName = "Leave early (minutes)";
                        workOvertime = table.Columns["workOvertime"].ColumnName = "Overtime (minutes)";
                        isAbsenteeism = table.Columns["isAbsenteeism"].ColumnName = "Absenteeism";
                        temperature = table.Columns["temperature"].ColumnName = "Body temperature (℃)";
                    }
                    else if (ApplicationData.LanguageSign.Contains("日本語"))
                    {
                        name = table.Columns["name"].ColumnName = "名前";
                        department = table.Columns["department"].ColumnName = "ポジション";
                        //string personId = table.Columns["UserKey"].ColumnName = "用户编码";
                        Employee_code = table.Columns["Employee_code"].ColumnName = "ユーザー番号";
                        Date = table.Columns["Date"].ColumnName = "勤務評定日";
                        Punchinformation = table.Columns["Punchinformation"].ColumnName = "出勤カード";
                        Punchinformation1 = table.Columns["Punchinformation1"].ColumnName = "退勤してカードを打つ";
                        Shiftinformation = table.Columns["Shiftinformation"].ColumnName = "シフト情報";
                        Duration = table.Columns["Duration"].ColumnName = "出勤時間（時間）";
                        late = table.Columns["late"].ColumnName = "遅れます";
                        Leaveearly = table.Columns["Leaveearly"].ColumnName = "早退（分）";
                        workOvertime = table.Columns["workOvertime"].ColumnName = "残業（分）";
                        isAbsenteeism = table.Columns["isAbsenteeism"].ColumnName = "会社を休むかどうか";
                        temperature = table.Columns["temperature"].ColumnName = "体温(℃)";
                    }
                    else
                    {
                        name = table.Columns["name"].ColumnName = "姓名";
                        department = table.Columns["department"].ColumnName = "部门";
                        //string personId = table.Columns["UserKey"].ColumnName = "用户编码";
                        Employee_code = table.Columns["Employee_code"].ColumnName = "员工编号";
                        Date = table.Columns["Date"].ColumnName = "考勤日期";
                        Punchinformation = table.Columns["Punchinformation"].ColumnName = "上班打卡";
                        Punchinformation1 = table.Columns["Punchinformation1"].ColumnName = "下班打卡";
                        Shiftinformation = table.Columns["Shiftinformation"].ColumnName = "班次信息";
                        Duration = table.Columns["Duration"].ColumnName = "应出勤时间(小时)";
                        late = table.Columns["late"].ColumnName = "迟到(分钟)";
                        Leaveearly = table.Columns["Leaveearly"].ColumnName = "早退(分钟)";
                        workOvertime = table.Columns["workOvertime"].ColumnName = "加班(分钟)";
                        isAbsenteeism = table.Columns["isAbsenteeism"].ColumnName = "是否旷工";
                        temperature = table.Columns["temperature"].ColumnName = "体温(℃)";
                    }

                    write.Write(name + ",");
                    write.Write(department + ",");
                    write.Write(Employee_code + ",");
                    write.Write(Date + ",");
                    write.Write(Punchinformation + ",");
                    write.Write(Punchinformation1 + ",");
                    write.Write(Shiftinformation + ",");
                    write.Write(late + ",");
                    write.Write(Leaveearly + ",");
                    write.Write(isAbsenteeism + ",");
                    write.Write(temperature + ",");
                    write.Write(Duration + ",");
                    write.Write(workOvertime + ",");

                    write.WriteLine();
                    //明细行
                    for (int row = 0; row < table.Rows.Count; row++)
                    {
                        string Tem = "";
                        for (int column = 1; column < table.Columns.Count - 2; column++)
                        {
                            if (column == 2)
                                continue;
                            if (table.Rows[row][column] != DBNull.Value)
                            {
                                if (column == 11)
                                {
                                    string TemString = table.Rows[row][column].ToString().Trim();
                                    if (TemString.Trim().Equals("0"))
                                    {
                                        TemString = "旷工";
                                        if (ApplicationData.LanguageSign.Contains("English"))
                                            TemString = "YES";
                                        else if (ApplicationData.LanguageSign.Contains("日本語"))
                                            TemString = "無断欠勤";
                                    }

                                    else
                                        TemString = "";
                                    Tem += TemString + "\t";
                                    Tem += ",";
                                }
                                //判断请假
                                else if (column == 6 || column == 7)
                                {
                                    string TemString = "";
                                    if (string.IsNullOrEmpty(table.Rows[row][column].ToString().Trim()) && table.Rows[row][table.Columns.Count - 1].ToString().Trim() == "3")
                                    {
                                        TemString = "请假";
                                        if (ApplicationData.LanguageSign.Contains("English"))
                                            TemString = "Leave";
                                        else if (ApplicationData.LanguageSign.Contains("日本語"))
                                            TemString = "休暇をとる";
                                    }
                                    else
                                        TemString = table.Rows[row][column].ToString().Trim();
                                    Tem += TemString + "\t";
                                    Tem += ",";
                                }
                                else
                                {
                                    string TemString = table.Rows[row][column].ToString().Trim();
                                    Tem += TemString + "\t";
                                    Tem += ",";
                                }
                            }
                            else
                            {
                                string TemString = "";
                                Tem += TemString;
                                Tem += ",";
                            }
                        }
                        write.WriteLine(Tem);
                    }
                    write.Flush();
                    write.Close();
                    string msg = "导出成功：";
                    if (ApplicationData.LanguageSign.Contains("English"))
                        msg = "Export succeeded：";
                    else if (ApplicationData.LanguageSign.Contains("日本語"))
                        msg = "エクスポート成功：";
                    MessageBox.Show(msg + saveDlg.FileName.ToString().Trim());
                }
                catch (Exception ex)
                {
                    string msg = "导出失败：";
                    if (ApplicationData.LanguageSign.Contains("English"))
                        msg = "Export failed：";
                    else if (ApplicationData.LanguageSign.Contains("日本語"))
                        msg = "エクスポート失敗：";
                    MessageBox.Show(msg);
                    write.Close();
                }
            }
        }


        public static void IsItExcessive(string fileName, DataTable tmpDataTable)
        {
            //获取数据
            if (tmpDataTable == null || tmpDataTable.Rows.Count == 0)
            {
                return;
            }

            DataTabletoExcel(tmpDataTable, fileName);
        }

        public static Microsoft.Office.Interop.Excel.Application m_xlApp = null;
        /// <summary>
        /// 将DataTable数据导出到Excel表
        /// </summary>
        /// <param name="tmpDataTable">要导出的DataTable</param>
        /// <param name="strFileName">Excel的保存路径及名称</param>
        public static void DataTabletoExcel(System.Data.DataTable table, string fileName)
        {
            //Thread.SetApartmentState(ApartmentState.STA);
            if (table == null || table.Rows.Count == 0)
            {
                return;
            }

            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "CSV文件(*.csv)|*.csv";
            saveDlg.FileName = fileName;

            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(saveDlg.FileName, FileMode.Create);
                StreamWriter write = new StreamWriter(fs, Encoding.Default);
                try
                {
                    //标题行
                    string addr_name = "";
                    string time = "";
                    string match_status = "";
                    string person_name = "";
                    string hatColor = "";
                    string wg_card_id = "";
                    string match_failed_reson = "";
                    string exist_mask = "";
                    string body_temp = "";
                    string device_sn = "";
                    string idcard_number = "";
                    string idcard_name = "";
                    string match_type = "";
                    string QRcodestatus = "";
                    string trip_infor = "";
                    string closeup = "";

                    if (ApplicationData.LanguageSign.Contains("English"))
                    {
                        person_name = table.Columns["person_name"].ColumnName = "Name";
                        addr_name = table.Columns["addr_name"].ColumnName = "Addr Name";
                        //string personId = table.Columns["UserKey"].ColumnName = "用户编码";
                        time = table.Columns["time"].ColumnName = "Time";
                        match_status = table.Columns["match_status"].ColumnName = "Match Status"; 
                        //hatColor = table.Columns["hatColor"].ColumnName = "HatColor";
                        wg_card_id = table.Columns["wg_card_id"].ColumnName = "Access card number";
                        match_failed_reson = table.Columns["match_failed_reson"].ColumnName = "Reasons for failure";
                        exist_mask = table.Columns["exist_mask"].ColumnName = "Exist Mask";
                        body_temp = table.Columns["body_temp"].ColumnName = "Temperature";
                        device_sn = table.Columns["device_sn"].ColumnName = "Device_sn";
                        idcard_number = table.Columns["idcard_number"].ColumnName = "Idcard Number";
                        idcard_name = table.Columns["idcard_name"].ColumnName = "Idcard Name";
                        //match_type = table.Columns["match_type"].ColumnName = "Match Type";
                        //QRcodestatus = table.Columns["QRcodestatus"].ColumnName = "QRcodestatus";

                        closeup = table.Columns["closeup"].ColumnName = "closeup"; 
                    }
                    else if (ApplicationData.LanguageSign.Contains("日本語"))
                    {
                        person_name = table.Columns["person_name"].ColumnName = "姓名";
                        addr_name = table.Columns["addr_name"].ColumnName = "Addr Name";
                        //string personId = table.Columns["UserKey"].ColumnName = "用户编码";
                        time = table.Columns["time"].ColumnName = "Time";
                        match_status = table.Columns["match_status"].ColumnName = "Match Status";
                        //hatColor = table.Columns["hatColor"].ColumnName = "HatColor";
                        wg_card_id = table.Columns["wg_card_id"].ColumnName = "Access card number";
                        match_failed_reson = table.Columns["match_failed_reson"].ColumnName = "Reasons for failure";
                        exist_mask = table.Columns["exist_mask"].ColumnName = "Exist Mask";
                        body_temp = table.Columns["body_temp"].ColumnName = "Temperature";
                        device_sn = table.Columns["device_sn"].ColumnName = "Device_sn";
                        idcard_number = table.Columns["idcard_number"].ColumnName = "Idcard Number";
                        idcard_name = table.Columns["idcard_name"].ColumnName = "Idcard Name";
                        //match_type = table.Columns["match_type"].ColumnName = "Match Type";
                        //QRcodestatus = table.Columns["QRcodestatus"].ColumnName = "QRcodestatus";
                        closeup = table.Columns["closeup"].ColumnName = "closeup";
                    }
                    else
                    {
                        person_name = table.Columns["person_name"].ColumnName = "姓名";
                        addr_name = table.Columns["addr_name"].ColumnName = "设备名称";
                        //string personId = table.Columns["UserKey"].ColumnName = "用户编码";
                        time = table.Columns["time"].ColumnName = "刷卡时间";
                        match_status = table.Columns["match_status"].ColumnName = "对比分数";
                        //hatColor = table.Columns["hatColor"].ColumnName = "安全帽颜色";
                        wg_card_id = table.Columns["wg_card_id"].ColumnName = "门禁卡号";
                        match_failed_reson = table.Columns["match_failed_reson"].ColumnName = "比对失败原因";
                        exist_mask = table.Columns["exist_mask"].ColumnName = "是否佩戴口罩";
                        body_temp = table.Columns["body_temp"].ColumnName = "体温";
                        device_sn = table.Columns["device_sn"].ColumnName = "设备序列号";
                        idcard_number = table.Columns["idcard_number"].ColumnName = "证件编号";
                        idcard_name = table.Columns["idcard_name"].ColumnName = "证件姓名";
                        //match_type = table.Columns["match_type"].ColumnName = "对比类型";
                        QRcodestatus = table.Columns["QRcodestatus"].ColumnName = "健康码状态";
                        trip_infor = table.Columns["trip_infor"].ColumnName = "行程信息";

                        closeup = table.Columns["closeup"].ColumnName = "抓拍图片路径";
                    }

                    
                    write.Write(addr_name + ",");
                    write.Write(time + ",");
                    write.Write(match_status + ",");
                    write.Write(person_name + ",");
                    //write.Write(hatColor + ",");
                    write.Write(wg_card_id + ",");
                    write.Write(match_failed_reson + ",");
                    write.Write(exist_mask + ",");
                    write.Write(body_temp + ",");
                    write.Write(device_sn + ",");
                    write.Write(idcard_number + ",");
                    write.Write(idcard_name + ",");
                    //write.Write(match_type + ",");
                    if (ApplicationData.LanguageSign.Contains("中文"))
                    {
                        write.Write(QRcodestatus + ",");
                        write.Write(trip_infor + ",");
                    } 
                    write.Write(closeup + ",");
                    write.WriteLine();
                    //明细行
                    for (int row = 0; row < table.Rows.Count; row++)
                    {
                        string Tem = "";
                        for (int column = 0; column < table.Columns.Count; column++)
                        {
                            if (table.Rows[row][column] != DBNull.Value)
                            {
                                if (column == 7)
                                {
                                    string TemString = table.Rows[row][column].ToString().Trim();
                                    if (TemString.Trim().Length>3)
                                    {
                                        TemString = TemString.Substring(0,4);
                                    }
                                    Tem += TemString + "\t";
                                    Tem += ",";
                                }
                                else if(column == 6)
                                {
                                    string TemString = table.Rows[row][column].ToString().Trim();
                                    if (TemString.Trim().Equals("1"))
                                    {
                                        TemString = "是";
                                        if (ApplicationData.LanguageSign.Contains("English"))
                                            TemString = "YES";
                                        else if (ApplicationData.LanguageSign.Contains("日本語"))
                                            TemString = "はい、";
                                    }

                                    else
                                        TemString = "";
                                    Tem += TemString + "\t";
                                    Tem += ",";
                                }
                                else if (column == 11)
                                {
                                    string TemString = table.Rows[row][column].ToString().Trim();
                                    if(TemString.Trim().Equals("0"))
                                    {
                                        TemString = "绿码";
                                        if (ApplicationData.LanguageSign.Contains("English"))
                                            TemString = "Green code";
                                        else if (ApplicationData.LanguageSign.Contains("日本語"))
                                            TemString = "";
                                    }
                                    else if(TemString.Trim().Equals("1"))
                                    {
                                        TemString = "红码";
                                        if (ApplicationData.LanguageSign.Contains("English"))
                                            TemString = "Red code";
                                        else if (ApplicationData.LanguageSign.Contains("日本語"))
                                            TemString = "";
                                    }
                                    else if(TemString.Trim().Equals("2"))
                                    {
                                        TemString = "黄码";
                                        if (ApplicationData.LanguageSign.Contains("English"))
                                            TemString = "Yellow code";
                                        else if (ApplicationData.LanguageSign.Contains("日本語"))
                                            TemString = "";
                                    }
                                    else if(TemString.Trim().Length>1)
                                    {
                                        TemString = TemString.Split(';')[0];
                                        
                                    }
                                    else
                                        TemString ="";
                                    Tem += TemString + "\t";
                                    Tem += ",";
                                }
                                else if (column == 9)
                                {
                                    string TemString = table.Rows[row][column].ToString().Trim();
                                    if (TemString.Trim().Length==17)
                                    {
                                        TemString = TemString + "X";
                                    }
                                    else if (TemString.Trim().Length <3)
                                    {
                                        TemString = "";
                                    }
                                    Tem += TemString + "\t";
                                    Tem += ",";
                                }
                                else
                                {
                                    string TemString = table.Rows[row][column].ToString().Trim();
                                    Tem += TemString + "\t";
                                    Tem += ",";
                                }
                            }
                            else
                            {
                                string TemString = "";
                                Tem += TemString;
                                Tem += ",";
                            }
                        }
                        write.WriteLine(Tem);
                    }
                    write.Flush();
                    write.Close();
                    string msg = "导出成功：";
                    if (ApplicationData.LanguageSign.Contains("English"))
                        msg = "Export succeeded：";
                    else if (ApplicationData.LanguageSign.Contains("日本語"))
                        msg = "エクスポート成功：";
                    MessageBox.Show(msg + saveDlg.FileName.ToString().Trim());
                }
                catch (Exception ex)
                {
                    string msg = "导出失败：";
                    if (ApplicationData.LanguageSign.Contains("English"))
                        msg = "Export failed：";
                    else if (ApplicationData.LanguageSign.Contains("日本語"))
                        msg = "エクスポート失敗：";
                    MessageBox.Show(msg);
                    write.Close();
                }
            }
        }

        public static void DataTabletoExcelforstaff(System.Data.DataTable table, string fileName)
        {

            Staff[] data;
            Employetype[] employeeTypes;
            Department[] departments;

            using (var conn = SQLiteHelper.GetConnection())
            {
                data = conn.GetAll<Staff>().ToArray();
                employeeTypes = conn.GetAll<Employetype>().ToArray();
                departments = conn.GetAll<Department>().ToArray();
            }

            var propertyNames = Tools.GetPropertyNames(nameof(Staff));
            Func<Staff, string, object, string> converter = (staff, propertyName, value) =>
            {
                switch (propertyName)
                {
                    case nameof(staff.Employetype_id):
                        return employeeTypes?.FirstOrDefault(x => x.id == (int)value)?.Employetype_name ?? "";
                    case nameof(staff.department_id):
                        return departments?.FirstOrDefault(x => x.id == (int)value)?.name ?? "";
                    default:
                        return value != null ? $"{value}" : "";
                }
            };

            var selectedProperties = new string[] { "name", "Email", "phone", "Employee_code", "picture", "publish_time", "IDcardNo", "face_idcard", "idcardtype", "department_id", "Employetype_id" };

            ExportDataToXlsx(fileName, data, propertyNames, converter, selectedProperties);

           
        }

        /// <summary>
        /// 退出报表时关闭Excel和清理垃圾Excel进程
        /// </summary>
        private static void EndReport()
        {
            object missing = System.Reflection.Missing.Value;
            try
            {
                m_xlApp.Workbooks.Close();
                m_xlApp.Workbooks.Application.Quit();
                m_xlApp.Application.Quit();
                m_xlApp.Quit();
            }
            catch { }
            finally
            {
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(m_xlApp.Workbooks);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(m_xlApp.Application);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(m_xlApp);
                    m_xlApp = null;
                }
                catch { }
                try
                {
                    //清理垃圾进程
                    killProcessThread();
                }
                catch { }
                GC.Collect();
            }
        }
        /// <summary>
        /// 杀掉不死进程
        /// </summary>
        private static void killProcessThread()
        {
            ArrayList myProcess = new ArrayList();
            for (int i = 0; i < myProcess.Count; i++)
            {
                try
                {
                    System.Diagnostics.Process.GetProcessById(int.Parse((string)myProcess[i])).Kill();
                }
                catch { }
            }
        }
    }
}

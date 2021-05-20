using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Excel1 = Microsoft.Office.Interop.Excel;

namespace huaanClient
{
    class ExportToExcel
    {
        public Excel1.Application m_xlApp = null;

        public void OutputAsExcelFile(DataTable dtTable, String numStr)
        {
            string filePath = "";
            SaveFileDialog s = new SaveFileDialog();
            s.Title = "保存Excel1文件";
            s.Filter = "Excel1文件(*.xls)|*.xls";
            s.FilterIndex = 1;
            s.FileName = numStr;
            if (s.ShowDialog() == DialogResult.OK)
                filePath = s.FileName;
            else
                return;
            
            //导出dataTable到Excel1  
            int rowNum = dtTable.Rows.Count;//行数  
            int columnNum = dtTable.Columns.Count;//列数 
            m_xlApp = new Excel1.Application();
            m_xlApp.DisplayAlerts = false;//不显示更改提示  
            m_xlApp.Visible = false;

            Excel1.Workbooks workbooks = m_xlApp.Workbooks;
            Excel1.Workbook workbook = workbooks.Add(Excel1.XlWBATemplate.xlWBATWorksheet);
            Excel1.Worksheet worksheet = (Excel1.Worksheet)workbook.Worksheets[1];//取得sheet1  

            try
            {
                string[,] datas = new string[rowNum + 1, columnNum];
                string[] sss = gettion();
                for (int i = 0; i < sss.Length; i++) //写入字段  
                    datas[0, i] = sss[i].ToString();
                //Excel1.Range range = worksheet.get_Range(worksheet.Cells[1, 1], worksheet.Cells[1, columnNum]);  
                Excel1.Range range = m_xlApp.Range[worksheet.Cells[1, 1], worksheet.Cells[1, columnNum]];
                range.Interior.ColorIndex = 15;//15代表灰色  
                range.Font.Bold = true;
                range.Font.Size = 15;

                int r = 0;
                for (r = 0; r < rowNum; r++)
                {
                    //int numRow = int.Parse(numArr[r]) - 1;
                    for (int i = 0; i < columnNum; i++)
                    {
                        object obj;
                        if (i == columnNum - 1)
                        {
                            obj = dtTable.Rows[r][dtTable.Columns[i].ToString()]; ;
                        }
                        else
                        {
                            obj = dtTable.Rows[r][dtTable.Columns[i].ToString()];
                        }
                        datas[r + 1, i] = obj == null ? "" : "'" + obj.ToString().Trim();//在obj.ToString()前加单引号是为了防止自动转化格式
                    }
                    System.Windows.Forms.Application.DoEvents();
                    //添加进度条  
                }
                //Excel1.Range fchR = worksheet.get_Range(worksheet.Cells[1, 1], worksheet.Cells[rowNum + 1, columnNum]);  
                Excel1.Range fchR = m_xlApp.Range[worksheet.Cells[1, 1], worksheet.Cells[rowNum + 1, columnNum]];
                fchR.Value2 = datas;

                worksheet.Columns.EntireColumn.AutoFit();//列宽自适应。  
                                                         //worksheet.Name = "dd";  

                //m_xlApp.WindowState = Excel1.XlWindowState.xlMaximized;  
                m_xlApp.Visible = false;

                // = worksheet.get_Range(worksheet.Cells[1, 1], worksheet.Cells[rowNum + 1, columnNum]);  
                range = m_xlApp.Range[worksheet.Cells[1, 1], worksheet.Cells[rowNum + 1, columnNum]];

                //range.Interior.ColorIndex = 15;//15代表灰色  
                range.Font.Size = 12;
                range.RowHeight = 15;
                range.Borders.LineStyle = 1;
                range.HorizontalAlignment = 1;
                workbook.Saved = true;
                workbook.SaveCopyAs(filePath);
            }
            catch (Exception ex)
            {
                string msg1 = "导出失败：";
                if (ApplicationData.LanguageSign.Contains("English"))
                    msg1 = "Export failed：";
                else if (ApplicationData.LanguageSign.Contains("日本語"))
                    msg1 = "エクスポート失敗：";
                MessageBox.Show(msg1 + ex.Message, msg1, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            finally
            {
                EndReport();
            }

            if (m_xlApp!=null)
            {
                m_xlApp.Workbooks.Close();
                m_xlApp.Workbooks.Application.Quit();
                m_xlApp.Application.Quit();
                m_xlApp.Quit();
            }
            string msg = "导出成功：";
            if (ApplicationData.LanguageSign.Contains("English"))
                msg = "Export succeeded：";
            else if (ApplicationData.LanguageSign.Contains("日本語"))
                msg = "エクスポート成功：";
            MessageBox.Show(msg + s.FileName.ToString().Trim(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void EndReport()
        {
            object missing = System.Reflection.Missing.Value;
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
                this.killProcessThread();
            }
            catch { }
            GC.Collect();
        }

        private void killProcessThread()
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

        public string[] gettion()
        {
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
                person_name = "Name";
                addr_name = "Addr Name";
                //string personId = table.Columns["UserKey"].ColumnName = "用户编码";
                time ="Time";
                match_status =  "Match Status";
                //hatColor = table.Columns["hatColor"].ColumnName = "HatColor";
                wg_card_id = "Access card number";
                match_failed_reson =  "Reasons for failure";
                exist_mask = "Exist Mask";
                body_temp =  "Temperature";
                device_sn =  "Device_sn";
                idcard_number =  "Idcard Number";
                idcard_name = "Idcard Name";
                //match_type = table.Columns["match_type"].ColumnName = "Match Type";
                QRcodestatus = "QRcodestatus";
                trip_infor = "trip_infor";
                closeup = "Closeup";
            }
            else if (ApplicationData.LanguageSign.Contains("日本語"))
            {
                person_name = "姓名";
                addr_name =  "Addr Name";
                //string personId = table.Columns["UserKey"].ColumnName = "用户编码";
                time = "Time";
                match_status = "Match Status";
                //hatColor = table.Columns["hatColor"].ColumnName = "HatColor";
                wg_card_id =  "Access card number";
                match_failed_reson = "Reasons for failure";
                exist_mask =  "Exist Mask";
                body_temp = "Temperature";
                device_sn = "Device_sn";
                idcard_number ="Idcard Number";
                idcard_name = "Idcard Name";
                //match_type = table.Columns["match_type"].ColumnName = "Match Type";
                QRcodestatus =  "QRcodestatus";
                trip_infor = "trip_infor";
                closeup = "Closeup";
            }
            else
            {
                person_name  = "姓名";
                addr_name = "设备名称";
                time = "刷卡时间";
                match_status = "对比分数";
                //hatColor = table.Columns["hatColor"].ColumnName = "安全帽颜色";
                wg_card_id =  "门禁卡号";
                match_failed_reson = "比对失败原因";
                exist_mask = "是否佩戴口罩";
                body_temp =  "体温";
                device_sn = "设备序列号";
                idcard_number = "证件编号";
                idcard_name =  "证件姓名";
                //match_type = table.Columns["match_type"].ColumnName = "对比类型";
                QRcodestatus = "健康码状态";
                trip_infor = "行程信息";
                closeup = "抓拍图片路径";
            }

            List<string> strList = new List<string>();

            strList.Add(addr_name);
            strList.Add(time);
            strList.Add(match_status);
            strList.Add(person_name);
            //write.Write(hatColor + ",");
            strList.Add(wg_card_id);
            strList.Add(match_failed_reson);
            strList.Add(exist_mask);
            strList.Add(body_temp);
            strList.Add(device_sn);
            strList.Add(idcard_number);
            strList.Add(idcard_name);
            //write.Write(match_type + ",");
            strList.Add(QRcodestatus);
            strList.Add(trip_infor);
            strList.Add(closeup);

            string[] strArray = strList.ToArray();
            return strArray;
        }

    }
}

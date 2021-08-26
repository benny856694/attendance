using HaSdkWrapper;
using huaanClient.Database;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace huaanClient
{
    class BatchImport
    {
        /// <summary>  
        /// 将excel导入到datatable  
        /// </summary>  
        /// <param name="filePath">excel路径</param>  
        /// <param name="isColumnName">第一行是否是列名</param>  
        /// <returns>返回datatable</returns>  
        public static DataTable ExcelToDataTable(string filePath, bool isColumnName)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 0;
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本  
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本  
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet  
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数  
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行  
                                int cellCount = firstRow.LastCellNum;//列数  

                                //构建datatable的列  
                                if (isColumnName)
                                {
                                    startRow = 1;//如果第一行是列名，则从第二行开始读取  
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        if (firstRow.FirstCellNum==-1)
                                        {
                                            continue;
                                        }
                                        cell = firstRow.GetCell(i);
                                        if (cell != null)
                                        {
                                            if (cell.StringCellValue != null)
                                            {
                                                column = new DataColumn(cell.StringCellValue);
                                                dataTable.Columns.Add(column);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        if (firstRow.FirstCellNum == -1)
                                        {
                                            continue;
                                        }
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }

                                //填充行  
                                for (int i = startRow; i <= rowCount; i++)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    if (row.FirstCellNum == -1)
                                    {
                                        continue;
                                    }

                                    dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)  
                                            switch (cell.CellType)
                                            {
                                                case CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理  
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                        dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                    break;
                                                case CellType.String:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }
        }


        /// <summary>  
        /// 将excel导入到datatable  
        /// </summary>  
        /// <param name="filePath">excel路径</param>  
        /// <param name="startRow">第几行开始</param>  
        /// <returns>返回datatable</returns>  
        public static DataTable ExcelToDataTable(string filePath, int startRow)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本  
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本  
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet  
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数  
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行  
                                int cellCount = firstRow.LastCellNum;//列数  

                                //构建datatable的列  
                                if (startRow>0)
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        if (firstRow.FirstCellNum == -1)
                                        {
                                            continue;
                                        }
                                        cell = firstRow.GetCell(i);
                                        if (cell != null)
                                        {
                                            if (cell.StringCellValue != null)
                                            {
                                                column = new DataColumn(cell.StringCellValue);
                                                dataTable.Columns.Add(column);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        if (firstRow.FirstCellNum == -1)
                                        {
                                            continue;
                                        }
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }

                                //填充行  
                                for (int i = startRow; i <= rowCount; i++)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    if (row.FirstCellNum == -1)
                                    {
                                        continue;
                                    }

                                    dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)  
                                            switch (cell.CellType)
                                            {
                                                case CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理  
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                        dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                    break;
                                                case CellType.String:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                workbook?.Close();
                fs?.Close();
                
            }
        }

        public static async Task<string> batchImport()
        {
            JObject obj = new JObject();
            obj["result"] = 0;
            obj["data"] = "";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = Properties.Strings.ExcelFile; //设置要选择的文件的类型
            string filePath = "";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
                filePath = openFileDialog.FileName;
            else
            {
                
                return obj.ToString();
            }

            string DirectoryName = System.IO.Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(filePath))
            {
                return obj.ToString();
            }
            //将选择的文件转换成 datatable
            DataTable dataTable = ExcelToDataTable(filePath, 2);

            if (dataTable==null)
            {
                string mes = Properties.Strings.ExcelCantBeAccessed; 
                MessageBox.Show(mes);
                return obj.ToString();
            }
            if (dataTable.Rows.Count == 0)
            {
                obj["result"] = 1;
                obj["data"] = Properties.Strings.ExcelIsEmpty;
                return obj.ToString();
            }
            int lastcell = dataTable.Columns.Count;

            await Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    try
                    {
                        string name = dataTable.Rows[i][0].ToString().Trim();
                        string staff_no = dataTable.Rows[i][1].ToString();
                        string phone = dataTable.Rows[i][2].ToString();
                        string email = dataTable.Rows[i][3].ToString();
                        //
                        string department = dataTable.Rows[i][4].ToString();
                        string Employetype = dataTable.Rows[i][5].ToString();
                        string face_idcard = dataTable.Rows[i][6].ToString();
                        //判断是否是数字
                        
                        //
                        string imge = "";
                        //如果上传了部门信息 先确认当前是否有这个部门id
                        string departmentid = "";
                        if (!string.IsNullOrEmpty(department))
                        {
                            departmentid = GetData.queydepartmentcode(department);
                            if (string.IsNullOrEmpty(departmentid))
                            {
                                departmentid = "";
                                //dataTable.Rows[i][lastcell - 2] = "失败";
                                //dataTable.Rows[i][lastcell-1] = "未查询到部门";
                                //if (ApplicationData.LanguageSign.Contains("English"))
                                //{
                                //    dataTable.Rows[i][lastcell - 2] = "fail";
                                //    dataTable.Rows[i][lastcell - 1] = "Department not found";
                                //}
                                //else if (ApplicationData.LanguageSign.Contains("日本語"))
                                //{
                                //    dataTable.Rows[i][lastcell - 2] = "失敗";
                                //    dataTable.Rows[i][lastcell - 1] = "部署に問い合わせていません";
                                //}

                                //continue;
                            }
                        }

                        //如果上传分类 先确认当前是否有这个分类id
                        string Employetypeid = "1";
                        Employetypeid = GetData.queydEmployetypeid(Employetype);
                        if (string.IsNullOrEmpty(Employetypeid))
                        {
                            Employetypeid = "1";
                            //dataTable.Rows[i][lastcell - 2] = "失败";
                            //dataTable.Rows[i][lastcell - 1] = "未查询到员工分类";

                            //if (ApplicationData.LanguageSign.Contains("English"))
                            //{
                            //    dataTable.Rows[i][lastcell - 2] = "fail";
                            //    dataTable.Rows[i][lastcell - 1] = "No employee classification found";
                            //}
                            //else if (ApplicationData.LanguageSign.Contains("日本語"))
                            //{
                            //    dataTable.Rows[i][lastcell - 2] = "失敗";
                            //    dataTable.Rows[i][lastcell - 1] = " ユーザーの分類に問い合わせていません";
                            //}

                            //continue;
                        }
                        string[] imgestrs = new string[] { ".jpg", ".png", ".JPEG" };
                        for (int s = 0; s < imgestrs.Length; s++)
                        {
                            if (System.IO.File.Exists(DirectoryName + "/" + name.Trim() + imgestrs[s]))
                            {
                                imge = DirectoryName + "/" + name.Trim() + imgestrs[s];
                                break;
                            }
                        }
                        string imgeurl = "";
                        if (!string.IsNullOrEmpty(imge))
                        {
                            //遍历找到对应的图片



                            //判断图片是否合格
                            //if (copyfile.getImageSize(imge) > 4)
                            //{
                            //    dataTable.Rows[i][lastcell - 2] = "失败";
                            //    dataTable.Rows[i][lastcell - 1] = "图片不能超过4M";

                            //    if (ApplicationData.LanguageSign.Contains("English"))
                            //    {
                            //        dataTable.Rows[i][lastcell - 2] = "fail";
                            //        dataTable.Rows[i][lastcell - 1] = "Pictures cannot exceed 4m";
                            //    }
                            //    else if (ApplicationData.LanguageSign.Contains("日本語"))
                            //    {
                            //        dataTable.Rows[i][lastcell - 2] = "失敗";
                            //        dataTable.Rows[i][lastcell - 1] = "写真は4 Mを超えてはいけません";
                            //    }
                            //    continue;
                            //}
                            byte[] imgData = copyfile.SaveImage(imge);

                            //判断图片大小
                            //Bitmap bitmap = copyfile.IsQualified(imge, 600, 800);
                            //if (bitmap != null)
                            //{
                            //    imgData = copyfile.SaveImage(bitmap);
                            //}
                            //else
                            //{
                            //    imgData = copyfile.SaveImage(imge);
                            //}

                            if (imgData == null)
                            {
                                dataTable.Rows[i][lastcell - 2] = Properties.Strings.Fail;
                                dataTable.Rows[i][lastcell - 1] = Properties.Strings.ImageMissing;

                                continue;
                            }
                            //判断是否合格
                            //HaCamera.InitEnvironment();
                            byte[][] re = HaCamera.HA_GetJpgFeatureImageNew(imgData);

                            if (re[2][0] != 0)
                            {
                                dataTable.Rows[i][lastcell - 2] = Properties.Strings.Fail;
                                dataTable.Rows[i][lastcell - 1] = Properties.Strings.StaffImageInValid;

                                continue;
                            }
                        }

                        imgeurl = copyfile.copyimge(imge, copyfile.GetTimeStamp());
                        if (string.IsNullOrEmpty(imgeurl))
                        {
                            dataTable.Rows[i][lastcell - 2] = Properties.Strings.Fail;
                            dataTable.Rows[i][lastcell - 1] = Properties.Strings.ImageMissing;

                            continue;
                        }

                        string idcardtype = string.Empty;
                        if (!string.IsNullOrEmpty(face_idcard))
                        {
                            try
                            {
                                int.Parse(face_idcard);
                                if (4294967295 < ulong.Parse(face_idcard)&&ulong.Parse(face_idcard) < 18446744073709551615)
                                {
                                    idcardtype = "64";
                                }
                                else if (0 < ulong.Parse(face_idcard) && ulong.Parse(face_idcard) <= 4294967295)
                                {
                                    idcardtype = "32";
                                }
                            }
                            catch
                            {
                            }
                        }
                        string data = "";
                        if (string.IsNullOrEmpty(idcardtype))
                        {
                            data = GetData.setStaf(name, staff_no, phone, email, departmentid, Employetypeid, imgeurl, "", "", "", "", Staff.STAFF_SOURCE_BATCH_IMPORT);
                        }
                        else
                        {
                            data = GetData.setStaf(name, staff_no, phone, email, departmentid, Employetypeid, imgeurl, "", "", face_idcard, idcardtype, Staff.STAFF_SOURCE_BATCH_IMPORT);
                        }
                        

                        JObject jObject = JObject.Parse(data);
                        if (jObject["result"].ToString() == "2")
                        {
                            dataTable.Rows[i][lastcell - 2] = Properties.Strings.Success;
                            dataTable.Rows[i][lastcell - 1] = jObject["data"].ToString();
                        }
                        else if (jObject["result"].ToString() != "2")
                        {
                            dataTable.Rows[i][lastcell - 2] = Properties.Strings.Fail;
                            dataTable.Rows[i][lastcell - 1] = jObject["data"].ToString();
                        }
                    }
                    catch (Exception e)
                    {
                        dataTable.Rows[i][lastcell - 2] = Properties.Strings.Fail;
                        dataTable.Rows[i][lastcell - 1] = e.Message;
                    }
                }
            });

            string sss = Properties.Strings.MessageSaveImportLog;
            DialogResult dr = MessageBox.Show(sss, "", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (dr == DialogResult.OK)
            {

                string path=ChoosePath();

                try
                {
                    DataTableToExcel(dataTable, path);
                        obj["result"] = 2;
                    obj["data"] = path;
                }
                catch (Exception ex)
                {
                    obj["result"] = 1;
                    obj["data"] = string.Format(Properties.Strings.ExportFileFailedWithError, ex.Message);
                }
                
                
            }
            return obj.ToString();
        }

        //生成一个datatable
        public static void DataTableToExcel(DataTable dt,string path)
        {
            IWorkbook workbook = null;
            FileStream fs = null;
            IRow row = null;
            ISheet sheet = null;
            ICell cell = null;
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    workbook = new XSSFWorkbook();
                    sheet = workbook.CreateSheet();//创建一个名称为Sheet0的表  
                    int rowCount = dt.Rows.Count;//行数  
                    int columnCount = dt.Columns.Count;//列数  

                    //设置列头  
                    row = sheet.CreateRow(0);//excel第一行设为列头  
                    for (int c = 0; c < columnCount; c++)
                    {
                        cell = row.CreateCell(c);
                        cell.SetCellValue(dt.Columns[c].ColumnName);
                    }

                    //设置每行每列的单元格,  
                    for (int i = 0; i < rowCount; i++)
                    {
                        row = sheet.CreateRow(i + 1);
                        for (int j = 0; j < columnCount; j++)
                        {
                            cell = row.CreateCell(j);//excel第二行开始写入数据  
                            cell.SetCellValue(dt.Rows[i][j].ToString());
                        }
                    }

                    
                    using (fs = File.OpenWrite(path))
                    {
                        workbook.Write(fs);//向打开的这个xls文件中写入数据  
                    }
                }
            }
            finally
            {
                fs?.Close();
            }
        }

        private static string ChoosePath()
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = Properties.Strings.ExcelFile;
            return sfd.ShowDialog() == DialogResult.OK ? sfd.FileName : null;
        }

        public static void Download()
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = Properties.Strings.ExcelFile;
            sfd.FileName = "Template";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var srcPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Template\{Properties.Strings.SrcStaffTemplateFileName}" );
                    File.Copy(srcPath, sfd.FileName, true);
                    MessageBox.Show(Properties.Strings.ExportFileSucceed);
                }
                catch (Exception ex)
                {
                    var errMsg = string.Format(Properties.Strings.ExportFileFailedWithError, ex.Message);
                    MessageBox.Show(errMsg);
                }
            }
        }
    }
}

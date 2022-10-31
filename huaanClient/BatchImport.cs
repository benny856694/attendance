using FileHelpers;
using FileHelpers.ExcelNPOIStorage;
using HaSdkWrapper;
using huaanClient.Database;
using InsuranceBrowser;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace huaanClient
{
    public enum PhotoNaming
    {
        EmployeeName,
        EmployeeNumber,
        EmployeePhone
    }

    [DelimitedRecord("|"), IgnoreEmptyLines(true)]
    class StaffRecord
    {
        public string Name { get; set; }
        public string StaffNo { get; set; }
        [FieldOptional]
        public string PhoneNo { get; set; }
        [FieldOptional]
        public string Email { get; set; }
        [FieldOptional]
        public string DepartmentName { get; set; }
        [FieldHidden]
        public int? DepartmentId { get; set; }
        [FieldOptional]
        public string JobClassification { get; set; }
        [FieldHidden]
        public int? EmployeeTypeId { get; set; }
        [FieldOptional]
        public string AccessCardNo { get; set; }
        [FieldOptional]
        public string CustomerText { get; set; }
        [FieldOptional, FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss")]
        public DateTime? BeginTime { get; set; }
        [FieldOptional, FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss")]
        public DateTime? EndTime { get; set; }
        [FieldOptional]
        public string Status { get; set; }
        [FieldOptional]
        public string Memo { get; set; }
    }

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
                                if (startRow > 0)
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


        public static bool IsMatch(string fullPath, string expectedFileNameWihoutExtension)
        {
            var regex = new Regex($@"{expectedFileNameWihoutExtension}(\.(jpg|jpeg|png))+$");
            return regex.IsMatch(fullPath);
        }

        public static async Task<string> batchImport(PhotoNaming photoNaming = PhotoNaming.EmployeeName)
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


            var excelReader = new ExcelNPOIStorage(typeof(StaffRecord))
            {
                StartRow = 2,
                StartColumn = 0,
                FileName = filePath,
            };
            var newStaffs = excelReader.ExtractRecords().Cast<StaffRecord>().ToArray();

            string DirectoryName = System.IO.Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(filePath))
            {
                return obj.ToString();
            }

            string photoExtensions = @"(\.(jpg|jpeg|png))$";
            var allPhotoFiles = Directory.GetFiles(DirectoryName).Where(s => Regex.IsMatch(s, photoExtensions)).ToList();

            if (newStaffs.Length == 0)
            {
                obj["result"] = 1;
                obj["data"] = Properties.Strings.ExcelIsEmpty;
                return obj.ToString();
            }
            int successCount = 0;
            int failCount = 0;

            await Task.Factory.StartNew(() =>
            {

                foreach (var staff in newStaffs)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(staff.AccessCardNo))
                        {
                            staff.AccessCardNo = "8";
                        }
                        string imge = "";
                        //如果上传了部门信息 先确认当前是否有这个部门id
                        if (!string.IsNullOrEmpty(staff.DepartmentName?.Trim()))
                        {
                            var department = GetData.queydepartmentByName(staff.DepartmentName);
                            if (department == null)
                            {
                                var no = GetData.GetMaxDepartmentNo();
                                department = new Database.Freesql.Department
                                {
                                    no = no + 1,
                                    ParentId = 1,
                                    name = staff.DepartmentName
                                };
                                GetData.AddDepartment(department);
                            }
                            staff.DepartmentId = department?.id;
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

                        //如果上传分类 先确认当前是否有这个分类id
                        if (!string.IsNullOrEmpty(staff.JobClassification?.Trim()))
                        {
                            var EmployeeType = GetData.queydEmployetypeidByName(staff.JobClassification);
                            if (EmployeeType == null)
                            {
                                if (!string.IsNullOrEmpty(staff.JobClassification))
                                {
                                    EmployeeType = GetData.AddEmployeeType(staff.JobClassification);
                                }
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
                            staff.EmployeeTypeId = EmployeeType.id;

                        }

                        var expectedPhotoName = photoNaming == PhotoNaming.EmployeeName ? staff.Name : staff.StaffNo;
                        if (!string.IsNullOrEmpty(expectedPhotoName))
                        {
                            imge = allPhotoFiles.FirstOrDefault(x => IsMatch(x, expectedPhotoName));
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
                                staff.Status = Properties.Strings.Fail;
                                staff.Memo = Properties.Strings.ImageMissing;
                                failCount++;
                                continue;
                            }
                            //判断是否合格
                            //HaCamera.InitEnvironment();
                            byte[][] re = HaCamera.HA_GetJpgFeatureImageNew(imgData);

                            if (re[2][0] != 0)
                            {
                                staff.Status = Properties.Strings.Fail;
                                staff.Memo = Properties.Strings.StaffImageInValid;
                                failCount++;
                                continue;
                            }
                        }

                        imgeurl = copyfile.copyimge(imge, copyfile.GetTimeStamp());
                        if (string.IsNullOrEmpty(imgeurl))
                        {
                            imgeurl = "";
                            //dataTable.Rows[i][lastcell - 2] = Properties.Strings.Fail;
                            staff.Memo = Properties.Strings.ImageMissing;

                            //continue;
                        }

                        string idcardtype = string.Empty;
                        if (!string.IsNullOrEmpty(staff.AccessCardNo))
                        {
                            try
                            {
                                if (4294967295 < ulong.Parse(staff.AccessCardNo) && ulong.Parse(staff.AccessCardNo) < 18446744073709551615)
                                {
                                    idcardtype = "64";
                                }
                                else if (0 < ulong.Parse(staff.AccessCardNo) && ulong.Parse(staff.AccessCardNo) <= 4294967295)
                                {
                                    idcardtype = "32";
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                        string data = "";
                        if (string.IsNullOrEmpty(idcardtype))
                        {
                            data = GetData.setStaf(staff.Name, staff.StaffNo, staff.PhoneNo, staff.Email, staff.DepartmentId, staff.EmployeeTypeId, imgeurl, "", "", "", "", Staff.STAFF_SOURCE_BATCH_IMPORT, staff.CustomerText, staff.BeginTime?.ToString(Constants.DateTimeFormat), staff.EndTime?.ToString(Constants.DateTimeFormat));
                        }
                        else
                        {
                            data = GetData.setStaf(staff.Name, staff.StaffNo, staff.PhoneNo, staff.Email, staff.DepartmentId, staff.EmployeeTypeId, imgeurl, "", "", staff.AccessCardNo, idcardtype, Staff.STAFF_SOURCE_BATCH_IMPORT, staff.CustomerText, staff.BeginTime?.ToString(Constants.DateTimeFormat), staff.EndTime?.ToString(Constants.DateTimeFormat));
                        }


                        JObject jObject = JObject.Parse(data);
                        if (jObject["result"].ToString() == "2")
                        {
                            staff.Status = Properties.Strings.Success;
                            staff.Memo = jObject["data"].ToString();
                            successCount++;
                        }
                        else if (jObject["result"].ToString() != "2")
                        {
                            staff.Status = Properties.Strings.Fail;
                            staff.Memo = jObject["data"].ToString();
                            failCount++;
                        }
                    }
                    catch (Exception e)
                    {
                        staff.Status = Properties.Strings.Fail;
                        staff.Memo = e.Message;
                        failCount++;
                    }
                }
            });

            string sss = Properties.Strings.MessageSaveImportLog;
            //MessageBox.Show($"成功{0},失败{1}");
            DialogResult dr = MessageBox.Show(string.Format(sss, successCount, failCount), "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dr == DialogResult.Yes)
            {

                string path = ChoosePath();
                if (path != null)
                {
                    try
                    {
                        var excelWriter = new ExcelNPOIStorage(typeof(StaffRecord))
                        {
                            StartRow = 0,
                            StartColumn = 0,
                            FileName = path
                        };
                        excelWriter.InsertRecords(newStaffs);

                        obj["result"] = 2;
                        obj["data"] = path;
                    }
                    catch (Exception ex)
                    {
                        obj["result"] = 1;
                        obj["data"] = string.Format(Properties.Strings.ExportFileFailedWithError, ex.Message);
                    }
                }
            }
            return obj.ToString();
        }

        //生成一个datatable
        public static void DataTableToExcel(DataTable dt, string path)
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
            sfd.FileName = Properties.Strings.BatchImportResult;
            sfd.Filter = Properties.Strings.ExcelFile;
            return sfd.ShowDialog() == DialogResult.OK ? sfd.FileName : null;
        }

        public static void Download()
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = Properties.Strings.ExcelFile;
            sfd.FileName = Properties.Strings.BatchImportModel;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var srcPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Template\{Properties.Strings.SrcStaffTemplateFileName}");
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

        public static async Task<string> DownloadImage(PhotoNaming pn)
        {
            JObject obj = new JObject();
            obj["result"] = 0;
            obj["data"] = "";
            obj["msg"] = "";
            int successCount = 0;
            int total = 0;
            string foldPath = GetData.getFoldPath();
            foldPath = foldPath + "staffPicture" + @"\";
            await Task.Factory.StartNew(() =>
            {
                //查询出所有人员信息
                var allStaffs = GetData.GetAllStaffs();
                total = allStaffs.Length;
                if (total == 0)
                {
                    obj["msg"] = "staff total is 0";
                    return;
                }
                Console.WriteLine("人数：" + total);

                if (string.IsNullOrEmpty(foldPath))
                {
                    obj["msg"] = "can't make directory" + foldPath;
                    return;
                }

                if (!Directory.Exists(foldPath))
                {
                    Directory.CreateDirectory(foldPath);
                }
                //遍历人员信息
                allStaffs.ToList().ForEach((item) =>
                {
                    var emp_code = item.Employee_code;
                    var name = item.name;
                    var phone = item.phone;
                    string imageName = "";
                    switch (pn)
                    {
                        case PhotoNaming.EmployeeName:
                            imageName = name; break;
                        case PhotoNaming.EmployeeNumber:
                            imageName = emp_code; break;
                        case PhotoNaming.EmployeePhone:
                            imageName = phone; break;
                        default:
                            imageName = name; break;
                    }
                    //源文件路径
                    var sourcePath = item.picture;
                    //目标文件路径
                    var targetPath = "";
                    if (!string.IsNullOrEmpty(imageName))
                    {
                        targetPath = foldPath + imageName + ".jpg";
                    }

                    if (copyfile.Copyfile(sourcePath, targetPath, imageName))
                    {
                        successCount++;
                    }
                });
            });

            if (successCount > 0)
            {
                obj["result"] = 2;
                var info = string.Format(Properties.Strings.SaveSuccess + "{0}/{1}", successCount, total);
                MessageBox.Show(foldPath, info);
            }
            return obj.ToString();
        }
    }
}

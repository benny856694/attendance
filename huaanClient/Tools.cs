using ClosedXML.Excel;
using huaanClient.Database;
using huaanClient.Properties;
using huaanClient.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace huaanClient
{
    class Tools
    {
        /// <summary>
        /// 判断是否是身份证号码
        /// </summary>
        /// <param name="idCardNumber">需要判断的身份证号码</param>
        /// <returns></returns>
        public static bool CheckChinaIDCardNumberFormat(string idCardNumber)
        {
            string idNumber = idCardNumber;
            bool result = true;
            try
            {
                if (idNumber.Length != 18)
                {
                    return false;
                }
                long n = 0;
                if (long.TryParse(idNumber.Remove(17), out n) == false
                    || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
                {
                    return false;//数字验证  
                }
                string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
                if (address.IndexOf(idNumber.Remove(2)) == -1)
                {
                    return false;//省份验证  
                }
                string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                DateTime time = new DateTime();
                if (DateTime.TryParse(birth, out time) == false)
                {
                    return false;//生日验证  
                }
                string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
                string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
                char[] Ai = idNumber.Remove(17).ToCharArray();
                int sum = 0;
                for (int i = 0; i < 17; i++)
                {
                    sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
                }
                int y = -1;
                Math.DivRem(sum, 11, out y);
                if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
                {
                    return false;//校验码验证  
                }
                return true;//符合GB11643-1999标准 

            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public static Dictionary<string, string> GetPropertyNames(string className)
        {
            var result = new Dictionary<string, string>();
            string[] property = null;
            string[] names = null;


            switch (className)
            {
                case nameof(AttendanceData):
                    property = Properties.Strings.AttendanceKeys.Split(',');
                    names = Properties.Strings.AttendanceNames.Split(',');
                    break;
                case nameof(Staff):
                    property = Properties.Strings.StaffKeys.Split(',');
                    names = Properties.Strings.StaffNames.Split(',');
                    break;
                case nameof(Capture_Data):
                    property = Properties.Strings.CaptureRecordColumnKeys.Split(',');
                    names = Properties.Strings.CaptureRecordColumnNames.Split(',');
                    break;
                case nameof(AttendanceDataMonthly):
                    property = Properties.Strings.AttendanceDataMonthlyKeys.Split(',');
                    names = Properties.Strings.AttendanceDataMonthlyNames.Split(',');
                    break;

                default:
                    throw new InvalidOperationException();
                    break;
            }

            for (int i = 0; i < property.Length; i++)
            {
                result.Add(property[i], names[i]);
            }
            return result;
        }


        public static void GenerateReport(DataContext dataContext, string fileName, IReporter reporter)
        {
            var res = ShowSaveFileAsXlsxDialog(fileName);
            if (res.result == DialogResult.OK)
            {
                try
                {
                    using (var wb = new XLWorkbook())
                    {
                        reporter.Generate(dataContext, wb);
                        wb.SaveAs(res.fileName);
                    }
                    MessageBox.Show($"{Strings.ExportFileSucceed}: {res.fileName}");
                }
                catch (Exception ex)
                {
                    var msg = string.Format(Strings.ExportFileFailedWithError, ex.Message);
                    MessageBox.Show(msg);
                }

            }
        }


        public static (DialogResult result, string fileName) ShowSaveFileAsXlsxDialog(string fileName)
        {
            return ShowSaveFileDialog(fileName, Strings.ExcelFile);
        }

        public static (DialogResult result, string fileName) ShowSaveFileDialog(string fileName, string filter)
        {
            var sd = new SaveFileDialog();
            sd.Filter = filter;
            sd.FileName = fileName;
            return (sd.ShowDialog(), sd.FileName);
        }
    }
}

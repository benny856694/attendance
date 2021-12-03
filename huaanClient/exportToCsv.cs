using huaanClient.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace huaanClient
{
    public static class exportToCsv
    {
        #region Json 字符串 转换为 DataTable数据集合
        /// <summary>
        /// Json 字符串 转换为 DataTable数据集合
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this string json)
        {
            DataTable dataTable = new DataTable();  //实例化
            DataTable result;
            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
                ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dictionary in arrayList)
                    {
                        if (dictionary.Keys.Count<string>() == 0)
                        {
                            result = dataTable;
                            return result;
                        }
                        if (dataTable.Columns.Count == 0)
                        {
                            foreach (string current in dictionary.Keys)
                            {
                                if(dictionary[current] != null)
                                {
                                    if (dictionary[current].ToString() == "person_id")
                                    {

                                    }
                                    else
                                    {
                                        dataTable.Columns.Add(current);
                                    }
                                }
                                else
                                {
                                    dataTable.Columns.Add(current);
                                }
                                
                            }
                        }
                        DataRow dataRow = dataTable.NewRow();
                        foreach (string current in dictionary.Keys)
                        {
                            if (dictionary[current]==null|| dictionary[current].ToString().Contains("null"))
                            {
                                dataRow[current] = "";
                            }
                            else
                            {
                                dataRow[current] = dictionary[current];
                            }
                           
                        }

                        dataTable.Rows.Add(dataRow); //循环添加行到DataTable中

                        int ss = dataTable.Rows.Count;
                    }
                }
            }
            catch(Exception ex)
            {
                
            }
            result = dataTable;
            return result;
        }
        #endregion
        /// <summary>
        /// 将json转换为DataTable
        /// </summary>
        /// <param name="strJson">得到的json</param>
        /// <returns></returns>
        public static DataTable JsonToDataTable(string strJson)
        {
            //转换json格式
            strJson = strJson.Replace(",\"", "*\"").Replace("\":", "\"#").ToString();
            //取出表名
            var rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
            string strName = rg.Match(strJson).Value;
            DataTable tb = null;
            //去除表名
            strJson = strJson.Substring(strJson.IndexOf("[") + 1);
            strJson = strJson.Substring(0, strJson.IndexOf("]"));

            //获取数据
            rg = new Regex(@"(?<={)[^}]+(?=})");
            MatchCollection mc = rg.Matches(strJson);
            for (int i = 0; i < mc.Count; i++)
            {
                string strRow = mc[i].Value;
                string[] strRows = strRow.Split('*');

                //创建表
                if (tb == null)
                {
                    tb = new DataTable();
                    tb.TableName = strName;
                    foreach (string str in strRows)
                    {
                        var dc = new DataColumn();
                        string[] strCell = str.Split('#');

                        if (strCell[0].Substring(0, 1) == "\"")
                        {
                            int a = strCell[0].Length;
                            dc.ColumnName = strCell[0].Substring(1, a - 2);
                        }
                        else
                        {
                            dc.ColumnName = strCell[0];
                        }
                        tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }

                //增加内容
                DataRow dr = tb.NewRow();
                for (int r = 0; r < strRows.Length; r++)
                {
                    dr[r] = strRows[r].Split('#')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
                }
                tb.Rows.Add(dr);
                tb.AcceptChanges();
            }

            return tb;
        }
        public static void export(string data,string date)
        {
            DataTable dataTable = JsonToDataTable(data);

            string msg = "月度考勤表";
            if (ApplicationData.LanguageSign.Contains("English"))
                msg = "Monthly attendance sheet";
            else if (ApplicationData.LanguageSign.Contains("日本語"))
                msg = "月間勤務評定表";

            DataToCsv.ExportDataToCSV(getGoupname()+"-"+msg, dataTable, date);
        }

        public static void exportForDay(AttendanceData[] data,string te, string[] selectedProperties)
        {




            string msg = "每日考勤表";
            if (ApplicationData.LanguageSign.Contains("English"))
                msg = "Daily attendance sheet";
            else if (ApplicationData.LanguageSign.Contains("日本語"))
                msg = "毎日勤務評定表";


            //todo
            Func<AttendanceData, string, object, string> convert = (att, propertyName, value) =>
            {
                switch (propertyName)
                {
                    case nameof(att.Punchinformation):
                    case nameof(att.Punchinformation1):
                        return att.Remarks == "3" ? Properties.Strings.DayOff : value.ToString();
                    case nameof(att.isAbsenteeism):
                        return value.ToString() == "0" ? Properties.Strings.Absent : "";
                    case nameof(att.Date):
                        return ((DateTime)value).ToString("d");
                    default:
                        return value == null ? "" : $"{value}";
                }
            };

            var propertyNames = Tools.GetPropertyNames(nameof(AttendanceData));

            DataToCsv.ExportDataToXlsx(getGoupname()+"-" + msg +"-"+ te, data, propertyNames, convert, selectedProperties);
        }

        public static void exportFor(string type, string data, string te)
        {
            DataTable dataTable = ToDataTable(data);

            int s = dataTable.Rows.Count;
            string msg = "门禁记录";
            if (ApplicationData.LanguageSign.Contains("English"))
                msg = "Access control record";
            else if (ApplicationData.LanguageSign.Contains("日本語"))
                msg = "門限記録";
            if (type=="1")
            {
                ExportToExcel dtTable = new ExportToExcel();
                dtTable.OutputAsExcelFile(dataTable, msg + "-" + te.Trim());
            }
            else
            {
                DataToCsv.IsItExcessive(msg + "-" + te.Trim(), dataTable);
            }  
        }

        public static void exportForstaff()
        {

            string msg = Properties.Strings.StaffExportExcelName;
            DataToCsv.DataTabletoExcelforstaff(null, msg);
        }

        public static void exportForDay1(string data, string te,string values)
        {
            DataTable dataTable = ToDataTable(data);


            string msg = "每日考勤表";
            if (ApplicationData.LanguageSign.Contains("English"))
                msg = "Daily attendance sheet";
            else if (ApplicationData.LanguageSign.Contains("日本語"))
                msg = "毎日勤務評定表";
            DataToCsv.ExportDataToCSVforDay1(getGoupname() + "-" + msg + "-" + te, dataTable, values);
        }

        public static string getGoupname()
        {
            string re = GetData.getGroupname();

            string zumin = string.Empty;
            try
            {
                JArray srjo = (JArray)JsonConvert.DeserializeObject(re);
                if (srjo.Count != 0)
                {
                    zumin = srjo[0]["name"].ToString();
                }
            }
            catch
            {

            }
            

            return zumin;
        }
    }
}

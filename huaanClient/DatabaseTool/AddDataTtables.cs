using Dapper;
using DBUtility.SQLite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace huaanClient.DatabaseTool
{
    class AddDataTtables
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task<bool> addData()
        {
            //判断数据库是否存在
            if (!File.Exists(ApplicationData.FaceRASystemToolUrl + "\\huaanDatabase.sqlite"))     // 返回bool类型，存在返回true，不存在返回false
            {
               SQLiteHelper.NewDbFile(ApplicationData.FaceRASystemToolUrl+"\\huaanDatabase.sqlite");
            }

            await Task.Factory.StartNew(() =>
            {
                //添加Capture_Data 先判断是否存在 如果不存在就创建
                dynamic tableColumns = null;
                using (var conn = SQLiteHelper.GetConnection())
                {
                     tableColumns = conn.Query("SELECT m.name AS TableName, p.name AS ColumnName " +
                        "FROM sqlite_master AS m JOIN pragma_table_info(m.name) AS p ORDER BY m.name, p.cid");
                }

                bool TableColumnExists(string tableName, string columnName = null)
                {
                    foreach (var item in tableColumns)
                    {
                        if (columnName == null)
                        {
                            if (item.TableName == tableName)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (item.TableName == tableName && item.ColumnName == columnName)
                            {
                                return true;
                            }
                        }
                        
                    }

                    return false;
                }

                for (int i = 0; i < tableName.tablename.Length; i++)
                {
                    try
                    {
                        if (!TableColumnExists(tableName.tablename[i].Trim()))
                        {
                            Logger.Debug($"create table {tableName.tablename[i]}");
                            //如果不存在创建数据库
                            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, "CREATE TABLE " + tableName.tablename[i].Trim() + " (id integer NOT NULL PRIMARY KEY AUTOINCREMENT)");
                            //添加列
                            string[] g = GetDatas(tableName.tablename[i].Trim());
                            if (g.Length > 0)
                            {
                                for (int m = 0; m < g.Length; m++)
                                {
                                    try
                                    {
                                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, "ALTER TABLE " + tableName.tablename[i].Trim() + " ADD " + g[m].Trim());
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex, "创建数据库表异常");
                                    }
                                }
                            }
                        }
                        //存在就直接添加
                        else
                        {
                            //添加列
                            string[] g = GetDatas(tableName.tablename[i].Trim());
                            if (g.Length > 0)
                            {
                                for (int m = 0; m < g.Length; m++)
                                {
                                    var t = tableName.tablename[i].Trim();
                                    var c = g[m].Trim();
                                    var columnName = c.Split(' ')[0];
                                    if (!TableColumnExists(t, columnName))
                                    {
                                        try
                                        {
                                            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, "ALTER TABLE " + t + " ADD " + c);
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Error(ex, $"add column({t}.{c}) exception");
                                        }
                                    }
                                    
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "创建数据库异常");
                    }
                }
            });
            
            //判断是否为测试
            Inihelper.FileName = Application.StartupPath + @"\\tool.ini";
            bool sss = Inihelper.ReadBool("Setting", "FirstRun", false);
            if (!Inihelper.ReadBool("Setting", "FirstRun", false))
            {
                //添加默认数据
                //更具不同语种添加不同的默认数据
                try
                {
                    if (ApplicationData.DefaultLanguage == 0)
                    {
                        //中文
                        string publishTime = DateTime.Now.ToString("yyyy-MM-dd");
                        string publishTime1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string sql = "INSERT INTO Shift (id,name,Duration,gotowork1,gotowork2,gooffwork3,rest_time,EffectiveTime,publish_time)" +
                            " VALUES (1, '班次一', 7, '09:30-18:00', '', '', '12:00-13:30', '00:01-11:30,12:00-23:00', '" + publishTime + "')";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO user VALUES (1,'admin', 123456)";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO Employetype VALUES (1, '其他')";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO department VALUES (1, '公司名称', '', 10000, '', '', 1, '" + publishTime1 + "', 0);";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO department VALUES (2, '下属部门', '', 10001, '', '', 2, '" + publishTime1 + "', 1)";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO AttendanceGroup VALUES (1, '{\"Monday\":\"1\",\"Tuesday\":\"1\",\"Wednesday\":\"1\",\"Thursday\":\"1\",\"Friday\":\"1\",\"Saturday\":\"0\",\"Sunday\":\"0\"}', '默认考勤组', '" + publishTime1 + "', 1);";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO Linefor_ VALUES (1,23, '36.5', 'ちゃんが学校に登校しました', 'ちゃんが学校に登校しました。体温は{0}℃でした。', 'ちゃんが学校に登校しました。体温は{0}℃でした。至急、学校へ連絡を下さい。', 'ちゃんが学校に遅刻しました。', 'ちゃんが学校に遅刻しました。体温は{0} ℃でした。', 'ちゃんが学校に遅刻しました。体温は(0) ℃でした。至急、学校へ連絡を下さい。', 'ちゃんが学校から早退しました。', 'ちゃんが学校から早退しました。体温は{0} ℃でした。', 'ちゃんが学校から早退しました。体温は{0}℃でした。至急、学校へ連絡を下さい。', 'ちゃんが学校から下校しました。', 'ちゃんが学校から下校しました。体温は{0}℃でした。', 'ちゃんが学校から下校しました。体温は{0} ℃でした。至急、学校へ連絡を下さい。', '', '', '', '', '', '')";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);
                    }
                    else if (ApplicationData.DefaultLanguage == 1)
                    {
                        //添加英文
                        //中文
                        string publishTime = DateTime.Now.ToString("yyyy-MM-dd");
                        string publishTime1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string sql = "INSERT INTO Shift (id,name,Duration,gotowork1,gotowork2,gooffwork3,rest_time,EffectiveTime,publish_time)" +
                            " VALUES (1, 'Shift 1', 7, '09:30-18:00', '', '', '12:00-13:30', '00:01-11:30,12:00-23:00', '" + publishTime + "')";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO Employetype VALUES (1, 'Other')";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO user VALUES (1,'admin', 123456)";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO department VALUES (1, 'Corporate name', '', 10000, '', '', 1, '" + publishTime1 + "', 0);";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO department VALUES (2, 'Subordinate departments', '', 10001, '', '', 2, '" + publishTime1 + "', 1)";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO AttendanceGroup VALUES (1, '{\"Monday\":\"1\",\"Tuesday\":\"1\",\"Wednesday\":\"1\",\"Thursday\":\"1\",\"Friday\":\"1\",\"Saturday\":\"0\",\"Sunday\":\"0\"}', 'Default Attendance Group', '" + publishTime1 + "', 1);";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO Linefor_ VALUES (1,23, '36.5', 'ちゃんが学校に登校しました', 'ちゃんが学校に登校しました。体温は{0}℃でした。', 'ちゃんが学校に登校しました。体温は{0}℃でした。至急、学校へ連絡を下さい。', 'ちゃんが学校に遅刻しました。', 'ちゃんが学校に遅刻しました。体温は{0} ℃でした。', 'ちゃんが学校に遅刻しました。体温は(0) ℃でした。至急、学校へ連絡を下さい。', 'ちゃんが学校から早退しました。', 'ちゃんが学校から早退しました。体温は{0} ℃でした。', 'ちゃんが学校から早退しました。体温は{0}℃でした。至急、学校へ連絡を下さい。', 'ちゃんが学校から下校しました。', 'ちゃんが学校から下校しました。体温は{0}℃でした。', 'ちゃんが学校から下校しました。体温は{0} ℃でした。至急、学校へ連絡を下さい。', '', '', '', '', '', '');";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);
                    }
                    else if (ApplicationData.DefaultLanguage == 2)
                    {
                        //添加日文
                        //中文
                        string publishTime = DateTime.Now.ToString("yyyy-MM-dd");
                        string publishTime1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string sql = "INSERT INTO Shift (id,name,Duration,gotowork1,gotowork2,gooffwork3,rest_time,EffectiveTime,publish_time)" +
                            " VALUES (1, 'Shift one', 7, '09:30-18:00', '', '', '12:00-13:30', '00:01-11:30,12:00-23:00', '" + publishTime + "')";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO Employetype VALUES (1, 'その他の職種')";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO user VALUES (1,'admin', 123456)";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO department VALUES (1, 'テスト会社', '', 10000, '', '', 1, '" + publishTime1 + "', 0);";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO department VALUES (2, '下部部門', '', 10001, '', '', 2, '" + publishTime1 + "', 1)";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO AttendanceGroup VALUES (1, '{\"Monday\":\"1\",\"Tuesday\":\"1\",\"Wednesday\":\"1\",\"Thursday\":\"1\",\"Friday\":\"1\",\"Saturday\":\"0\",\"Sunday\":\"0\"}', 'テスト班', '" + publishTime1 + "', 1);";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        sql = "INSERT INTO Linefor_ (id,temperature) VALUES (1,'36.5')";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);

                        //日文多一个默认
                        sql = "INSERT INTO Linefor_ VALUES (1,23, '36.5', 'ちゃんが学校に登校しました', 'ちゃんが学校に登校しました。体温は{0}℃でした。', 'ちゃんが学校に登校しました。体温は{0}℃でした。至急、学校へ連絡を下さい。', 'ちゃんが学校に遅刻しました。', 'ちゃんが学校に遅刻しました。体温は{0} ℃でした。', 'ちゃんが学校に遅刻しました。体温は(0) ℃でした。至急、学校へ連絡を下さい。', 'ちゃんが学校から早退しました。', 'ちゃんが学校から早退しました。体温は{0} ℃でした。', 'ちゃんが学校から早退しました。体温は{0}℃でした。至急、学校へ連絡を下さい。', 'ちゃんが学校から下校しました。', 'ちゃんが学校から下校しました。体温は{0}℃でした。', 'ちゃんが学校から下校しました。体温は{0} ℃でした。至急、学校へ連絡を下さい。', '', '', '', '', '', '');";
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "初始化数据库异常");
                }
                
                Inihelper.WriteBool("Setting", "FirstRun", true);
            }

            return true;
        }


        private static string[] GetDatas(string name)
        {
            var type = typeof(tablecolumn);
            var flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            var field = type.GetField(name, flag);
            var result = field.GetValue(null) as string[];
            return result;
        }
        
    }
    class tableName
    {
        public static string[] tablename = { "AttendanceGroup", "Attendance_Data", "Capture_Data", "Employetype",
            "Equipment_distribution", "LineFor_list", "Linefor_", "MyDevice", "Shift", "Special_date", "department"
        , "staff"
        , "user","CsvSettings","Pdfconfiguration","Visitor","DataSyn"}; 
    }

    class tablecolumn
    {

        public static string[] AttendanceGroup =
        {
            "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT",
  "attribute TEXT",
  "name TEXT",
  "publishtime TEXT",
  "isdefault TEXT"
        };

        public static string[] Attendance_Data =
        {
            "id integer NOT NULL PRIMARY KEY AUTOINCREMENT",
  "name TEXT",
  "personId text",
  "department TEXT",
  "Employee_code TEXT",
  "Date text",
  "Punchinformation TEXT",
  "Punchinformation1 TEXT",
  "Punchinformation2 TEXT",
  "Punchinformation22 TEXT",
  "Punchinformation3 TEXT",
  "Punchinformation33 TEXT",
  "Shiftinformation TEXT",
  "late TEXT",
  "Leaveearly TEXT",
  "isAbsenteeism TEXT",
  "isAbsenteeism2 TEXT",
  "isAbsenteeism3 TEXT",
  "temperature TEXT",
  "Duration TEXT",
  "workOvertime TEXT",
  "Todaylate TEXT",
  "Remarks TEXT",
  "IsAcrossNight TEXT"
        };

        public static string[] Capture_Data =
        {
            "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT",
  "sequnce TEXT",
  "device_id text",
  "addr_name TEXT",
  "time TEXT",
  "match_status TEXT",
  "match_type TEXT",
  "person_id TEXT",
  "person_name TEXT",
  "hatColor TEXT",
  "wg_card_id text",
  "match_failed_reson TEXT",
  "exist_mask TEXT",
  "body_temp TEXT",
  "device_sn TEXT",
  "idcard_number TEXT",
  "idcard_name TEXT",
  "closeup TEXT",
  "QRcodestatus TEXT",
  "QRcode TEXT",
  "trip_infor TEXT"
        };

        public static string[] Employetype =
        {
            "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT",
  "Employetype_name TEXT NOT NULL DEFAULT ''"
        };

        public static string[] Equipment_distribution =
        {
            "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT",
  "userid INTEGER NOT NULL  DEFAULT ''",
  "deviceid INTEGER NOT NULL  DEFAULT ''",
  "status TEXT",
  "type TEXT",
  "date TEXT",
  "code TEXT",
  "isDistributedByEmployeeCode INTEGER default 0",
  "employeeCode TEXT DEFAULT ''",
  "errMsg TEXT DEFAULT ''",
  "retryCount INTEGER DEFAULT 0",
        };

        public static string[] LineFor_list =
        {
             "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT",
  "line_userid text",
  "message TEXT",
  "type TEXT",
  "name TEXT",
  "Date TEXT",
  "time TEXT",
  "temperature TEXT",
  "late TEXT",
  "Leaveearly TEXT",
  "status TEXT"
        };

        public static string[] Linefor_ =
        {
            "userid text",
  "temperature TEXT",
  "Message TEXT",
  "Message2 TEXT",
  "Message3 TEXT",
  "Message4 TEXT",
  "Message5 TEXT",
  "Message6 TEXT",
  "Message7 TEXT",
  "Message8 TEXT",
  "Message9 TEXT",
  "Message10 TEXT",
  "Message11 TEXT",
  "Message12 TEXT",
  "lineRQcode TEXT",
  "lineRQcodeEmail TEXT",
  "line_url TEXT",
  "ftpserver TEXT",
  "ftpusername TEXT",
  "ftppassword TEXT"
        };

        public static string[] MyDevice =
        {
            "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT",
  "DeviceName TEXT",
  "number text",
  "ipAddress TEXT",
  "Last_query TEXT",
  "time_syn TEXT",
  "IsEnter INTEGER DEFAULT -1", //设备进出标志，-1：未定义，1：进，0：出
        };

        public static string[] Shift =
        {
            "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT",
  "name text",
  "Duration TEXT",
  "gotowork1 TEXT",
  "gotowork2 TEXT",
  "gooffwork3 TEXT",
  "rest_time TEXT",
  "EffectiveTime TEXT",
  "EffectiveTime2 TEXT",
  "EffectiveTime3 TEXT",
  "publish_time TEXT",
  "IsAcrossNight TEXT"
        };

        public static string[] Special_date =
        {
             "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT",
  "date TEXT",
  "Shiftid INTEGER",
  "datetype TEXT",
  "AttendanceGroupid INTEGER"
        };

        public static string[] department =
        {
             "id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT",
  "name TEXT",
  "phone TEXT",
  "no INTEGER NOT NULL  DEFAULT ''",
  "address TEXT",
  "explain TEXT",
  "code integer",
  "publish_time TEXT",
  "ParentId INTEGER"
        };

        public static string[] staff =
        {
             "id integer NOT NULL PRIMARY KEY AUTOINCREMENT",
  "name text",
  "Email text",
  "phone TEXT",
  "Employee_code TEXT",
  "status text",
  "department_id INTEGER NOT NULL DEFAULT ''",
  "picture text DEFAULT ''",
  "publish_time TIMESTAMP NOT NULL DEFAULT ''",
  "Employetype_id integer",
  "AttendanceGroup_id INTEGER",
  "IDcardNo text",
  "line_userid text",
  "line_code TEXT",
  "line_type TEXT",
  "line_codemail TEXT",
  "islineAdmin TEXT",
  "face_idcard TEXT",
  "source TEXT",
  "idcardtype TEXT",
  //face_idcard
        };

        public static string[] user =
        {
               "username text NOT NULL DEFAULT ''",
  "password text NOT NULL DEFAULT ''"
        };

        public static string[] CsvSettings =
        {
               "keyStr text",
  "valuesStr text"
        };
        //
        public static string[] Pdfconfiguration =
        {
               "pdftitle text",
               "rows1 text",
               "rows2 text",
               "rows3 text",
               "rows4 text",
               "rows5 text",
               "rows5 text",
               "rows6 text",
               "rows7 text",
               "rows8 text",
               "rows9 text",
               "rows10 text",
               "rows11 text",
               "rows12 text"
        };

        public static string[] Visitor =
        {
            "id INTEGER NOT NULL PRIMARY KEY",
  "name TEXT",
  "phone TEXT",
  "imge TEXT",
  "staTime TEXT",
  "endTime TEXT",
  "isDown TEXT"
        };

        //DataSyn role
        public static string[] DataSyn =
        {
            "id INTEGER NOT NULL PRIMARY KEY",
  "name TEXT",
  "imge TEXT",
  "personid TEXT",
  "publishtime TEXT",
  "role TEXT",
  "term_start TEXT",
  "term TEXT",
  "wg_card_id TEXT",
  "long_card_id TEXT",
  "addr_name TEXT",
  "device_sn TEXT",
  "model TEXT",
  "stutas TEXT"
        };
    }
}

using DBUtility.SQLite;
using HaSdkWrapper;
using huaanClient.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using huaanClient.Properties;
using System.Globalization;
using System.Diagnostics;
using InsuranceBrowser;

namespace huaanClient
{
    class GetData
    {
        //返回json
        public static JObject obj = null;
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static IDbConnection GetConnection() => SQLiteHelper.GetConnection();
        public static string getDepartmentDataI()
        {

            string commandText = "SELECT * ,name as title FROM 'department'";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);


            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            List<recursionJSON.TreeModel11> list = Serializer.Deserialize<List<recursionJSON.TreeModel11>>(sr);
            List<recursionJSON.TreeModel> treeViewModels = new List<recursionJSON.TreeModel>();
            //list为待处理数据集，pID为父级节点ID；然后分别传入待处理数据集中ID的字段名称，父级ID的字段名称和需要展示Name字段名称
            treeViewModels = recursionJSON.ConversionList(list, "0", "id", "ParentId", "name", "phone", "no", "address", "explain", "code", "title");

            string treeViewModelsJsonStr = JsonConvert.SerializeObject(treeViewModels);

            return treeViewModelsJsonStr;
        }


        public static string updateDepartmentData(string name, string explain, string phone, string address, string no)
        {
            obj = new JObject();
            obj["result"] = "1";
            obj["data"] = "";


            string commandTextdepartmentid = "SELECT COUNT(id) as len FROM department de WHERE de.no='" + no + "'";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
            if (!string.IsNullOrEmpty(sr))
            {
                JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                string reint = jo[0]["len"].ToString();
                if (int.Parse(reint) > 0)
                {
                    string publish_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string commandText = "UPDATE department SET name='" + name.Trim() + "',explain='" + explain.Trim() + "',phone='" + phone.Trim() + "',address='" + address.Trim() + "',publish_time='" + publish_time + "' WHERE no=" + no.Trim();

                    int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                    if (re == 1)
                    {
                        obj["result"] = 2;
                        obj["data"] = Strings.SaveSuccess;
                    }
                    else
                    {
                        obj["result"] = 1;
                        obj["data"] = Strings.SaveFailed;
                    }

                }
            }
            return obj.ToString();
        }

        public static string AddDepartmentData(string name, string explain, string phone, string address, string no, string ParentId)
        {
            obj = new JObject();
            obj["result"] = "1";
            obj["data"] = "";

            string commandTextdepartmentid = "SELECT COUNT(id) as len FROM department de WHERE de.no='" + no + "'";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
            if (!string.IsNullOrEmpty(sr))
            {
                JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                string reint = jo[0]["len"].ToString();
                string code = GetTimeStamp();
                if (int.Parse(reint) == 0)
                {
                    string publish_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string commandText = "INSERT INTO department (name, phone,no,address,explain,code,publish_time,ParentId) VALUES ('" + name + "', '" + phone + "','" + no + "','" + address + "','" + explain + "','" + code + "','" + publish_time + "','" + ParentId + "')";

                    int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                    if (re == 1)
                    {
                        obj["result"] = 2;
                        obj["data"] = Strings.SaveSuccess;
                    }
                    else
                    {
                        obj["result"] = 1;
                        obj["data"] = Strings.SaveFailed;
                    }

                }
            }
            return obj.ToString();
        }
        public static string getDepartmentNo()
        {

            string commandText = "SELECT MAX(no) as deparmentNo FROM department";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getEmployetype()
        {

            string commandText = "select * FROM Employetype WHERE Employetype_name!=''";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }
        public static void deleteEmployetype(string val)
        {
            Employetype et = null;
            using (var c = GetConnection())
            {
                et = c.QueryFirstOrDefault<Employetype>($"SELECT * FROM Employetype WHERE Employetype_name ='{val}'");
                if (et != null)
                {
                    c.Execute($"DELETE FROM RuleDistributionItem WHERE GroupId = {et.id} AND GroupType = 0");
                }
            }
            string commandText = "UPDATE Employetype SET Employetype_name ='' WHERE Employetype_name = '" + val.Trim() + "'";
            int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
            
        }
        public static void addEmployetype(string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                string[] s = val.Split('-');
                if (s.Length == 0)
                    return;

                for (int i = 0; i < s.Length; i++)
                {
                    string commandText = "INSERT into Employetype (Employetype_name) VALUES ('" + s[i].Trim() + "')";
                    SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                }
            }
        }
        public static bool delDepartmentData(string no, string sedata)
        {
            if (string.IsNullOrEmpty(no))
            {
                return false;
            }
            string id = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(sedata))
                {
                    while (true)
                    {
                        sedata = sedata.Replace(":", "").Replace("\"", "");
                        int sss = sedata.IndexOf("id");
                        if (sss == -1)
                        {
                            break;
                        }
                        sedata = sedata.Substring(sss + 2);

                        //获取iD
                        if (string.IsNullOrEmpty(id))
                        {
                            id = id + sedata.Substring(0, sedata.IndexOf(","));
                        }
                        else
                        {
                            id = id + "," + sedata.Substring(0, sedata.IndexOf(",")).Trim();
                        }
                    }
                }
                //删除所有的ID
                if (!string.IsNullOrEmpty(id))
                {
                    string[] ids = id.Split(',');
                    if (ids.Length > 1)
                    {
                        string commandText = "delete from department where id in (" + id.Substring(id.IndexOf(",") + 1) + ")";
                        int sr = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                    }
                }
            }
            catch (Exception ex) { }
            try
            {

                string commandText = "delete from department where no = " + no;
                int sr = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                if (sr == 1)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string getStaffData(string page, string limt)
        {
            int page1 = int.Parse(page) - 1;
            int pageint = page1 * int.Parse(limt);

            string commandText = "SELECT cast(staf.id as text) id, staf.*,ifnull(de.id,0) as deid,de.name as departmentname ,em.Employetype_name as Employetypename, " +
                " (SELECT COUNT(*) FROM MyDevice) as decount ,(SELECT COUNT(*) FROM Equipment_distribution WHERE userid=staf.id and status = 'success') as eqcount " +
                "FROM staff staf " +
                "LEFT JOIN department de ON de.id=staf.department_id  " +
                "LEFT JOIN Employetype em ON em.id = staf.Employetype_id " +
                "LIMIT " + pageint + "," + limt;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getStaffDatacount()
        {

            string commandText = "SELECT COUNT(*) as count FROM staff";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }


        public static string getline_userid()
        {

            string commandText = "SELECT a.userid as userid FROM Linefor_ a";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static bool setline_userid(string id, string userid)
        {

            string commandText = "UPDATE staff SET line_userid='" + userid + "' WHERE id=" + id;
            int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
            if (re == 1)
            {
                return true;
            }
            else
                return false;
        }

        public static bool setline_Email(string id, string Email)
        {

            string commandText = "UPDATE staff SET Email='" + Email + "' WHERE id=" + id;
            int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
            if (re == 1)
            {
                return true;
            }
            else
                return false;
        }

        public static string getline()
        {

            string commandText = "SELECT userid ,IFNULL(Message,'') as  Message," +
                "IFNULL(Message2,'') as  Message2," +
                "IFNULL(temperature,'') as  temperature," +
                "IFNULL(Message3,'') as  Message3," +
                "IFNULL(Message4,'') as  Message4," +
                "IFNULL(Message5,'') as  Message5," +
                "IFNULL(Message6,'') as  Message6," +
                "IFNULL(Message7,'') as  Message7," +
                "IFNULL(Message8,'') as  Message8," +
                "IFNULL(Message9,'') as  Message9," +
                "IFNULL(Message10,'') as  Message10," +
                "IFNULL(Message11,'') as  Message11," +
                "IFNULL(Message12,'') as  Message12," +
                "IFNULL(line_url,'') as  line_url," +
                "IFNULL(ftpserver,'') as  ftpserver," +
                "IFNULL(ftppassword,'') as  ftppassword," +
                "IFNULL(ftpusername,'') as  ftpusername" +
                " FROM Linefor_ a";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getPdfconfiguration()
        {

            string commandText = "SELECT IFNULL(pdftitle,'') as  pdftitle," +
                "IFNULL(rows1,'') as  rows1," +
                "IFNULL(rows2,'') as  rows2," +
                "IFNULL(rows3,'') as  rows3," +
                "IFNULL(rows4,'') as  rows4," +
                "IFNULL(rows5,'') as  rows5," +
                "IFNULL(rows6,'') as  rows6," +
                "IFNULL(rows7,'') as  rows7," +
                "IFNULL(rows8,'') as  rows8," +
                "IFNULL(rows9,'') as  rows9," +
                "IFNULL(rows10,'') as  rows10," +
                "IFNULL(rows11,'') as  rows11," +
                "IFNULL(rows12,'') as  rows12" +
                " FROM Pdfconfiguration a";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getline_useridforstaff(string id)
        {

            string commandText = "select IFNULL(line_userid,'') as line_userid ,IFNULL(line_type,'') AS line_type,IFNULL(Email,'') AS Email from staff WHERE id=" + id;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static AttendanceDataMonthly[] getMonthlyData(string date, string name, string departments)
        {
            if (string.IsNullOrEmpty(date))
                return new AttendanceDataMonthly[0];
            else
            {
                string commandText = "SELECT name,personId,department,Employee_code,strftime( '%Y-%m', date ) as nowdate,count( isAbsenteeism != '0' OR isAbsenteeism !=NULL ) AS Attendance,(julianday( strftime( '%Y-%m-%d', '" + DateTime.Parse(date).AddMonths(1).ToShortDateString() + "' ) ) - julianday( strftime( '%Y-%m', date ) || '-01' ) ) -count ( name ) as restcount,count( CASE WHEN late != '' THEN 0 ELSE NULL END ) || '/' || sum( late ) as latedata,count( CASE WHEN Leaveearly != '' THEN 0 ELSE NULL END ) || '/' || sum( Leaveearly ) as Leaveearlydata,count( CASE WHEN isAbsenteeism == '0' THEN 0 ELSE NULL END ) as AbsenteeismCount," +
                    "count( CASE WHEN Remarks == '3' AND Punchinformation=='' AND Punchinformation1=='' THEN 0 ELSE NULL END )   AS LeaveCount ,count(CASE WHEN(Punchinformation != '' OR Punchinformation1 != '') AND Remarks == '3' THEN 0 ELSE NULL END)   AS LeaveCount1 FROM Attendance_Data WHERE strftime( '%Y-%m', date ) = '" + date + "' ";
                if (!string.IsNullOrEmpty(departments))
                {
                    var split = departments.Split(',');
                    string dts = string.Join(",", split.Select(x => $"'{x}'"));
                    commandText+=" AND department in( " + dts.Trim() + ")";
                }
                if (!string.IsNullOrEmpty(name))

                {
                    commandText = commandText + $" 	AND name LIKE '%{name.Trim()}%'";
                }
                commandText = commandText + " GROUP BY name,personId,strftime( '%Y-%m', date )";
                using (var conn = SQLiteHelper.GetConnection())
                {
                    var data = conn.Query<AttendanceDataMonthly>(commandText).ToArray();
                    return data;
                }
            }
        }


        //获取人员ID列表
        public static string getStaffa()
        {

            string commandText = "SELECT sta.id as  personId ,sta.AttendanceGroup_id,sta.Employee_code as Employee_code,sta.name,IFNULL(de.name,'') as department FROM staff sta  LEFT JOIN department de on sta.department_id=de.id";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }
        /// <summary>
        /// 获取抓拍记录的最晚时间
        /// </summary>
        /// <param name="re"></param>
        /// <returns></returns>
        public static string getLatestforCapture_Data()
        {

            string commandText = "SELECT time FROM Capture_Data WHERE time = (SELECT MAX(time) FROM Capture_Data);";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }
        public static string GetShiftById(string Id)
        {

            string commandText = "SELECT * FROM Shift WHERE id=" + Id;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getAtt_attribute(string id)
        {

            string commandText = "SELECT attribute FROM AttendanceGroup WHERE id=" + id;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getStaffData(string name, string no, string qu_phone, string page, string limt)
        {
            int page1 = int.Parse(page) - 1;
            int pageint = page1 * int.Parse(limt);

            StringBuilder st = new StringBuilder("SELECT staf.*,ifnull(de.id,0) as deid,de.name as departmentname ,em.Employetype_name as Employetypename, " +
                "(SELECT COUNT(*) FROM MyDevice) as decount ,(SELECT COUNT(*) FROM Equipment_distribution WHERE userid=staf.id  and status = 'success') as eqcount " +
                "FROM staff staf LEFT JOIN department de ON de.id=staf.department_id LEFT JOIN Employetype em ON em.id = staf.Employetype_id WHERE 1=1 AND");
            if (!string.IsNullOrEmpty(name))
            {
                st.Append(" staf.name LIKE '%" + name.Trim() + "%' AND");
            }
            if (!string.IsNullOrEmpty(no))
            {
                st.Append(" staf.Employee_code='" + no.Trim() + "' AND");
            }
            if (!string.IsNullOrEmpty(qu_phone))
            {
                st.Append(" staf.phone='" + qu_phone.Trim() + "' AND");
            }


            string commandText = st.ToString().Substring(0, st.ToString().Length - 3).ToString()
               + " LIMIT " + pageint + "," + limt;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getStaffData()
        {
            StringBuilder st = new StringBuilder("SELECT staf.*,ifnull(de.id,0) as deid,de.name as departmentname ,em.Employetype_name as Employetypename, " +
                "(SELECT COUNT(*) FROM MyDevice) as decount ,(SELECT COUNT(*) FROM Equipment_distribution WHERE userid=staf.id) as eqcount " +
                "FROM staff staf LEFT JOIN department de ON de.id=staf.department_id LEFT JOIN Employetype em ON em.id = staf.Employetype_id WHERE 1=1  AND");

            string commandText = st.ToString().Substring(0, st.ToString().Length - 3).ToString();
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getStaffDataforcount(string name, string no, string qu_phone)
        {

            StringBuilder st = new StringBuilder("SELECT COUNT(*) as count FROM staff staf LEFT JOIN department de ON de.id=staf.department_id LEFT JOIN Employetype em ON em.id = staf.Employetype_id WHERE 1=1  AND");
            if (!string.IsNullOrEmpty(name))
            {
                st.Append(" staf.name LIKE '%" + name.Trim() + "%' AND");
            }
            if (!string.IsNullOrEmpty(no))
            {
                st.Append(" staf.Employee_code='" + no.Trim() + "' AND");
            }
            if (!string.IsNullOrEmpty(qu_phone))
            {
                st.Append(" staf.phone='" + qu_phone.Trim() + "' AND");
            }

            string commandText = st.ToString().Substring(0, st.ToString().Length - 3).ToString();
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }
        public static string getlEmployetypedata()
        {

            string commandText = "SELECT E.id as value,e.Employetype_name as name FROM Employetype E WHERE E.Employetype_name!=''";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }
        public static string getDeviceDiscover()
        {
            List<CameraConfigPort> Devicelistdata = Deviceinfo.MyDevicelist;
            string DevicelistdataJsonStr = JsonConvert.SerializeObject(Devicelistdata);

            return DevicelistdataJsonStr;
        }


        public static async Task<string> getallDeviceDiscover()
        {
            var ips = await GetDevinfo.getDevinfo();

            string DevicelistdataJsonStr = JsonConvert.SerializeObject(ips);

            return DevicelistdataJsonStr;
        }

        public static string getDeviceforMyDevice()
        {

            string commandText = "SELECT * FROM MyDevice";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static MyDevice[] getAllMyDevice()
        {
            using (var conn = SQLiteHelper.GetConnection())
            {
                return conn.GetAll<MyDevice>().ToArray();
            }
        }

        //更新编号和名字
        public static void setDevicenumber(string number, string id)
        {

            string commandText = "UPDATE MyDevice set number='" + number.Trim() + "' WHERE id=" + id;
            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
        }

        public static string AddIPtoMydevice(string IP, string DeviceName, int inout,string username, string password)
        {
            obj = new JObject();
            obj["result"] = 0;
            obj["data"] = "";

            if (!string.IsNullOrEmpty(IP))
            {
                string commandTextdepartmentid = "SELECT COUNT(id) as len FROM MyDevice WHERE ipAddress= '" + IP.Trim() + "'";

                string quIPsr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
                if (!string.IsNullOrEmpty(quIPsr))
                {
                    JArray jo = (JArray)JsonConvert.DeserializeObject(quIPsr);
                    string reint = jo[0]["len"].ToString();
                    if (int.Parse(reint) > 0)
                    {
                        obj["result"] = 1;
                        obj["data"] = Strings.IPAlreadyExists;
                        return obj.ToString();
                    }
                    else
                    {
                        var myDev = new MyDevice
                        {
                            ipAddress = IP,
                            DeviceName = DeviceName,
                            IsEnter = inout,
                            username = username,
                            password = password
                        };
                        using (var conn = SQLiteHelper.GetConnection())
                        {
                            var re = conn.Insert(myDev);
                            if (re != 0)
                            {
                                obj["result"] = 2;
                                obj["data"] = Strings.SaveSuccess;
                            }
                        }

                    }
                }
                else
                {
                    obj["result"] = 3;
                    obj["data"] = Strings.SaveFailed;
                }
            }
            return obj.ToString();
        }

        public static string UpdatIPtoMydevice(string oldIp, string IP, string DeviceName, int inout,string username,string password)
        {
            obj = new JObject();
            obj["result"] = 0;
            obj["data"] = "";

            if (oldIp.Length < 3)
            {
                return obj.ToString();
            }
            if (!string.IsNullOrEmpty(IP))
            {
                string commandText = $"UPDATE  MyDevice  SET ipAddress='{IP}', DeviceName='{DeviceName}', IsEnter={inout},username='{username}',password='{password}' WHERE ipAddress='{oldIp}'";
                int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                if (re == 1)
                {
                    Deviceinfo.MyDevicelist.RemoveAll(c => c.IP == oldIp.Trim());
                    obj["result"] = 2;
                    obj["data"] = Strings.SaveSuccess;
                }
            }
            return obj.ToString();
        }

        public static bool DeleteIPtoMydevice(string IP)
        {
            bool result = false;
            if (string.IsNullOrEmpty(IP))
                return result;


            MyDevice d = null;
            using (var c = GetConnection())
            {
                d = c.QueryFirstOrDefault<MyDevice>($"SELECT * FROM MyDevice WHERE ipAddress = '{IP}'");
            }

            string commandText = "delete FROM MyDevice WHERE ipAddress='" + IP + "'";

            //先删除下发队列中的设备
            string updatessql = "delete FROM Equipment_distribution WHERE deviceid=(SELECT id FROM MyDevice WHERE ipAddress='" + IP + "')";
            int DetoeqRe = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
            if (DetoeqRe >= 0)
            {
                int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                if (re >= 0)
                {
                    Deviceinfo.MyDevicelist.RemoveAll(c => c.IP == IP);
                    result = true;
                }
                if (d != null)
                {
                    using (var c = GetConnection())
                    {
                        c.ExecuteScalar($"DELETE FROM RuleDistributionDevice WHERE DeviceId = {d.id}");
                    }
                }
                
            }
            return result;
        }



        public static string queryPerson(string id)
        {
            obj = new JObject();
            obj["result"] = "error";
            obj["data"] = "";
            if (!string.IsNullOrEmpty(id))
            {

                string commandText = $"SELECT sta.name,sta.picture  as imgeurl FROM staff sta WHERE sta.id= '{id}'";
                string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

                JArray jArray = (JArray)JsonConvert.DeserializeObject(sr);
                if (jArray.Count > 0)
                {
                    string name = jArray[0]["name"].ToString();
                    string imgeurl = jArray[0]["imgeurl"].ToString();
                    if (!string.IsNullOrEmpty(imgeurl))
                    {
                        if (System.IO.File.Exists(@imgeurl))
                        {
                            obj["result"] = "success";
                            obj["name"] = name;
                            obj["imgeurl"] = imgeurl;
                        }

                    }
                    else
                    {
                        obj["result"] = "success";
                        obj["name"] = name;
                    }
                }
            }
            return obj.ToString();
        }

        public static string getShiftData()
        {

            string commandText = "SELECT *  FROM Shift sh";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }
        public static string getGroup()
        {

            string commandText = "SELECT at.id,at.attribute,name,isdefault ,(SELECT COUNT(id) FROM staff WHERE staff.AttendanceGroup_id=at.id)  as count FROM AttendanceGroup at";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }
        public static string getGroup(string id)
        {

            string commandText = "SELECT * FROM AttendanceGroup WHERE id=" + id;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getStaffIdsInAttendanceGroup(int attendanceGroupId)
        {
            using (var conn = SQLiteHelper.GetConnection())
            {
                var ids = conn.Query<string>($"SELECT cast(id as text)  from staff where {nameof(Staff.AttendanceGroup_id)} = {attendanceGroupId}").ToArray();
                var json = JsonConvert.SerializeObject(ids);
                return json;

            }
        }

        public static string getGroupname()
        {

            string commandText = "SELECT name FROM department WHERE id=1";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getstaffinfo(string id)
        {

            string commandText = "SELECT s.name as name,IFNULL(s.line_code,0) as line,IFNULL(s.line_codemail,0) as linecodemail,(SELECT name FROM department WHERE id=1) as zumin FROM staff s WHERE id=" + id;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getstaffline_code(string id)
        {
            string commandText = "SELECT IFNULL(line_code,'')  as line_code ,IFNULL(line_codemail,'')  as line_codemail ,IFNULL(line_userid,'')  as line_userid,IFNULL(Email,'')  as Email FROM staff WHERE id=" + id;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static void setstaffline_code(string id, string code)
        {
            string commandText = "UPDATE staff SET line_code='" + code + "' WHERE id=" + id;
            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
        }

        public static void setstaffline_codemail(string id, string code)
        {
            string commandText = "UPDATE staff SET line_codemail='" + code + "' WHERE id=" + id;
            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
        }

        public static string getEffectiveTime(string id)
        {

            string commandText = "SELECT EffectiveTime FROM Shift WHERE id=" + id;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string queryAttendanceinformationcount(string starttime, string endtime, string name, string late, string Leaveearly, string isAbsenteeism)
        {
            StringBuilder commandText = new StringBuilder("SELECT COUNT(*) as count FROM  Attendance_Data  att WHERE att.Date>='" + starttime.Trim() + "' AND att.Date<='" + endtime.Trim() + "' AND");
            if (!string.IsNullOrEmpty(name))
            {
                commandText.Append(" att.name LIKE '%" + name.Trim() + "%' AND");
            }
            if (late.Trim().Equals("1"))
            {
                commandText.Append(" att.late>0 AND");
            }
            if (Leaveearly.Trim().Equals("1"))
            {
                commandText.Append(" att.Leaveearly>0 AND");
            }
            if (isAbsenteeism.Trim().Equals("1"))
            {
                commandText.Append(" att.isAbsenteeism==0 AND");
            }
            string commandText2 = commandText.ToString().Substring(0, commandText.ToString().Length - 3).ToString();
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText2.ToString());
            return sr;
        }

        public static string queryAttendanceinformation(string starttime, string endtime, string name, string late, string Leaveearly, string isAbsenteeism, string page, string limt,string departments)
        {

            int page1 = int.Parse(page) - 1;
            int pageint = page1 * int.Parse(limt);

            StringBuilder commandText = new StringBuilder("SELECT * FROM  Attendance_Data  att WHERE att.Date>='" + starttime.Trim() + "' AND att.Date<='" + endtime.Trim() + "' AND");
            if (!string.IsNullOrEmpty(name))
            {
                commandText.Append(" att.name LIKE '%" + name.Trim() + "%' AND");
            }
            if (late.Trim().Equals("1"))
            {
                commandText.Append(" att.late>0 AND");
            }
            if (Leaveearly.Trim().Equals("1"))
            {
                commandText.Append(" att.Leaveearly>0 AND");
            }
            if (isAbsenteeism.Trim().Equals("1"))
            {
                commandText.Append(" att.isAbsenteeism==0 AND");
            }
            if (!string.IsNullOrEmpty(departments))
            {
                var split = departments.Split(',');
                string dts = string.Join(",", split.Select(x => $"'{x}'"));
                commandText.Append(" att.department in( " + dts.Trim() + ") AND");
            }


            string commandText2 = commandText.ToString().Substring(0, commandText.ToString().Length - 3).ToString()
                + " ORDER BY name, date LIMIT " + pageint + "," + limt;
            Action<DataTable> convertCelsiusToFahreinheit = null;
            if (!ChromiumForm.userSettings.ShowTemperatureInCelsius)
            {
                convertCelsiusToFahreinheit = ConvertCelsiusToFahreinheit("temperature");
            }
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText2.ToString(), convertCelsiusToFahreinheit);
            return sr;
        }

        public static string queryAttendanceinformation(string personId)
        {
            DateTime dateTime = DateTime.Now;
            string end = dateTime.ToString("yyyy-MM-dd") + " 23:59:59";
            string sta = dateTime.AddDays(-(dateTime.Day + 1)).ToString("yyyy-MM-dd") + " 00:00:00";
            StringBuilder commandText = new StringBuilder("SELECT * FROM  Attendance_Data  att WHERE att.Date>='" + sta.Trim() + "' AND att.Date<='" + end.Trim() + "' AND personId='" + personId + "'");
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText.ToString());
            return sr;
        }

        public static string getCsvSettings()
        {
            string sql = "SELECT IFNULL(keyStr,'') as keyStr,IFNULL(valuesStr,'') as valuesStr FROM CsvSettings LIMIT 1";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, sql);
            return sr;
        }

        public static bool setCsvSettings(string key, string values)
        {
            bool result = false;
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(values))
            { key = ""; values = ""; }
            try
            {
                string sql = "SELECT COUNT(*) as len FROM CsvSettings";
                string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, sql);
                JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                if (jo.Count > 0)
                {
                    string len = jo[0]["len"].ToString();
                    if (int.Parse(len) == 0)
                    {
                        sql = "INSERT INTO CsvSettings (keyStr, valuesStr) VALUES ('" + key + "', '" + values + "')";
                        int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);
                        if (re > 0)
                        {
                            result = true;
                        }
                    }
                    else if (int.Parse(len) == 1)
                    {
                        sql = "SELECT id FROM CsvSettings";
                        sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, sql);
                        jo = (JArray)JsonConvert.DeserializeObject(sr);
                        string id = jo[0]["id"].ToString();
                        sql = "UPDATE CsvSettings SET keyStr = '" + key + "' ,valuesStr='" + values + "' WHERE id =" + id;
                        int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);
                        if (re > 0)
                        {
                            result = true;
                        }
                    }
                    else if (int.Parse(len) > 1)
                    {
                        //先全部删除
                        sql = "DELETE FROM CsvSettings";
                        int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);
                        if (re > 0)
                        {
                            sql = "INSERT INTO CsvSettings (keyStr, valuesStr) VALUES ('" + key + "', '" + values + "')";
                            re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);
                            if (re > 0)
                            {
                                result = true;
                            }
                        }
                    }
                }
                return result;

            }
            catch (Exception ex)
            {
                return result;
            }
        }


        public static AttendanceData[] queryAttendanceinformation(string starttime, string endtime, string name, string late, string Leaveearly, string isAbsenteeism, string departments, string other)
        {
            var pg = new DapperExtensions.PredicateGroup() { Operator = DapperExtensions.GroupOperator.And, Predicates = new List<DapperExtensions.IPredicate>() };
            pg.Predicates.Add(DapperExtensions.Predicates.Between<AttendanceData>(
                a => a.Date,
                new DapperExtensions.BetweenValues { Value1 = starttime, Value2 = endtime }));

            if (!string.IsNullOrEmpty(name))
            {
                pg.Predicates.Add(DapperExtensions.Predicates.Field<AttendanceData>(a => a.name, DapperExtensions.Operator.Like, $"%{name}%"));
            }
            if (late.Trim().Equals("1"))
            {
                pg.Predicates.Add(DapperExtensions.Predicates.Field<AttendanceData>(a => a.late, DapperExtensions.Operator.Gt, 0));
            }
            if (Leaveearly.Trim().Equals("1"))
            {
                pg.Predicates.Add(DapperExtensions.Predicates.Field<AttendanceData>(a => a.Leaveearly, DapperExtensions.Operator.Gt, 0));
            }
            if (isAbsenteeism.Trim().Equals("1"))
            {
                pg.Predicates.Add(DapperExtensions.Predicates.Field<AttendanceData>(a => a.isAbsenteeism, DapperExtensions.Operator.Eq, 0));
            }
            if (!string.IsNullOrEmpty(departments))
            {
                
                // string dts = string.Join(",", split.Select(x => $"'{x}'"));
                //commandText.Append(" att.department in( " + dts.Trim() + ") AND");
                var pgDepartments = new DapperExtensions.PredicateGroup() { Operator = DapperExtensions.GroupOperator.Or, Predicates = new List<DapperExtensions.IPredicate>() };
                var split = departments.Split(',');
                foreach (var dep in split)
                {
                    pgDepartments.Predicates.Add(DapperExtensions.Predicates.Field<AttendanceData>(a => a.department, DapperExtensions.Operator.Eq, dep));

                }
                pg.Predicates.Add(pgDepartments);

            }


            var sort = new List<DapperExtensions.ISort>();
            sort.Add(new DapperExtensions.Sort { PropertyName = nameof(AttendanceData.name), Ascending = true });
            sort.Add(new DapperExtensions.Sort { PropertyName = nameof(AttendanceData.Date), Ascending = true });

            using (var con = SQLiteHelper.GetConnection())
            {
                var data = DapperExtensions.DapperExtensions.GetList<AttendanceData>(con, pg, sort).ToArray();
                if (!ChromiumForm.userSettings.ShowTemperatureInCelsius)
                {
                    foreach (var item in data)
                    {
                        item.temperature = item.temperature.toFahreinheit();
                    }
                }
                return data;
            }

        }
        public static string queryAttendanceinformation_deprecated(string starttime, string endtime, string name, string late, string Leaveearly, string isAbsenteeism)
        {

            StringBuilder commandText = new StringBuilder("SELECT * FROM  Attendance_Data  att WHERE att.Date>='" + starttime.Trim() + "' AND att.Date<='" + endtime.Trim() + "' AND");
            if (!string.IsNullOrEmpty(name))
            {
                commandText.Append(" att.name LIKE '%" + name.Trim() + "%' AND");
            }
            if (late.Trim().Equals("1"))
            {
                commandText.Append(" att.late>0 AND");
            }
            if (Leaveearly.Trim().Equals("1"))
            {
                commandText.Append(" att.Leaveearly>0 AND");
            }
            if (isAbsenteeism.Trim().Equals("1"))
            {
                commandText.Append(" att.isAbsenteeism=0 AND");
            }
            string commandText2 = commandText.ToString().Substring(0, commandText.ToString().Length - 3).ToString();
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText2.ToString());
            return sr;
        }
        public static string queryAttendanceinformation(string starttime, string endtime, string name, string late, string Leaveearly, string isAbsenteeism, string values)
        {
            StringBuilder commandText = new StringBuilder("SELECT " + values + " FROM  Attendance_Data  att WHERE att.Date>='" + starttime.Trim() + "' AND att.Date<='" + endtime.Trim() + "' AND");
            if (!string.IsNullOrEmpty(name))
            {
                commandText.Append(" att.name LIKE '%" + name.Trim() + "%'  AND");
            }
            if (late.Trim().Equals("1"))
            {
                commandText.Append(" att.late>0 AND");
            }
            if (Leaveearly.Trim().Equals("1"))
            {
                commandText.Append(" att.Leaveearly>0 AND");
            }
            if (isAbsenteeism.Trim().Equals("1"))
            {
                commandText.Append(" att.isAbsenteeism=0 AND");
            }
            string commandText2 = commandText.ToString().Substring(0, commandText.ToString().Length - 3).ToString();
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText2.ToString());
            return sr;
        }

        public static bool DeleteShift(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            try
            {

                string commandText = "delete from Shift where id = " + id;
                int sr = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);

                if (sr == 1)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static string setShiftData(string data)
        {
            obj = new JObject();
            obj["result"] = "1";
            obj["data"] = "";

            if (string.IsNullOrEmpty(data))
            {
                return obj.ToString();
            }
            else
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(data);


                string publish_time = DateTime.Now.ToString("yyyy-MM-dd");
                string commandText = "Insert into Shift (publish_time,name, Duration, gotowork1, gotowork2, gooffwork3,rest_time,EffectiveTime,EffectiveTime2,EffectiveTime3,IsAcrossNight) " +
               "values('" + publish_time + "','" + jo["name"].ToString()
               + "', '" + jo["Duration"].ToString() + "','" + jo["gotowork1"].ToString() +
               "',' " + jo["gotowork2"].ToString() + "', '" + jo["gooffwork3"].ToString() +
               "', '" + jo["rest_time"].ToString() + "','" + jo["EffectiveTime"].ToString() + "','" + jo["EffectiveTime2"].ToString() + "','" + jo["EffectiveTime3"].ToString() + "','" + jo["IsAcrossNight"].ToString().ToLower() + "')";


                int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                if (re == 1)
                {
                    obj["result"] = 2;
                    obj["data"] = Strings.SaveSuccess;
                }
                else
                {
                    obj["result"] = 1;
                    obj["data"] = Strings.SaveFailed;
                }
            }
            return obj.ToString();
        }

        public static string setShiftData(string data, string id)
        {
            obj = new JObject();
            obj["result"] = "1";
            obj["data"] = "";

            if (string.IsNullOrEmpty(data))
            {
                return obj.ToString();
            }
            else
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(data);


                string publish_time = DateTime.Now.ToString("yyyy-MM-dd");
                string commandText = "UPDATE Shift SET " +
                    "publish_time = '" + publish_time + "'," +
                     "name='" + jo["name"].ToString() + "'," +
                     "Duration='" + jo["Duration"].ToString() + "'," +
                     "gotowork1='" + jo["gotowork1"].ToString() + "'," +
                     "gotowork2='" + jo["gotowork2"].ToString() + "'," +
                     "gooffwork3='" + jo["gooffwork3"].ToString() + "'," +
                     "rest_time='" + jo["rest_time"].ToString() + "'," +
                     "EffectiveTime='" + jo["EffectiveTime"].ToString() + "'," +
                     "EffectiveTime2='" + jo["EffectiveTime2"].ToString() + "'," +
                     "EffectiveTime3='" + jo["EffectiveTime3"].ToString() + "'" +
                     " WHERE id='" + id + "'";


                int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                if (re == 1)
                {
                    obj["result"] = 2;
                    obj["data"] = Strings.SaveSuccess;
                }
                else
                {
                    obj["result"] = 1;
                    obj["data"] = Strings.SaveFailed;
                }
            }
            return obj.ToString();
        }
        /// <summary>
        /// 批量下发/指定单个相机一键下发
        /// </summary>
        /// <param name="data">可以是字符串数组（包含相机ID与人员ID对应关系），也可以是单个相机ID</param>
        public static void setAddPersonToEquipment_distribution(string data)
        {
            //传入是相机ID与员工ID对应关系的数组
            if(data.StartsWith("[") && data.EndsWith("]"))
            {
                JArray jo = (JArray)JsonConvert.DeserializeObject(data);
                if (jo.Count > 0)
                {
                    foreach (JObject s in jo)
                    {
                        string userid = s["userid"].ToString().Trim();
                        string deviceid = s["deviceid"].ToString().Trim();
                        MyDevice device = null;
                        Staff staff = null;
                        var distributeByCode = getIscode_syn();
                        using (var conn = SQLiteHelper.GetConnection())
                        {
                            device = conn.Get<MyDevice>(deviceid);
                            staff = conn.Get<Staff>(userid);
                            DistributeStaffToDevice(staff, device, distributeByCode, conn);
                        }
                    }
                }
            }
            else
            {
                //传入的是deviceId
                if (data.Length > 0)
                {
                    try
                    {
                        int deviceId = int.Parse(data);
                        MyDevice device = null;
                        IEnumerable<Staff> staffs = null;
                        var distributeByCode = getIscode_syn();
                        using (var conn = SQLiteHelper.GetConnection())
                        {
                            //var predicte = DapperExtensions.Predicates.Field<Staff>(s=>s.picture, DapperExtensions.Operator.Eq, null, true);
                            device=DapperExtensions.DapperExtensions.Get<MyDevice>(conn, deviceId);
                            staffs = DapperExtensions.DapperExtensions.GetList<Staff>(conn);
                            foreach (var staff in staffs)
                            {
                                DistributeStaffToDevice(staff, device, distributeByCode, conn);
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        Logger.Error(e.Message);
                    }
                }
            }
           
        }
        //一键下发
        public static bool setAddPersonToEquipment_distribution()
        {
            try
            {
                IEnumerable<MyDevice> devices = null;
                IEnumerable<Staff> staffs = null;
                var distributeByCode = getIscode_syn();

                using (var conn = SQLiteHelper.GetConnection())
                {

                    devices = DapperExtensions.DapperExtensions.GetList<MyDevice>(conn);

                    //var predicte = DapperExtensions.Predicates.Field<Staff>(s=>s.picture, DapperExtensions.Operator.Eq, null, true);
                    staffs = DapperExtensions.DapperExtensions.GetList<Staff>(conn);
                    foreach (var device in devices)
                    {
                        foreach (var staff in staffs)
                        {
                            DistributeStaffToDevice(staff, device, distributeByCode, conn);
                        }
                    }
                }



                return true;
            }
            catch
            {
                return false;
            }

        }


        public static void setAddPersonToEquipment(string id)
        {
            try
            {
                var distributeByCode = getIscode_syn();
                IEnumerable<MyDevice> myDevices = null;
                Staff staff = null;
                using (var conn = SQLiteHelper.GetConnection())
                {
                    myDevices = conn.GetAll<MyDevice>();
                    staff = conn.Get<Staff>(id);
                    foreach (var d in myDevices)
                    {
                        DistributeStaffToDevice(staff, d, distributeByCode, conn);
                    }
                }


            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Add Data to distribution error");
                throw;
            }
        }

        private static void DistributeStaffToDevice(Staff staff, MyDevice device, bool distributeByCode, IDbConnection conn)
        {

            var distributions = conn.Query<EquipmentDistribution>(
                    "select * from Equipment_distribution " +
                    "where userid = @userid and deviceid = @deviceid and isDistributedByEmployeeCode = @distributeByCode",
                    new { userid = staff.id, deviceid = device.id, distributeByCode });
            if (distributions.Count() == 0)
            {
                var distro = new EquipmentDistribution()
                {
                    userid = staff.id,
                    deviceid = device.id,
                    status = "inprogress"
                };

                if (distributeByCode)
                {
                    distro.isDistributedByEmployeeCode = 1;
                    distro.employeeCode = staff.Employee_code;
                }

                SqlMapperExtensions.Insert(conn, distro);
            }
            else
            {
                foreach (var distro in distributions)
                {
                    distro.MarkForDistribution();
                }
                conn.Update(distributions);
            }

        }

        public static void setAddPersonToEquipmentOld(string id)
        {
            try
            {
                //查找出设备
                string commandText_dev = "SELECT id FROM MyDevice";
                string data_dev = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText_dev);
                JArray jo_dev = (JArray)JsonConvert.DeserializeObject(data_dev);

                if (jo_dev.Count > 0)
                {
                    foreach (JObject s in jo_dev)
                    {
                        string userid = id.ToString().Trim();
                        string deviceid = s["id"].ToString().Trim();

                        string commandText = "SELECT COUNT(userid) as len ,type from Equipment_distribution WHERE userid=" + userid + " AND deviceid=" + deviceid;
                        string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);
                        if (!string.IsNullOrEmpty(sr))
                        {
                            JArray srjo = (JArray)JsonConvert.DeserializeObject(sr);
                            string reint = srjo[0]["len"].ToString();
                            if (int.Parse(reint) == 0)
                            {
                                string updatessql = "INSERT INTO Equipment_distribution (type,status,userid, deviceid) VALUES (0,'inprogress'," + userid + "," + deviceid + ")";
                                SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
                            }
                            if (int.Parse(reint) == 1)
                            {
                                string updatessql = "UPDATE Equipment_distribution SET status='',type=0 WHERE userid=" + userid;
                                SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
                            }
                        }
                    }
                }
            }
            catch
            {
            }

        }
        //0 成功  1失败
        public static string setAddPerson(string ip, string name, string imgeurl, string Idcode)
        {
            obj = new JObject();
            obj["result"] = "1";
            obj["data"] = "";

            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(imgeurl))
            {
                return obj.ToString();
            }
            else
            {
                string imgebase64str = ReadImageFile(imgeurl);
                CameraConfigPort CameraConfigPortlist = Deviceinfo.MyDevicelist.Find(d => d.IP == ip);
                if (CameraConfigPortlist.IsConnected)
                {
                    JObject jo = (JObject)JsonConvert.DeserializeObject(UtilsJson.PersonJson32);
                    if (jo != null)
                    {
                        jo["id"] = Idcode.Trim();
                        jo["name"] = name;
                        jo["reg_images"][0]["image_data"] = imgebase64str;
                    }
                    string s = jo.ToString();

                    JObject deleteJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.deleteJson);
                    if (deleteJson != null)
                    {
                        deleteJson["id"] = Idcode.Trim();
                    }
                    //先执行删除操作
                    string ss = GetDevinfo.request(CameraConfigPortlist, deleteJson.ToString());
                    string restr = GetDevinfo.request(CameraConfigPortlist, jo.ToString());
                    JObject restr_json = (JObject)JsonConvert.DeserializeObject(restr.Trim());
                    if (restr_json != null)
                    {
                        string code = restr_json["code"].ToString();
                        int code_int = int.Parse(code);
                        if (code_int == 0)
                        {
                            obj["result"] = 0;
                            obj["data"] = "下发成功";
                        }
                        else if (code_int == 35 || code_int == 36 || code_int == 37 || code_int == 38 || code_int == 39 || code_int == 40 || code_int == 41)
                        {
                            obj["data"] = Strings.StaffImageInValid;
                        }
                    }
                }
            }
            return obj.ToString();
        }

        public static bool UpdateDeviceName(string newname, string ip)
        {
            bool re = false;
            CameraConfigPort CameraConfigPortlist = Deviceinfo.Devicelist.Find(d => d.IP == ip);//new CameraConfigPort(ip);
            if (CameraConfigPortlist == null || !CameraConfigPortlist.IsConnected) return false;
            string result = GetDevinfo.request(CameraConfigPortlist);
            if (!string.IsNullOrEmpty(result))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if (jo != null)
                {
                    jo.Remove("code");
                    jo.Remove("device_sn");
                    jo.Remove("reply");

                    jo["cmd"] = "update app params";
                    jo["device_info"]["addr_name"] = newname.Trim();
                    jo.Add(new JProperty("version", "0.2"));

                    string request_json_str = jo.ToString();
                    string restr = GetDevinfo.request(CameraConfigPortlist, request_json_str);
                    JObject restr_json = (JObject)JsonConvert.DeserializeObject(restr.Trim());
                    if (restr_json != null)
                    {
                        string code = restr_json["code"].ToString();
                        int code_int = int.Parse(code);
                        if (code_int == 0)
                        {
                            re = true;
                            Deviceinfo.Devicelist.ForEach(i => { if (i.IP == ip) { i.DeviceName = newname; } });
                        }
                    }
                }
            }
            return re;
        }

        public static bool Open(string ip)
        {
            bool re = false;
            CameraConfigPort CameraConfigPortlist = Deviceinfo.MyDevicelist.Find(d => d.IP == ip);//new CameraConfigPort(ip);
            if (CameraConfigPortlist == null || !CameraConfigPortlist.IsConnected) return false;
            string result = UtilsJson.openJson;
            string ttspaly = UtilsJson.ttsPlay;

            string restr = GetDevinfo.request(CameraConfigPortlist, result);
            JObject restr_json = (JObject)JsonConvert.DeserializeObject(restr.Trim());
            if (restr_json != null)
            {
                string code = restr_json["code"].ToString();
                int code_int = int.Parse(code);
                if (code_int == 0)
                {
                    GetDevinfo.request(CameraConfigPortlist, ttspaly);
                    re = true;
                }
            }

            return re;
        }
        //getlEmployetypedata

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }
        public static void setAttendance_Data(reData reData, string stagotowork1, string endgotowork1)
        {
            string commandText = "";



            string commandTextdepartmentid = "SELECT COUNT(id) as len ,Punchinformation,Punchinformation1,Remarks FROM  Attendance_Data  att WHERE att.personId='" + reData.personId.Trim() + "' AND att.Date= '" + reData.Date.Replace(@"\", "-").Trim() + "'";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
            if (!string.IsNullOrEmpty(sr))
            {
                Dictionary<string, string> ValueList = new Dictionary<string, string>();
                string isAbsenteeism = "0";
                //if (!string.IsNullOrEmpty(reData.isAbsenteeism))
                /*isAbsenteeism = reData.isAbsenteeism.Trim(); */
                ValueList.Add("isAbsenteeism", isAbsenteeism);
                JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                string reint = jo[0]["len"].ToString();
                string Remarks = jo[0]["Remarks"].ToString();
                string name = "";
                if (!string.IsNullOrEmpty(reData.name))
                    name = reData.name.TrimEnd('\0');

                string department = "";
                if (!string.IsNullOrEmpty(reData.department))
                    department = reData.department.TrimEnd('\0');

                string personId = "";
                if (!string.IsNullOrEmpty(reData.personId))
                    personId = reData.personId.Trim();

                string Employee_code = "";
                if (!string.IsNullOrEmpty(reData.Employee_code))
                { Employee_code = reData.Employee_code.Trim(); ValueList.Add("Employee_code", Employee_code); }

                string Date = "";
                if (!string.IsNullOrEmpty(reData.Date))
                    Date = reData.Date.Trim(); ValueList.Add("Date", Date);

                string Punchinformation = "";
                if (!string.IsNullOrEmpty(reData.Punchinformation))
                { Punchinformation = reData.Punchinformation.Trim(); ValueList.Add("Punchinformation", Punchinformation); }

                string Punchinformation1 = "";
                if (!string.IsNullOrEmpty(reData.Punchinformation1))
                { Punchinformation1 = reData.Punchinformation1.Trim(); ValueList.Add("Punchinformation1", Punchinformation1); }

                string Shiftinformation = "";
                if (!string.IsNullOrEmpty(reData.Shiftinformation))
                { Shiftinformation = reData.Shiftinformation.Trim(); ValueList.Add("Shiftinformation", Shiftinformation); }

                string Duration = "";
                if (!string.IsNullOrEmpty(reData.Duration))
                { Duration = reData.Duration.Trim(); ValueList.Add("Duration", Duration); }

                string late = "";
                //if (!string.IsNullOrEmpty(reData.late))
                //    late = reData.late.Trim(); ValueList.Add("late", late);

                string workOvertime = "";
                if (!string.IsNullOrEmpty(reData.workOvertime))
                { workOvertime = reData.workOvertime.Trim(); ValueList.Add("workOvertime", workOvertime); }

                string Leaveearly = "";
                //if (!string.IsNullOrEmpty(reData.Leaveearly))
                //    Leaveearly = reData.Leaveearly.Trim(); ValueList.Add("Leaveearly", Leaveearly);

                string temperature = "";
                if (!string.IsNullOrEmpty(reData.temperature) && !reData.temperature.Trim().Equals("0"))
                { 
                    temperature = reData.temperature.Trim(); 
                    ValueList.Add("temperature", temperature); 
                }
                else if (!string.IsNullOrEmpty(reData.temperature1) && !reData.temperature1.Trim().Equals("0"))
                {
                    temperature = reData.temperature1.Trim();
                    ValueList.Add("temperature", temperature);
                }

                string IsAcrossNight = "";
                IsAcrossNight = reData.IsAcrossNight.ToString().Trim(); ValueList.Add("IsAcrossNight", IsAcrossNight);

                if (int.Parse(reint) > 0)
                {
                    string Punch = jo[0]["Punchinformation"].ToString();
                    string Punch1 = jo[0]["Punchinformation1"].ToString();
                    var temp = jo[0]["temperature"].ToString();
                    commandText = "UPDATE Attendance_Data SET ";
                    foreach (var li in ValueList)
                    {
                        if (li.Key == "Punchinformation")
                        {
                            if (!string.IsNullOrEmpty(Punch) && !string.IsNullOrEmpty(reData.Punchinformation))
                            {
                                if (int.Parse(Punch.Replace(":", "")) < int.Parse(reData.Punchinformation.Replace(":", "")))
                                    continue;
                            }
                            else if (!string.IsNullOrEmpty(reData.Punchinformation))
                                commandText = commandText + li.Key + "='" + li.Value + "',";
                        }
                        //else if (li.Key == "isAbsenteeism")
                        //{
                        //    if (!string.IsNullOrEmpty(Punch)&& !string.IsNullOrEmpty(reData.Punchinformation1))
                        //    {
                        //        reData.isAbsenteeism = "1";
                        //        commandText = commandText + li.Key + "='1',";
                        //    }
                        //}
                        else if (li.Key == "Punchinformation1")
                        {
                            if (!string.IsNullOrEmpty(Punch1) && !string.IsNullOrEmpty(reData.Punchinformation1))
                            {
                                if (int.Parse(Punch1.Replace(":", "")) > int.Parse(reData.Punchinformation1.Replace(":", "")))
                                    continue;
                                else
                                    commandText = commandText + li.Key + "='" + li.Value + "',";
                            }
                            else if (!string.IsNullOrEmpty(reData.Punchinformation1))
                                commandText = commandText + li.Key + "='" + li.Value + "',";
                        }
                        else if(li.Key == "temperature")
                        {
                            if (string.IsNullOrEmpty(temp))//只有记录第一次温度
                            {
                                commandText = commandText + li.Key + "='" + li.Value + "',";
                            }
                        }
                        else
                        {
                            commandText = commandText + li.Key + "='" + li.Value + "',";
                        }
                    }
                    commandText = commandText + " Todaylate = '0',";
                    commandText = commandText.Substring(0, commandText.Length - 1) +
                        " WHERE personId='" + personId + "' AND Date ='" + Date.Trim() + "'";
                }
                else
                {
                    commandText = @"Insert into Attendance_Data (name,personId,Employee_code,Date,Punchinformation,Punchinformation1,Shiftinformation,Duration,late,workOvertime,Leaveearly,department,temperature,IsAcrossNight,isAbsenteeism,Todaylate) " +
                    "values('" + name.Trim() +
                    "','" + personId.Trim() + "', '"
                    + Employee_code.Trim() + "','"
                    + Date.Replace(@"\", "-").Trim() + "','"
                    + Punchinformation.Trim() + "','"
                    + Punchinformation1.Trim() + "', '"
                    + Shiftinformation.Trim() + "','"
                    + Duration.Trim() + "','"
                    + "','"
                    + workOvertime.Trim() + "','"
                    + "','"
                    + department.Trim() + "','"
                    + temperature.Trim() + "','"
                    + IsAcrossNight.Trim() + "','"
                    + isAbsenteeism.Trim() + "','0')";
                    //暂时采取只增不更新的操作  
                }
                if (string.IsNullOrEmpty(Remarks))
                {
                    int Intresult = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                    if (Intresult > 0)
                    {
                        //再次计算 当天是否已经打了两次卡 然后在写入考勤中
                        string sss = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
                        JArray sssjo = (JArray)JsonConvert.DeserializeObject(sss);

                        string Punch = sssjo[0]["Punchinformation"].ToString();
                        string Punch1 = sssjo[0]["Punchinformation1"].ToString();

                        if (!string.IsNullOrEmpty(Punch) && !string.IsNullOrEmpty(Punch1))
                        {
                            //更新sql操作
                            if (int.Parse(Punch.Replace(":", "")) > int.Parse(stagotowork1))
                            {
                                late = AttendanceAlgorithm.DateDiff(Punch.Replace(":", ""), stagotowork1);
                            }
                            if (int.Parse(Punch1.Replace(":", "")) < int.Parse(endgotowork1))
                            {
                                Leaveearly = AttendanceAlgorithm.DateDiff(Punch1.Replace(":", ""), endgotowork1);
                            }

                            string sql1 = "UPDATE Attendance_Data SET late='" + late + "',Leaveearly= '" + Leaveearly + "',isAbsenteeism= ''" + " WHERE personId='" + personId + "' AND Date ='" + Date.Trim() + "'";
                            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql1);
                        }
                    }
                }
            }
        }

        public static void setAttendance_Data(reData reData, string stagotowork1, string endgotowork1, string stagotowork2, string endgotowork2)
        {
            string commandText = string.Empty;
            string commandTextdepartmentid = "SELECT COUNT(id) as len ,Punchinformation,Punchinformation1,Punchinformation2,Punchinformation22,late,Leaveearly,Remarks FROM  Attendance_Data  att WHERE att.personId='" + reData.personId.Trim() + "' AND att.Date= '" + reData.Date.Replace(@"\", "-").Trim() + "'";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
            if (!string.IsNullOrEmpty(sr))
            {
                Dictionary<string, string> ValueList = new Dictionary<string, string>();
                string isAbsenteeism = "0";
                ValueList.Add("isAbsenteeism", isAbsenteeism);
                JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                string reint = jo[0]["len"].ToString();
                string Remarks = jo[0]["Remarks"].ToString();
                string name = "";
                if (!string.IsNullOrEmpty(reData.name))
                    name = reData.name.TrimEnd('\0');

                string department = "";
                if (!string.IsNullOrEmpty(reData.department))
                    department = reData.department.TrimEnd('\0');

                string personId = "";
                if (!string.IsNullOrEmpty(reData.personId))
                    personId = reData.personId.Trim();

                string Employee_code = "";
                if (!string.IsNullOrEmpty(reData.Employee_code))
                { Employee_code = reData.Employee_code.Trim(); ValueList.Add("Employee_code", Employee_code); }

                string Date = "";
                if (!string.IsNullOrEmpty(reData.Date))
                    Date = reData.Date.Trim(); ValueList.Add("Date", Date);

                string Punchinformation = "";
                if (!string.IsNullOrEmpty(reData.Punchinformation))
                { Punchinformation = reData.Punchinformation.Trim(); ValueList.Add("Punchinformation", Punchinformation); }

                string Punchinformation1 = "";
                if (!string.IsNullOrEmpty(reData.Punchinformation1))
                { Punchinformation1 = reData.Punchinformation1.Trim(); ValueList.Add("Punchinformation1", Punchinformation1); }

                string Punchinformation2 = "";
                if (!string.IsNullOrEmpty(reData.Punchinformation2))
                { Punchinformation2 = reData.Punchinformation2.Trim(); ValueList.Add("Punchinformation2", Punchinformation2); }

                string Punchinformation22 = "";
                if (!string.IsNullOrEmpty(reData.Punchinformation22))
                { Punchinformation22 = reData.Punchinformation22.Trim(); ValueList.Add("Punchinformation22", Punchinformation22); }

                string Shiftinformation = "";
                if (!string.IsNullOrEmpty(reData.Shiftinformation))
                { Shiftinformation = reData.Shiftinformation.Trim(); ValueList.Add("Shiftinformation", Shiftinformation); }

                string Duration = "";
                if (!string.IsNullOrEmpty(reData.Duration))
                { Duration = reData.Duration.Trim(); ValueList.Add("Duration", Duration); }

                string temperature = "";
                if (!string.IsNullOrEmpty(reData.temperature) && !reData.temperature.Trim().Equals("0"))
                { temperature = reData.temperature.Trim(); ValueList.Add("temperature", temperature); }

                string IsAcrossNight = "";
                IsAcrossNight = reData.IsAcrossNight.ToString().Trim(); ValueList.Add("IsAcrossNight", IsAcrossNight);

                if (int.Parse(reint) > 0)
                {
                    string Punch = jo[0]["Punchinformation"].ToString();
                    string Punch1 = jo[0]["Punchinformation1"].ToString();
                    string Punch2 = jo[0]["Punchinformation2"].ToString();
                    string Punch22 = jo[0]["Punchinformation22"].ToString();
                    commandText = "UPDATE Attendance_Data SET ";
                    foreach (var li in ValueList)
                    {
                        if (li.Key == "Punchinformation")
                        {
                            if (!string.IsNullOrEmpty(Punch) && !string.IsNullOrEmpty(reData.Punchinformation))
                            {
                                if (int.Parse(Punch.Replace(":", "")) < int.Parse(reData.Punchinformation.Replace(":", "")))
                                    continue;
                            }
                            else if (!string.IsNullOrEmpty(reData.Punchinformation))
                                commandText = commandText + li.Key + "='" + li.Value + "',";
                        }
                        else if (li.Key == "Punchinformation1")
                        {
                            if (!string.IsNullOrEmpty(Punch1) && !string.IsNullOrEmpty(reData.Punchinformation1))
                            {
                                if (int.Parse(Punch1.Replace(":", "")) > int.Parse(reData.Punchinformation1.Replace(":", "")))
                                    continue;
                                else
                                    commandText = commandText + li.Key + "='" + li.Value + "',";
                            }
                            else if (!string.IsNullOrEmpty(reData.Punchinformation1))
                                commandText = commandText + li.Key + "='" + li.Value + "',";
                        }
                        else if (li.Key == "Punchinformation2")
                        {
                            if (!string.IsNullOrEmpty(Punch2) && !string.IsNullOrEmpty(reData.Punchinformation2))
                            {
                                if (int.Parse(Punch2.Replace(":", "")) < int.Parse(reData.Punchinformation2.Replace(":", "")))
                                    continue;
                            }
                            else if (!string.IsNullOrEmpty(reData.Punchinformation2))
                                commandText = commandText + li.Key + "='" + li.Value + "',";
                        }
                        else if (li.Key == "Punchinformation22")
                        {
                            if (!string.IsNullOrEmpty(Punch22) && !string.IsNullOrEmpty(reData.Punchinformation22))
                            {
                                if (int.Parse(Punch22.Replace(":", "")) > int.Parse(reData.Punchinformation22.Replace(":", "")))
                                    continue;
                                else
                                    commandText = commandText + li.Key + "='" + li.Value + "',";
                            }
                            else if (!string.IsNullOrEmpty(reData.Punchinformation22))
                                commandText = commandText + li.Key + "='" + li.Value + "',";
                        }

                        else
                        {
                            commandText = commandText + li.Key + "='" + li.Value + "',";
                        }
                    }
                    commandText = commandText + " Todaylate = '0',";
                    commandText = commandText.Substring(0, commandText.Length - 1) +
                        " WHERE personId='" + personId + "' AND Date ='" + Date.Trim() + "'";
                }
                else
                {
                    commandText = @"Insert into Attendance_Data (name,personId,Employee_code,Date,Punchinformation,Punchinformation1,
                    Punchinformation2,Punchinformation22,Shiftinformation,Duration,late,Leaveearly,department,temperature,isAbsenteeism2,isAbsenteeism,Todaylate) " +
                    "values('" + name.Trim() +
                    "','" + personId.Trim() + "', '"
                    + Employee_code.Trim() + "','"
                    + Date.Replace(@"\", "-").Trim() + "','"
                    + Punchinformation.Trim() + "','"
                    + Punchinformation1.Trim() + "','"
                    + Punchinformation2.Trim() + "','"
                    + Punchinformation22.Trim() + "','"
                    + Shiftinformation.Trim() + "','"
                    + Duration.Trim() + "','"
                    + "','"
                    + "','"
                    + department.Trim() + "','"
                    + temperature.Trim() + "','"
                    + isAbsenteeism.Trim() + "','"
                    + isAbsenteeism.Trim() + "','0')";
                    //暂时采取只增不更新的操作  
                }
                if (string.IsNullOrEmpty(Remarks))
                {
                    int Intresult = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                    if (Intresult > 0)
                    {
                        //再次计算 当天是否已经打了两次卡 然后在写入考勤中
                        string sss = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
                        JArray sssjo = (JArray)JsonConvert.DeserializeObject(sss);

                        string Punch = sssjo[0]["Punchinformation"].ToString().Trim();
                        string Punch1 = sssjo[0]["Punchinformation1"].ToString().Trim();

                        string Punch2 = sssjo[0]["Punchinformation2"].ToString().Trim();
                        string Punch22 = sssjo[0]["Punchinformation22"].ToString().Trim();

                        if (!string.IsNullOrEmpty(Punch) && !string.IsNullOrEmpty(Punch1) && !string.IsNullOrEmpty(Punch2) && !string.IsNullOrEmpty(Punch22))
                        {
                            string late = "", Leaveearly = "", sql1 = "", workOvertime = "";
                            //更新sql操作
                            if (int.Parse(Punch.Replace(":", "")) > int.Parse(stagotowork1))
                            {
                                late = AttendanceAlgorithm.DateDiff(Punch.Replace(":", ""), stagotowork1);
                            }

                            if (int.Parse(Punch1.Replace(":", "")) < int.Parse(endgotowork1))
                            {
                                Leaveearly = AttendanceAlgorithm.DateDiff(Punch1.Replace(":", ""), endgotowork1);
                            }

                            //计算一时段加班
                            if (int.Parse(Punch1.Replace(":", "")) > int.Parse(endgotowork1.Replace(":", "")))
                            {
                                workOvertime = AttendanceAlgorithm.DateDiff(endgotowork1.Replace(":", ""), Punch1.Replace(":", ""));
                            }

                            sql1 = "UPDATE Attendance_Data SET late='" + late + "',Leaveearly= '" + Leaveearly + "',workOvertime= '" + workOvertime + "',isAbsenteeism= ''" + " WHERE personId='" + personId + "' AND Date ='" + Date.Trim() + "'";


                            if (int.Parse(Punch2.Replace(":", "")) > int.Parse(stagotowork2))
                            {
                                if (!string.IsNullOrEmpty(late))
                                {
                                    late = (int.Parse(AttendanceAlgorithm.DateDiff(Punch2.Replace(":", ""), stagotowork2)) + int.Parse(late)).ToString();
                                }
                                else
                                {
                                    late = AttendanceAlgorithm.DateDiff(Punch2.Replace(":", ""), stagotowork2);
                                }
                            }
                            if (int.Parse(Punch22.Replace(":", "")) < int.Parse(endgotowork2))
                            {
                                if (!string.IsNullOrEmpty(Leaveearly))
                                {
                                    Leaveearly = (int.Parse(AttendanceAlgorithm.DateDiff(Punch22.Replace(":", ""), endgotowork2)) + int.Parse(Leaveearly)).ToString();
                                }
                                else
                                {
                                    Leaveearly = AttendanceAlgorithm.DateDiff(Punch22.Replace(":", ""), endgotowork2);
                                }
                            }

                            //计算二时段加班

                            if (int.Parse(Punch22.Replace(":", "")) > int.Parse(endgotowork2.Replace(":", "")))
                            {
                                if (!string.IsNullOrEmpty(workOvertime))
                                {
                                    workOvertime = (int.Parse(AttendanceAlgorithm.DateDiff(endgotowork2.Replace(":", ""), Punch2.Replace(":", ""))) + int.Parse(workOvertime)).ToString();
                                }
                                else
                                    workOvertime = AttendanceAlgorithm.DateDiff(endgotowork2.Replace(":", ""), Punch2.Replace(":", ""));
                            }

                            sql1 = "UPDATE Attendance_Data SET late='" + late + "',Leaveearly= '" + Leaveearly + "',workOvertime= '" + workOvertime + "',isAbsenteeism= ''" + ",isAbsenteeism2= ''" + " WHERE personId='" + personId + "' AND Date ='" + Date.Trim() + "'";

                            //计算加班
                            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql1);
                        }
                    }
                }
            }
        }
        public static string queydepartmentcode(string depardmentname)
        {
            string commandTextdepartmentid = "SELECT id FROM department WHERE name='" + depardmentname + "'";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
            if (!string.IsNullOrEmpty(sr))
            {
                JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                if (jo.Count > 0)
                {
                    string id = jo[0]["id"].ToString();
                    if (string.IsNullOrEmpty(id))
                        return null;
                    else
                        return id;
                }
                else
                    return null;

            }
            else
                return null;

        }

        public static string queydEmployetypeid(string Employetype)
        {
            string commandTextdepartmentid = "SELECT id FROM Employetype WHERE Employetype_name='" + Employetype + "'";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
            if (!string.IsNullOrEmpty(sr))
            {
                JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                if (jo.Count > 0)
                {
                    string id = jo[0]["id"].ToString();
                    if (string.IsNullOrEmpty(id))
                        return null;
                    else
                        return id;
                }
                else
                    return null;

            }
            else
                return null;

        }

        public static string setStaf(string name, string staff_no, string phone, string email, string department, string Employetype, string imge, string lineType, string line_userid, string face_idcard, string idcardtype, string source,string customer_text,string term_start,string term)
        {

            if (string.IsNullOrEmpty(staff_no))
            {
                return JsonConvert.SerializeObject(new
                {
                    result = 1,
                    data = Properties.Strings.StaffCodeIsEmpty
                });
            }
            else
            {
                using (var con = SQLiteHelper.GetConnection())
                {
                    var codeExists = con.Query<Staff>("SELECT Employee_code from staff where Employee_code = @code", new { code = staff_no })
                        .Any();
                    if (codeExists)
                    {
                        return JsonConvert.SerializeObject(new
                        {
                            result = 1,
                            data = Properties.Strings.StaffCodeExists,
                        });
                    }
                }
            }

            if (!string.IsNullOrEmpty(imge))
            {
                byte[] imgData;
                imgData = copyfile.SaveImage(imge);
                byte[][] re = HaCamera.HA_GetJpgFeatureImageNew(imgData);

                if (re[2][0] != 0)
                {
                    return JsonConvert.SerializeObject(new
                    {
                        result = 1,
                        data = Properties.Strings.StaffImageInValid
                    });
                }
            }

            if (string.IsNullOrEmpty(term_start))
            {
                term_start = "";
            }
            else
            {
                bool startFlag = IsCorrectTimeFormat(term_start);
                term_start = startFlag ? term_start : "";
            }

            if (string.IsNullOrEmpty(term))
            {
                term = "";
            }
            else
            {
                bool endFlag = IsCorrectTimeFormat(term);
                term = endFlag ? term : "";
            }

            AttendanceGroup attGroup = null;
            using (var con = SQLiteHelper.GetConnection())
            {
                attGroup = con.Query<AttendanceGroup>("SELECT id FROM AttendanceGroup WHERE isdefault=1")
                    .FirstOrDefault();
            }

            var staff = new Staff();
            staff.id = staff_no;
            staff.name = name;
            staff.Employee_code = staff_no;
            staff.phone = phone;
            staff.Email = email;
            staff.source = source;
            staff.customer_text = customer_text;
            staff.term_start = term_start;
            staff.term = term;
            if (!string.IsNullOrEmpty(department))
            {
                staff.department_id = int.Parse(department);

            }
            staff.Employetype_id = int.Parse(string.IsNullOrEmpty(Employetype) ? "1" : Employetype);

            staff.idcardtype = idcardtype;
            staff.face_idcard = face_idcard;
            staff.picture = imge;
            if (attGroup != null)
            {
                staff.AttendanceGroup_id = Convert.ToInt32(attGroup.id);
            }
            
            if (Thread.CurrentThread.CurrentUICulture.Name.Contains("ja"))
            {
                staff.line_type = lineType;
                staff.line_userid = line_userid;
            }

            try
            {
                using (var con = SQLiteHelper.GetConnection())
                {
                    var id = SqlMapperExtensions.Insert(con, staff);
                    if (ChromiumForm.userSettings.AutoIssue)//是否自动下发
                        setAddPersonToEquipment(staff.id);
                    return JsonConvert.SerializeObject(new
                    {
                        result = 2,
                        data = Properties.Strings.SaveSuccess
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Save Staff error");
                return JsonConvert.SerializeObject(new
                {
                    result = 1,
                    data = Properties.Strings.SaveFailed
                });
            }

            
        }

        private static bool IsCorrectTimeFormat(string authorizedTime)
        {
            var authorizedTimeFormat = "yyyy-MM-dd HH:mm:ss";
            var formatResult = DateTime.TryParseExact(authorizedTime, authorizedTimeFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _);
            return formatResult;
        }



        //0未传值 1保存失败 2成功
        public static string setStaf_Deprecated(string name, string staff_no, string phone, string email, string department, string Employetype, string imge, string lineType, string line_userid, string face_idcard, string idcardtype)
        {
            if (string.IsNullOrEmpty(Employetype))
            {
                Employetype = "1";
            }
            obj = new JObject();
            string sr = "";

            if (string.IsNullOrEmpty(staff_no))
            {
                obj["result"] = 0;
                obj["data"] = Strings.StaffCodeIsEmpty;
                return obj.ToString();
            }
            else
            {
                if (!string.IsNullOrEmpty(imge))
                {
                    byte[] imgData;
                    imgData = copyfile.SaveImage(imge);
                    byte[][] re = HaCamera.HA_GetJpgFeatureImageNew(imgData);

                    if (re[2][0] != 0)
                    {
                        obj["result"] = 1;
                        obj["data"] = Strings.StaffImageInValid;

                        return obj.ToString();
                    }
                }
                string commandTextdepartmentid = "SELECT COUNT(id) as len FROM staff sta WHERE sta.Employee_code= '" + staff_no + "'";
                sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
                if (!string.IsNullOrEmpty(sr))
                {
                    JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                    string reint = jo[0]["len"].ToString();
                    if (int.Parse(reint) > 0)
                    {
                        obj["result"] = 1;
                        obj["data"] = Strings.StaffCodeExists;

                        return obj.ToString();
                    }
                    else
                    {
                        //获取默认考勤组ID
                        string Groupid = "";
                        string commandTextForGroupid = "SELECT id FROM AttendanceGroup WHERE isdefault=1";
                        string GroupidStr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextForGroupid);
                        if (!string.IsNullOrEmpty(GroupidStr))
                        {
                            JArray GroupidStrToJson = (JArray)JsonConvert.DeserializeObject(GroupidStr);
                            if (GroupidStrToJson.Count > 0)
                            {
                                Groupid = GroupidStrToJson[0]["id"].ToString();
                            }
                        }

                        string publish_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string commandText = "";
                        if (ApplicationData.LanguageSign.Contains("日本語"))
                        {
                            if (!string.IsNullOrEmpty(imge))
                            {
                                commandText = @"Insert into staff (publish_time,name, Employee_code, phone, Email, line_userid,line_type,department_id,Employetype_id,face_idcard,idcardtype,picture,AttendanceGroup_id) " +//idcardtype
                                "values('" + publish_time + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + line_userid + "','" + lineType + "','" + department + "','" + Employetype + "','" + face_idcard + "','" + idcardtype + "','" + imge + "','" + Groupid + "')";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(Groupid))
                                {
                                    commandText = @"Insert into staff (publish_time,name, Employee_code, phone, Email,line_userid,line_type, department_id,Employetype_id,face_idcard,idcardtype,AttendanceGroup_id) " +//"',line_type='" + lineType +
                                "values('" + publish_time + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + line_userid + "','" + lineType + "','" + department + "','" + Employetype + "','" + face_idcard + "','" + idcardtype + "','" + Groupid + "')";
                                }
                                else
                                {
                                    commandText = @"Insert into staff (publish_time,name, Employee_code, phone, Email, line_userid,department_id,Employetype_id,idcardtype,face_idcard) " +
                                "values('" + publish_time + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + line_userid + "','" + lineType + "','" + department + "','" + Employetype + "','" + idcardtype + "','" + face_idcard + "')";
                                }

                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(imge))
                            {
                                if (!string.IsNullOrEmpty(Groupid))
                                {
                                    commandText = @"Insert into staff (publish_time,name, Employee_code, phone, Email, department_id,Employetype_id,face_idcard,idcardtype,picture,AttendanceGroup_id) " +
                                "values('" + publish_time + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + department + "','" + Employetype + "','" + face_idcard + "','" + idcardtype + "','" + imge + "'," + Groupid + ")";
                                }
                                else
                                {
                                    commandText = @"Insert into staff (publish_time,name, Employee_code, phone, Email, department_id,Employetype_id,face_idcard,idcardtype,picture) " +
                                "values('" + publish_time + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + department + "','" + Employetype + "','" + face_idcard + "','" + idcardtype + "','" + imge + "')";
                                }


                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(Groupid))
                                {
                                    commandText = @"Insert into staff (publish_time,name, Employee_code, phone, Email, department_id,Employetype_id,face_idcard,idcardtype,AttendanceGroup_id) " +
                               "values('" + publish_time + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + department + "'," + Employetype + "," + face_idcard + "','" + idcardtype + "','" + Groupid + "')";
                                }
                                else
                                {
                                    commandText = @"Insert into staff (publish_time,name, Employee_code, phone, Email, department_id,Employetype_id,idcardtype,face_idcard) " +
                               "values('" + publish_time + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + department + "'," + Employetype + "," + idcardtype + "','" + face_idcard + "')";
                                }

                            }
                        }


                        int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                        if (re == 1)
                        {
                            obj["result"] = 2;
                            obj["data"] = Strings.SaveSuccess;

                            //自动下发
                            string sql = "SELECT id FROM staff WHERE Employee_code = '" + staff_no + "'";
                            sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, sql);
                            if (!string.IsNullOrEmpty(sr))
                            {
                                JArray restr = (JArray)JsonConvert.DeserializeObject(sr);
                                if (restr.Count > 0)
                                {
                                    string reid = restr[0]["id"].ToString();

                                    setAddPersonToEquipment(reid);
                                }

                            }
                        }
                        else
                        {
                            obj["result"] = 1;
                            obj["data"] = Strings.SaveFailed;
                        }

                    }
                }
                else
                {
                    obj["result"] = 1;
                    obj["data"] = Strings.SaveFailed;
                }

            }
            return obj.ToString();
        }

        public static string setStafforDataSyn(string name, string staff_no, string phone, string email, string department, string Employetype, string imge, string lineType, string line_userid, string face_idcard, string idcardtype, string source)
        {
            if (string.IsNullOrEmpty(Employetype))
            {
                Employetype = "1";
            }
            obj = new JObject();
            string sr = "";

            if (string.IsNullOrEmpty(staff_no))
            {
                obj["result"] = 0;
                obj["data"] = Strings.StaffCodeIsEmpty;
                return obj.ToString();
            }
            else
            {
                string commandTextdepartmentid = "SELECT COUNT(id) as len FROM staff sta WHERE sta.Employee_code= '" + staff_no + "'";
                sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
                if (!string.IsNullOrEmpty(sr))
                {
                    JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                    string reint = jo[0]["len"].ToString();
                    if (int.Parse(reint) > 0)
                    {
                        obj["result"] = 1;
                        obj["data"] = Strings.StaffCodeExists;

                        return obj.ToString();
                    }
                    else
                    {
                        //获取默认考勤组ID
                        string Groupid = "";
                        string commandTextForGroupid = "SELECT id FROM AttendanceGroup WHERE isdefault=1";
                        string GroupidStr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextForGroupid);
                        if (!string.IsNullOrEmpty(GroupidStr))
                        {
                            JArray GroupidStrToJson = (JArray)JsonConvert.DeserializeObject(GroupidStr);
                            if (GroupidStrToJson.Count > 0)
                            {
                                Groupid = GroupidStrToJson[0]["id"].ToString();
                            }
                        }
                        string publish_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string commandText = "";
                        if (ApplicationData.LanguageSign.Contains("日本語"))
                        {
                            if (!string.IsNullOrEmpty(imge))
                            {
                                commandText = @"Insert into staff (publish_time,source,name, Employee_code, phone, Email, line_userid,line_type,department_id,Employetype_id,face_idcard,idcardtype,picture,AttendanceGroup_id) " +//idcardtype
                                "values('" + publish_time + "','" + source + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + line_userid + "','" + lineType + "','" + department + "','" + Employetype + "','" + face_idcard + "','" + idcardtype + "','" + imge + "','" + Groupid + "')";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(Groupid))
                                {
                                    commandText = @"Insert into staff (publish_time,source,name, Employee_code, phone, Email,line_userid,line_type, department_id,Employetype_id,face_idcard,idcardtype,AttendanceGroup_id) " +//"',line_type='" + lineType +
                                "values('" + publish_time + "','" + source + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + line_userid + "','" + lineType + "','" + department + "','" + Employetype + "','" + face_idcard + "','" + idcardtype + "','" + Groupid + "')";
                                }
                                else
                                {
                                    commandText = @"Insert into staff (publish_time,source,name, Employee_code, phone, Email, line_userid,department_id,Employetype_id,idcardtype,face_idcard) " +
                                "values('" + publish_time + "','" + source + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + line_userid + "','" + lineType + "','" + department + "','" + Employetype + "','" + idcardtype + "','" + face_idcard + "')";
                                }

                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(imge))
                            {
                                if (!string.IsNullOrEmpty(Groupid))
                                {
                                    commandText = @"Insert into staff (publish_time,source,name, Employee_code, phone, Email, department_id,Employetype_id,face_idcard,idcardtype,picture,AttendanceGroup_id) " +
                                "values('" + publish_time + "','" + source + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + department + "','" + Employetype + "','" + face_idcard + "','" + idcardtype + "','" + imge + "'," + Groupid + ")";
                                }
                                else
                                {
                                    commandText = @"Insert into staff (publish_time,source,name, Employee_code, phone, Email, department_id,Employetype_id,face_idcard,idcardtype,picture) " +
                                "values('" + publish_time + "','" + source + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + department + "','" + Employetype + "','" + face_idcard + "','" + idcardtype + "','" + imge + "')";
                                }


                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(Groupid))
                                {
                                    commandText = @"Insert into staff (publish_time,source,name, Employee_code, phone, Email, department_id,Employetype_id,face_idcard,idcardtype,AttendanceGroup_id) " +
                               "values('" + publish_time + "','" + source + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + department + "'," + Employetype + "," + face_idcard + "','" + idcardtype + "','" + Groupid + "')";
                                }
                                else
                                {
                                    commandText = @"Insert into staff (publish_time,source,name, Employee_code, phone, Email, department_id,Employetype_id,idcardtype,face_idcard) " +
                               "values('" + publish_time + "','" + source + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + department + "'," + Employetype + "," + idcardtype + "','" + face_idcard + "')";
                                }

                            }
                        }


                        int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                        if (re == 1)
                        {
                            obj["result"] = 2;
                            obj["data"] = Strings.SaveSuccess;

                            //自动下发
                            string sql = "SELECT id FROM staff WHERE Employee_code = '" + staff_no + "'";
                            sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, sql);
                            if (!string.IsNullOrEmpty(sr))
                            {
                                JArray restr = (JArray)JsonConvert.DeserializeObject(sr);
                                if (restr.Count > 0)
                                {
                                    string reid = restr[0]["id"].ToString();

                                    setAddPersonToEquipment(reid);
                                }

                            }
                        }
                        else
                        {
                            obj["result"] = 1;
                            obj["data"] = Strings.SaveFailed;
                        }

                    }
                }
                else
                {
                    obj["result"] = 1;
                    obj["data"] = Strings.SaveFailed;
                }

            }
            return obj.ToString();
        }

        //0未传值 1保存失败 2成功
        public static string setStaf(string id, string name, string staff_no, string phone, string email, string department, string Employetype, string imge, string lineType, string line_userid, string face_idcard, string idcardtype, string source)
        {
            if (string.IsNullOrEmpty(Employetype))
            {
                Employetype = "1";
            }
            obj = new JObject();
            string sr = "";

            if (string.IsNullOrEmpty(staff_no))
            {
                obj["result"] = 0;
                obj["data"] = Strings.StaffCodeIsEmpty;
                return obj.ToString();
            }
            else
            {
                string commandTextdepartmentid = "SELECT COUNT(id) as len FROM staff sta WHERE sta.Employee_code= '" + staff_no + "'";
                sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
                if (!string.IsNullOrEmpty(sr))
                {
                    JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                    string reint = jo[0]["len"].ToString();
                    if (int.Parse(reint) > 0)
                    {
                        obj["result"] = 1;
                        obj["data"] = Strings.StaffCodeExists;

                        return obj.ToString();
                    }
                    else
                    {
                        //获取默认考勤组ID
                        string Groupid = "";
                        string commandTextForGroupid = "SELECT id FROM AttendanceGroup WHERE isdefault=1";
                        string GroupidStr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextForGroupid);
                        if (!string.IsNullOrEmpty(GroupidStr))
                        {
                            JArray GroupidStrToJson = (JArray)JsonConvert.DeserializeObject(GroupidStr);
                            if (GroupidStrToJson.Count > 0)
                            {
                                Groupid = GroupidStrToJson[0]["id"].ToString();
                            }
                        }

                        string publish_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string commandText = "";

                        if (!string.IsNullOrEmpty(imge))
                        {
                            if (!string.IsNullOrEmpty(Groupid))
                            {
                                commandText = @"Insert into staff (id,publish_time,source,name, Employee_code, phone, Email, department_id,Employetype_id,face_idcard,idcardtype,picture,AttendanceGroup_id) " +
                            "values('" + id + "','" + publish_time + "','" + source + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + department + "','" + Employetype + "','" + face_idcard + "','" + idcardtype + "','" + imge + "'," + Groupid + ")";
                            }
                            else
                            {
                                commandText = @"Insert into staff (id,publish_time,source,name, Employee_code, phone, Email, department_id,Employetype_id,face_idcard,idcardtype,picture) " +
                            "values('" + id + "','" + publish_time + "','" + source + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + department + "','" + Employetype + "','" + face_idcard + "','" + idcardtype + "','" + imge + "')";
                            }


                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(Groupid))
                            {
                                commandText = @"Insert into staff (id,publish_time,source,name, Employee_code, phone, Email, department_id,Employetype_id,face_idcard,idcardtype,AttendanceGroup_id) " +
                           "values('" + id + "','" + publish_time + "','" + source + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + department + "'," + Employetype + "," + face_idcard + "','" + idcardtype + "','" + Groupid + "')";
                            }
                            else
                            {
                                commandText = @"Insert into staff (id,publish_time,source,name, Employee_code, phone, Email, department_id,Employetype_id,idcardtype,face_idcard) " +
                           "values('" + id + "','" + publish_time + "','" + source + "','" + name + "', '" + staff_no + "','" + phone + "',' " + email + "', '" + department + "'," + Employetype + "," + idcardtype + "','" + face_idcard + "')";
                            }

                        }


                        int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                        if (re == 1)
                        {
                            obj["result"] = 2;
                            obj["data"] = Strings.SaveSuccess;
                        }
                        else
                        {
                            obj["result"] = 1;
                            obj["data"] = Strings.SaveFailed;
                        }

                    }
                }
                else
                {
                    obj["result"] = 1;
                    obj["data"] = Strings.SaveFailed;
                }

            }
            return obj.ToString();
        }

        public static void setStaf(string id, string name, string imge, string face_idcard, string source)
        {
            string staff_no = GetTimeStamp().Trim();
            string idcardtype = "";
            if (!string.IsNullOrEmpty(face_idcard))
            {
                if (Int64.Parse(face_idcard) > 4294967296)
                {
                    idcardtype = "64";
                }
                else
                    idcardtype = "32";
            }
            //获取默认考勤组ID
            string Groupid = "";
            string commandTextForGroupid = "SELECT id FROM AttendanceGroup WHERE isdefault=1";
            string GroupidStr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextForGroupid);
            if (!string.IsNullOrEmpty(GroupidStr))
            {
                JArray GroupidStrToJson = (JArray)JsonConvert.DeserializeObject(GroupidStr);
                if (GroupidStrToJson.Count > 0)
                {
                    Groupid = GroupidStrToJson[0]["id"].ToString();
                }
            }
            string commandText = "";
            string publish_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (!string.IsNullOrEmpty(imge))
            {
                if (!string.IsNullOrEmpty(Groupid))
                {
                    commandText = @"Insert into staff (id,Employetype_id,department_id,publish_time,source,name,  Employee_code,face_idcard,idcardtype,picture,AttendanceGroup_id) " +
                "values('" + id + "','1','1','" + publish_time + "','" + source + "','" + name + "', '" + staff_no + "','" + face_idcard + "','" + idcardtype + "','" + imge + "'," + Groupid + ")";
                }
                else
                {
                    commandText = @"Insert into staff (id,Employetype_id,department_id,publish_time,source,name, Employee_code,face_idcard,idcardtype, picture) " +
                "values('" + id + "','1','1','" + publish_time + "','" + source + "','" + name + "', '" + staff_no + "','" + face_idcard + "','" + idcardtype + "','" + imge + "')";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Groupid))
                {
                    commandText = @"Insert into staff (id,Employetype_id,department_id,publish_time,source,name, Employee_code,face_idcard,idcardtype,AttendanceGroup_id) " +
               "values('" + id + "','1','1','" + publish_time + "','" + source + "','" + name + "', '" + staff_no + "','" + face_idcard + "','" + idcardtype + "','" + Groupid + "')";
                }
                else
                {
                    commandText = @"Insert into staff (id,Employetype_id,department_id,publish_time,source,name, Employee_code,face_idcard,idcardtype) " +
               "values('" + id + "','1','1','" + publish_time + "','" + source + "','" + name + "', '" + staff_no + "','" + face_idcard + "','" + idcardtype + "','" + "')";
                }

            }
            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
        }

        //0未传值 1保存失败 2成功
        public static string setVisitor(string name, string phone, string imge, string staTime, string endTime)
        {
            obj = new JObject();
            if (!string.IsNullOrEmpty(imge))
            {
                byte[] imgData;
                imgData = copyfile.SaveImage(imge);
                //判断图片大小
                //Bitmap bitmap= IsQualified(PicFileName,600,800);
                //if (bitmap!=null)
                //{
                //    imgData = SaveImage(bitmap);
                //}
                //else
                //{
                //    imgData = SaveImage(PicFileName);
                //}

                //判断是否合格
                //HaCamera.InitEnvironment();


                byte[][] re = HaCamera.HA_GetJpgFeatureImageNew(imgData);

                if (re[2][0] != 0)
                {
                    obj["result"] = 1;
                    obj["data"] = Strings.StaffImageInValid;

                    return obj.ToString();
                }
            }
            string commandText = @"Insert into Visitor (id,name,phone, imge, staTime, endTime,isDown) " +
                "values('" + GetTimeStamp().Trim() + "','" + name.Trim() + "','" + phone.Trim() + "', '" + imge.Trim() + "','" + staTime.Trim() + "','" + endTime.Trim() + "','0')"; ;
            int rejosn = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
            if (rejosn == 1)
            {
                obj["result"] = 2;
                obj["data"] = Strings.SaveSuccess;
            }
            else
            {
                obj["result"] = 1;
                obj["data"] = Strings.SaveFailed;
            }

            return obj.ToString();
        }

        public static string editVisitor(string name, string phone, string imge, string staTime, string endTime, string id)
        {
            obj = new JObject();
            if (!string.IsNullOrEmpty(imge))
            {
                byte[] imgData;
                imgData = copyfile.SaveImage(imge);
                //判断图片大小
                //Bitmap bitmap= IsQualified(PicFileName,600,800);
                //if (bitmap!=null)
                //{
                //    imgData = SaveImage(bitmap);
                //}
                //else
                //{
                //    imgData = SaveImage(PicFileName);
                //}

                //判断是否合格
                //HaCamera.InitEnvironment();


                byte[][] re = HaCamera.HA_GetJpgFeatureImageNew(imgData);

                if (re[2][0] != 0)
                {
                    obj["result"] = 1;
                    obj["data"] = Strings.StaffImageInValid;

                    return obj.ToString();
                }
            }

            string commandText = @"UPDATE Visitor SET phone='" + phone + "',name='" + name + "', staTime='" + staTime + "', endTime='" + endTime + "', imge='" + imge + "'" + "  WHERE id=" + id + "";

            int reint = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
            if (reint == 1)
            {
                obj["result"] = 2;
                obj["data"] = Strings.SaveSuccess;
            }
            else
            {
                obj["result"] = 1;
                obj["data"] = Strings.SaveFailed;
            }

            return obj.ToString();
        }

        public static string setlineQRcode(string imge)
        {
            obj = new JObject();

            if (string.IsNullOrEmpty(imge))
            {
                obj["result"] = 1;
                obj["data"] = Strings.SaveFailed;

                return obj.ToString();
            }
            string commandText = "UPDATE Linefor_ SET lineRQcode='" + imge + "'";

            string lineRQcode = string.Empty;
            string sql = "SELECT lineRQcode FROM Linefor_";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, sql);
            if (!string.IsNullOrEmpty(sr))
            {
                JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                if (jo.Count > 0)
                {
                    lineRQcode = jo[0]["lineRQcode"].ToString();
                }
            }

            int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
            if (re == 1)
            {
                obj["result"] = 2;
                obj["data"] = Strings.SaveSuccess;

                if (!string.IsNullOrEmpty(lineRQcode))
                {
                    DeleteFile(lineRQcode);
                }
            }
            else
            {
                obj["result"] = 1;
                obj["data"] = Strings.SaveFailed;
            }
            return obj.ToString();
        }


        public static string setlineQRcodeEmail(string imge)
        {
            obj = new JObject();

            if (string.IsNullOrEmpty(imge))
            {
                obj["result"] = 1;
                obj["data"] = Strings.SaveFailed;

                return obj.ToString();
            }
            string commandText = "UPDATE Linefor_ SET lineRQcodeEmail='" + imge + "'";

            string lineRQcode = string.Empty;
            string sql = "SELECT lineRQcodeEmail FROM Linefor_";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, sql);
            if (!string.IsNullOrEmpty(sr))
            {
                JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                if (jo.Count > 0)
                {
                    lineRQcode = jo[0]["lineRQcodeEmail"].ToString();
                }
            }

            int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
            if (re == 1)
            {
                obj["result"] = 2;
                obj["data"] = Strings.SaveSuccess;

                if (!string.IsNullOrEmpty(lineRQcode))
                {
                    DeleteFile(lineRQcode);
                }
            }
            else
            {
                obj["result"] = 1;
                obj["data"] = Strings.SaveFailed;
            }
            return obj.ToString();
        }

        public static string getlineQRcode()
        {
            string commandText = "SELECT IFNULL(lineRQcode,'') as lineRQcode FROM Linefor_  ";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getlineEmailQRcode()
        {
            string commandText = "SELECT IFNULL(lineRQcodeEmail,'') as lineRQcodeEmail FROM Linefor_  ";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getlineQRcodeEmail()
        {
            string commandText = "SELECT IFNULL(lineRQcodeEmail,'') as lineRQcodeEmail FROM Linefor_  ";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getwg_card_id(string userid)
        {
            string commandText = "SELECT face_idcard FROM staff WHERE id='" + userid + "'";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getindexforAttendanceToday()
        {
            string tody = DateTime.Now.ToString("yyyy-MM-dd");
            string commandText = "SELECT COUNT(*) AS count  FROM Attendance_Data WHERE Date ='" + tody + "' AND Punchinformation!=''";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }
        public static string getindexforlate()
        {
            string tody = DateTime.Now.ToString("yyyy-MM-dd");
            string commandText = "SELECT COUNT(*) AS count  FROM Attendance_Data WHERE Date ='" + tody + "' AND late!=''";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }
        public static string getindexforLeaveEarly()
        {
            string tody = DateTime.Now.ToString("yyyy-MM-dd");
            string commandText = "SELECT COUNT(*) AS count  FROM Attendance_Data WHERE Date ='" + tody + "' AND Leaveearly!=''";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }
        public static string getindexforleave()
        {
            string tody = DateTime.Now.ToString("yyyy-MM-dd");

            string commandText = "SELECT COUNT(*) AS count  FROM Attendance_Data WHERE Date ='" + tody + "' AND Remarks ==3";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getindexforNumberRegist()
        {
            string commandText = "SELECT COUNT(*) AS count  FROM staff";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }
        public static string getindexforNumberequipment()
        {
            string re = Deviceinfo.MyDevicelist.Count(t => t.IsConnected == true).ToString();
            return re;
        }


        public static string getindexforNumberequipments()
        {
            string commandText = "SELECT COUNT(*) AS count  FROM MyDevice";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string EditLine(string temperature, string Message, string Message2, string Message3, string Message4
            , string Message5
            , string Message6
            , string Message7, string Message8, string Message9, string Message10, string Message11, string Message12, string line_url
            , string ftpserver, string ftppassword, string ftpusername)
        {
            obj = new JObject();

            if (string.IsNullOrEmpty(Message2) && string.IsNullOrEmpty(Message)
                && string.IsNullOrEmpty(Message3)
                && string.IsNullOrEmpty(Message4)
                && string.IsNullOrEmpty(Message5)
                && string.IsNullOrEmpty(Message6)
                && string.IsNullOrEmpty(Message7)
                && string.IsNullOrEmpty(Message8)
                && string.IsNullOrEmpty(Message9)
                && string.IsNullOrEmpty(Message10)
                && string.IsNullOrEmpty(Message11)
                && string.IsNullOrEmpty(Message12) && string.IsNullOrEmpty(line_url))
            {
                obj["result"] = 0;
                obj["data"] = "参数不能为空";
                return obj.ToString();
            }
            else
            {
                string commandText = @"UPDATE Linefor_ set Message2='" + Message2.Trim() + "' " +
                    ",temperature='" + temperature.Trim() + "' " +
                    ",Message='" + Message.Trim() + "' " +
                    ",Message3='" + Message3.Trim() + "' " +
                    ",Message4='" + Message4.Trim() + "' " +
                    ",Message5='" + Message5.Trim() + "' " +
                    ",Message6='" + Message6.Trim() + "' " +
                    ",Message7='" + Message7.Trim() + "' " +
                    ",Message8='" + Message8.Trim() + "' " +
                    ",Message9='" + Message9.Trim() + "' " +
                    ",Message10='" + Message10.Trim() + "' " +
                    ",Message11='" + Message11.Trim() + "' " +
                    ",line_url='" + line_url.Trim() + "' " +
                    ",Message12='" + Message12.Trim() + "' " +

                    ",ftpserver='" + ftpserver.Trim() + "' " +
                    ",ftppassword='" + ftppassword.Trim() + "' " +
                    ",ftpusername='" + ftpusername.Trim() + "' ";

                int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                if (re == 1)
                {
                    obj["result"] = 2;
                    obj["data"] = Strings.SaveSuccess;

                    //更新本地的message和userid参数
                    ApplicationData.lineMessage = Message.Trim();
                    ApplicationData.lineMessage2 = Message2.Trim();
                    ApplicationData.lineMessage3 = Message3.Trim();
                    ApplicationData.lineMessage4 = Message4.Trim();
                    ApplicationData.lineMessage5 = Message5.Trim();
                    ApplicationData.lineMessage6 = Message6.Trim();
                    ApplicationData.lineMessage9 = Message9.Trim();
                    ApplicationData.lineMessage7 = Message7.Trim();
                    ApplicationData.lineMessage8 = Message8.Trim();
                    ApplicationData.lineMessage10 = Message10.Trim();
                    ApplicationData.lineMessage11 = Message11.Trim();
                    ApplicationData.lineMessage12 = Message12.Trim();

                    ApplicationData.temperature = temperature.Trim();
                    ApplicationData.ftpserver = ftpserver.Trim();
                    ApplicationData.ftppassword = ftppassword.Trim();
                    ApplicationData.ftpusername = ftpusername.Trim();
                    ApplicationData.line_url = line_url.Trim();
                }
                else
                {
                    obj["result"] = 1;
                    obj["data"] = Strings.SaveFailed;
                }

            }
            return obj.ToString();
        }


        public static string EdPdfconfiguration(string pdftitle, string rows1, string rows2, string rows3, string rows4
            , string rows5
            , string rows6
            , string rows7, string rows8, string rows9, string rows10, string rows11, string rows12)
        {
            obj = new JObject();

            if (string.IsNullOrEmpty(pdftitle))
            {
                obj["result"] = 0;
                obj["data"] = "パラメータが空ではありません";
                return obj.ToString();
            }
            else
            {
                string commandText = string.Empty;
                string sql = "SELECT COUNT(*) as len FROM Pdfconfiguration ";
                string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, sql);
                if (!string.IsNullOrEmpty(sr))
                {
                    JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                    string reint = jo[0]["len"].ToString();
                    if (int.Parse(reint) > 0)
                    {
                        commandText = @"UPDATE Pdfconfiguration set rows2='" + rows2.Trim() + "' " +
                    ",pdftitle='" + pdftitle.Trim() + "' " +
                    ",rows1='" + rows1.Trim() + "' " +
                    ",rows3='" + rows3.Trim() + "' " +
                    ",rows4='" + rows4.Trim() + "' " +
                    ",rows5='" + rows5.Trim() + "' " +
                    ",rows6='" + rows6.Trim() + "' " +
                    ",rows7='" + rows7.Trim() + "' " +
                    ",rows8='" + rows8.Trim() + "' " +
                    ",rows9='" + rows9.Trim() + "' " +
                    ",rows10='" + rows10.Trim() + "' " +
                    ",rows11='" + rows11.Trim() + "' " +
                    ",rows12='" + rows12.Trim() + "' ";
                    }
                    else
                    {
                        commandText = @"INSERT INTO Pdfconfiguration 
(pdftitle, rows1,rows2,rows3,rows4,rows5,rows6,rows7,rows8,rows9,rows10,rows11,rows12) VALUES 
('" + pdftitle.Trim() + "', '" + rows1.Trim() + "','" + rows2.Trim() + "','" + rows3.Trim() + "','" + rows4.Trim() + "','" + rows5.Trim()
+ "','" + rows6.Trim() + "','" + rows7.Trim() + "','" + rows8.Trim() + "','" + rows9.Trim()
+ "','" + rows10.Trim() + "','" + rows11.Trim() + "','" + rows12.Trim() + "')";
                    }
                }

                int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                if (re == 1)
                {
                    obj["result"] = 2;
                    obj["data"] = Strings.SaveSuccess;

                    //更新本地的message和userid参数
                    ApplicationData.pdftitle = pdftitle.Trim();
                    ApplicationData.rows1 = rows1.Trim();
                    ApplicationData.rows2 = rows2.Trim();
                    ApplicationData.rows4 = rows4.Trim();
                    ApplicationData.rows5 = rows5.Trim();
                    ApplicationData.rows6 = rows6.Trim();
                    ApplicationData.rows9 = rows9.Trim();
                    ApplicationData.rows7 = rows7.Trim();
                    ApplicationData.rows8 = rows8.Trim();
                    ApplicationData.rows10 = rows10.Trim();
                    ApplicationData.rows11 = rows11.Trim();
                    ApplicationData.rows3 = rows3.Trim();
                    ApplicationData.rows12 = rows12.Trim();
                }
                else
                {
                    obj["result"] = 1;
                    obj["data"] = Strings.SaveFailed;
                }

            }
            return obj.ToString();
        }
        public static string eidStaf(string name, string staff_no, string phone, string email, string department, string Employetype, string imge, string line_userid, string lineType, string id, string face_idcard, string idcardtype, string customer_text, string term_start, string term)
        {
            if (string.IsNullOrEmpty(Employetype))
            {
                Employetype = "1";
            }
            byte[] imageBytes = null;
            obj = new JObject();
            string sr = "";

            if (string.IsNullOrEmpty(staff_no))
            {
                obj["result"] = 0;
                obj["data"] = Strings.StaffCodeIsEmpty;
                return obj.ToString();
            }
            else
            {
                string commandTextdepartmentid = "SELECT COUNT(id) as len ,ifnull(sta.id,0) as id FROM staff sta WHERE sta.Employee_code= '" + staff_no + "'";
                sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
                if (!string.IsNullOrEmpty(sr))
                {
                    JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                    string reint = jo[0]["len"].ToString();
                    string reid = jo[0]["id"].ToString();
                    if (int.Parse(reint) > 0 && string.Compare(id, reid, true) != 0)
                    {
                        obj["result"] = 1;
                        obj["data"] = Strings.StaffCodeExists;
                        return obj.ToString();
                    }
                    else
                    {
                        string publish_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string commandText = "";

                        if (ApplicationData.LanguageSign.Contains("日本語"))
                        {
                            if (!string.IsNullOrEmpty(imge))
                            {
                                commandText = @"UPDATE staff SET publish_time='" + publish_time + "',name='" + name + "', Employee_code='" + staff_no + "', phone='" + phone + "', Email='" + email + "',line_userid='" + line_userid + "',line_type='" + lineType + "', department_id='" + department + "',Employetype_id='" + Employetype + "',face_idcard='" + face_idcard + "',idcardtype='" + idcardtype + "', picture='" + imge + "' WHERE id=" + id + "";
                            }
                            else
                            {
                                commandText = @"UPDATE staff SET publish_time='" + publish_time + "',name='" + name + "', Employee_code='" + staff_no + "', phone='" + phone + "', Email='" + email + "',line_userid='" + line_userid + "',line_type='" + lineType + "', department_id='" + department + "',Employetype_id='" + Employetype + "',face_idcard='" + face_idcard + "',idcardtype='" + idcardtype + "'  WHERE id=" + id + "";
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(imge))
                            {
                                commandText = @"UPDATE staff SET publish_time='" + publish_time + "',name='" + name + "', Employee_code='" + staff_no + "', phone='" + phone + "', Email='" + email + "', department_id='" + department + "',Employetype_id='" + Employetype + "',face_idcard='" + face_idcard + "',idcardtype='" + idcardtype + "', picture='" + imge + "',customer_text="+$"'{customer_text}'"+",term_start="+$"'{term_start}'"+",term="+$"'{term}'"+" WHERE id=" + $"'{id}'";
                            }
                            else
                            {
                                commandText = @"UPDATE staff SET publish_time='" + publish_time + "',name='" + name + "', Employee_code='" + staff_no + "', phone='" + phone + "', Email='" + email + "', department_id='" + department + "',Employetype_id='" + Employetype + "',face_idcard='" + face_idcard + "',idcardtype='" + idcardtype + "',customer_text=" + $"'{customer_text}'" + ",term_start=" + $"'{term_start}'" + ",term=" + $"'{term}'" + "  WHERE id=" + $"'{id}'";
                            }
                        }

                        int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                        if (re == 1)
                        {
                            obj["result"] = 2;
                            obj["data"] = Strings.SaveSuccess;

                            //修改成功后 修改考勤表中的名字
                            string sql1 = $"UPDATE Attendance_Data set name = '{name}' WHERE personId = '{id}' ";
                            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql1);

                            //自动下发
                            string sql = "SELECT id FROM staff WHERE Employee_code = '" + staff_no + "'";
                            sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, sql);
                            if (!string.IsNullOrEmpty(sr))
                            {
                                JArray restr = (JArray)JsonConvert.DeserializeObject(sr);
                                if (restr.Count > 0)
                                {
                                    string staffid = restr[0]["id"].ToString();

                                    setAddPersonToEquipment(staffid);
                                }

                            }
                        }
                        else
                        {
                            obj["result"] = 1;
                            obj["data"] = Strings.SaveFailed;
                        }

                    }
                }
                else
                {
                    obj["result"] = 1;
                    obj["data"] = Strings.SaveFailed;
                }

            }
            return obj.ToString();
        }

        //0未传值 1保存失败 2成功
        public static string setGroup(string attribute, string name, string isdefault, string ids, string timestamp)
        {
            obj = new JObject();
            string sr = "";

            if (string.IsNullOrEmpty(name))
            {
                obj["result"] = 0;
                obj["data"] = Properties.Strings.AttendanceNameCanBeNotNull;
                return obj.ToString();
            }
            else
            {
                if (isdefault.Trim() == "1")
                {
                    SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, "UPDATE AttendanceGroup SET isdefault=0");
                }
                string publish_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string commandText = @"Insert into AttendanceGroup(name,attribute,isdefault,publishtime) VALUES ('" + name + "','" + attribute + "','" + isdefault + "','" + publish_time + "')";
                int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                if (re == 1)
                {
                    obj["result"] = 2;
                    obj["data"] = Strings.SaveSuccess;

                    //将绑定的员工的AttendanceGroup_id添加进去
                    string commandTextdepartmentid = "SELECT id FROM AttendanceGroup  WHERE publishtime='" + publish_time + "'";
                    sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
                    if (!string.IsNullOrEmpty(sr))
                    {
                        JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                        string id = jo[0]["id"].ToString();
                        //封装sql
                        string arrayids = ids.Replace("{", "").Replace("}", "");
                        string[] s = arrayids.Split(',');
                        for (int i = 0; i < s.Length; i++)
                        {
                            string sql = "UPDATE staff SET AttendanceGroup_id='" + id + "' WHERE id='" + s[i].Trim() + "'";
                            int ret = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);
                        }

                        //将必须打卡日期和不必打卡日期 进行绑定
                        if (!timestamp.Trim().Equals("0"))
                        {
                            string sql = "UPDATE Special_date SET AttendanceGroupid=" + id + " WHERE AttendanceGroupid=" + timestamp.Trim();
                            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);
                        }
                    }



                }
                else
                {
                    obj["result"] = 1;
                    obj["data"] = Strings.SaveFailed;
                }
            }
            return obj.ToString();
        }

        public static string editGroup(string attribute, string name, string isdefault, string ids, string id)
        {
            var obj = new JObject();

            if (string.IsNullOrEmpty(name))
            {
                obj["result"] = 0;
                obj["data"] = Properties.Strings.AttendanceNameCanBeNotNull;
                return obj.ToString();
            }
            else
            {

                string publish_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if (isdefault.Trim() == "1")
                {
                    SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, "UPDATE AttendanceGroup SET isdefault=0");
                }
                string commandText = $@"UPDATE AttendanceGroup SET attribute='{attribute}',name='{name}',isdefault='{isdefault}',publishtime='{publish_time}' WHERE id='{id}'";
                int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                if (re == 1)
                {
                    obj["result"] = 2;
                    obj["data"] = Strings.SaveSuccess;
                }
                else
                {
                    obj["result"] = 1;
                    obj["data"] = Strings.SaveFailed;
                }


                if (ids.Trim().Length > 0)
                {
                    var userIds = ids.Split(',');
                    foreach (var userId in userIds)
                    {
                        string sql = $"UPDATE staff SET AttendanceGroup_id='{id}' WHERE id='{userId}'";
                        int ret = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);
                    }
                }

                obj["result"] = 2;
                obj["data"] = Properties.Strings.SaveSuccess;
            }
            return obj.ToString();
        }

        public static bool DeleteGroup(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            try
            {

                string commandText = "delete FROM AttendanceGroup WHERE id= " + id;
                int sr = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);

                if (sr == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool set_default(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            try
            {


                //先全部置为非默认
                int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, "UPDATE AttendanceGroup SET isdefault=0");
                if (re != 0)
                {
                    string commandText = "UPDATE AttendanceGroup SET isdefault=1 WHERE id=" + id;
                    int sr = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);

                    if (sr == 1)
                        return true;
                    else
                        return false;
                }
                else
                    return false;

            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 获取数据库考勤信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string getCapture_Data(string statime, string endtime)
        {

            string commandText = "SELECT sta.Employee_code,IFNULL(ca.closeup,'') as closeup ,ca.time as captureTime,ca.match_status as matchScore,ca.person_name as personName,ca.body_temp as temperature,ca.person_id as personId FROM Capture_Data ca LEFT JOIN staff sta  WHERE (sta.id = ca.person_id or sta.Employee_code=ca.person_id) and ca.time<='" + endtime.Trim() + "' AND ca.time>'" + statime + "' AND ca.person_id !=0";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getCapture_Data()
        {

            string commandText = "SELECT time FROM Capture_Data WHERE time = (SELECT MAX(time) FROM Capture_Data);";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getEmail(string id)
        {

            string commandText = "SELECT IFNULL(Email,'') as Email FROM staff  WHERE id=" + id;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string gettemperature()
        {

            string commandText = "SELECT IFNULL(temperature,'') as temperature FROM Linefor_ ";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }
        public static string getstaffforlineAdminData()
        {
            string commandText = "SELECT name as title,id as value FROM staff WHERE (line_type= '2' or line_type='3') AND (Email is not null and Email <> '') ";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getstaffforlineAdmin()
        {
            string commandText = "SELECT name as title,id as value FROM staff WHERE (line_type= '2' or line_type='3') AND (Email is not null and Email <> '')  AND islineAdmin==1";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getstaffforlineAdminEmail()
        {
            string commandText = "SELECT Email FROM staff WHERE (line_type= '2' or line_type='3') AND (Email is not null and Email <> '')  AND islineAdmin==1";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static bool setstaffforlineAdmin(string ids)
        {
            bool result = false;


            string sql1 = "UPDATE staff SET islineAdmin  = '0'";
            int re1 = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql1);

            if (!string.IsNullOrEmpty(ids))
            {
                string sql = "UPDATE staff SET islineAdmin  = '1' WHERE id IN(" + ids + ")";
                int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);
                if (re > 0)
                {
                    result = true;
                    //再次将管理员写到本地静态数据
                    HandleCaptureData.getstaffforlineAdminEmail();
                }
            }
            else
            {
                result = true;
                //再次将管理员写到本地静态数据
                HandleCaptureData.getstaffforlineAdminEmail();
            }


            return result;
        }

        public static string getCapture_Datacuont(string statime, string endtime, string name, string devname, string selectedPersonTypes, string HealthCodeType, float? tempFrom, float? tempTo)
        {

            StringBuilder commandText = new StringBuilder("SELECT COUNT(*) as count FROM Capture_Data  WHERE 1=1 AND");
            if (!string.IsNullOrEmpty(statime) && !string.IsNullOrEmpty(endtime))
            {
                commandText.Append(" '" + statime + "' < time AND  time < '" + endtime + "' AND");
            }
            if (!string.IsNullOrEmpty(name))
            {
                commandText.Append(" person_name LIKE '%" + name.Trim() + "%' AND");
            }
            if (!string.IsNullOrEmpty(devname))
            {
                commandText.Append(" device_sn='" + devname + "' AND");
            }
            if (HealthCodeType != "0")
            {
                if (HealthCodeType == "1")
                {
                    commandText.Append(" QRcodestatus LIKE '%绿码%' AND");
                }
                else if (HealthCodeType == "2")
                {
                    commandText.Append(" QRcodestatus LIKE '%黄码%' AND");
                }
                else if (HealthCodeType == "3")
                {
                    commandText.Append(" QRcodestatus LIKE '%红码%' AND");
                }
            }
            if (!string.IsNullOrEmpty(selectedPersonTypes))
            {
                var sb = new StringBuilder(" ");
                var sections = selectedPersonTypes.Split(',')
                    .Select(x => x == "1" ? " person_id is not null" : " person_id is null");
                var s = String.Join(" or ", sections);

                sb.AppendFormat(" ( {0} ) AND", s);

                commandText.Append(sb.ToString());

            }

            if (tempFrom != null)
            {
                if (!ChromiumForm.userSettings.ShowTemperatureInCelsius)
                {
                    tempFrom = tempFrom.Value.toCelsius();
                }
                commandText.Append($" body_temp >= {tempFrom.Value.ToString(CultureInfo.InvariantCulture)} AND");
            }

            if (tempTo != null)
            {
                if (!ChromiumForm.userSettings.ShowTemperatureInCelsius)
                {
                    tempFrom = tempFrom.Value.toCelsius();
                }
                commandText.Append($" body_temp <= {tempTo.Value.ToString(CultureInfo.InvariantCulture)} AND");
            }

            string commandText2 = commandText.ToString().Substring(0, commandText.ToString().Length - 3).ToString();

            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText2);

            return sr;
        }

        public static string getVisitorcuont(string statime, string statime1, string endtime, string endtime2, string name, string phone, string isDown)
        {

            StringBuilder commandText = new StringBuilder("SELECT COUNT(*) as count FROM Visitor  WHERE 1=1 AND");
            if (!string.IsNullOrEmpty(statime) && !string.IsNullOrEmpty(endtime))
            {
                commandText.Append(" '" + statime + "' < staTime AND  staTime < '" + statime1 + "' AND");
            }
            if (!string.IsNullOrEmpty(endtime) && !string.IsNullOrEmpty(endtime2))
            {
                commandText.Append(" '" + endtime + "' < endTime AND  endTime < '" + endtime2 + "' AND");
            }
            if (!string.IsNullOrEmpty(name))
            {
                commandText.Append(" name LIKE '%" + name.Trim() + "%' AND");
            }
            if (!string.IsNullOrEmpty(phone))
            {
                commandText.Append(" phone='" + phone + "' AND");
            }
            if (!string.IsNullOrEmpty(isDown))
            {
                if (isDown.Trim() != "2")
                {
                    commandText.Append(" isDown='" + isDown.Trim() + "' AND");
                }
            }

            string commandText2 = commandText.ToString().Substring(0, commandText.ToString().Length - 3).ToString();

            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText2);

            return sr;
        }

        /// <summary>
        /// 写入最后一次写入考勤信息表的时间
        /// </summary>
        /// <param name="time"></param>
        public static void setMyDeviceforLast_query(string time, string ip)
        {

            string commandText = "UPDATE MyDevice SET Last_query='" + time.Trim() + "' WHERE ipAddress='" + ip.Trim() + "'";
            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
        }
        /// <summary>
        /// 获取最后一次写入考勤信息表的时间
        /// </summary>
        /// <param name="time"></param>
        public static string getMyDeviceforLast_query(string ip)
        {

            string commandText = "select Last_query FROM MyDevice WHERE ipAddress='" + ip.Trim() + "'";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);
            return sr;
        }
        public static bool DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            try
            {
                try
                {
                    Staff staff = null;
                    using (var conn = SQLiteHelper.GetConnection())
                    {
                        staff = conn.Get<Staff>(id);
                        conn.Execute($"DELETE FROM RuleDistributionItem WHERE StaffId = '{id}'");
                    }
                    DeleteFile(staff?.picture);

                }
                catch (IOException)
                {
                }

                //先删除所有设备上的人员
                //JObject deleteJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.deleteJson);
                //if (deleteJson != null)
                //{
                //    deleteJson["id"] = id;
                //}
                //List<CameraConfigPort> Devicelistdata = Deviceinfo.MyDevicelist;
                //Devicelistdata.ForEach(s => {
                //    if(s.IsConnected)
                //        GetDevinfo.request(s, deleteJson.ToString());
                //});
                using (var conn = SQLiteHelper.GetConnection())
                {
                    conn.Execute($"UPDATE Equipment_distribution SET type=1,status='' WHERE userid='{id}'");
                    conn.Delete<Staff>(new Staff { id = id });
                }
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string getCapture_Data(string statime, string endtime, string name, string devname, string selectedPersonTypes, string HealthCodeType, float? tempFrom, float? tempTo, string page, string limt)
        {
            int page1 = int.Parse(page) - 1;
            int pageint = page1 * int.Parse(limt);
            StringBuilder commandText = new StringBuilder("SELECT ca.* ,sta.picture as TemplateImage FROM Capture_Data ca LEFT JOIN staff sta on sta.id=ca.person_id WHERE 1=1 AND");
            if (!string.IsNullOrEmpty(statime) && !string.IsNullOrEmpty(endtime))
            {
                commandText.Append(" '" + statime + "' < time AND  time < '" + endtime + "' AND");
            }
            if (!string.IsNullOrEmpty(name))
            {
                commandText.Append(" person_name LIKE '%" + name.Trim() + "%' AND");
            }
            if (!string.IsNullOrEmpty(devname))
            {
                commandText.Append(" device_sn='" + devname + "' AND");
            }
            if (HealthCodeType != "0")
            {
                if (HealthCodeType == "1")
                {
                    commandText.Append(" QRcodestatus LIKE '%绿码%' AND");
                }
                else if (HealthCodeType == "2")
                {
                    commandText.Append(" QRcodestatus LIKE '%黄码%' AND");
                }
                else if (HealthCodeType == "3")
                {
                    commandText.Append(" QRcodestatus LIKE '%红码%' AND");
                }
            }
            if (!string.IsNullOrEmpty(selectedPersonTypes))
            {
                var sb = new StringBuilder(" ");
                var sections = selectedPersonTypes.Split(',')
                    .Select(x => x == "1" ? " person_id is not null" : " person_id is null");
                var s = String.Join(" or ", sections);

                sb.AppendFormat(" ({0}) AND", s);
                commandText.Append(sb.ToString());
            }

            if (tempFrom != null)
            {
                if(!ChromiumForm.userSettings.ShowTemperatureInCelsius)
                {
                    tempFrom = tempFrom.Value.toCelsius();
                }
                commandText.Append($" body_temp >= {tempFrom.Value.ToString(CultureInfo.InvariantCulture)} AND");
            }

            if (tempTo != null)
            {
                if (!ChromiumForm.userSettings.ShowTemperatureInCelsius)
                {
                    tempFrom = tempFrom.Value.toCelsius();
                }
                commandText.Append($" body_temp <= {tempTo.Value.ToString(CultureInfo.InvariantCulture)} AND");
            }

            string commandText2 = commandText.ToString().Substring(0, commandText.ToString().Length - 3).ToString();
            commandText2 = commandText2 +
                "order by ca.id DESC LIMIT " + pageint + "," + limt;
            Action<DataTable> convertCelsiusToFahreinheit = null;
            if (!InsuranceBrowser.ChromiumForm.userSettings.ShowTemperatureInCelsius)
            {
                convertCelsiusToFahreinheit = ConvertCelsiusToFahreinheit("body_temp");
            }
            return SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText2.ToString(), convertCelsiusToFahreinheit);
        }

        private static Action<DataTable> ConvertCelsiusToFahreinheit(string columnName)
        {
            return (table) =>
            {
                foreach (DataRow row in table.Rows)
                {
                    var s = row[columnName] as string;
                    var newS = s.toFahreinheit();
                    row[columnName] = newS;
                }
            };
        }

        public static string getVisitor(string statime, string statime1, string endtime, string endtime1, string name, string phone, string isDown, string page, string limt)
        {
            int page1 = int.Parse(page) - 1;
            int pageint = page1 * int.Parse(limt);
            StringBuilder commandText = new StringBuilder("SELECT * FROM Visitor  WHERE 1=1 AND");
            if (!string.IsNullOrEmpty(statime) && !string.IsNullOrEmpty(statime1))
            {
                commandText.Append(" '" + statime + "' < staTime AND  staTime < '" + statime1 + "' AND");
            }
            if (!string.IsNullOrEmpty(endtime) && !string.IsNullOrEmpty(endtime1))
            {
                commandText.Append(" '" + endtime + "' < endTime AND  endTime < '" + endtime1 + "' AND");
            }
            if (!string.IsNullOrEmpty(name))
            {
                commandText.Append(" name LIKE '%" + name.Trim() + "%' AND");
            }
            if (!string.IsNullOrEmpty(phone))
            {
                commandText.Append(" phone='" + phone + "' AND");
            }
            if (!string.IsNullOrEmpty(isDown))
            {
                if (isDown.Trim() != "2")
                {
                    if (isDown.Trim() == "1")
                    {
                        commandText.Append(" isDown='" + isDown.Trim() + "' AND");
                    }
                    else
                    {
                        commandText.Append(" isDown='0' or isDown is null AND");
                    }
                }
            }

            string commandText2 = commandText.ToString().Substring(0, commandText.ToString().Length - 3).ToString();
            commandText2 = commandText2 +
                "order by id DESC LIMIT " + pageint + "," + limt;
            return SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText2.ToString());
        }

        public static Capture_Data[] getCapture_Data1(string statime, string endtime, string name, string devname, string selectedPersonTypes, string HealthCodeType, float? tempFrom, float? tempTo)
        {
            var pg = new DapperExtensions.PredicateGroup
            {
                Operator = DapperExtensions.GroupOperator.And,
                Predicates = new List<DapperExtensions.IPredicate>()
            };

            if (!string.IsNullOrEmpty(statime) && !string.IsNullOrEmpty(endtime))
            {
                pg.Predicates.Add(DapperExtensions.Predicates.Field<Capture_Data>(x => x.time, DapperExtensions.Operator.Gt, statime));
                pg.Predicates.Add(DapperExtensions.Predicates.Field<Capture_Data>(x => x.time, DapperExtensions.Operator.Lt, endtime));
            }

            if (!string.IsNullOrEmpty(name))
            {
                pg.Predicates.Add(DapperExtensions.Predicates.Field<Capture_Data>(x => x.person_name, DapperExtensions.Operator.Like, $"%{name}%"));
            }

            if (!string.IsNullOrEmpty(devname))
            {
                pg.Predicates.Add(DapperExtensions.Predicates.Field<Capture_Data>(x => x.device_sn, DapperExtensions.Operator.Eq, devname));
            }

            if (HealthCodeType != "0")
            {
                if (HealthCodeType == "1")
                {
                    pg.Predicates.Add(DapperExtensions.Predicates.Field<Capture_Data>(x => x.QRcodestatus, DapperExtensions.Operator.Like, "%绿码%"));
                }
                else if (HealthCodeType == "2")
                {
                    pg.Predicates.Add(DapperExtensions.Predicates.Field<Capture_Data>(x => x.QRcodestatus, DapperExtensions.Operator.Like, "%黄码%"));
                }
                else if (HealthCodeType == "3")
                {
                    pg.Predicates.Add(DapperExtensions.Predicates.Field<Capture_Data>(x => x.QRcodestatus, DapperExtensions.Operator.Like, "%红码%"));
                }
            }
            if (!string.IsNullOrEmpty(selectedPersonTypes))
            {
                var personTypeGroup = new DapperExtensions.PredicateGroup()
                {
                    Operator = DapperExtensions.GroupOperator.Or,
                    Predicates = new List<DapperExtensions.IPredicate>()
                };
                var sections = selectedPersonTypes.Split(',');
                foreach (var personType in sections)
                {
                    switch (personType)
                    {
                        case "1":
                            personTypeGroup.Predicates.Add(DapperExtensions.Predicates.Field<Capture_Data>(x => x.person_id, DapperExtensions.Operator.Eq, null, true));
                            break;
                        case "0":
                            personTypeGroup.Predicates.Add(DapperExtensions.Predicates.Field<Capture_Data>(x => x.person_id, DapperExtensions.Operator.Eq, null));
                            break;
                    }
                }
                pg.Predicates.Add(personTypeGroup);
            }

            if (tempFrom != null)
            {
                pg.Predicates.Add(DapperExtensions.Predicates.Field<Capture_Data>(x => x.body_temp, DapperExtensions.Operator.Ge, tempFrom.Value));
            }

            if (tempTo != null)
            {
                pg.Predicates.Add(DapperExtensions.Predicates.Field<Capture_Data>(x => x.body_temp, DapperExtensions.Operator.Le, tempTo.Value));
            }


            using (var conn = SQLiteHelper.GetConnection())
            {

                var data = DapperExtensions.DapperExtensions.GetList<Capture_Data>(
                    conn, 
                    pg, 
                    new List<DapperExtensions.ISort>() { new DapperExtensions.Sort() { PropertyName = nameof(Capture_Data.time), Ascending = true } }
                    ).ToArray();
                if (!ChromiumForm.userSettings.ShowTemperatureInCelsius)
                {
                    foreach (var item in data)
                    {
                        item.body_temp = item.body_temp.toFahreinheit();
                    }
                }
                return data;
            }


            //            StringBuilder commandText = new StringBuilder("SELECT ca.addr_name as addr_name"+
            //", ca.time as time" + 
            //", ca.match_status as match_status" + 
            //", ca.person_name as person_name" + 
            //", ca.wg_card_id as wg_card_id" + 
            //", ca.match_failed_reson as match_failed_reson" + 
            //", ca.exist_mask as exist_mask" + 
            //", ca.body_temp as body_temp" + 
            //", ca.device_sn as device_sn" + 
            //", ca.idcard_number as idcard_number" + 
            //", ca.idcard_name as idcard_name" + 
            //", ca.QRcodestatus as QRcodestatus "+
            //", ca.trip_infor as trip_infor " +
            //", ca.closeup as closeup " +
            //" FROM Capture_Data ca LEFT JOIN staff sta on sta.id=ca.person_id WHERE 1=1 AND");
            //            if (!string.IsNullOrEmpty(statime) && !string.IsNullOrEmpty(endtime))
            //            {
            //                commandText.Append(" '" + statime + "' < time AND  time < '" + endtime + "' AND");
            //            }
            //            if (!string.IsNullOrEmpty(name))
            //            {
            //                commandText.Append(" person_name LIKE '%" + name.Trim() + "%' AND");
            //            }
            //            if (!string.IsNullOrEmpty(devname))
            //            {
            //                commandText.Append(" device_sn='" + devname + "' AND");
            //            }
            //            if (HealthCodeType != "0")
            //            {
            //                if (HealthCodeType == "1")
            //                {
            //                    commandText.Append(" QRcodestatus LIKE '%绿码%' AND");
            //                }
            //                else if (HealthCodeType == "2")
            //                {
            //                    commandText.Append(" QRcodestatus LIKE '%黄码%' AND");
            //                }
            //                else if (HealthCodeType == "3")
            //                {
            //                    commandText.Append(" QRcodestatus LIKE '%红码%' AND");
            //                }
            //            }
            //            if (!string.IsNullOrEmpty(stranger))
            //            {
            //                if (stranger.Trim() == "1")
            //                {
            //                    commandText.Append(" match_status='0' or match_status='-1' AND");
            //                }
            //            }

            //            string commandText2 = commandText.ToString().Substring(0, commandText.ToString().Length - 3).ToString();
            //            commandText2 = commandText2 +
            //                "order by ca.id DESC";
            //            return SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText2.ToString());
        }

        public static bool delCapture_DataForid(string id)
        {
            if (id.Trim().Length < 2)
            {
                return false;
            }
            string sql = "DELETE FROM Capture_Data WHERE id in (" + id.ToString().Substring(0, id.ToString().Length - 1).ToString() + ")";

            int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);
            if (re > 0)
            {
                return true;
            }
            else
                return false;
        }

        public static bool delVisitorForid(string id)
        {
            if (id.Trim().Length < 2)
            {
                return false;
            }

            Deviceinfo.MyDevicelist.ForEach(d =>
            {
                if (d.IsConnected == true)
                {
                    JObject deleteJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.deleteJson);
                    if (deleteJson != null)
                    {
                        deleteJson["id"] = id.Replace(",", "").Trim();
                    }
                    //先执行删除操作
                    string sss = GetDevinfo.request(d, deleteJson.ToString());
                }
            });

            string sql = "DELETE FROM Visitor WHERE id in (" + id.ToString().Substring(0, id.ToString().Length - 1).ToString() + ")";

            int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);
            if (re > 0)
            {
                return true;
            }
            else
                return false;
        }

        public static void ubpdateEquipment_distributionfordel(string id)
        {
            string updatessql = "UPDATE Equipment_distribution SET type=1,status='success',date='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE userid=" + id;
            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
        }


        public static bool CardReplacement(string type, string id, string staTime, string endTime, string timeInterval, string number)
        {
            string disparity = string.Empty;
            if (!string.IsNullOrEmpty(staTime) && !string.IsNullOrEmpty(endTime))
            {
                disparity = AttendanceAlgorithm.DateDiff(staTime.Replace(":", ""), endTime.Replace(":", ""));
            }

            string updatessql = string.Empty;
            if (timeInterval.Trim() == "1")
            {
                if (type == "01")
                {
                    updatessql = "UPDATE Attendance_Data SET  Punchinformation='" + endTime + "' WHERE id=" + id;
                }
                else if (type == "02")
                {
                    updatessql = "UPDATE Attendance_Data SET  Punchinformation1='" + endTime + "' WHERE id=" + id;
                }
                else if (type == "1")
                {
                    updatessql = "UPDATE Attendance_Data SET late='" + (int.Parse(disparity) + int.Parse(number)).ToString() + "',Punchinformation='" + staTime + "' WHERE id=" + id;
                }
                else if (type == "2")
                {
                    updatessql = "UPDATE Attendance_Data SET Leaveearly='" + (int.Parse(disparity) + int.Parse(number)).ToString() + "',Punchinformation1='" + staTime + "'  WHERE id=" + id;
                }
                else if (type == "3")
                {
                    updatessql = "UPDATE Attendance_Data SET Remarks=3,isAbsenteeism='' WHERE id=" + id;
                }
            }
            else if (timeInterval.Trim() == "2")
            {
                if (type == "01")
                {
                    updatessql = "UPDATE Attendance_Data SET  Punchinformation2='" + endTime + "' WHERE id=" + id;
                }
                else if (type == "02")
                {
                    updatessql = "UPDATE Attendance_Data SET  Punchinformation22='" + endTime + "' WHERE id=" + id;
                }
                else if (type == "1")
                {
                    updatessql = "UPDATE Attendance_Data SET late='" + (int.Parse(disparity) + int.Parse(number)).ToString() + "',Punchinformation2='" + staTime + "' WHERE id=" + id;
                }
                else if (type == "2")
                {
                    updatessql = "UPDATE Attendance_Data SET Leaveearly='" + (int.Parse(disparity) + int.Parse(number)).ToString() + "',Punchinformation22='" + staTime + "'  WHERE id=" + id;
                }
                else if (type == "3")
                {
                    updatessql = "UPDATE Attendance_Data SET Remarks=3,isAbsenteeism='' WHERE id=" + id;
                }
            }
            //三段考勤
            else if (timeInterval.Trim() == "3")
            {

            }

            int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
            if (re > 0)
            {
                //计算是否旷工
                //if (timeInterval.Trim() == "1")
                //{
                //    isAbsenteeism(id);
                //}
                //else if (timeInterval.Trim() == "2")
                //{
                //    isAbsenteeism2(id);
                //}
                isAbsenteeism(id);
                return true;
            }
            else return false;
        }
        //计算是否旷工
        public static void isAbsenteeism(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;
            string commandTextdepartmentid = "SELECT Punchinformation,Punchinformation1,Shiftinformation,Punchinformation2,Punchinformation22," +
                "Remarks FROM  Attendance_Data  att WHERE att.id=" + id;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
            if (!string.IsNullOrEmpty(sr))
            {
                JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                string Punchinformation = jo[0]["Punchinformation"].ToString();
                string Punchinformation1 = jo[0]["Punchinformation1"].ToString();
                string Shiftinformation = jo[0]["Shiftinformation"].ToString();
                string Remarks = jo[0]["Remarks"].ToString();
                if (Shiftinformation.Contains(";"))
                {
                    string Punchinformation2 = jo[0]["Punchinformation2"].ToString();
                    string Punchinformation22 = jo[0]["Punchinformation22"].ToString();
                    if (string.IsNullOrEmpty(Punchinformation) || string.IsNullOrEmpty(Punchinformation1)
                        || string.IsNullOrEmpty(Punchinformation2) || string.IsNullOrEmpty(Punchinformation22))
                    {
                        if (Remarks.Trim() == "3")
                        {
                            string updatessql = "UPDATE Attendance_Data SET isAbsenteeism='' WHERE id=" + id;
                            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
                        }
                        else
                        {
                            string updatessql = "UPDATE Attendance_Data SET isAbsenteeism='0' WHERE id=" + id;
                            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
                        }
                    }
                    else if (!string.IsNullOrEmpty(Punchinformation) && !string.IsNullOrEmpty(Punchinformation1)
                        && !string.IsNullOrEmpty(Punchinformation2) && !string.IsNullOrEmpty(Punchinformation22))
                    {
                        string updatessql = "UPDATE Attendance_Data SET Remarks='',isAbsenteeism='' WHERE id=" + id;
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(Punchinformation) || string.IsNullOrEmpty(Punchinformation1))
                    {
                        if (Remarks.Trim() == "3")
                        {
                            string updatessql = "UPDATE Attendance_Data SET isAbsenteeism='' WHERE id=" + id;
                            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
                        }
                        else
                        {
                            string updatessql = "UPDATE Attendance_Data SET isAbsenteeism='0' WHERE id=" + id;
                            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
                        }
                    }
                    else if (!string.IsNullOrEmpty(Punchinformation) && !string.IsNullOrEmpty(Punchinformation1))
                    {
                        string updatessql = "UPDATE Attendance_Data SET Remarks='',isAbsenteeism='' WHERE id=" + id;
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
                    }
                }
            }
        }

        public static void isAbsenteeism2(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;
            string commandTextdepartmentid = "SELECT Punchinformation,Punchinformation1,Punchinformation2,Punchinformation22,Remarks FROM  Attendance_Data  att WHERE att.id=" + id;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
            if (!string.IsNullOrEmpty(sr))
            {
                JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                string Punchinformation = jo[0]["Punchinformation"].ToString();
                string Punchinformation1 = jo[0]["Punchinformation1"].ToString();
                string Punchinformation2 = jo[0]["Punchinformation2"].ToString();
                string Punchinformation22 = jo[0]["Punchinformation22"].ToString();
                string Remarks = jo[0]["Remarks"].ToString();
                if (string.IsNullOrEmpty(Punchinformation) || string.IsNullOrEmpty(Punchinformation1)
                    || string.IsNullOrEmpty(Punchinformation2) || string.IsNullOrEmpty(Punchinformation22))
                {
                    if (Remarks.Trim() == "3")
                    {
                        string updatessql = "UPDATE Attendance_Data SET isAbsenteeism=' ' WHERE id=" + id;
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
                    }
                    else
                    {
                        string updatessql = "UPDATE Attendance_Data SET isAbsenteeism='0' WHERE id=" + id;
                        SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
                    }
                }
                else if (!string.IsNullOrEmpty(Punchinformation) && !string.IsNullOrEmpty(Punchinformation1)
                    && !string.IsNullOrEmpty(Punchinformation2) && !string.IsNullOrEmpty(Punchinformation22))
                {
                    string updatessql = "UPDATE Attendance_Data SET Remarks='',isAbsenteeism='' WHERE id=" + id;
                    SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
                }
            }
        }

        //获取下发记录总数
        public static string getcountforEquipment_distribution(string name, string ip, string status, string DeviceName)
        {
            StringBuilder commandText = new StringBuilder("SELECT COUNT(*) as count from Equipment_distribution eq LEFT JOIN MyDevice my on my.id=eq.deviceid  LEFT JOIN staff st on st.id=eq.userid  WHERE type != '1' AND 1=1 AND");
            if (!string.IsNullOrEmpty(name))
            {
                commandText.Append(" st.name='" + name.Trim() + "' AND");
            }
            if (!string.IsNullOrEmpty(ip))
            {
                commandText.Append(" my.ipAddress='" + ip.Trim() + "' AND");
            }
            if (!string.IsNullOrEmpty(DeviceName))
            {
                commandText.Append(" my.DeviceName='" + DeviceName.Trim() + "' AND");
            }
            if (!status.Trim().Equals("10"))
            {
                if (status.Trim().Equals("1"))
                {
                    commandText.Append(" eq.status='fail' AND");
                }
                else if (status.Trim().Equals("0"))
                {
                    commandText.Append(" eq.status='success' AND");
                }
                else if (status.Trim().Equals("2"))
                {
                    commandText.Append(" eq.status='inprogress' AND");
                }
            }

            string commandText2 = commandText.ToString().Substring(0, commandText.ToString().Length - 3).ToString();

            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText2);

            return sr;
        }

        //获取下发列表数据
        public static string getEquipment_distribution(string page, string limt, string name, string ip, string status, string DeviceName)
        {
            if (string.IsNullOrEmpty(page) || string.IsNullOrEmpty(limt))
                return "";

            int page1 = int.Parse(page) - 1;
            int pageint = page1 * int.Parse(limt);
            //DeviceName
            StringBuilder commandText = new StringBuilder("" +
                "SELECT " +
                "my.DeviceName," +
                "my.number,st.name,my.ipAddress,eq.status,eq.date,eq.code, eq.errMsg " +
                "from Equipment_distribution eq LEFT JOIN MyDevice my on my.id=eq.deviceid  LEFT JOIN staff st on st.id=eq.userid  WHERE type != '1' AND 1=1 AND");
            if (!string.IsNullOrEmpty(name))
            {
                commandText.Append(" st.name='" + name.Trim() + "' AND");
            }
            if (!string.IsNullOrEmpty(ip))
            {
                commandText.Append(" my.ipAddress='" + ip.Trim() + "' AND");
            }
            if (!string.IsNullOrEmpty(DeviceName))
            {
                commandText.Append(" my.DeviceName='" + DeviceName.Trim() + "' AND");
            }
            if (!status.Trim().Equals("10"))
            {
                if (status.Trim().Equals("1"))
                {
                    commandText.Append(" eq.status='fail' AND");
                }
                else if (status.Trim().Equals("0"))
                {
                    commandText.Append(" eq.status='success' AND");
                }
                else if (status.Trim().Equals("2"))
                {
                    commandText.Append(" eq.status='inprogress' AND");
                }
            }

            string commandText2 = commandText.ToString().Substring(0, commandText.ToString().Length - 3).ToString();
            commandText2 = commandText2 +
                "LIMIT " + pageint + "," + limt;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText2);

            return sr;
        }
        /// <summary>
        /// 日本的line接口 将迟到早退写入数据库
        /// </summary>
        /// <param name="imagepath"></param>
        /// <returns></returns>
        public static void setLine_list(reData reData, string msg, string type, string temperature, string Date, string time, string line_userid, string status)
        {
            //0  line 1 mailTo
            string lineType = "0";
            string commandText = "INSERT INTO LineFor_list (message,type,name,Date,time,temperature,late,Leaveearly,line_userid,status)VALUES" +
                "('" + msg + "','" + type + "','" + reData.name + "','" + Date + "','" + time + "','" + temperature + "','" + reData.late + "','" + reData.Leaveearly + "','" + line_userid + "','" + status + "')";
            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
        }

        public static bool getLine_list_data(string data, string AttendanceType, string line_userid)
        {
            string commandText = "SELECT COUNT(*) as len FROM LineFor_list WHERE Date='" + data + "' AND  status='1' AND  type='" + AttendanceType + "' AND line_userid='" + line_userid.Trim() + "'";
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);
            if (!string.IsNullOrEmpty(sr))
            {
                JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                string reint = jo[0]["len"].ToString();
                if (int.Parse(reint) > 0)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public static string getcountforLineFor_list(string name, string stadate, string endate, string type)
        {

            StringBuilder st = new StringBuilder("SELECT COUNT(*) AS count FROM LineFor_list li WHERE 1=1  AND");
            if (!string.IsNullOrEmpty(name))
            {
                st.Append(" li.name LIKE '%" + name.Trim() + "%' AND");
            }
            if (!string.IsNullOrEmpty(stadate))
            {
                st.Append(" li.Date>='" + stadate.Trim() + "' AND");
            }
            if (!string.IsNullOrEmpty(endate))
            {
                st.Append(" li.Date<='" + endate.Trim() + "' AND");
            }

            if (!string.IsNullOrEmpty(type))
            {
                if (type.Trim() == "1")
                {
                    st.Append(" li.type='late' AND");
                }
                else if (type.Trim() == "2")
                {
                    st.Append(" li.type='Leave' AND");
                }
                else if (type.Trim() == "3")
                {
                    st.Append(" li.type='Attendance' OR li.type='Offduty' AND");
                }
            }

            string commandText = st.ToString().Substring(0, st.ToString().Length - 3).ToString();
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        //获取line接口迟到早退总人数
        public static string getforLineFor_list(string name, string stadate, string endate, string type, string page, string limt)
        {
            if (string.IsNullOrEmpty(page) || string.IsNullOrEmpty(limt))
                return "";

            int page1 = int.Parse(page) - 1;
            int pageint = page1 * int.Parse(limt);

            StringBuilder st = new StringBuilder("SELECT  li.Date,li.status,li.time,li.Leaveearly,li.late,li.name,li.temperature,li.id " +
                " FROM LineFor_list li WHERE 1=1  AND");
            if (!string.IsNullOrEmpty(name))
            {
                st.Append(" li.name LIKE '%" + name.Trim() + "%' AND");
            }
            if (!string.IsNullOrEmpty(stadate))
            {
                st.Append(" li.Date>='" + stadate.Trim() + "' AND");
            }
            if (!string.IsNullOrEmpty(endate))
            {
                st.Append(" li.Date<='" + endate.Trim() + "' AND");
            }

            if (!string.IsNullOrEmpty(type))
            {
                if (type.Trim() == "1")
                {
                    st.Append(" li.type='late' AND");
                }
                else if (type.Trim() == "2")
                {
                    st.Append(" li.type='Leave' AND");
                }
                else if (type.Trim() == "3")
                {
                    st.Append(" li.type='Attendance' OR li.type='Offduty' AND");
                }
            }

            string commandText = st.ToString().Substring(0, st.ToString().Length - 3).ToString()
                + "order by id desc LIMIT " + pageint + "," + limt;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getSpecial_datecount(string editId, string type)
        {
            string commandText = "SELECT COUNT(*) as count FROM Special_date WHERE datetype=" + type + " AND AttendanceGroupid=" + editId;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getSpecial_date(string editId, string type, string page, string limt)
        {

            if (string.IsNullOrEmpty(page) || string.IsNullOrEmpty(limt))
                return "";

            int page1 = int.Parse(page) - 1;
            int pageint = page1 * int.Parse(limt);

            string st = "SELECT sp.id,sp.date,sp.datetype as type ,IFNULL(sh.name,'0') as name ,sp.AttendanceGroupid" +
                " FROM Special_date sp LEFT JOIN Shift sh on sp.Shiftid=sh.id WHERE datetype=" + type + " AND AttendanceGroupid=" + editId;


            string commandText = st + " LIMIT " + pageint + "," + limt;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getSpecial_datefordate(string data, string AttendanceGroup_id)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(AttendanceGroup_id))
                return "";

            string commandText = "SELECT Shiftid FROM Special_date WHERE date='" + data + "' AND AttendanceGroupid=" + AttendanceGroup_id;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static bool delSpecial_date(string ids)
        {
            bool result = false;
            string arrayids = ids.Replace("{", "").Replace("}", "");
            arrayids = "(" + arrayids + ")";
            string commandText = "DELETE FROM Special_date WHERE id in " + arrayids;
            int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
            if (re > 0)
            {
                result = true;
            }
            return result;
        }
        public static bool saveSpecial_date(string editId, string type, string date, string Shiftid)
        {
            bool result = false;
            string ss = "SELECT COUNT(*) as len  FROM Special_date  WHERE date='" + date + "' AND AttendanceGroupid='" + editId + "'";
            string isrepeat = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, ss);
            if (!string.IsNullOrEmpty(isrepeat))
            {
                JArray jo = (JArray)JsonConvert.DeserializeObject(isrepeat);
                string reint = jo[0]["len"].ToString();
                if (int.Parse(reint) > 0)
                {
                    return false;
                }
            }
            string commandText = "INSERT INTO Special_date(date,Shiftid,datetype,AttendanceGroupid) VALUES ('" + date + "','" + Shiftid + "','" + type + "','" + editId + "')";
            int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
            if (re > 0)
            {
                result = true;
            }
            return result;
        }

        public static bool sendOutforLine(string id)
        {
            bool re = false;
            try
            {
                if (string.IsNullOrEmpty(id))
                    return re;

                string commandTextdepartmentid = "SELECT li.line_userid,li.message FROM LineFor_list li WHERE id=" + id;
                string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
                if (!string.IsNullOrEmpty(sr))
                {
                    JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                    string line_userid = jo[0]["line_userid"].ToString();
                    string message = jo[0]["message"].ToString();

                    //发送到接口
                    string url = ApplicationData.line_url + "/sendmessage.php?userid=" + line_userid + "&message=" + message;
                    string INUserIDs_str = HandleCaptureData.HttpGet(url);
                    JObject jObject = (JObject)JsonConvert.DeserializeObject(INUserIDs_str);
                    if (jObject["result"].ToString().Contains("OK"))
                    {
                        //修改发送状态 1成功 0失败
                        string commandText = "UPDATE LineFor_list SET status = '1' WHERE  id=" + id;
                        int de = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                        if (de > 0)
                            re = true;
                    }
                }
            }
            catch
            {

            }
            return re;
        }
        public static bool getvolume(out string volume, CameraConfigPort ca)
        {
            try
            {
                string camera_volume_str = GetDevinfo.request(ca, "{\"cmd\": \"camera volume\",\"method\": \"GET\"}");
                JObject camera_volume_json = JObject.Parse(camera_volume_str.Trim());
                string code = camera_volume_json["code"].ToString();
                if (code == "0")
                {
                    volume = camera_volume_json["volume"].ToString();
                    return true;
                }
                else
                {
                    volume = "";
                    return false;
                }
            }
            catch (Exception x)
            {
                volume = "";
                return false;
            }
        }

        public static bool getlcdscreensaver(out string volume, CameraConfigPort ca)
        {
            try
            {
                string camera_volume_str = GetDevinfo.request(ca, "{\"cmd\": \"request lcd screensaver\"}");
                JObject camera_volume_json = JObject.Parse(camera_volume_str.Trim());
                string code = camera_volume_json["code"].ToString();
                if (code == "0")
                {
                    volume = camera_volume_json["screensaver_mode"].ToString();
                    return true;
                }
                else
                {
                    volume = "";
                    return false;
                }
            }
            catch (Exception x)
            {
                volume = "";
                return false;
            }
        }
        public static string getCameraParameters(string ip)
        {
            string re = "0";
            List<CameraConfigPort> Devicelistdata = Deviceinfo.MyDevicelist;
            Devicelistdata.ForEach(s =>
            {
                if (s.IP == ip.Trim())
                {
                    if (s.IsConnected)
                    {
                        //通过HTTP请求回相机的参数
                        try
                        {
                            string outstr = "";
                            string screensaver_mode = "";
                            re = GetDevinfo.request(s);
                            if (getvolume(out outstr, s))
                            {
                                JObject jObj = JObject.Parse(re.Trim());
                                jObj.Add(new JProperty("volume", outstr));

                                if (getlcdscreensaver(out screensaver_mode, s))
                                {
                                    jObj.Add(new JProperty("screensaver_mode", screensaver_mode));
                                }
                                re = jObj.ToString();
                            }
                        }
                        catch
                        {
                            re = "-1";
                        }

                    }
                    else
                        re = "1";
                }
            });

            return re;
        }

        public static bool setCameraParameters(string ip, string dereplication,
            string enable_alive,
            string enable,
            string limit,
            string led_mode,
            string led_brightness,
            string led_sensitivity,
            string screensaver_mode,
            string output_not_matched,
            string volume
            )
        {
            bool re = false;
            List<CameraConfigPort> Devicelistdata = Deviceinfo.MyDevicelist;
            Devicelistdata.ForEach(s =>
            {
                //先设置息屏模式 成功后在设置其他参数，如果不成功直接返回失败；
                if (s.IP == ip.Trim())
                {
                    if (s.IsConnected)
                    {
                        //通过HTTP请求回相机的参数
                        try
                        {
                            string CameraParameterforlcd = UtilsJson.CameraParameterforlcd;
                            CameraParameterforlcd = string.Format(CameraParameterforlcd, screensaver_mode);
                            string requestre = GetDevinfo.request(s, CameraParameterforlcd);
                            JObject restr_json = (JObject)JsonConvert.DeserializeObject(requestre.Trim());
                            if (restr_json != null)
                            {
                                string code = restr_json["code"].ToString();
                                int code_int = int.Parse(code);
                                if (code_int == 0)
                                {
                                    //先设置声音
                                    if (!volume.Contains("no"))
                                    {
                                        string camera_volume = UtilsJson.camera_volume;
                                        camera_volume = string.Format(camera_volume, volume);
                                        GetDevinfo.request(s, camera_volume);
                                    }
                                    string CameraParameter = "";
                                    //在设置基础参数
                                    if (output_not_matched.Contains("no"))
                                    {
                                        CameraParameter = UtilsJson.CameraParameter;
                                        CameraParameter = string.Format(CameraParameter,
                                            dereplication, enable_alive,
                                            enable, limit, led_mode,
                                            led_brightness, led_sensitivity);
                                    }
                                    else
                                    {
                                        CameraParameter = UtilsJson.CameraParameter_output_not_matched;
                                        CameraParameter = string.Format(CameraParameter,
                                            dereplication, output_not_matched, enable_alive,
                                            enable, limit, led_mode,
                                            led_brightness, led_sensitivity);
                                    }

                                    requestre = GetDevinfo.request(s, CameraParameter);
                                    restr_json = (JObject)JsonConvert.DeserializeObject(requestre.Trim());
                                    if (restr_json != null)
                                    {
                                        code = restr_json["code"].ToString();
                                        code_int = int.Parse(code);
                                        if (code_int == 0)
                                        {
                                            re = true;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }

                    }
                }
            });
            return re;
        }

        public static string GetNetworkInfo(string ip)
        {
            string paramString = "";
            try
            {
                Deviceinfo.MyDevicelist.ForEach(s =>
                {
                    if (s.IP == ip.Trim())
                    {
                        string gateway, netmask, dns = string.Empty;
                        if (s.GetNetworkInfo(out ip, out gateway, out netmask, out dns))
                        {
                            JObject postedJObject = new JObject();
                            postedJObject.Add("ip", ip);
                            postedJObject.Add("gateway", gateway);
                            postedJObject.Add("netmask", netmask);
                            postedJObject.Add("dns", dns);
                            paramString = postedJObject.ToString(Newtonsoft.Json.Formatting.None, null);
                        }
                    }
                });
            }
            catch { }
            return paramString;
        }

        public static bool SetNetworkInfo(string ip, string oldip, string gateway, string netmask, string dns)
        {
            bool re = false;
            try
            {
                Deviceinfo.MyDevicelist.ForEach(s =>
                {
                    if (s.IP == oldip.Trim())
                    {
                        re = s.SetNetworkInfo(ip, gateway, netmask, dns);
                    }
                });
            }
            catch { }
            return re;
        }
        public static bool getIscode_syn()
        {
            bool result = false;
            try
            {
                Inihelper1.FileName = ApplicationData.FaceRASystemToolUrl + @"\\setting.ini";
                if (Inihelper1.ReadBool("Setting", "Iscode_syn", false))
                    result = true;
            }
            catch { }
            return result;
        }
        public static bool setIscode_syn(string nooff)
        {
            if (nooff.Contains("rue"))
                nooff = "true";
            else
                nooff = "false";
            bool result = false;
            try
            {
                Inihelper1.FileName = ApplicationData.FaceRASystemToolUrl + @"\\setting.ini";
                if (nooff.Contains("true"))
                {
                    Inihelper1.WriteBool("Setting", "Iscode_syn", true);
                    result = true;
                }
                else
                {
                    Inihelper1.WriteBool("Setting", "Iscode_syn", false);
                    result = true;
                }
            }
            catch { }
            return result;
        }
        public static bool setIsNtpSync(string trurOrFalse)
        {
            if (trurOrFalse.Contains("true"))
                trurOrFalse = "true";
            else
                trurOrFalse = "false";
            bool result = false;
            try
            {
                //string commandText = "UPDATE MyDevice SET time_syn='" + nooff.Trim() + "'";

                //int re = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, commandText);
                //if (re > 0)
                //{
                //    result = true;
                //}
                Inihelper1.FileName = ApplicationData.FaceRASystemToolUrl + @"\\setting.ini";
                if (trurOrFalse.Contains("true"))
                {
                    Inihelper1.WriteBool("Setting", "time_syn", true);
                    result = true;
                }
                else
                {
                    Inihelper1.WriteBool("Setting", "time_syn", false);
                    result = true;
                }
            }
            catch { }
            return result;
        }

        public static bool IsNoNoviceGuide()
        {
            bool result = false;
            try
            {
                Inihelper1.FileName = ApplicationData.FaceRASystemToolUrl + @"\\setting.ini";
                if (!Inihelper1.ReadBool("Setting", "NoviceGuide", false))
                    result = true;

                Inihelper1.WriteBool("Setting", "NoviceGuide", true);
            }
            catch { }
            return result;
        }

        public static string GetIpforPC()
        {
            string result = "";
            try
            {
                var ip = GetLocalIP();
                Console.WriteLine(ip);
                //var dns = GetPrimaryDNS();
                //Console.WriteLine(dns);
                var gateway = GetGateway();
                Console.WriteLine(gateway);
                result = gateway + ";" + ip+";";

                //获得到的本机IP不可靠，取消该获取方式
                //int cou = 0;
                //NetworkInterface[] NetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                //foreach (NetworkInterface NetworkIntf in NetworkInterfaces)
                //{
                //    IPInterfaceProperties IPInterfaceProperties = NetworkIntf.GetIPProperties();
                //    UnicastIPAddressInformationCollection UnicastIPAddressInformationCollection = IPInterfaceProperties.UnicastAddresses;
                //    foreach (UnicastIPAddressInformation UnicastIPAddressInformation in UnicastIPAddressInformationCollection)
                //    {
                //        if (UnicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                //        {
                //            result = result + UnicastIPAddressInformation.Address.ToString() + ";";
                //            cou++;
                //            if (cou == 2)
                //            {
                //                if (!string.IsNullOrEmpty(result))
                //                    result = result.Remove(result.Length - 1, 1);
                //                return result;
                //            }
                //        }
                //    }
                //}
            }
            catch { }
            if (!string.IsNullOrEmpty(result))
                result = result.Remove(result.Length - 1, 1);
            return result;
        }

        /// <summary> 
        /// 获取当前使用的IP 
        /// </summary> 
        /// <returns></returns> 
        public static string GetLocalIP()
        {
            string result = RunApp("route", "print", true);
            Match m = Regex.Match(result, @"0.0.0.0\s+0.0.0.0\s+(\d+.\d+.\d+.\d+)\s+(\d+.\d+.\d+.\d+)");
            if (m.Success)
            {
                return m.Groups[2].Value;
            }
            else
            {
                try
                {
                    System.Net.Sockets.TcpClient c = new System.Net.Sockets.TcpClient();
                    c.Connect("www.baidu.com", 80);
                    string ip = ((System.Net.IPEndPoint)c.Client.LocalEndPoint).Address.ToString();
                    c.Close();
                    return ip;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary> 
        /// 运行一个控制台程序并返回其输出参数。 
        /// </summary> 
        /// <param name="filename">程序名</param> 
        /// <param name="arguments">输入参数</param> 
        /// <returns></returns> 
        public static string RunApp(string filename, string arguments, bool recordLog)
        {
            try
            {
                if (recordLog)
                {
                    Trace.WriteLine(filename + " " + arguments);
                }
                Process proc = new Process();
                proc.StartInfo.FileName = filename;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                using (System.IO.StreamReader sr = new System.IO.StreamReader(proc.StandardOutput.BaseStream, Encoding.Default))
                {
                    //string txt = sr.ReadToEnd(); 
                    //sr.Close(); 
                    //if (recordLog) 
                    //{ 
                    // Trace.WriteLine(txt); 
                    //} 
                    //if (!proc.HasExited) 
                    //{ 
                    // proc.Kill(); 
                    //} 
                    //上面标记的是原文，下面是我自己调试错误后自行修改的 
                    Thread.Sleep(100);  //貌似调用系统的nslookup还未返回数据或者数据未编码完成，程序就已经跳过直接执行 
                                        //txt = sr.ReadToEnd()了，导致返回的数据为空，故睡眠令硬件反应 
                    if (!proc.HasExited)  //在无参数调用nslookup后，可以继续输入命令继续操作，如果进程未停止就直接执行 
                    {    //txt = sr.ReadToEnd()程序就在等待输入，而且又无法输入，直接掐住无法继续运行 
                        proc.Kill();
                    }
                    string txt = sr.ReadToEnd();
                    sr.Close();
                    if (recordLog)
                        Trace.WriteLine(txt);
                    return txt;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return ex.Message;
            }
        }

        /// <summary>
        /// 尝试Ping指定IP是否能够Ping通
        /// </summary>
        /// <param name="strIP">指定IP</param>
        /// <returns>true 是 false 否</returns>
        public static bool IsPingIP(string strIP)
        {
            try
            {
                //创建Ping对象
                Ping ping = new Ping();
                //接受Ping返回值
                PingReply reply = ping.Send(strIP, 1000);
                //Ping通
                return true;
            }
            catch
            {
                //Ping失败
                return false;
            }
        }

        //得到网关地址
        public static string GetGateway()
        {
            //网关地址
            string strGateway = "";
            //获取所有网卡
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            //遍历数组
            foreach (var netWork in nics)
            {
                //单个网卡的IP对象
                IPInterfaceProperties ip = netWork.GetIPProperties();
                //获取该IP对象的网关
                GatewayIPAddressInformationCollection gateways = ip.GatewayAddresses;
                foreach (var gateWay in gateways)
                {
                    //如果能够Ping通网关
                    if (IsPingIP(gateWay.Address.ToString()))
                    {
                        //WriteLog("Gateway:" + gateWay.Address.ToString());
                        //得到网关地址
                        if (gateWay.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            if (!gateWay.Address.ToString().Contains("10"))
                            {
                                strGateway = gateWay.Address.ToString();
                                //WriteLog("realGateway:" + strGateway);
                                //跳出循环
                                break;
                            }
                        }

                    }
                }

                //如果已经得到网关地址
                if (strGateway.Length > 0)
                {
                    //跳出循环
                    break;
                }
            }
            //返回网关地址
            return strGateway;
        }

        /// <summary> 
        /// 获取本机主DNS 
        /// </summary> 
        /// <returns></returns> 
        public static string GetPrimaryDNS()
        {
            string result = RunApp("nslookup", "", true);
            Match m = Regex.Match(result, @"\d+\.\d+\.\d+\.\d+");
            if (m.Success)
            {
                return m.Value;
            }
            else
            {
                return null;
            }
        }

        public static bool getIsNtpSync()
        {
            bool result = false;
            try
            {
                Inihelper1.FileName = ApplicationData.FaceRASystemToolUrl + @"\\setting.ini";
                if (Inihelper1.ReadBool("Setting", "time_syn", false))
                    result = true;
            }
            catch { }
            return result;
        }
        public static string[] getIDsforstaffAndDataSyn()
        {
            using (var conn = SQLiteHelper.GetConnection())
            {
                string sql = "SELECT id FROM staff UNION SELECT personid FROM DataSyn";
                return conn.Query<string>(sql).ToArray();

            }
        }

        public static bool deleteDataSyn(string id, string device_sn)
        {
            bool re = false;
            CameraConfigPort cameraConfigPort = Deviceinfo.MyDevicelist.Find(a => a.DeviceNo == device_sn);
            if (cameraConfigPort != null)
            {
                //先刪除相机上的人员
                JObject deleteJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.deleteJson);
                if (deleteJson != null)
                {
                    deleteJson["id"] = id;
                }
                string restr = GetDevinfo.request(cameraConfigPort, deleteJson.ToString());

                JObject restr_json = (JObject)JsonConvert.DeserializeObject(restr.Trim());
                if (restr_json != null)
                {
                    string code = restr_json["code"].ToString();
                    int code_int = int.Parse(code);
                    if (code_int == 0 || code_int == 22)
                    {
                        string sql = "delete from DataSyn where id = " + id;
                        int sr = SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);
                        if (sr > 0)
                        {
                            re = true;
                        }
                    }
                }
            }
            return re;
        }

        public static void setDataSyn(JObject jObject)
        {
            string publishtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "INSERT INTO DataSyn (name, imge,personid,publishtime,role," +
                "term_start,term,wg_card_id,long_card_id,device_sn,model,addr_name) VALUES " +
                "('" + jObject["name"] + "', '" + jObject["imge"] + "','" + jObject["personid"]
                + "','" + publishtime + "','" + jObject["role"]
                + "','" + jObject["term_start"] + "','" + jObject["term"] + "','"
                + jObject["wg_card_id"] + "','"
                + jObject["long_card_id"] + "','"
                + jObject["device_sn"] + "','"
                + jObject["model"] + "','"
                + jObject["addr_name"] + "')";
            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, sql);
        }
        public static string getDataSyn(string name, string role, string stutas, string addr_name, string page, string limt)
        {
            int page1 = int.Parse(page) - 1;
            int pageint = page1 * int.Parse(limt);
            StringBuilder st = new StringBuilder("SELECT * FROM DataSyn WHERE 1=1  AND");
            if (!string.IsNullOrEmpty(name))
            {
                st.Append(" name LIKE '%" + name.Trim() + "%' AND");
            }
            if (!string.IsNullOrEmpty(role))
            {
                st.Append(" role='" + role.Trim() + "' AND");
            }
            if (!string.IsNullOrEmpty(stutas))
            {
                st.Append(" stutas='" + stutas.Trim() + "' AND");
            }
            if (!string.IsNullOrEmpty(addr_name))
            {
                st.Append(" addr_name='" + addr_name.Trim() + "' AND");
            }

            string commandText = st.ToString().Substring(0, st.ToString().Length - 3).ToString()
               + " LIMIT " + pageint + "," + limt;
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        public static string getDataSynCount(string name, string role, string stutas, string addr_name)
        {
            StringBuilder st = new StringBuilder("SELECT COUNT(*) as count FROM DataSyn WHERE 1=1  AND");
            if (!string.IsNullOrEmpty(name))
            {
                st.Append(" name LIKE '%" + name.Trim() + "%' AND");
            }
            if (!string.IsNullOrEmpty(role))
            {
                st.Append(" role='" + role.Trim() + "' AND");
            }
            if (!string.IsNullOrEmpty(stutas))
            {
                st.Append(" stutas='" + stutas.Trim() + "' AND");
            }
            if (!string.IsNullOrEmpty(addr_name))
            {
                st.Append(" addr_name='" + addr_name.Trim() + "' AND");
            }

            string commandText = st.ToString().Substring(0, st.ToString().Length - 3).ToString();
            string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandText);

            return sr;
        }

        //一键注册
        public static void registDataSynTostaff(string dataname, string role, string stutas, string addr_name)
        {
            //先查询所有的人员
            //先查询有多少条 如果大于1000条 分组进行查 然后新增到人员列表
            string sql = "SELECT COUNT(*) as len FROM DataSyn";

            StringBuilder st = new StringBuilder(" WHERE 1=1  AND");
            if (!string.IsNullOrEmpty(dataname))
            {
                st.Append(" name LIKE '%" + dataname.Trim() + "%' AND");
            }
            if (!string.IsNullOrEmpty(role))
            {
                st.Append(" role='" + role.Trim() + "' AND");
            }
            if (!string.IsNullOrEmpty(stutas))
            {
                st.Append(" stutas='" + stutas.Trim() + "' AND");
            }
            if (!string.IsNullOrEmpty(addr_name))
            {
                st.Append(" addr_name='" + addr_name.Trim() + "' AND");
            }

            string commandText = st.ToString().Substring(0, st.ToString().Length - 3).ToString();

            string data = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, sql + commandText);
            if (!string.IsNullOrEmpty(data))
            {
                JArray jo = (JArray)JsonConvert.DeserializeObject(data);
                string reint = jo[0]["len"].ToString();
                if (int.Parse(reint) > 0)
                {
                    sql = "SELECT * FROM DataSyn";
                    data = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, sql + commandText);
                    jo = (JArray)JsonConvert.DeserializeObject(data);
                    if (jo.Count > 0)
                    {
                        for (var i = 0; i < jo.Count; i++)
                        {
                            try
                            {
                                string personid = jo[i]["personid"].ToString();

                                Int64.Parse(personid);
                                string name = jo[i]["name"].ToString();
                                string imge = jo[i]["imge"].ToString();
                                string wg_card_id = jo[i]["wg_card_id"].ToString();
                                string long_card_id = jo[i]["long_card_id"].ToString();
                                string source = jo[i]["device_sn"].ToString();
                                string card_id = "";
                                if (string.IsNullOrEmpty(wg_card_id))
                                {
                                    card_id = long_card_id;
                                }
                                else if (string.IsNullOrEmpty(long_card_id))
                                {
                                    card_id = wg_card_id;
                                }
                                setStaf(personid, name, imge, card_id, source);
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
        }


        //查询七天的抓拍数据每天的条数
        public static string getCapture_Data7day()
        {
            DateTime newdate = DateTime.Now;
            JObject obj = new JObject();
            for (var i = 0; i < 7; i++)
            {
                string dateTime = newdate.AddDays(-i).ToString("yyyy-MM-dd");
                string sql = "SELECT COUNT(*) as count FROM Capture_Data WHERE time>'" + dateTime + " 00:00:00' AND time<'" + dateTime + " 23:59:59'";

                string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, sql);
                JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                string len = jo[0]["count"].ToString();
                obj[dateTime] = len;
            }

            return obj.ToString();
        }

        /// <summary>
        /// 通过FileStream 来打开文件，这样就可以实现不锁定Image文件，到时可以让多用户同时访问Image文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadImageFile(string imagepath)
        {
            FileStream fs = new FileStream(imagepath, FileMode.Open);//可以是其他重载方法 
            byte[] byData = new byte[fs.Length];
            fs.Read(byData, 0, byData.Length);
            fs.Close();
            return Convert.ToBase64String(byData);
        }
        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param fileFullPath="fileFullPath">文件路径</param>
        /// <returns></returns>
        public static void DeleteFile(string fileFullPath)
        {
            // 1、首先判断文件或者文件路径是否存在
            if (File.Exists(fileFullPath))
            {
                // 2、根据路径字符串判断是文件还是文件夹
                FileAttributes attr = File.GetAttributes(fileFullPath);
                // 3、根据具体类型进行删除
                if (attr == FileAttributes.Directory)
                {
                    // 3.1、删除文件夹
                    Directory.Delete(fileFullPath, true);
                }
                else
                {
                    // 3.2、删除文件
                    File.Delete(fileFullPath);
                }
                File.Delete(fileFullPath);
            }
        }

        public static (int In, int Out) getInOutCount(DateTime day)
        {
            var i = 0;
            var o = 0;
            using (var conn = SQLiteHelper.GetConnection())
            {
                var res = conn.Query($"select count(ca.id) as count, dev.IsEnter from Capture_Data ca LEFT JOIN MyDevice dev " +
                    $"on ca.device_sn = dev.number where ca.time like '{day:yyyy-MM-dd}%' group by dev.IsEnter");
                foreach (dynamic item in res)
                {
                    var v = Convert.ToInt32(item.count);
                    switch (item.IsEnter)
                    {
                        case 1L:
                            i = v;
                            break;
                        case 0L:
                            o = v;
                            break;
                    }
                }
            }

            return (i, o);

        }

        public static dynamic[] getCaptureDataByIdForDate(string personId, DateTime date)
        {
            var sql = $"SELECT ca.closeup, ca.time, ca.body_temp, dev.DeviceName, dev.number, dev.IsEnter from Capture_Data ca LEFT JOIN MyDevice dev ON ca.device_sn = dev.number WHERE person_id = '{personId}' and time LIKE '{date:yyyy-MM-dd}%' ORDER BY time";
            using (var conn = SQLiteHelper.GetConnection())
            {
                return conn.Query(sql).ToArray();
            }
        }

        public static AccessRule AddAccessRule(string name, RepeatType repeatType)
        {
            using (var conn = SQLiteHelper.GetConnection())
            {
                var ar = new AccessRule { Name = name, RepeatType = repeatType };
                conn.Insert(ar);
                ar.Days = new List<Database.Day>();
                if (repeatType == RepeatType.RepeatByWeek)
                {
                    foreach (DayOfWeek dow in Enum.GetValues(typeof(DayOfWeek)))
                    {
                        var d = new Database.Day { DayOfWeek = dow, AccessRuleId = ar.Id, TimeSegments = new List<TimeSegment>() };
                        conn.Insert(d);
                        ar.Days.Add(d);
                    }
                }
                else
                {
                    var d = new Database.Day { DayOfWeek = DayOfWeek.Sunday, AccessRuleId = ar.Id, TimeSegments = new List<TimeSegment>() };
                    conn.Insert(d);
                    ar.Days.Add(d);
                }
                return ar;
            }
        }

        public static void RemoveAccessRuleById(int id)
        {
            using (var c = GetConnection())
            {
                var days = c.Query<Database.Day>($"SELECT * FROM Day WHERE AccessRuleId = {id}");
                foreach (var d in days)
                {
                    c.Execute($"DELETE FROM TimeSegment WHERE DayOfWeekId = {d.Id}");
                    c.Delete(d);
                }
                c.ExecuteScalar($"DELETE FROM AccessRule WHERE Id = {id}");
                c.Execute($"UPDATE RuleDistribution SET AccessRuleId = null WHERE AccessRuleId = {id}");
            }
        }

        public static Database.Day AddDayToAccessRule(int accessRuleId, DayOfWeek day)
        {
            using (var conn = SQLiteHelper.GetConnection())
            {
                var d = new Database.Day { DayOfWeek = day, AccessRuleId = accessRuleId };
                conn.Insert(d);
                return d;
            }
        }

        public static TimeSegment AddTimeSegmentToDay(int dayId, string from, string to)
        {
            using (var conn = SQLiteHelper.GetConnection())
            {
                var seg = new TimeSegment { DayOfWeekId = dayId, Start = from, End = to };
                conn.Insert(seg);
                return seg;
            }
        }

        public static void RemoveTimeSegmentById(int id)
        {
            using (var c = GetConnection())
            {
                var ts = c.Get<TimeSegment>(id);
                c.Delete(ts);
            }
        }

        public static AccessRule[] GetAllAccessRules()
        {
            var sql = "SELECT * FROM AccessRule AS ar LEFT JOIN Day AS d ON ar.Id = d.AccessRuleId LEFT JOIN TimeSegment AS ts ON ts.DayOfWeekId = d.Id;";
            using (var conn = GetConnection())
            {
                var accessRuleDictionary = new Dictionary<int, AccessRule>();
                var dayDictionary = new Dictionary<int, Database.Day>();

                return conn.Query<AccessRule, Database.Day, TimeSegment, AccessRule>(
                    sql,
                    (accessRule, day, timeSegment) =>
                    {
                        if (!accessRuleDictionary.TryGetValue(accessRule.Id, out var accessRuleEntry))
                        {
                            accessRuleEntry = accessRule;
                            accessRuleEntry.Days = new List<Database.Day>();
                            accessRuleDictionary.Add(accessRuleEntry.Id, accessRuleEntry);
                        }


                        if (day != null)
                        {
                            if (!dayDictionary.TryGetValue(day.Id, out var dayEntry))
                            {
                                dayEntry = day;
                                dayEntry.TimeSegments = new List<TimeSegment>();
                                dayDictionary.Add(dayEntry.Id, dayEntry);
                            }
                            if (timeSegment != null)
                            {
                                dayEntry.TimeSegments.Add(timeSegment);
                            }
                            if (!accessRuleEntry.Days.Contains(day))
                            {
                                accessRuleEntry.Days.Add(dayEntry);
                            }
                        }

                        return accessRuleEntry;
                    }
                    )
                    .Distinct()
                    .ToArray();
            }
        }

        public static RuleDistribution AddRuleDistribution(string name, DistributionItemType distributionItemType)
        {
            using (var c = GetConnection())
            {
                var rd = new RuleDistribution() { Name = name, DistributionItemType = distributionItemType };
                c.Insert(rd);
                return rd;
            }
        }

        public static RuleDistribution GetRuleDistributionById(int id)
        {
            using (var c = GetConnection())
            {
                return c.Get<RuleDistribution>(id);
            }
        }

        public static RuleDistributionDevice AddDeviceToRuleDistribution(int distributionId, int deviceId)
        {
            using (var c = GetConnection())
            {
                var d = c.Get<MyDevice>(deviceId);
                var rdd = new RuleDistributionDevice { RuleDistributionId = distributionId, DeviceId = deviceId, Name = d.DeviceName };
                c.Insert(rdd);
                return rdd;
            }
        }

        public static RuleDistributionItem AddStaffToRuleDistribution(int distributionId, string staffId)
        {
            using (var c = GetConnection())
            {
                var staff = c.Get<Staff>(staffId);
                var rdi = new RuleDistributionItem { StaffId = staffId, Name = staff.name, RuleDistributionId = distributionId };
                c.Insert(rdi);
                return rdi;
            }
        }

        public static void RemoveRuleDistributionItem(int id)
        {
            using (var c = GetConnection())
            {
                c.ExecuteScalar($"DELETE FROM RuleDistributionItem WHERE Id = {id}");
            }
        }

        public static void RemoveRuleDistributionDevice(int id)
        {
            using (var c = GetConnection())
            {
                c.ExecuteScalar($"DELETE FROM RuleDistributionDevice WHERE Id = {id}");
            }
        }

        public static RuleDistributionItem AddGroupToRuleDistribution(int distributionId, int groupId, GroupIdType groupIdType)
        {
            using (var c = GetConnection())
            {
                var name = string.Empty;
                switch (groupIdType)
                {
                    case GroupIdType.EmployeeType:
                        var et = c.Get<Employetype>(groupId);
                        name = et.Employetype_name;
                        break;
                    case GroupIdType.Department:
                        var dp = c.Get<Department>(groupId);
                        name = dp.name;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                var item = new RuleDistributionItem { GroupId = groupId, Name = name, GroupType = groupIdType, RuleDistributionId = distributionId };
                c.Insert(item);
                return item;
            }
        }

        public static void SetAccessRuleToDistribution(int distributionId, int accessRuleId)
        {
            using (var c = GetConnection())
            {
                var d = c.Get<RuleDistribution>(distributionId);
                d.AccessRuleId = accessRuleId;
                c.Update(d);
            }
        }

        public static RuleDistribution[] GetAllRuleDistribution()
        {
            var sql = "SELECT * FROM RuleDistribution AS rd LEFT JOIN RuleDistributionDevice AS rdd ON rd.Id = rdd.RuleDistributionId LEFT JOIN RuleDistributionItem AS rdi ON rdi.RuleDistributionId  = rd.Id;";
            using (var conn = GetConnection())
            {
                var ruleDistributionDictionary = new Dictionary<int, RuleDistribution>();
                var all = conn.Query<RuleDistribution, RuleDistributionDevice, RuleDistributionItem, RuleDistribution>(
                    sql,
                    (ruleDistribution, distributionDevice, distributionItem) =>
                    {
                        if (!ruleDistributionDictionary.TryGetValue(ruleDistribution.Id, out var ruleDistEntry))
                        {
                            ruleDistEntry = ruleDistribution;
                            ruleDistEntry.Items = new List<RuleDistributionItem>();
                            ruleDistEntry.Devices = new List<RuleDistributionDevice>();
                            ruleDistributionDictionary.Add(ruleDistEntry.Id, ruleDistEntry);
                        }

                        if (distributionItem != null && !ruleDistEntry.Items.Contains(distributionItem))
                        {
                            ruleDistEntry.Items.Add(distributionItem);
                        }

                        if (distributionDevice != null && !ruleDistEntry.Devices.Contains(distributionDevice))
                        {
                            ruleDistEntry.Devices.Add(distributionDevice);
                        }
                        return ruleDistEntry;
                    }
                    );
                var res = all
                    .Distinct()
                    .ToArray();
                return res;
            }

        }

        public static Department[] getAllDepartment()
        {
            using (var c = GetConnection())
            {
                var dps = c.GetAll<Department>();
                return dps.ToArray();
            }
        }

        public static Employetype[] getAllEmployeeType()
        {
            using (var c = GetConnection())
            {
                var et = c.GetAll<Employetype>().Where(x=>!string.IsNullOrEmpty(x.Employetype_name));
                return et.ToArray();
            }
        }

        public static void RemoveDistribution(int Id)
        {
            using (var c = GetConnection())
            {
                var dist = c.Get<RuleDistribution>(Id);
                c.ExecuteScalar($"DELETE FROM RuleDistributionItem WHERE RuleDistributionId = {Id}");
                c.ExecuteScalar($"DELETE FROM RuleDistributionDevice WHERE RuleDistributionId = {Id}");
                c.Delete(dist);
            }
        }

        //模糊查询人名字
        public static Staff[] GetStaffByNameFuzzy(string query)
        {
            using (var c = GetConnection())
            {
                var staffs = c.Query<Staff>($"SELECT * FROM staff WHERE name LIKE '%{query}%' LIMIT 10");
                return staffs.ToArray();
            }
        }

        public static Staff[] GetAllStaffs()
        {
            using (var c = GetConnection())
            {
                var staffs = c.GetAll<Staff>();
                return staffs.ToArray();
            }
        }
    }
}

using DBUtility.SQLite;
using HaSdkWrapper;
using huaanClient.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Dynamic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

namespace huaanClient
{
    class DistributeToequipment
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static List<Task> taskList = new List<Task>();
        public static void distrbute()
        {
            var startTime = DateTime.Now;
            Logger.Info("开始下发...");
            //string connectionString = "Data Source=" + Application.StartupPath + @"\huaanDatabase.sqlite;Version=3;";
            string connectionString = ApplicationData.connectionString;
            string commandText = "SELECT * FROM Equipment_distribution WHERE status <> 'success' AND type != 2 limit 500";
            string sr = SQLiteHelper.SQLiteDataReader(connectionString, commandText);
            
            if (!string.IsNullOrEmpty(sr))
            {
                JArray srjo = (JArray)JsonConvert.DeserializeObject(sr);
                if (srjo.Count>0)
                {
                    foreach (JObject jo in srjo)
                    {
                        var t = Task.Run(() => {
                        try
                        {
                              handleOneDistribute(jo, connectionString);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "process distribution exception");
                        }
                        });
                        taskList.Add(t);
                    }
                }
            }
            Task.WaitAll(taskList.ToArray());
            taskList.Clear();
            var endTime = DateTime.Now;
            Logger.Info("下发结束!用时{0}",(endTime-startTime).ToString());
        }


        private static void handleOneDistribute(JObject distribute, string connectionString)
        {
            //type 0 下发 1删除 2异常
            if (distribute["type"].ToString().Trim().Equals("0") && !distribute["status"].ToString().Trim().Equals("success"))
            {
                string userid = distribute["userid"].ToString().Trim();
                string id = distribute["id"].ToString().Trim();
                string deviceid = distribute["deviceid"].ToString().Trim();

                string downid = userid;
                //获取对应的名字和IP地址
                string sql = $"SELECT staff.face_idcard,staff.source,staff.Employee_code,staff.idcardtype,staff.name,staff.picture,MyDevice.ipAddress,customer_text,department_id,term_start,term FROM staff LEFT JOIN MyDevice WHERE staff.id = '{userid}' AND MyDevice.id={deviceid}";
                string sqldata = SQLiteHelper.SQLiteDataReader(connectionString, sql);
                JArray sqldatajo = (JArray)JsonConvert.DeserializeObject(sqldata);
                var distroParams = sqldatajo.FirstOrDefault() as JObject;

                if (GetData.getIscode_syn())
                {
                    downid = sqldatajo[0]["Employee_code"].ToString().Trim();
                }

                if (distroParams != null)
                {
                    innerHandleDistribute(distribute, connectionString, id, downid, distroParams);
                }
            }
            else if (distribute["type"].ToString().Trim().Equals("1") && !distribute["status"].ToString().Trim().Equals("success"))
            {
                DeleteDistribute(distribute, connectionString);
            }

        }

        private static void DeleteDistribute(JObject distribute, string connectionString)
        {
            var staffId = distribute["userid"].ToString().Trim();
            var distroId = (int) distribute["id"];
            var employeeCode = distribute["employeeCode"].ToString();
            var isDistributeByCode = (int) distribute["isDistributedByEmployeeCode"];

            var Devicelistdata = Deviceinfo.MyDevicelist;
            Devicelistdata.ForEach(s =>
            {
                if (s.IsConnected)
                {
                    JObject deleteJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.deleteJson);
                    if (deleteJson != null)
                    {
                        deleteJson["id"] = isDistributeByCode == 1 ? employeeCode : staffId ;
                    }
                    string restr = GetDevinfo.request(s, deleteJson.ToString());
                    JObject restr_json = (JObject)JsonConvert.DeserializeObject(restr.Trim());
                    if (restr_json != null)
                    {
                        string code = restr_json["code"].ToString();
                        int code_int = int.Parse(code);
                        if (code_int == 0 || code_int == 22)
                        {
                            using (var conn = SQLiteHelper.GetConnection())
                            {
                                var distro = new EquipmentDistribution()
                                {
                                    id = distroId,
                                    type = "1",
                                    status = "success",
                                    date = DateTime.Now
                                };
                                conn.Execute("UPDATE Equipment_distribution " +
                                    "SET type = @type, status = @status, date = @date where id = @id", distro);
                            }
                        }
                    }
                }
            });
        }

        private static void DeleteDistributeOld(JObject distribute, string connectionString)
        {
            string id = distribute["userid"].ToString().Trim();
            if (GetData.getIscode_syn())
            {
                //获取对应的名字和IP地址
                string sql = "SELECT staff.Employee_code FROM staff  WHERE staff.id=" + distribute["userid"].ToString();
                string sqldata = SQLiteHelper.SQLiteDataReader(connectionString, sql);
                JArray sqldatajo = (JArray)JsonConvert.DeserializeObject(sqldata);

                id = sqldatajo[0]["Employee_code"].ToString().Trim();
            }


            JObject deleteJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.deleteJson);
            if (deleteJson != null)
            {
                deleteJson["id"] = id;
            }
            List<CameraConfigPort> Devicelistdata = Deviceinfo.MyDevicelist;
            Devicelistdata.ForEach(s =>
            {
                if (s.IsConnected)
                {
                    string restr = GetDevinfo.request(s, deleteJson.ToString());
                    JObject restr_json = (JObject)JsonConvert.DeserializeObject(restr.Trim());
                    if (restr_json != null)
                    {
                        string code = restr_json["code"].ToString();
                        int code_int = int.Parse(code);
                        if (code_int == 0 || code_int == 22)
                        {
                            GetData.ubpdateEquipment_distributionfordel(distribute["userid"].ToString());
                        }
                    }
                }
            });
        }

        private static void innerHandleDistribute(JObject distribute, string connectionString, string id,  string downid, JObject distributeParams)
        {
            /*JObject PersonJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.PersonJson)*/
            ;
            string PersonJson = string.Empty;
            string ip = distributeParams["ipAddress"].ToString().Trim();
            string source = distributeParams["source"].ToString().Trim();
            string term_start= distributeParams["term_start"].ToString().Trim().Length>1? distributeParams["term_start"].ToString().Replace("-", "/").Trim():"useless";
            string term = distributeParams["term"].ToString().Trim().Length>1? distributeParams["term"].ToString().Replace("-","/").Trim():"forever";
            CameraConfigPort CameraConfigPortlist = Deviceinfo.MyDevicelist.Find(d => d.IP == ip);
            if (CameraConfigPortlist == null)
                return;
            if (CameraConfigPortlist.IsConnected)
            {
                lock (CameraConfigPortlist)
                {
                    Console.WriteLine("下发id:{0}，相机IP:{1},时间：{2}", id, CameraConfigPortlist.IP, DateTime.Now.ToString());
                    if (PersonJson != null)
                    {
                        //PersonJson["id"] = userid;
                        //PersonJson["name"] = sqldatajo[0]["name"].ToString().Trim();

                        string thumb, twis, reg_images = string.Empty, norm_images = string.Empty;
                        var picturePath = distributeParams["picture"]?.ToString();

                        //自定义字段
                        string customer_text = Properties.Strings.DefaultCustomerText;
                        if (string.IsNullOrEmpty(distributeParams["customer_text"]?.ToString()))
                        {
                            //如果customer_text没有值则将部门作为customer_text
                            string departmentId = distributeParams["department_id"]?.ToString();
                            if (!string.IsNullOrEmpty(departmentId) && departmentId != "0")
                            {
                                string sql = $"SELECT name FROM department WHERE department.id = '{departmentId}'";
                                string sqldata = SQLiteHelper.SQLiteDataReader(connectionString, sql);
                                JArray sqldatajo = (JArray)JsonConvert.DeserializeObject(sqldata);
                                var departmentInfo = sqldatajo.FirstOrDefault() as JObject;
                                string departmentName = departmentInfo["name"]?.ToString();
                                if (!string.IsNullOrEmpty(departmentName) && departmentName.Length < 23)
                                    customer_text = departmentName;
                            }
                        }
                        else
                        {
                            customer_text = distributeParams["customer_text"].ToString().Trim();
                        }


                        //判断图片是否存在 如果不存在直接更新信息
                        if (string.IsNullOrEmpty(picturePath))
                        {
                            dynamic o = new ExpandoObject();
                            o.version = "0.2";
                            o.cmd = "upload person";
                            o.id = downid;
                            o.name = distributeParams["name"].ToString().Trim();
                            o.customer_text = customer_text;
                            o.term_start = term_start;
                            o.term = term;

                            var idCardType = distributeParams["idcardtype"].Value<string>();
                            var idCard = distributeParams["face_idcard"].Value<string>();
                            if (!string.IsNullOrEmpty(idCardType) && !string.IsNullOrEmpty(idCard))
                            {
                                var idNumber = Convert.ToUInt64(idCard);
                                if (idCardType == "64")
                                {
                                    o.long_card_id = idNumber;
                                }
                                else
                                {
                                    o.wg_card_id = idNumber;
                                }

                            }

                            PersonJson = JsonConvert.SerializeObject(o);
                        }
                        else
                        {
                            if (!File.Exists(picturePath))
                            {
                                string updatessql = $"UPDATE Equipment_distribution SET status='fail', type='2', errMsg='{Properties.Strings.ImageMissing}', date='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE id={id}";
                                SQLiteHelper.ExecuteNonQuery(connectionString, updatessql);
                                return;

                            }
                            else
                            {
                                //来源于设备同步
                                if (Regex.IsMatch(source, "^.{6,6}-.{6,6}-.{6,6}$"))
                                {

                                    string ss = distributeParams["picture"].ToString().Trim();
                                    string ss1 = distributeParams["picture"].ToString().Trim().Substring(0, distributeParams["picture"].ToString().Trim().Length - 4)
                                        + "reg_images" + ".jpg";
                                    thumb = Convert.ToBase64String(File.ReadAllBytes(distributeParams["picture"].ToString().Trim()));
                                    twis = Convert.ToBase64String(File.ReadAllBytes(distributeParams["picture"].ToString().Trim().Substring(0, distributeParams["picture"].ToString().Trim().Length - 4)
                                        + "reg_images" + ".jpg"));
                                    if (File.ReadAllBytes(distributeParams["picture"].ToString().Trim()).Length == 112 * 112 * 3)
                                    {
                                        norm_images = string.Format("{{\"width\": 112,\"height\": 112,\"image_data\":\"{0}\"}}", thumb);
                                    }
                                    else
                                        norm_images = string.Format("{{\"width\": 150,\"height\": 150,\"image_data\":\"{0}\"}}", thumb);
                                    reg_images = string.Format("{{\"format\": \"jpg\",\"image_data\":\"{0}\"}}", twis);
                                }
                                else
                                {
                                    //将图片转换成符合相机需求
                                    if (twistImageCore(File.ReadAllBytes(distributeParams["picture"].ToString().Trim()), CameraConfigPortlist.DevicVersion, out thumb, out twis, out bool IsNew))
                                    {
                                        reg_images = string.Format("{{\"format\": \"jpg\",\"image_data\":\"{0}\"}}", thumb);

                                        if (IsNew)
                                        {
                                            norm_images = string.Format("{{\"width\": 112,\"height\": 112,\"image_data\":\"{0}\"}}", twis);
                                        }
                                        else
                                            norm_images = string.Format("{{\"width\": 150,\"height\": 150,\"image_data\":\"{0}\"}}", twis);

                                    }
                                }
                                if (distributeParams["idcardtype"].ToString().Trim() == "64")
                                {
                                    PersonJson = string.Format(UtilsJson.PersonJson64, downid, distributeParams["name"].ToString().Trim(), reg_images, norm_images, distributeParams["face_idcard"].ToString().Trim(), customer_text, term_start, term);
                                }
                                else if (distributeParams["idcardtype"].ToString().Trim() == "32")
                                {
                                    PersonJson = string.Format(UtilsJson.PersonJson32, downid, distributeParams["name"].ToString().Trim(), reg_images, norm_images, distributeParams["face_idcard"].ToString().Trim(), customer_text, term_start, term);
                                }
                                else
                                {
                                    PersonJson = string.Format(UtilsJson.PersonJson, downid, distributeParams["name"].ToString().Trim(), reg_images, norm_images, customer_text, term_start, term);
                                }

                            }


                        }

                        //string imgebase64str = ReadImageFile(sqldatajo[0]["picture"].ToString().Trim());
                        //PersonJson["reg_images"][0]["image_data"] = imgebase64str;


                    }

                    JObject deleteJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.deleteJson);
                    if (deleteJson != null)
                    {
                        deleteJson["id"] = downid;
                    }
                    //先执行删除操作
                    string sss = GetDevinfo.request(CameraConfigPortlist, deleteJson.ToString());
                    //在执行下发操作
                    string restr = GetDevinfo.request(CameraConfigPortlist, PersonJson);
                    JObject restr_json = (JObject)JsonConvert.DeserializeObject(restr.Trim());
                    lock (Logger)//更新数据库需要阻塞，避免数据库lock
                    {
                        if (restr_json != null)
                        {
                            string code = restr_json["code"].ToString();
                            int code_int = int.Parse(code);
                            if (code_int == 0)
                            {
                                string updatessql = "UPDATE Equipment_distribution SET errMsg='', status='success',date='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE id=" + id;
                                SQLiteHelper.ExecuteNonQuery(connectionString, updatessql);
                            }
                            else
                            {
                                string updatessql = "UPDATE Equipment_distribution SET status='fail', type='2', code='" + code_int + "',date='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE id=" + id;
                                SQLiteHelper.ExecuteNonQuery(connectionString, updatessql);
                            }
                            //else if (code_int == 35 || code_int == 36 || code_int == 37 || code_int == 38 || code_int == 39 || code_int == 40 || code_int == 41)
                            //{
                            //    obj["data"] = "照片不合格";
                            //}
                        }
                        else
                        {
                            lock (Logger)
                            {
                                Logger.Warn("{0}下发失败，人员信息：{1}",ip, PersonJson);
                                string updatessql = "UPDATE Equipment_distribution SET status='fail',date='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE id=" + id;
                                SQLiteHelper.ExecuteNonQuery(connectionString, updatessql);
                            }
                        }
                    }
                }
            }
            else
            {
                string updatessql = $"UPDATE Equipment_distribution SET status='fail', errMsg='{Properties.Strings.DeviceOffline}', date='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE id={id}";
                SQLiteHelper.ExecuteNonQuery(connectionString, updatessql);
            }
        }


        public static void distributeNew()
        {
            //bin
            List<EquipmentDistribution> distributions = new List<EquipmentDistribution>();
            using (var db = SQLiteHelper.GetConnection())
            {
                distributions = db.Query<EquipmentDistribution>($"select * from {DbConstants.TableEquipementDistribution} where status != \"{DbConstants.success}\"")
                    .ToList();
            }

            foreach (var distribute in distributions)
            {
                if (distribute.type == "0" && distribute.status != DbConstants.success)
                {
                    MyDevice deviceInfo = null;
                    Staff staff = null;
                    using (var conn = SQLiteHelper.GetConnection())
                    {
                        var cmd = $"select * from {DbConstants.TableStaff} where id = @StaffId; select * from {DbConstants.TableMyDevice} where id = @MyDeviceId;";
                        var multi = conn.QueryMultiple(cmd, new { StaffId = distribute.userid, MyDeviceId = distribute.deviceid });
                        staff = multi.Read<Staff>().FirstOrDefault();
                        deviceInfo = multi.Read<MyDevice>().FirstOrDefault();
                    }

                    string distributeId = distribute.userid.ToString();
                    if (GetData.getIscode_syn())
                    {
                        distributeId = staff.Employee_code;
                    }

                    if (staff != null && deviceInfo != null)
                    {
                        var device = Deviceinfo.MyDevicelist.FirstOrDefault(x => x.IP == deviceInfo.ipAddress);
                        if (device?.IsConnected == true)
                        {
                            if (!File.Exists(staff.picture))
                            {
                                distribute.type = "2";
                                distribute.status = "fail";
                                distribute.date = DateTime.Now;//.ToString(DbConstants.dt_format);
                                var cmd = $"UPDATE {DbConstants.TableEquipementDistribution} SET type = @type, status = @status, date = @date WHERE id = @id";
                                UpdateDistribution(distribute, cmd);
                                continue;
                            }


                        }
                    }

                }
                else if (distribute.type == "1" && distribute.status != DbConstants.success)
                {

                }

            }

        }


        private static void UpdateDistribution(EquipmentDistribution distribute, string cmd)
        {
            using (var conn = SQLiteHelper.GetConnection())
            {
                conn.Execute(cmd, distribute);
            }
        }

        private static string PrepareJson(string downid, JToken sqldatajo, string reg_images, string norm_images)
        {
            string PersonJson;
            if (sqldatajo["idcardtype"].ToString().Trim() == "64")
            {
                PersonJson = string.Format(UtilsJson.PersonJson64, downid, sqldatajo["name"].ToString().Trim(), reg_images, norm_images, sqldatajo[0]["face_idcard"].ToString().Trim());
            }
            else if (sqldatajo["idcardtype"].ToString().Trim() == "32")
            {
                PersonJson = string.Format(UtilsJson.PersonJson32, downid, sqldatajo["name"].ToString().Trim(), reg_images, norm_images, sqldatajo[0]["face_idcard"].ToString().Trim());
            }
            else
            {
                PersonJson = string.Format(UtilsJson.PersonJson, downid, sqldatajo["name"].ToString().Trim(), reg_images, norm_images);
            }

            return PersonJson;
        }

        private static void PrepareImages(string imagePath, string source, CameraConfigPort CameraConfigPortlist, 
            out string thumb, 
            out string twis, 
            out string reg_images, 
            out string norm_images)
        {
            reg_images = string.Empty;
            norm_images = string.Empty;

            if (source.Length > 4)
            {

                string ss = imagePath;
                string ss1 = imagePath.Substring(0, imagePath.Length - 4)
                    + "reg_images" + ".jpg";
                thumb = Convert.ToBase64String(File.ReadAllBytes(imagePath));
                twis = Convert.ToBase64String(File.ReadAllBytes(imagePath.Substring(0, imagePath.Length - 4)
                    + "reg_images" + ".jpg"));
                if (File.ReadAllBytes(imagePath).Length == 112 * 112 * 3)
                {
                    norm_images = string.Format("{{\"width\": 112,\"height\": 112,\"image_data\":\"{0}\"}}", thumb);
                }
                else
                    norm_images = string.Format("{{\"width\": 150,\"height\": 150,\"image_data\":\"{0}\"}}", thumb);
                reg_images = string.Format("{{\"format\": \"jpg\",\"image_data\":\"{0}\"}}", twis);
            }
            else
            {
                //将图片转换成符合相机需求
                if (twistImageCore(File.ReadAllBytes(imagePath), CameraConfigPortlist.DevicVersion, out thumb, out twis, out bool IsNew))
                {
                    reg_images = string.Format("{{\"format\": \"jpg\",\"image_data\":\"{0}\"}}", thumb);

                    if (IsNew)
                    {
                        norm_images = string.Format("{{\"width\": 112,\"height\": 112,\"image_data\":\"{0}\"}}", twis);
                    }
                    else
                        norm_images = string.Format("{{\"width\": 150,\"height\": 150,\"image_data\":\"{0}\"}}", twis);

                }
            }
        }

        public static bool distrbute(string name,string imgeurl,string statime,string endtime,string id)
        {
            statime= statime.Replace("-", "/").Trim()+ ":00";
            endtime = endtime.Replace("-", "/").Trim() + ":00";
            bool re = false;
            if (string.IsNullOrEmpty(imgeurl)|| Deviceinfo.MyDevicelist.Count==0 || string.IsNullOrEmpty(id))
            {
                return re;
            }
            Deviceinfo.MyDevicelist.ForEach(d => {
                if (d.IsConnected==true)
                {
                    string PersonJson = string.Empty;
                    string thumb, twis, reg_images = string.Empty, norm_images = string.Empty;


                    //将图片转换成符合相机需求
                    if (twistImageCore(File.ReadAllBytes(imgeurl.Trim()), d.DevicVersion, out thumb, out twis, out bool IsNew))
                    {
                        reg_images = string.Format("{{\"format\": \"jpg\",\"image_data\":\"{0}\"}}", thumb);

                        if (IsNew)
                        {
                            norm_images = string.Format("{{\"width\": 112,\"height\": 112,\"image_data\":\"{0}\"}}", twis);
                        }
                        else
                            norm_images = string.Format("{{\"width\": 150,\"height\": 150,\"image_data\":\"{0}\"}}", twis);
                    }

                    PersonJson = string.Format(UtilsJson.PersonJsonforterm, id, name.Trim(), reg_images, norm_images, endtime,statime);

                    JObject deleteJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.deleteJson);
                    if (deleteJson != null)
                    {
                        deleteJson["id"] = id;
                    }
                    //先执行删除操作
                    string sss = GetDevinfo.request(d, deleteJson.ToString());
                    //在执行下发操作
                    string restr = GetDevinfo.request(d, PersonJson);
                    JObject restr_json = (JObject)JsonConvert.DeserializeObject(restr.Trim());
                    if (restr_json != null)
                    {
                        string code = restr_json["code"].ToString();
                        int code_int = int.Parse(code);
                        if (code_int != 0)
                        {
                            re = false;
                        }
                        else
                        {
                            re = true;
                            string updatessql = "UPDATE Visitor SET isDown='1' WHERE id=" + id;
                            SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
                        }
                    } 
                }
            });
            return re;
        }

        public static void delVisitorforId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }
            string[] s = id.Split(',');
            if (s.Length>0)
            {
                for (int i=0;i< s.Length;i++)
                {
                    if (!string.IsNullOrEmpty(s[i]))
                    {
                        Deviceinfo.MyDevicelist.ForEach(d => {
                            if (d.IsConnected == true)
                            {
                                JObject deleteJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.deleteJson);
                                if (deleteJson != null)
                                {
                                    deleteJson["id"] = s[i].Replace(",", "");
                                }
                                //先执行删除操作
                                GetDevinfo.request(d, deleteJson.ToString());
                            }
                        });
                    }  
                }
            }
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

        //判断本地文件是否存在
        public static bool IsExis(string rul)
        {
            if (System.IO.File.Exists(@rul))
                return true;
            else return false;
        }

        public static bool twistImageCore(byte[] imageData,string twist_version, out string thumb, out string twist,out bool IsNew)
        {
            thumb = null;
            twist = null;
            IsNew = false;
            byte[][] objs = HaCamera.HA_GetJpgFeatureImageNew(imageData);
            if (objs[2][0] != 0) return false;

            thumb = Convert.ToBase64String(objs[0]);


            byte[][] twist_byte = null;
            if (string.IsNullOrEmpty(twist_version))
            {
                twist_byte = HaCamera.HA_GetJpgFeatureImageNew(imageData, "hv09");
                twist = Convert.ToBase64String(twist_byte[1]);
            }
            else
            {
                twist_byte = HaCamera.HA_GetJpgFeatureImageNew(imageData, twist_version);
                twist = Convert.ToBase64String(twist_byte[1]);
            }

            if (twist_byte[1].Length==112*112*3)
            {
                IsNew = true;
            }
            return true;
        }
    }
}

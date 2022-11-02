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
        private readonly static ManualResetEvent signal = new ManualResetEvent(true);

        public static void Wait()
        {
            signal.WaitOne(TimeSpan.FromMinutes(5));
        }

        public static void Wakeup()
        {
            signal.Set();
        }

        public static void Sleep()
        {
            signal.Reset();
        }

        public static void distrbute(CancellationToken token)
        {
            var startTime = DateTime.Now;
            Logger.Debug("开始查询待下发数据...");
            string connectionString = ApplicationData.connectionString;
            string commandText = "SELECT * FROM Equipment_distribution WHERE status <> 'success' AND type != 2 ORDER BY userid ASC limit 500";
            string sr = SQLiteHelper.SQLiteDataReader(connectionString, commandText);

            if (!string.IsNullOrEmpty(sr))
            {
                JArray srjo = (JArray)JsonConvert.DeserializeObject(sr);
                if (srjo.Count > 0)
                {
                    var results = new List<FaceDeploymentResult>();
                    foreach (JObject jo in srjo)
                    {
                        if (token.IsCancellationRequested)
                        {
                            Logger.Debug("取消下发");
                            break;
                        }
                        var staffDistribution = jo.ToObject<EquipmentDistribution>();
                        var msg = $"开始下发staff({staffDistribution.userid})到device({staffDistribution.deviceid})";
                        try
                        {
                            Logger.Debug(msg);
                            var res = handleOneDistribute(jo, connectionString);
                            results.Add(res);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, msg);
                        }
                    }

                    if (!token.IsCancellationRequested)
                    {
                        if (results.All(x => x == FaceDeploymentResult.DeviceOffline)
                         || results.All(x => x == FaceDeploymentResult.Timeout))
                        {
                            Thread.Sleep(60 * 1000);
                        }
                        else
                        {
                            Thread.Sleep(10 * 1000);
                        }
                    }
                }
                else
                {
                    Logger.Info("没有待下发数据,下发线程开始睡眠");
                    Sleep();
                }
            }

            var endTime = DateTime.Now;
            Logger.Debug("下发结束!用时{0}", (endTime - startTime).ToString());
        }


        private static FaceDeploymentResult handleOneDistribute(JObject distribute, string connectionString)
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
                    return innerHandleDistribute(distribute, connectionString, id, downid, distroParams);
                }

                return FaceDeploymentResult.Failed;
            }
            else if (distribute["type"].ToString().Trim().Equals("1") && !distribute["status"].ToString().Trim().Equals("success"))
            {
                DeleteDistribute(distribute, connectionString);
                return FaceDeploymentResult.Success;
            }

            return FaceDeploymentResult.Success;

        }

        private static void DeleteDistribute(JObject distribute, string connectionString)
        {
            var deviceId = distribute["deviceid"].ToString().Trim();
            var staffId = distribute["userid"].ToString().Trim();
            var distroId = (int)distribute["id"];
            var employeeCode = distribute["employeeCode"].ToString();
            var isDistributeByCode = (int)distribute["isDistributedByEmployeeCode"];

            var Devicelistdata = Deviceinfo.GetAllMyDevices();
            Array.ForEach(Devicelistdata, s =>
            {
                if (s.IsConnected && s.Deviceid == deviceId)
                {
                    JObject deleteJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.deleteJson);
                    if (deleteJson != null)
                    {
                        deleteJson["id"] = isDistributeByCode == 1 ? employeeCode : staffId;
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
            var Devicelistdata = Deviceinfo.GetAllMyDevices();
            Array.ForEach(Devicelistdata, s =>
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

        private static FaceDeploymentResult innerHandleDistribute(JObject distribute, string connectionString, string id, string downid, JObject distributeParams)
        {
            /*JObject PersonJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.PersonJson)*/
            var uploadPersonCmd = UtilsJson.UploadPersonCmd;
            string ip = distributeParams["ipAddress"].ToString().Trim();
            string source = distributeParams["source"].ToString().Trim();
            string term_start = distributeParams["term_start"].ToString().Trim().Length > 1 ? distributeParams["term_start"].ToString().Replace("-", "/").Trim() : "useless";
            string term = distributeParams["term"].ToString().Trim().Length > 1 ? distributeParams["term"].ToString().Replace("-", "/").Trim() : "forever";
            var CameraConfigPortlist = Deviceinfo.GetByIp(ip);
            if (CameraConfigPortlist == null)
                return FaceDeploymentResult.Failed;
            Logger.Debug("开始下发id:{0}，相机IP:{1},人员ID：{2}", id, CameraConfigPortlist.IP, distribute["userid"]);

            if (CameraConfigPortlist.IsConnected)
            {

                //PersonJson["id"] = userid;
                //PersonJson["name"] = sqldatajo[0]["name"].ToString().Trim();


                //自定义字段
                string customer_text = distributeParams["customer_text"]?.ToString();
                if (string.IsNullOrEmpty(customer_text))
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


                uploadPersonCmd[UtilsJson.UPLOAD_PERSON_FIELD_ID] = downid;
                uploadPersonCmd[UtilsJson.UPLOAD_PERSON_FIELD_NAME] = distributeParams["name"].ToString().Trim();
                if (customer_text != "")
                {
                    uploadPersonCmd[UtilsJson.UPLOAD_PERSON_FIELD_CUSTOMER_TEXT] = customer_text;
                }
                uploadPersonCmd[UtilsJson.UPLOAD_PERSON_FIELD_TERM_START] = term_start;
                uploadPersonCmd[UtilsJson.UPLOAD_PERSON_FIELD_TERM] = term;


                var idCardType = distributeParams["idcardtype"].Value<string>();
                var idCard = distributeParams["face_idcard"].Value<string>();
                if (!string.IsNullOrEmpty(idCardType) && !string.IsNullOrEmpty(idCard))
                {
                    var idNumber = Convert.ToUInt64(idCard);
                    if (idCardType == "64")
                    {
                        uploadPersonCmd[UtilsJson.UPLOAD_PERSON_FIELD_LONG_WG_CARD_ID] = idNumber;
                    }
                    else
                    {
                        uploadPersonCmd[UtilsJson.UPLOAD_PERSON_FIELD_WG_CARD_ID] = idNumber;
                    }

                }

                var picturePath = distributeParams["picture"]?.ToString();

                if (!string.IsNullOrEmpty(picturePath))
                {

                    if (!File.Exists(picturePath))
                    {
                        string updatessql = $"UPDATE Equipment_distribution SET status='fail', type='2', errMsg='{Properties.Strings.ImageMissing}', date='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE id={id}";
                        SQLiteHelper.ExecuteNonQuery(connectionString, updatessql);
                        return FaceDeploymentResult.Failed;

                    }
                    else
                    {
                        string reg_images = null;
                        //来源于设备同步
                        var regImgPath = picturePath.Substring(0, picturePath.Length - 4)
                                + "reg_images" + ".jpg";
                        if (File.Exists(regImgPath))
                        {
                            reg_images = Convert.ToBase64String(File.ReadAllBytes(regImgPath));
                        }
                        else
                        {
                            if (Tools.TryDownscaleImage(picturePath, out var array, Properties.Settings.Default.resizeImageWidthTo))
                            {
                                reg_images = Convert.ToBase64String(array);
                            }
                            else
                            {
                                reg_images = Convert.ToBase64String(File.ReadAllBytes(picturePath));
                            }
                        }

                        if (!string.IsNullOrEmpty(reg_images))
                        {
                            uploadPersonCmd[UtilsJson.UPLOAD_PERSON_FIELD_REG_IMAGE] = reg_images;
                        }
                    }
                }

                //string imgebase64str = ReadImageFile(sqldatajo[0]["picture"].ToString().Trim());
                //PersonJson["reg_images"][0]["image_data"] = imgebase64str;


                //JObject deleteJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.deleteJson);
                //if (deleteJson != null)
                //{
                //    deleteJson["id"] = downid;
                //}
                //先执行删除操作
                //string sss = GetDevinfo.request(CameraConfigPortlist, deleteJson.ToString());
                //在执行下发操作
                var json = JsonConvert.SerializeObject(uploadPersonCmd);
                string restr = GetDevinfo.request(CameraConfigPortlist, json);
                JObject restr_json = (JObject)JsonConvert.DeserializeObject(restr.Trim());
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
                    var msg = code_int == 0 ? "成功" : "失败";
                    Logger.Debug($"开始下发id:{0}，相机IP:{1},人员ID：{2} {msg}", id, CameraConfigPortlist.IP, distribute["userid"]);
                    if (Properties.Settings.Default.saveFailedImage && code_int != 0)
                    {
                        var array = Convert.FromBase64String(uploadPersonCmd[UtilsJson.UPLOAD_PERSON_FIELD_REG_IMAGE]?.Value<string>());
                        if (array != null)
                        {
                            var name = uploadPersonCmd[UtilsJson.UPLOAD_PERSON_FIELD_NAME];
                            var fullPath = Tools.GetFullPathForFile($@"failedImage\{name}-{code_int}.jpg");
                            File.WriteAllBytes(fullPath, array);
                        }
                    }
                    return code_int == 0 ? FaceDeploymentResult.Success : FaceDeploymentResult.Failed;
                }
                else
                {
                    Logger.Debug("{0}下发失败，人员信息：{1} 超时", ip, uploadPersonCmd);
                    if (Properties.Settings.Default.saveFailedImage)
                    {
                        var array = Convert.FromBase64String(uploadPersonCmd[UtilsJson.UPLOAD_PERSON_FIELD_REG_IMAGE]?.Value<string>());
                        if (array != null)
                        {
                            var name = uploadPersonCmd[UtilsJson.UPLOAD_PERSON_FIELD_NAME];
                            var fullPath = Tools.GetFullPathForFile($@"failedImage\{name}-超时.jpg");
                            File.WriteAllBytes(fullPath, array);
                        }
                    }

                    string updatessql = $"UPDATE Equipment_distribution SET status='fail', errMsg='{Properties.Strings.TimeOut}', date='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE id={id}";
                    SQLiteHelper.ExecuteNonQuery(connectionString, updatessql);
                    return FaceDeploymentResult.Timeout;
                }
            }
            else
            {
                Logger.Debug("下发失败 id:{0}，相机IP:{1},人员ID：{2} 设备离线", id, CameraConfigPortlist.IP, distribute["userid"]);
                string updatessql = $"UPDATE Equipment_distribution SET status='fail', type = '2', errMsg='{Properties.Strings.DeviceOffline}', date='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE id={id}";
                SQLiteHelper.ExecuteNonQuery(connectionString, updatessql);
                return FaceDeploymentResult.DeviceOffline;

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
                        var device = Deviceinfo.GetByIp(deviceInfo.ipAddress);
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

        public static bool distrbuteVisitor(string name, string imgeurl, string statime, string endtime, string id, string idNumber, string deivces)
        {
            Console.WriteLine(deivces);
            JArray arrDevices = (JArray)JsonConvert.DeserializeObject(deivces);
            JArray arrDetail = new JArray();
            statime = statime.Replace("-", "/").Trim() + ":00";
            endtime = endtime.Replace("-", "/").Trim() + ":00";
            bool re = true;
            if (string.IsNullOrEmpty(imgeurl) || Deviceinfo.Count == 0 || string.IsNullOrEmpty(id))
            {
                return false;
            }

            foreach (var device in arrDevices)
            {
                var d = Deviceinfo.GetByDeviceId(device["Deviceid"].Value<string>());
                if (d?.IsConnected == true)
                {
                    string PersonJson = string.Empty;

                    //将图片转换成符合相机需求
                    var reg_image = Convert.ToBase64String(File.ReadAllBytes(imgeurl));

                    PersonJson = string.Format(UtilsJson.PersonJsonforVisitor, string.IsNullOrEmpty(idNumber) ? id : idNumber, name.Trim(), reg_image, endtime, statime);

                    string restr = GetDevinfo.request(d, PersonJson);
                    JObject restr_json = (JObject)JsonConvert.DeserializeObject(restr.Trim());
                    if (restr_json != null)
                    {
                        string code = restr_json["code"].ToString();
                        int code_int = int.Parse(code);

                        JObject jsonDownStatus = new JObject();
                        jsonDownStatus["Deviceid"] = (string)device["Deviceid"];
                        jsonDownStatus["code"] = code_int;
                        arrDetail.Add(jsonDownStatus);
                        if (code_int != 0)
                        {
                            re = false;
                        }
                        else
                        {

                        }
                    }
                }
                else
                {
                    JObject jsonDownStatus = new JObject();
                    jsonDownStatus["Deviceid"] = (string)device["Deviceid"];
                    jsonDownStatus["errMsg"] = Properties.Strings.DeviceOffline;
                    arrDetail.Add(jsonDownStatus);
                    re = false;
                }
            }

            string donwDetail = JsonConvert.SerializeObject(arrDetail);
            if (re)
            {
                string updatessql = "UPDATE Visitor SET isDown='1',downDetail='" + donwDetail + "' WHERE id=" + id;
                SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
            }
            else
            {
                string updatessql = "UPDATE Visitor SET isDown='2',downDetail='" + donwDetail + "' WHERE id=" + id;
                SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
            }

            #region 注释掉之前方法：不记录下发状态
            //Deviceinfo.MyDevicelist.ForEach(d => {
            //    if (d.IsConnected==true)
            //    {
            //        string PersonJson = string.Empty;
            //        string thumb, twis, reg_images = string.Empty, norm_images = string.Empty;


            //        //将图片转换成符合相机需求
            //        if (twistImageCore(File.ReadAllBytes(imgeurl.Trim()), d.DevicVersion, out thumb, out twis, out bool IsNew))
            //        {
            //            reg_images = string.Format("{{\"format\": \"jpg\",\"image_data\":\"{0}\"}}", thumb);

            //            if (IsNew)
            //            {
            //                norm_images = string.Format("{{\"width\": 112,\"height\": 112,\"image_data\":\"{0}\"}}", twis);
            //            }
            //            else
            //                norm_images = string.Format("{{\"width\": 150,\"height\": 150,\"image_data\":\"{0}\"}}", twis);
            //        }

            //        PersonJson = string.Format(UtilsJson.PersonJsonforterm, id, name.Trim(), reg_images, norm_images, endtime,statime);

            //        JObject deleteJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.deleteJson);
            //        if (deleteJson != null)
            //        {
            //            deleteJson["id"] = id;
            //        }
            //        //先执行删除操作
            //        string sss = GetDevinfo.request(d, deleteJson.ToString());
            //        //在执行下发操作
            //        string restr = GetDevinfo.request(d, PersonJson);
            //        JObject restr_json = (JObject)JsonConvert.DeserializeObject(restr.Trim());
            //        if (restr_json != null)
            //        {
            //            string code = restr_json["code"].ToString();
            //            int code_int = int.Parse(code);
            //            if (code_int != 0)
            //            {
            //                re = false;
            //            }
            //            else
            //            {
            //                re = true;
            //                string updatessql = "UPDATE Visitor SET isDown='1' WHERE id=" + id;
            //                SQLiteHelper.ExecuteNonQuery(ApplicationData.connectionString, updatessql);
            //            }
            //        } 
            //    }
            //});
            #endregion
            return re;
        }

        public static void delVisitorByIds(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return;
            }
            string[] s = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length > 0)
            {
                foreach (string id in s)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        string idNumber = null;
                        using (var conn = SQLiteHelper.GetConnection())
                        {
                            idNumber = conn.Get<Visitor>(id)?.idNumber;
                        }

                        Array.ForEach(Deviceinfo.GetAllMyDevices(), d =>
                        {
                            if (d.IsConnected)
                            {
                                JObject deleteJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.deleteJson);
                                if (deleteJson != null)
                                {
                                    deleteJson["id"] = string.IsNullOrEmpty(idNumber) ? id : idNumber;
                                }
                                //先执行删除操作
                                GetDevinfo.request(d, deleteJson.ToString());
                                using (var c = SQLiteHelper.GetConnection())
                                {
                                    c.ExecuteScalar<int>($"delete FROM Visitor WHERE id = {id}");
                                }
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

        public static bool twistImageCore(byte[] imageData, string twist_version, out string thumb, out string twist, out bool IsNew)
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

            if (twist_byte[1].Length == 112 * 112 * 3)
            {
                IsNew = true;
            }
            return true;
        }
    }
}

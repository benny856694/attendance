﻿using DBUtility.SQLite;
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

namespace huaanClient
{
    class DistributeToequipment
    {
        public static void distrbute()
        {
            //string connectionString = "Data Source=" + Application.StartupPath + @"\huaanDatabase.sqlite;Version=3;";
            string connectionString = ApplicationData.connectionString;
            string commandText = "SELECT * FROM Equipment_distribution WHERE status <> 'success'";
            string sr = SQLiteHelper.SQLiteDataReader(connectionString, commandText);

            //bin
            List<Model.EquipmentDistribution> distributions = new List<Model.EquipmentDistribution>();
            using (var db = new SQLiteConnection(connectionString))
            {
                distributions = db.Query<DbEquipmentDistribution>($"select * from {DbNames.TableEquipementDistribution}")
                    .Select(x=>x.toModel()).ToList();
            }


            if (!string.IsNullOrEmpty(sr))
            {
                JArray srjo = (JArray)JsonConvert.DeserializeObject(sr);
                if (srjo.Count>0)
                {
                    foreach (JObject jo in srjo)
                    {
                        //type 0 下发 1删除 2异常
                        if (jo["type"].ToString().Trim().Equals("0") && !jo["status"].ToString().Trim().Equals("success"))
                        {
                            string userid = jo["userid"].ToString().Trim();
                            string id = jo["id"].ToString().Trim();
                            string deviceid= jo["deviceid"].ToString().Trim();

                            string downid = userid;
                            //获取对应的名字和IP地址
                            string sql = "SELECT staff.face_idcard,staff.source,staff.Employee_code,staff.idcardtype,staff.name,staff.picture,MyDevice.ipAddress FROM staff LEFT JOIN MyDevice WHERE staff.id=" + userid + " AND MyDevice.id="+ deviceid;
                            string sqldata = SQLiteHelper.SQLiteDataReader(connectionString, sql);
                            JArray sqldatajo = (JArray)JsonConvert.DeserializeObject(sqldata);

                            if (GetData.getIscode_syn())
                            {
                                downid = sqldatajo[0]["Employee_code"].ToString().Trim();
                            }

                            if (sqldatajo.Count>0)
                            {
                                /*JObject PersonJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.PersonJson)*/;
                                string PersonJson = string.Empty;
                                string ip= sqldatajo[0]["ipAddress"].ToString().Trim();
                                string source = sqldatajo[0]["source"].ToString().Trim();
                                CameraConfigPort CameraConfigPortlist = Deviceinfo.MyDevicelist.Find(d => d.IP == ip);
                                if (CameraConfigPortlist == null)
                                    continue;
                                if (CameraConfigPortlist.IsConnected) 
                                {
                                    if (PersonJson != null)
                                    {
                                        //PersonJson["id"] = userid;
                                        //PersonJson["name"] = sqldatajo[0]["name"].ToString().Trim();

                                        string thumb, twis, reg_images = string.Empty, norm_images = string.Empty;

                                        //判断图片是否存在 如果不存在
                                        if (!IsExis(sqldatajo[0]["picture"].ToString()))
                                        {
                                            string updatessql = "UPDATE Equipment_distribution SET status='fail',type='2',date=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " WHERE id=" + id;
                                            SQLiteHelper.ExecuteNonQuery(connectionString, updatessql);
                                            return;
                                        }
                                        if (source.Length>4)
                                        {

                                            string ss= sqldatajo[0]["picture"].ToString().Trim();
                                            string ss1 = sqldatajo[0]["picture"].ToString().Trim().Substring(0, sqldatajo[0]["picture"].ToString().Trim().Length - 4)
                                                + "reg_images" + ".jpg";
                                            thumb = Convert.ToBase64String(File.ReadAllBytes(sqldatajo[0]["picture"].ToString().Trim()));
                                            twis = Convert.ToBase64String(File.ReadAllBytes(sqldatajo[0]["picture"].ToString().Trim().Substring(0, sqldatajo[0]["picture"].ToString().Trim().Length - 4)
                                                + "reg_images"+".jpg"));
                                            if (File.ReadAllBytes(sqldatajo[0]["picture"].ToString().Trim()).Length== 112 * 112 * 3)
                                            {
                                                norm_images = string.Format("{{\"width\": 112,\"height\": 112,\"image_data\":\"{0}\"}}", thumb);
                                            }else
                                                norm_images = string.Format("{{\"width\": 150,\"height\": 150,\"image_data\":\"{0}\"}}", thumb);
                                            reg_images = string.Format("{{\"format\": \"jpg\",\"image_data\":\"{0}\"}}", twis);
                                        }
                                        else
                                        {
                                            //将图片转换成符合相机需求
                                            if (twistImageCore(File.ReadAllBytes(sqldatajo[0]["picture"].ToString().Trim()), CameraConfigPortlist.DevicVersion, out thumb, out twis, out bool IsNew))
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
                                        if (sqldatajo[0]["idcardtype"].ToString().Trim()=="64")
                                        {
                                            PersonJson = string.Format(UtilsJson.PersonJson64, downid, sqldatajo[0]["name"].ToString().Trim(), reg_images, norm_images, sqldatajo[0]["face_idcard"].ToString().Trim());
                                        }
                                        else if (sqldatajo[0]["idcardtype"].ToString().Trim() == "32")
                                        {
                                            PersonJson = string.Format(UtilsJson.PersonJson32, downid, sqldatajo[0]["name"].ToString().Trim(), reg_images, norm_images, sqldatajo[0]["face_idcard"].ToString().Trim());
                                        }
                                        else
                                        {
                                            PersonJson = string.Format(UtilsJson.PersonJson, downid, sqldatajo[0]["name"].ToString().Trim(), reg_images, norm_images);
                                        }

                                        //string imgebase64str = ReadImageFile(sqldatajo[0]["picture"].ToString().Trim());
                                        //PersonJson["reg_images"][0]["image_data"] = imgebase64str;

                                        
                                    }
                                    string s = jo.ToString();

                                    JObject deleteJson = (JObject)JsonConvert.DeserializeObject(UtilsJson.deleteJson);
                                    if (deleteJson != null)
                                    {
                                        deleteJson["id"] = downid;
                                    }
                                    //先执行删除操作
                                    string sss=GetDevinfo.request(CameraConfigPortlist, deleteJson.ToString());
                                    //在执行下发操作
                                    string restr = GetDevinfo.request(CameraConfigPortlist, PersonJson);
                                    JObject restr_json = (JObject)JsonConvert.DeserializeObject(restr.Trim());
                                    if (restr_json != null)
                                    {
                                        string code = restr_json["code"].ToString();
                                        int code_int = int.Parse(code);
                                        if (code_int == 0)
                                        {
                                            string updatessql = "UPDATE Equipment_distribution SET status='success',date='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE id=" + id;
                                            SQLiteHelper.ExecuteNonQuery(connectionString, updatessql);
                                        }
                                        else
                                        {
                                            string updatessql = "UPDATE Equipment_distribution SET status='fail',code='"+ code_int + "',date='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE id=" + id;
                                            SQLiteHelper.ExecuteNonQuery(connectionString, updatessql);
                                        }
                                        //else if (code_int == 35 || code_int == 36 || code_int == 37 || code_int == 38 || code_int == 39 || code_int == 40 || code_int == 41)
                                        //{
                                        //    obj["data"] = "照片不合格";
                                        //}
                                    }
                                    else
                                    {
                                        string updatessql = "UPDATE Equipment_distribution SET status='fail',date='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE id=" + id;
                                        SQLiteHelper.ExecuteNonQuery(connectionString, updatessql);
                                    }
                                }
                            }
                        }
                        else if (jo["type"].ToString().Trim().Equals("1") && !jo["status"].ToString().Trim().Equals("success"))
                        {
                            string id= jo["userid"].ToString().Trim();
                            if (GetData.getIscode_syn())
                            {
                                //获取对应的名字和IP地址
                                string sql = "SELECT staff.Employee_code FROM staff  WHERE staff.id=" + jo["userid"].ToString();
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
                            Devicelistdata.ForEach(s => {
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
                                            GetData.ubpdateEquipment_distributionfordel(jo["userid"].ToString());
                                        }
                                    }
                                }  
                            });
                        }
                    }
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

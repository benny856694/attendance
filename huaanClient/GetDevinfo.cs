﻿using huaanClient.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace huaanClient
{
    class GetDevinfo
    {
        public static  void getDevinfo()
        {

            //List<CameraConfigPort> devicelist = new List<CameraConfigPort>();
            //CameraConfigPort CameraConfigPortlist = null;
            List<(string mac, string ip, string mask, string platform, string system)> ds = null;

            ds = DeviceDiscover.Search(2);
            ds.ForEach(d => Console.WriteLine(d.ip));

            //筛选是否为平台的人脸设备 通过关键字去判断
            ds = getEffective(ds);

            List<Mydeviceinfo> Mydevicelist = null;
            string mydevice = GetData.getDeviceforMyDevice();
            if (mydevice.Length > 2)
            {
                int IndexofA = mydevice.IndexOf("[");
                int IndexofB = mydevice.IndexOf("]");
                string Ru = mydevice.Substring(IndexofA, IndexofB - IndexofA + 1);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Mydevicelist = Serializer.Deserialize<List<Mydeviceinfo>>(Ru);
            }

            if (Mydevicelist != null)
            {
                Mydevicelist.ForEach(m =>
                {
                    ds.RemoveAll(c => c.ip == m.ipAddress);
                });
            }

            Deviceinfo.ds = ds;
            //ds.ForEach(d => {
            //    var cam = Deviceinfo.Devicelist.Find(c => c.IP == d.ip);
            //    bool exists = true;
            //    if(cam == null)
            //    {
            //        cam = new CameraConfigPort(d.ip);
            //        //测试
            //        cam.Username = "123";
            //        cam.Password = "123";
            //        cam.Connect();
            //        exists = false;
            //    }
            //    if(cam.IsConnected && cam.DeviceNo == default)
            //    {
            //        string result = request(cam);
            //        JObject jo = (JObject)JsonConvert.DeserializeObject(result);
            //        if (jo != null)
            //        {
            //            JToken deviceNoJObject = jo["device_sn"];
            //            if (deviceNoJObject != null)
            //            {
            //                string device_no = deviceNoJObject.ToString();
            //                cam.DeviceNo = device_no;
            //                Deviceinfo.Devicelist.RemoveAll(c_ => c_.DeviceNo == device_no);
            //            }
            //            JObject device_info = (JObject)jo["device_info"];
            //            if (device_info != null)
            //            {
            //                JToken addr_nameJObject = device_info["addr_name"];
            //                if (addr_nameJObject != null)
            //                {
            //                    cam.DeviceName = addr_nameJObject.ToString();
            //                }
            //            }
            //        }
            //    }
            //    if(!exists)
            //        Deviceinfo.Devicelist.Add(cam);
            //});
        }
        /// <summary>
        /// 获取当前数据库保存的设备并进行处理
        /// </summary>
        public static void getinfoToMyDev()
        {
            List<Mydeviceinfo> Mydevicelist = null;
            string mydevice = GetData.getDeviceforMyDevice();
            if (mydevice.Length > 2)
            {
                int IndexofA = mydevice.IndexOf("[");
                int IndexofB = mydevice.IndexOf("]");
                string Ru = mydevice.Substring(IndexofA, IndexofB - IndexofA + 1);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Mydevicelist = Serializer.Deserialize<List<Mydeviceinfo>>(Ru);
            }

            if (Mydevicelist!=null)
            {
                Mydevicelist.ForEach(d => {
                    string number = d.number;
                    var cam = Deviceinfo.MyDevicelist.Find(c => c.IP == d.ipAddress);
                    bool exists = true;
                    if (cam == null)
                    {
                        cam = new CameraConfigPort(d.ipAddress);
                        cam.CaptureData += (s, e) => {
                            Console.WriteLine(e.person_id);
                        };
                        cam.Username = "123";
                        cam.Password = "123";
                        cam.Deviceid = d.id;
                        cam.Connect();
                        exists = false;
                    }
                    if (cam.IsConnected && cam.DeviceNo == default)
                    {
                        //设置默认参数
                        string SettingParameters = UtilsJson.SettingParameters;
                        if (!string.IsNullOrEmpty(d.DeviceName))
                        {
                            SettingParameters = string.Format(UtilsJson.SettingParametersFormat, d.DeviceName);
                        }
                        
                        string restr = GetDevinfo.request(cam, SettingParameters);

                        string result = request(cam);

                        JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                        if (jo != null)
                        {
                            JToken deviceNoJObject = jo["device_sn"];
                            if (deviceNoJObject != null)
                            {
                                string device_no = deviceNoJObject.ToString();
                                if (!string.IsNullOrEmpty(device_no))
                                {
                                    if (string.IsNullOrEmpty(number))
                                    {
                                        try
                                        {
                                            //保存设备编号
                                            GetData.setDevicenumber(device_no, d.id);
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                }
                                cam.DeviceNo = device_no;
                                Deviceinfo.MyDevicelist.RemoveAll(c_ => c_.DeviceNo == device_no);
                            }
                            //JObject device_info = (JObject)jo["device_info"];
                            //if (device_info != null)
                            //{
                            //    JToken addr_nameJObject = device_info["addr_name"];
                            //    if (addr_nameJObject != null)
                            //    {
                            //        cam.DeviceName = addr_nameJObject.ToString();
                            //    }
                            //}
                        }
                    }
                    if (!string.IsNullOrEmpty(d.DeviceName))
                    {
                        cam.DeviceName = d.DeviceName;
                    }
                    if (!exists)
                        Deviceinfo.MyDevicelist.Add(cam);
                });
            }
        }
        /// <summary>
        /// 设置相机时间同步
        /// </summary>
        public static void timeSynchronization()
        {
            //先判断是否开启时间同步开关
            if (!GetData.gettime_syn())
            {
                return;
            }
            for (var i=0;i< Deviceinfo.MyDevicelist.Count();i++)
            {
                if (Deviceinfo.MyDevicelist[i].IsConnected)
                {
                    //获取ntp是否打开 如果打开则先关闭
                    if (Deviceinfo.MyDevicelist[i].GetNtpOnoff())
                    {
                        //关两次
                        Deviceinfo.MyDevicelist[i].ToggleNtpEnable(false);
                        Deviceinfo.MyDevicelist[i].ToggleNtpEnable(false);
                    }

                    //同步当前时间到相机 2020/03/18 16:00:00
                    string s= request(Deviceinfo.MyDevicelist[i], "{\"cmd\":\"update date time\",\"date_time\":\""+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").Replace("-", "/") + "\"}");
                }
            }
        }
        public static string request(CameraConfigPort re)
        {
            string request_json = "{\"version\": \"0.2\",\"cmd\": \"request app params\"}";
            string resultjson=re.JsonInteractive(request_json);
            return resultjson;
        }
        public static string request(CameraConfigPort re,string request_json_str)
        {
            string resultjson = re.JsonInteractive(request_json_str);
            return resultjson;
        }
        public static List<(string mac, string ip, string mask, string platform, string system)> getEffective(List<(string mac, string ip, string mask, string platform, string system)> ds)
        {
            //string[] systemDictionary = new string[4] { "HI3516CV500", "HI3516DV300", "HI3516EV300", "HI3516AV200" };
            List < (string mac, string ip, string mask, string platform, string system)> Effective = new List<(string mac, string ip, string mask, string platform, string system)>();
            foreach (var d in ds)
            {
                if (d.platform.Trim().Contains("HI3516CV500")
                    || d.platform.Trim().Contains("HI3516DV300") 
                    || d.platform.Trim().Contains("HI3516EV300")
                    || d.platform.Trim().Contains("HI3516AV200"))
                {
                    Effective.Add(d);
                }
            }

            return Effective;
        }
    }
}

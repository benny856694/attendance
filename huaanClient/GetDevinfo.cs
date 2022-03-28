using HaSdkWrapper;
using huaanClient.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace huaanClient
{
    class GetDevinfo
    {
        private static DateTime _lastDeviceFound = DateTime.Now;
        private static object _locker = new object();
        private static ConcurrentStack<(string mac, string ip, string mask, string platform, string system)>
            _ipsFound = new ConcurrentStack<(string mac, string ip, string mask, string platform, string system)>();
        private static Timer t;
        private static TaskCompletionSource<(string mac, string ip, string mask, string platform, string system)[]>
            _tcs;

        static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static DateTime LastDeviceFound
        {
            get
            {
                lock (_locker)
                {
                    return _lastDeviceFound;

                }
            }

            set
            {
                lock (_locker)
                {
                    _lastDeviceFound = value;
                }
            }
        }

        public static Task<(string mac, string ip, string mask, string platform, string system)[]> getDevinfo()
        {
            _ipsFound.Clear();
            HaCamera.DeviceDiscovered += HaCamera_DeviceDiscovered;
            HaCamera.DiscoverDevice();
            LastDeviceFound = DateTime.Now;
            t?.Dispose();
            _tcs?.TrySetCanceled();
            t = new Timer(callback, null, 2000, 2000);
            _tcs = new TaskCompletionSource<(string mac, string ip, string mask, string platform, string system)[]>();
            return _tcs.Task;

        }

        private static void callback(object state)
        {
            var noFoundingAnymore = (DateTime.Now - LastDeviceFound).Duration() > TimeSpan.FromSeconds(5);
            if (noFoundingAnymore)
            {
                HaCamera.DeviceDiscovered -= HaCamera_DeviceDiscovered;
                t?.Dispose();
                var ds = getEffective(_ipsFound.ToList());
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

                _tcs.SetResult(ds.ToArray());
            }

        }

        private static void HaCamera_DeviceDiscovered(object sender, DeviceDiscoverdEventArgs e)
        {
            LastDeviceFound = DateTime.Now;
            _ipsFound.Push((e.Mac, e.IP, e.NetMask, e.Plateform, e.System));
        }

        public static  void getDevinfo_deprecated()
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

            //Deviceinfo.ds = ds;
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
            
            var str = GetData.getDeviceforMyDevice();
            var Mydevicelist = JsonConvert.DeserializeObject<List<Mydeviceinfo>>(str) ;

            if (Mydevicelist!=null)
            {
                Mydevicelist.ForEach(d => {
                    string number = d.number;
                    var cam = Deviceinfo.GetByIp(d.ipAddress);
                    bool exists = true;
                    if (cam == null)
                    {
                        cam = new CameraConfigPort(d.ipAddress);
                        cam.CaptureData += (s, e) => {
                            Console.WriteLine(e.person_id);
                        };
                        cam.Username = d.username ?? "admin";
                        cam.Password = d.password ?? "admin";
                        //cam.Username = "123";
                        //cam.Password = "123";
                        cam.Deviceid = d.id;
                        cam.Connect();
                        exists = false;
                    }
                    if (cam.IsConnected && cam.DeviceNo == default)
                    {
                        //设置默认参数
                        var brandJson = Tools.GetBrandObjectInJson();
                        var brandObj = JObject.Parse(brandJson);
                        bool? autoCleanExpiredVisitor = (bool?)brandObj["autoCleanExpiredVisitor"];
                        var cmd = UtilsJson.GetSettingObject(d.DeviceName, autoCleanExpiredVisitor);
                        var json = JsonConvert.SerializeObject(cmd);
                        var _ = request(cam, json);

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
                                Deviceinfo.RemoveAll(c_ => c_.DeviceNo == device_no);
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
                    {
                        Deviceinfo.Add(cam);
                    }
                        
                });
            }
        }
        /// <summary>
        /// 设置相机时间同步
        /// </summary>
        public static void timeSynchronization()
        {
            //先判断是否开启时间同步开关
            var isSyncByNtp = GetData.getIsNtpSync();
            var devices = Deviceinfo.GetAll();
            foreach (var device in devices)
            {
                if (device.IsConnected)
                {
                    try
                    {
                        //获取ntp是否打开 如果打开则先关闭
                        var isDeviceSyncByNtp = device.GetNtpOnoff();
                        if (isDeviceSyncByNtp != isSyncByNtp)
                        {
                            //关两次
                            device.ToggleNtpEnable(isSyncByNtp);
                            //Deviceinfo.MyDevicelist[i].ToggleNtpEnable(isSyncByNtp);
                        }

                        //同步当前时间到相机 2020/03/18 16:00:00
                        if (!isSyncByNtp)
                        {
                            var cmd = new
                            {
                                version = "0.2",
                                cmd = "update date time",
                                date_time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                            };

                            var json = JsonConvert.SerializeObject(cmd);
                            string s = request(device, json);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, $"同步时间：{device.IP}");
                    }
                    
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
                //if (d.platform.Trim().Contains("HI3516CV500")
                //    || d.platform.Trim().Contains("HI3516DV300") 
                //    || d.platform.Trim().Contains("HI3516EV300")
                //    || d.platform.Trim().Contains("HI3516AV200"))
                {
                    Effective.Add(d);
                }
            }

            return Effective;
        }
    }
}

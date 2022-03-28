using CefSharp.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace huaanClient.Database
{
    class Deviceinfo
    {
        private static object _lockerMyDeviceList = new object();
        //public static List<(string mac, string ip, string mask, string platform, string system)> ds { get; set; }
        //当前局域网下所有的IP
        public static List<CameraConfigPort> Devicelist { get; set; }

        //目前所添加的IP地址
        private static List<CameraConfigPort> MyDevicelist { get; set; }

        public static CameraConfigPort GetByDeviceSN(string sn)
        {
            lock(_lockerMyDeviceList)
            {
                return GetBy(x => x.DeviceNo == sn);
            }
        }

        public static CameraConfigPort GetByIp(string ip)
        {
            lock(_lockerMyDeviceList)
            {
                return GetBy(x => x.IP == ip);
            }
        }

        public static CameraConfigPort GetByAddrName(string name)
        {
            lock(_lockerMyDeviceList)
            {
                return GetBy(x => x.DeviceName == name);
            }
        }

        public static CameraConfigPort GetByDeviceId(string id)
        {
            lock(_lockerMyDeviceList)
            {
                return GetBy(x => x.Deviceid == id);
            }
        }

        public static CameraConfigPort GetBy(Predicate<CameraConfigPort> predicate)
        {
            lock(_lockerMyDeviceList)
            {
                return MyDevicelist.FirstOrDefault(x => predicate(x));
            }
        }

        public static int GetOnlineCount()
        {
            lock(_lockerMyDeviceList)
            {
                return MyDevicelist.Count(x => x.IsConnected);
            }
        }

        public static void RemoveAll(Predicate<CameraConfigPort> predict)
        {
            lock(_lockerMyDeviceList)
            {
                MyDevicelist.RemoveAll(predict);
            }
        }

        public static void ClearMyDevices()
        {
            lock(_lockerMyDeviceList)
            {
                MyDevicelist.Clear();
            }
        }

        public static void Add(CameraConfigPort cam)
        {
            lock(_lockerMyDeviceList)
            {
                MyDevicelist.Add(cam);
            }
        }

        public static CameraConfigPort[] GetAllMyDevices()
        {
            lock(_lockerMyDeviceList)
            {
                return MyDevicelist.ToArray();
            }

        }

        public static int Count
        {
            get
            {
                lock(_lockerMyDeviceList)
                {
                    return MyDevicelist.Count;
                }
            }
        }

        static Deviceinfo()
        {
            //ds = new List<(string mac, string ip, string mask, string platform, string system)>();
            MyDevicelist = new List<CameraConfigPort>();
            Devicelist = new List<CameraConfigPort>();
        }
    }
    class Mydeviceinfo
    {
        public string id { get; set; }
        /// <summary>
        /// 设备IP
        /// </summary>
        public string ipAddress { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 设备序列号
        /// </summary>
        public string number { get; set; }
        public string username { get; set; }
        public string password { get; set; }

    }
    class ALLdeviceinfo
    {
        public string mac { get; set; }
        /// <summary>
        /// 设备IP
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string mask { get; set; }
        /// <summary>
        /// 设备序列号
        /// </summary>
        public string platform { get; set; }
        public string system { get; set; }

    }
}

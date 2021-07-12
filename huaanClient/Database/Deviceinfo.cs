using CefSharp.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace huaanClient.Database
{
    class Deviceinfo
    {
        //public static List<(string mac, string ip, string mask, string platform, string system)> ds { get; set; }
        //当前局域网下所有的IP
        public static List<CameraConfigPort> Devicelist { get; set; }

        //目前所添加的IP地址
        public static List<CameraConfigPort> MyDevicelist { get; set; }


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

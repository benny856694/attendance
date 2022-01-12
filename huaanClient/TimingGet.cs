using huaanClient.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace huaanClient
{
    class TimingGet
    {
        static NLog.Logger Logger= NLog.LogManager.GetCurrentClassLogger();
        public static void Timingquery()
        {
            DateTime endtime = DateTime.Now;
            List<CameraConfigPort> Devicelistdata = Deviceinfo.MyDevicelist;

            if (Devicelistdata.Count < 1)
            {
                return;
            }
            Devicelistdata.ForEach(s =>
            {
                try
                {
                    Logger.Debug(s.IP+"主动查询抓拍开始");
                    DownloadOneDevice(s, endtime);
                    Logger.Debug(s.IP + "主动查询抓拍结束!\n");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"下载{s.IP}抓拍数据异常");
                }
            });
            
        }

        private static void DownloadOneDevice(CameraConfigPort s, DateTime endtime)
        {
            string ATT_STA_time;
            //获取开始时间时如果数据库没有就默认取值当天时间
            string statime_str = DateTime.Now.ToString("yyyy-MM") + "-01 00:00:00";

            string time_json = GetData.getMyDeviceforLast_query(s.IP);
            if (!string.IsNullOrEmpty(time_json))
            {
                JArray jArray = (JArray)JsonConvert.DeserializeObject(time_json);
                {
                    if (jArray.Count != 0)
                    {
                        if (!string.IsNullOrEmpty(jArray[0]["Last_query"].ToString()))
                        {
                            statime_str = jArray[0]["Last_query"].ToString();
                        }
                    }
                }
            }

            ATT_STA_time = statime_str;
            DateTime statime = Convert.ToDateTime(statime_str);

            if (s.IsConnected)
            {
                Logger.Debug($"beging query data from camera {statime}-{endtime}");
                var list = s.GetRecords(statime, endtime, 3000, 30000);
                if (list.Count > 0)
                {
                    var time = list.Max(t => t.time);

                    list.ForEach(l =>
                    {
                        Logger.Debug($"save capture data to db seq: {l.sequnce}");
                        HandleCaptureData.setCaptureDataToDatabase(l, s.DeviceNo, s.DeviceName);
                    });
                    if (!string.IsNullOrEmpty(time.ToString("yyyy-MM-dd HH:mm:ss.fff")))
                    {
                        //保存最后一条的记录
                        GetData.setMyDeviceforLast_query(time.ToString("yyyy-MM-dd HH:mm:ss.fff"), s.IP);
                    }
                }
                else
                {
                    Logger.Debug("no record");
                }
                if (!string.IsNullOrEmpty(ATT_STA_time))
                    AttendanceAlgorithm.getpersonnel(ATT_STA_time, endtime.ToString("yyyy-MM-dd HH:mm:ss") + ".999", 1);

            }

        }
    }
}

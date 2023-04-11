using huaanClient.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace huaanClient
{
    class TimingGet
    {
        static NLog.Logger Logger= NLog.LogManager.GetCurrentClassLogger();
        public static bool Timingquery(CancellationToken token)
        {
            var hasData = false;
            DateTime endtime = DateTime.Now;
            var Devicelistdata = Deviceinfo.GetAllMyDevices();

            if (Devicelistdata.Length < 1)
            {
                return hasData;
            }
            //准备容器存储相机和对应last_query时间
            Dictionary<CameraConfigPort, DateTime> cameraQueryTimes = new Dictionary<CameraConfigPort, DateTime>();
            //保存所有相机首次查询
            List<DateTime> firstQuerys=new List<DateTime>();
            foreach (var s in Devicelistdata)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                try
                {
                    Logger.Debug(s.IP + "主动查询抓拍开始....");
                    var notEmpty = DownloadOneDevice(s, endtime, firstQuerys, cameraQueryTimes, token);
                    if (notEmpty)
                    {
                        hasData = true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"下载{s.IP}抓拍数据异常");
                }
            }

            
            if (Properties.Settings.Default.calculateAttendanceData
                && firstQuerys.Count > 0)
            {
                //计算最早查询
                DateTime firstQuery = firstQuerys[0];
                foreach(var item in firstQuerys)
                {
                    int compNum=DateTime.Compare(firstQuery, item);
                    if (compNum > 0)
                    {
                        firstQuery = item;
                    }
                }
                new AttendanceAlgorithm().getpersonnel(firstQuery.ToString("yyyy-MM-dd HH:mm:ss") + ".999", endtime.ToString("yyyy-MM-dd HH:mm:ss") + ".999", 1, token);//计算考勤
            }
            if (cameraQueryTimes.Count > 0)
            {
                //遍历相机，保存last_query
                foreach (var cameraQueryTime in cameraQueryTimes)
                {
                    var s = cameraQueryTime.Key;
                    var time = cameraQueryTime.Value;
                    //保存最后一条的记录
                    GetData.setMyDeviceforLast_query(time.ToString("yyyy-MM-dd HH:mm:ss.fff"), s.IP);
                    Logger.Debug($"set {s.IP}  lastquery time to {time} in db");
                }
            }
            
            Logger.Info($"主动获取抓拍并计算考勤完成。查询相机数：{cameraQueryTimes.Count}/{Devicelistdata.Length}");
            return hasData;

        }

        private static bool DownloadOneDevice(CameraConfigPort s,DateTime endtime, List<DateTime> firstQuerys, Dictionary<CameraConfigPort, DateTime> cameraQueryTimes, CancellationToken token)
        {
            string ATT_STA_time;
            //获取开始时间时如果数据库没有就默认取值当天时间
            string statime_str = DateTime.Now.ToString("yyyy-MM") + "-01 00:00:00";
            var hasData = false;

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
                var result = s.GetRecords(statime, endtime, token);
                if (result.count > 0)
                {
                    hasData = true;
                    firstQuerys.Add(statime);//保存所有首次查询
                    cameraQueryTimes.Add(s, result.lastRecordTime);//抓拍记录保存成功后，保存相机和lastquery对应关系
                    /*改为保存考勤记录之后再保存抓拍记录
                    if (!string.IsNullOrEmpty(time.ToString("yyyy-MM-dd HH:mm:ss.fff")))
                    {
                        //保存最后一条的记录
                        GetData.setMyDeviceforLast_query(time.ToString("yyyy-MM-dd HH:mm:ss.fff"), s.IP);
                    }
                    */
                }
                else
                {
                    Logger.Debug("no record");
                }
                /* 改为所有相机保存抓拍记录完成之后再计算考勤
                if (!string.IsNullOrEmpty(ATT_STA_time)&&list.Count>0)
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    Console.WriteLine("计算考勤开始。。。");
                    AttendanceAlgorithm.getpersonnel(ATT_STA_time, endtime.ToString("yyyy-MM-dd HH:mm:ss") + ".999", 1);
                    Console.WriteLine("计算考勤结束。。。");
                    watch.Stop();
                    string time = watch.ElapsedMilliseconds.ToString();
                    Console.WriteLine("考勤计算用时毫秒：" + time);
                }
                */    
            }

            return hasData;

        }
    }
}

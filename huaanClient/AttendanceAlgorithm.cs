using huaanClient.Database;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace huaanClient
{
    class AttendanceAlgorithm
    {
        static List<data> listAll = new List<data>();
        //每个人的数据
        static List<data> listPerson = new List<data>();
        //reData
        static List<reData> relistAll = new List<reData>();
        static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static string getpersonnel(string starttime, string endtime,int type)
        {
            try
            {
                relistAll.Clear();
                listAll.Clear();
                string re = GetData.getStaffa();
 
                string data = "";
                JArray jArray = (JArray)JsonConvert.DeserializeObject(re);

                //主动从设备获取数据
                if (type == 0)
                {
                    //getpersondata(personIds["personId"].ToString().Trim(), starttime, endtime);
                    Task[] tasks = new Task[jArray.Count];
                    for (int i = 0; i < jArray.Count; i++)
                    {
                        var personId = jArray[i];
                        tasks[i] = Task.Factory.StartNew(() => getpersondata(personId["personId"].ToString().Trim(), starttime, endtime, personId["Employee_code"].ToString().Trim()));
                    }
                    Task.WaitAll(tasks);
                }
                else if (type == 1)
                {
                    string Capture_DataRe = GetData.getCapture_Data(starttime, endtime);
                    JArray Capture_DatajArray = (JArray)JsonConvert.DeserializeObject(Capture_DataRe);
                    if (Capture_DatajArray.Count() > 0)
                    {
                        //从数据库获取考勤数据
                        for (int i = 0; i < Capture_DatajArray.Count; i++)
                        {
                            data da = new data();
                            //DateTime dateTime = DateTime.ParseExact(da.ToString(),"YYYY/MM/DD HH：MM：ss", null);
                            da.captureTime = Convert.ToDateTime(Capture_DatajArray[i]["captureTime"].ToString());
                            //int.Parse(str);
                            if (!string.IsNullOrEmpty(Capture_DatajArray[i]["matchScore"].ToString()))
                            {
                                da.matchScore = int.Parse(Capture_DatajArray[i]["matchScore"].ToString().Trim());
                            }
                            if (!string.IsNullOrEmpty(Capture_DatajArray[i]["temperature"].ToString()))
                            {
                                da.temperature = float.Parse(Capture_DatajArray[i]["temperature"].ToString().Trim());
                            }
                            da.personName = Capture_DatajArray[i]["personName"].ToString().Trim();
                            da.Employee_code = Capture_DatajArray[i]["Employee_code"].ToString().Trim();
                            da.personId = Capture_DatajArray[i]["personId"].ToString().Trim();
                            da.closeup= Capture_DatajArray[i]["closeup"].ToString().Trim();
                            if (!string.IsNullOrEmpty(da.personId) )
                            {
                                listAll.Add(da);
                            } 
                        }

                        //从相机主动去取考勤数据
                        //List<Task> taskList = new List<Task>();
                        //foreach (var personId in jArray)
                        //{
                        //    taskList.Add(Task.Factory.StartNew(() =>
                        //    {
                        //        getpersondata(personId["personId"].ToString().Trim(), starttime, endtime);
                        //    }));
                        //}
                        //Task.WaitAll(taskList);
                    }
                }

                //if (listAll == null || listAll.Count == 0)
                //{
                //    return data;
                //}
                //每个人的数据
                if (jArray.Count > 0)
                {
                    for (int i = 0; i < jArray.Count; i++)
                    {
                        listPerson.Clear();
                        listAll.ForEach(s => {
                            if (!string.IsNullOrEmpty(s.personId.ToString().Trim()))
                            {
                                if (!s.personId.ToString().Contains("*") && !s.personId.ToString().Trim().Equals("0"))
                                {
                                    if (s.personId.Trim() == jArray[i]["personId"].ToString().Trim())
                                    {
                                        listPerson.Add(s);
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(jArray[i]["Employee_code"].ToString().Trim()))
                                        {
                                            if (Tools.CheckChinaIDCardNumberFormat(jArray[i]["Employee_code"].ToString().Trim()))
                                            {
                                                if(s.personId.Trim() == jArray[i]["Employee_code"].ToString().Trim())
                                                    listPerson.Add(s);
                                            }
                                        }
                                    }
                                }
                            } 
                        });

                        DateTime sta = Convert.ToDateTime(starttime.Split(' ')[0].Trim());
                        DateTime end = Convert.ToDateTime(endtime.Split(' ')[0].Trim());


                        TimeSpan sp = end.Subtract(sta);
                        int day = sp.Days;

                        if (day == 0 && listPerson.Count == 0)
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(jArray[i]["AttendanceGroup_id"].ToString()))
                        {
                            getEffectiveTime(starttime, endtime, jArray[i]["personId"].ToString(), jArray[i]["AttendanceGroup_id"].ToString(), jArray[i]["Employee_code"].ToString(), jArray[i]["name"].ToString(), jArray[i]["department"].ToString(), day);
                        }
                    }
                }

                //if (relistAll.Count > 0)
                //{
                //    data = JsonConvert.SerializeObject(relistAll);
                //}

                return data;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "考勤计算异常");
                return "";
            } 
        }
        /// <summary>
        /// 判断字符串是否是数字
        /// </summary>
        public static bool IsNumber(string s)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(s)) return false;
                const string pattern = "^[0-9]*$";
                Regex rx = new Regex(pattern);
                return rx.IsMatch(s);
            }
            catch
            {
                return false;
            }
        }
        public static void getpersondata(string personId, string starttime, string endtime, string Employee_code)
        {
            DateTime sta = Convert.ToDateTime(starttime);
            DateTime end = Convert.ToDateTime(endtime);

            List<CameraConfigPort> Devicelistdata = Deviceinfo.MyDevicelist;

            if (Devicelistdata.Count < 1) {
                return;
            }
            //var cam = Devicelistdata.Find(c => c.IP == "192.168.0.182");

            ////测试用
            //if (cam.IsConnected)
            //{
            //    var list = cam.GetRecords(sta, end, personId, 3000);
            //    for (int i = 0; i < list.Count; i++)
            //    {
            //        data da = new data();
            //        da.captureTime = list[i].captureTime;
            //        da.matchScore = list[i].matchScore;
            //        da.personName = list[i].personName;
            //        da.personRole = list[i].personRole;
            //        da.temperature = list[i].temperature;
            //        da.personId = personId;
            //        listAll.Add(da);
            //    }

            //}
            //循环设备
            for (int m = 0; m < Devicelistdata.Count; m++)
            {
                if (Devicelistdata[m].IsConnected)
                {
                    var list = Devicelistdata[m].GetRecords(sta, end, personId, 1500);
                    for (int i = 0; i < list.Count; i++)
                    {
                        data da = new data();
                        da.captureTime = list[i].captureTime;
                        da.matchScore = list[i].matchScore;
                        da.personName = list[i].personName;
                        da.temperature = list[i].temperature;
                        da.Employee_code = Employee_code;
                        da.personId = personId;
                        listAll.Add(da);
                    }

                }
            }

            //废弃
            //foreach (var Device in Devicelistdata)
            //{
            //    if (Device.IsConnected)
            //    {
            //        var list = Device.GetRecords(sta, end, personId, 1500);
            //        for (int i = 0; i < list.Count; i++)
            //        {
            //            data da = new data();
            //            da.captureTime = list[i].captureTime;
            //            da.matchScore = list[i].matchScore;
            //            da.personName = list[i].personName;
            //            da.personRole = list[i].personRole;
            //            da.temperature = list[i].temperature;
            //            da.personId = personId;
            //            listAll.Add(da);
            //        }
            //    }
            //}
        }
        public static string GetShiftId(string AttendanceGroup_id,string date)
        {
            string attribute_str = "";
            string re = "0";
            if (!string.IsNullOrEmpty(AttendanceGroup_id))
            {
                //优先使用特殊日期的打卡逻辑
                string Shiftid_str = GetData.getSpecial_datefordate(date,AttendanceGroup_id);
                if (Shiftid_str.Length>3)
                {
                    JArray jArray = (JArray)JsonConvert.DeserializeObject(Shiftid_str);
                    if (jArray.Count > 0)
                    {
                        string Shiftid = jArray[0]["Shiftid"].ToString();
                        if (!string.IsNullOrEmpty(Shiftid))
                            return Shiftid;
                    }
                     
                }

                attribute_str = GetData.getAtt_attribute(AttendanceGroup_id);

                if (!string.IsNullOrEmpty(attribute_str))
                {
                    JArray jArray = (JArray)JsonConvert.DeserializeObject(attribute_str);
                    if (jArray.Count > 0)
                    {
                        string Today = GetWeek(date);
                        string attribute = jArray[0]["attribute"].ToString();

                        JObject jObject = (JObject)JsonConvert.DeserializeObject(attribute);
                        re = jObject[Today].ToString();
                    }
                }
            }
            return re;
        }
        
        public static string getAtt_attribute(string AttendanceGroup_id)
        {
            string attribute_str = "";
            string re = "0";
            if (!string.IsNullOrEmpty(AttendanceGroup_id))
            {
                attribute_str = GetData.getAtt_attribute(AttendanceGroup_id);
                if (!string.IsNullOrEmpty(attribute_str))
                {
                    JArray jArray = (JArray)JsonConvert.DeserializeObject(attribute_str);
                    if (jArray.Count > 0)
                    {
                        string Today = GetWeek();
                        string attribute = jArray[0]["attribute"].ToString();

                        JObject jObject = (JObject)JsonConvert.DeserializeObject(attribute);
                        re = jObject[Today].ToString();
                    }
                }
            }
            return re;
        }
        //根据personId找到对应的考勤组
        public static void getEffectiveTime(
            string starttime, 
            string endtime,
            string personId,
            string AttendanceGroup_id,
            string Employee_code, 
            string  name,
            string department,
            int day)
        {
            if (DateTimeFormatInfo.CurrentInfo != null)
            {
                var type = DateTimeFormatInfo.CurrentInfo.GetType();
                var field = type.GetField("generalLongTimePattern", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                    field.SetValue(DateTimeFormatInfo.CurrentInfo, "yyyy-MM-dd HH:mm:ss");
            }
            //找到考勤组
            DateTime sta = Convert.ToDateTime(starttime);
            DateTime end = Convert.ToDateTime(endtime);
            for (int i = 0; i < day+1; i++)
            {
                reData reData = new reData();
                DateTime today = sta.AddDays(i);
                string strToday = today.ToString("yyyy-MM-dd").Replace(@"\", "-");

                //昨天的时间
                string strYesterday = today.AddDays(-1).ToString("yyyy-MM-dd").Replace(@"\", "-");

                //根据当前人员从考勤组找到今天的考勤班次
                string idToday = GetShiftId(AttendanceGroup_id, strToday);
                if (idToday == "0" && GetShiftId(AttendanceGroup_id, strYesterday)=="0")
                    continue;
                var strDateBeingCalculated = strToday;
                var dateBeingCalculated = today;
                string shiftInfo = GetData.GetShiftById(idToday);
                if (idToday == "0")
                {
                    shiftInfo = GetData.GetShiftById(GetShiftId(AttendanceGroup_id, strYesterday));
                    strDateBeingCalculated = strYesterday;
                    dateBeingCalculated = today.AddDays(-1);
                }
                
                if (!string.IsNullOrEmpty(shiftInfo))
                {
                    JArray jArray1 = (JArray)JsonConvert.DeserializeObject(shiftInfo);
                    if (jArray1.Count > 0)
                    {
                        //后期根据考勤组表找到这个班次id
                        JToken personIds = jArray1[0];
                        if (personIds.Count() > 0)
                        {
                            //上下班时间
                            string stagotowork1 = "";
                            string endgotowork1 = "";
                            //二时段上下班时间
                            string stagotowork2 = "";
                            string endgotowork2 = "";
                            //三时段上下班时间
                            //string stagotowork3 = "";
                            //string endgotowork3 = "";
                            //班次信息
                            string Shiftinformation = personIds["name"].ToString();
                            string gotowork1 = personIds["gotowork1"].ToString().Trim();
                            string gotowork2 = personIds["gotowork2"].ToString().Trim();
                            string gotowork3 = personIds["gooffwork3"].ToString().Trim();
                            string IsAcrossNight = personIds["IsAcrossNight"].ToString();
                            if (!string.IsNullOrEmpty(gotowork1))
                            {
                                string[] gotowork1Arry = gotowork1.Split(new char[1] { '-' });
                                if (gotowork1Arry.Length > 1)
                                {
                                    stagotowork1 = gotowork1Arry[0];
                                    reData.stagotowork1 = stagotowork1;
                                    endgotowork1 = gotowork1Arry[1];
                                }
                            }
                            string EffectiveTime = personIds["EffectiveTime"].ToString();

                            string[] sArray = EffectiveTime.Split(new char[1] { ',' });
                            string sbEffectiveTime = sArray[0];
                            string sbEffectiveTime_sat = "";
                            string sbEffectiveTime_end = "";
                            string xbEffectiveTime_sta = "";
                            string xbEffectiveTime_end = "";

                            if (!string.IsNullOrEmpty(sbEffectiveTime))
                            {
                                string[] sbArray = sbEffectiveTime.Split(new char[1] { '-' });
                                sbEffectiveTime_sat = sbArray[0].Trim().Replace(":", "");
                                sbEffectiveTime_end = sbArray[1].Trim().Replace(":", "");
                            }
                            string xbEffectiveTime = sArray[1];
                            if (!string.IsNullOrEmpty(xbEffectiveTime))
                            {
                                string[] xbArray = xbEffectiveTime.Split(new char[1] { '-' });
                                xbEffectiveTime_sta = xbArray[0].Trim().Replace(":", "");
                                xbEffectiveTime_end = xbArray[1].Trim().Replace(":", "");
                            }
                            reData.Shiftinformation = Shiftinformation + "-" + gotowork1;

                            reData.Duration = personIds["Duration"].ToString();
                            reData.Date = strDateBeingCalculated;

                            reData.personId =personId;
                            reData.name = name;
                            if (IsAcrossNight.Contains("rue")) {
                                reData.IsAcrossNight = true;
                            }
                            reData.department = department;

                            reData.Employee_code = Employee_code;
                            //声明当天的数据list
                            List<data> listone = new List<data>();

                            //声明次日的数据list 需要跨夜打卡的时候 需要
                            //List<data> listonefornext = new List<data>();

                            //声明上班时段的list
                            List<data> liststa = new List<data>();
                            
                            ////声明三时段上班时段的list
                            //List<data> liststa3 = new List<data>();
                            //声明下班时段的list
                            List<data> listend = new List<data>();
                            ////声明三时段下班时段的list
                            //List<data> listend3 = new List<data>();

                            //获取当天的数据list
                            listPerson.ForEach(s =>
                            {
                                if (s.captureTime.ToString("yyyy-MM-dd").Trim() == dateBeingCalculated.ToString("yyyy-MM-dd").Trim())
                                {
                                    listone.Add(s);
                                }
                            });

                            //一时段
                            if( 
                                !string.IsNullOrEmpty(gotowork1) && 
                                string.IsNullOrEmpty(gotowork2) && 
                                string.IsNullOrEmpty(gotowork3)
                                )
                            {
                                //跨夜的情况下
                                if (IsAcrossNight.Contains("rue"))
                                {
                                    List<data> listlastend = new List<data>();
                                    //先算昨天下班时间 写进数据库
                                    if (listone.Count > 0)
                                    {
                                        foreach (var li in listone)
                                        {
                                            string cap = li.captureTime.ToString("t").Replace(":", "");
                                            //判断是否在有效区间
                                            if (int.Parse(cap) <= int.Parse(xbEffectiveTime_end) && int.Parse(cap) >= int.Parse(xbEffectiveTime_sta))
                                            {
                                                listlastend.Add(listone[listone.IndexOf(li)]);
                                            }
                                        }
                                    }

                                    if (listlastend.Count > 0)
                                    {
                                        {
                                            reData reDataLast = new reData();
                                            DateTime FastDateTime = listlastend[0].captureTime;
                                            for (int min = 0; min < listlastend.Count; min++)
                                            {
                                                if (DateTime.Compare(FastDateTime, listlastend[min].captureTime) <= 0)
                                                {
                                                    FastDateTime = listlastend[min].captureTime;
                                                }
                                            }
                                            reDataLast.temperature1 = Math.Round(listlastend[0].temperature, 2).ToString();
                                            reDataLast.closeup = listlastend[0].closeup;
                                            reDataLast.Punchinformation1 = FastDateTime.ToString("t");
                                            string FastDateTimestr = FastDateTime.ToString("t").Replace(":", "");
                                            if (int.Parse(FastDateTimestr) < int.Parse(endgotowork1.Replace(":", "")))
                                            {
                                                reDataLast.ISLeaveearly = true;
                                                reDataLast.Leaveearly = DateDiff(FastDateTimestr, endgotowork1.Replace(":", ""));
                                            }//是否加班
                                            else if (int.Parse(FastDateTimestr) > int.Parse(endgotowork1.Replace(":", "")))
                                            {
                                                reDataLast.workOvertime = DateDiff(endgotowork1.Replace(":", ""), FastDateTimestr);
                                            }


                                            reDataLast.Shiftinformation = Shiftinformation + "-" + gotowork1;

                                            reDataLast.Duration = personIds["Duration"].ToString();

                                            //处理是否是上周

                                            string lastdate = today.AddDays(-1).ToString("yyyy-MM-dd").Trim().Replace(@"\", "-");

                                            reDataLast.Date = lastdate;

                                            reDataLast.personId = personId;
                                            reDataLast.name = name;

                                            reDataLast.department = department;

                                            reDataLast.Employee_code = Employee_code;

                                            reDataLast.IsAcrossNight = true;
                                            setAttendance_Data(reDataLast, stagotowork1.Replace(":", ""), endgotowork1.Replace(":", ""));
                                        }
                                    }

                                    //在写 当天的 上班时间
                                    if (idToday != "0")
                                    {
                                        //筛选上班有效班时间
                                        if (listone.Count > 0)
                                        {
                                            foreach (var li in listone)
                                            {
                                                string cap = li.captureTime.ToString("t").Replace(":", "");
                                                //判断是否在有效区间
                                                if (int.Parse(cap) <= int.Parse(sbEffectiveTime_end) && int.Parse(cap) >= int.Parse(sbEffectiveTime_sat))
                                                {
                                                    liststa.Add(listone[listone.IndexOf(li)]);
                                                }
                                            }
                                        }
                                        //找到上班时段的list最早的时间记录
                                        if (liststa.Count > 0)
                                        {
                                            DateTime FastDateTime = liststa[0].captureTime;
                                            for (int min = 0; min < liststa.Count; min++)
                                            {
                                                if (DateTime.Compare(FastDateTime, liststa[min].captureTime) >= 0)
                                                {
                                                    FastDateTime = liststa[min].captureTime;
                                                }
                                                //else
                                                //{
                                                //    FastDateTime = FastDateTime;
                                                //}
                                            }
                                            reData.IsAcrossNight = true;
                                            reData.temperature = Math.Round(liststa[0].temperature, 2).ToString();
                                            reData.closeup = liststa[0].closeup;
                                            reData.Punchinformation = FastDateTime.ToString("t");
                                            //计算是否迟到
                                            string FastDateTimestr = FastDateTime.ToString("t").Replace(":", "");
                                            if (int.Parse(FastDateTimestr) > int.Parse(stagotowork1.Replace(":", "")))
                                            {
                                                reData.late = DateDiff(FastDateTimestr, stagotowork1.Replace(":", ""));
                                            }
                                        }

                                        //如果下一天不上班 则一并处理今天的下班时间
                                        listPerson.ForEach(s =>
                                        {
                                            //if (s.captureTime.ToString("yyyy-MM-dd").Trim() == today.ToString("yyyy-MM-dd").Trim())
                                            {
                                                //listonefornext.Add(s);
                                            }
                                        });


                                        //把考勤数据存到数据库
                                        if (!string.IsNullOrEmpty(reData.personId))
                                        {
                                            setAttendance_Data(reData, stagotowork1.Replace(":", ""), endgotowork1.Replace(":", ""));
                                        }
                                    }
                                }
                                else if (idToday != "0")
                                {
                                    reData.IsAcrossNight = false;
                                    //筛选上下班有效班时间
                                    if (listone.Count > 0)
                                    {
                                        foreach (var li in listone)
                                        {
                                            string cap = li.captureTime.ToString("t").Replace(":", "");
                                            //判断是否在有效区间
                                            if (int.Parse(cap) <= int.Parse(sbEffectiveTime_end) && int.Parse(cap) >= int.Parse(sbEffectiveTime_sat))
                                            {
                                                liststa.Add(listone[listone.IndexOf(li)]);
                                            }
                                            else if (int.Parse(cap) <= int.Parse(xbEffectiveTime_end) && int.Parse(cap) >= int.Parse(xbEffectiveTime_sta))
                                            {
                                                listend.Add(listone[listone.IndexOf(li)]);
                                            }
                                        }
                                    }
                                    int isLackcards = 0;
                                    //找到上班时段的list最早的时间记录
                                    if (liststa.Count > 0)
                                    {
                                        DateTime FastDateTime = liststa[0].captureTime;
                                        for (int min = 0; min < liststa.Count; min++)
                                        {
                                            if (DateTime.Compare(FastDateTime, liststa[min].captureTime) >= 0)
                                            {
                                                FastDateTime = liststa[min].captureTime;
                                            }
                                            //else
                                            //{
                                            //    FastDateTime = FastDateTime;
                                            //}
                                        }
                                        isLackcards++;
                                        reData.temperature = Math.Round(liststa[0].temperature, 2).ToString();
                                        reData.closeup = liststa[0].closeup;
                                        reData.Punchinformation = FastDateTime.ToString("t");
                                        //计算是否迟到
                                        string FastDateTimestr = FastDateTime.ToString("t").Replace(":", "");
                                        if (int.Parse(FastDateTimestr) > int.Parse(stagotowork1.Replace(":", "")))
                                        {
                                            reData.late = DateDiff(FastDateTimestr, stagotowork1.Replace(":", ""));
                                        }
                                    }
                                    //找到最晚的时间记录
                                    if (listend.Count > 0)
                                    {
                                        DateTime FastDateTime = listend[0].captureTime;
                                        for (int min = 0; min < listend.Count; min++)
                                        {
                                            if (DateTime.Compare(FastDateTime, listend[min].captureTime) <= 0)
                                            {
                                                FastDateTime = listend[min].captureTime;
                                            }
                                            //else
                                            //{
                                            //    FastDateTime = FastDateTime;
                                            //}
                                        }
                                        //计算是否早退
                                        isLackcards++;
                                        reData.temperature1 = Math.Round(listend[0].temperature, 2).ToString();
                                        reData.closeup = listend[0].closeup;
                                        reData.Punchinformation1 = FastDateTime.ToString("t");
                                        string FastDateTimestr = FastDateTime.ToString("t").Replace(":", "");
                                        if (int.Parse(FastDateTimestr) < int.Parse(endgotowork1.Replace(":", "")))
                                        {
                                            reData.ISLeaveearly = true;
                                            reData.Leaveearly = DateDiff(FastDateTimestr, endgotowork1.Replace(":", ""));
                                        }//是否加班
                                        else if (int.Parse(FastDateTimestr) > int.Parse(endgotowork1.Replace(":", "")))
                                        {
                                            reData.workOvertime = DateDiff(endgotowork1.Replace(":", ""), FastDateTimestr);
                                        }

                                    }
                                    if (isLackcards < 2)
                                    {
                                        reData.isAbsenteeism = "0";
                                    }
                                    //把考勤数据存到数据库
                                    if (!string.IsNullOrEmpty(reData.personId))
                                    {
                                        setAttendance_Data(reData, stagotowork1.Replace(":", ""), endgotowork1.Replace(":", ""));
                                        if (ApplicationData.LanguageSign.Contains("日本語"))
                                        {
                                            HandleCaptureData.httptoline(reData);
                                        }
                                    }
                                }
                            }
                            //二时段
                            else if (!string.IsNullOrEmpty(gotowork1) && !string.IsNullOrEmpty(gotowork2)&& string.IsNullOrEmpty(gotowork3))
                            {
                                //二时段
                                string sbEffectiveTime_sat2 = "";
                                string sbEffectiveTime_end2 = "";
                                string xbEffectiveTime_sta2 = "";
                                string xbEffectiveTime_end2 = "";

                                if (!string.IsNullOrEmpty(gotowork2))
                                {
                                    string[] gotowork1Arry = gotowork2.Split(new char[1] { '-' });
                                    if (gotowork1Arry.Length > 1)
                                    {
                                        stagotowork2 = gotowork1Arry[0];
                                        endgotowork2 = gotowork1Arry[1];
                                    }
                                }
                                string EffectiveTime2 = personIds["EffectiveTime2"].ToString();

                                string[] sArray2 = EffectiveTime2.Split(new char[1] { ',' });
                                string sbEffectiveTime2 = sArray2[0];

                                if (!string.IsNullOrEmpty(sbEffectiveTime2))
                                {
                                    string[] sbArray2 = sbEffectiveTime2.Split(new char[1] { '-' });
                                    sbEffectiveTime_sat2 = sbArray2[0].Trim().Replace(":", "");
                                    sbEffectiveTime_end2 = sbArray2[1].Trim().Replace(":", "");
                                }
                                string xbEffectiveTime2 = sArray2[1];
                                if (!string.IsNullOrEmpty(xbEffectiveTime2))
                                {
                                    string[] xbArray2 = xbEffectiveTime2.Split(new char[1] { '-' });
                                    xbEffectiveTime_sta2 = xbArray2[0].Trim().Replace(":", "");
                                    xbEffectiveTime_end2 = xbArray2[1].Trim().Replace(":", "");
                                }
                                reData.Shiftinformation = Shiftinformation + "-" + gotowork1 + ";" + gotowork2.Trim();

                                //声明二时段上班时段的list
                                List<data> liststa2 = new List<data>();
                                //声明二时段下班时段的list
                                List<data> listend2 = new List<data>();

                                reData.IsAcrossNight = false;
                                //筛选上下班有效班时间
                                if (listone.Count > 0)
                                {
                                    foreach (var li in listone)
                                    {
                                        string cap = li.captureTime.ToString("t").Replace(":", "");
                                        //判断是否在有效区间
                                        if (int.Parse(cap) <= int.Parse(sbEffectiveTime_end) && int.Parse(cap) >= int.Parse(sbEffectiveTime_sat))
                                        {
                                            liststa.Add(listone[listone.IndexOf(li)]);
                                        }
                                        else if (int.Parse(cap) <= int.Parse(xbEffectiveTime_end) && int.Parse(cap) >= int.Parse(xbEffectiveTime_sta))
                                        {
                                            listend.Add(listone[listone.IndexOf(li)]);
                                        }
                                        else if (int.Parse(cap) <= int.Parse(sbEffectiveTime_end2) && int.Parse(cap) >= int.Parse(sbEffectiveTime_sat2))
                                        {
                                            liststa2.Add(listone[listone.IndexOf(li)]);
                                        }
                                        else if (int.Parse(cap) <= int.Parse(xbEffectiveTime_end2) && int.Parse(cap) >= int.Parse(xbEffectiveTime_sta2))
                                        {
                                            listend2.Add(listone[listone.IndexOf(li)]);
                                        }
                                    }
                                }
                                //找到上班时段的list最早的时间记录
                                if (liststa.Count > 0)
                                {
                                    DateTime FastDateTime = liststa[0].captureTime;
                                    for (int min = 0; min < liststa.Count; min++)
                                    {
                                        if (DateTime.Compare(FastDateTime, liststa[min].captureTime) >= 0)
                                        {
                                            FastDateTime = liststa[min].captureTime;
                                        }
                                    }
                                    reData.temperature = Math.Round(liststa[0].temperature, 2).ToString();
                                    reData.closeup = liststa[0].closeup;
                                    reData.Punchinformation = FastDateTime.ToString("t");
                                    //计算是否迟到
                                    string FastDateTimestr = FastDateTime.ToString("t").Replace(":", "");
                                    if (int.Parse(FastDateTimestr) > int.Parse(stagotowork1.Replace(":", "")))
                                    {
                                        reData.late = DateDiff(FastDateTimestr, stagotowork1.Replace(":", ""));
                                    }
                                }
                                //找到最晚的时间记录
                                if (listend.Count > 0)
                                {
                                    DateTime FastDateTime = listend[0].captureTime;
                                    for (int min = 0; min < listend.Count; min++)
                                    {
                                        if (DateTime.Compare(FastDateTime, listend[min].captureTime) <= 0)
                                        {
                                            FastDateTime = listend[min].captureTime;
                                        }
                                        //else
                                        //{
                                        //    FastDateTime = FastDateTime;
                                        //}
                                    }
                                    //计算是否早退
                                    reData.temperature1 = Math.Round(listend[0].temperature, 2).ToString();
                                    reData.closeup = listend[0].closeup;
                                    reData.Punchinformation1 = FastDateTime.ToString("t");
                                    string FastDateTimestr = FastDateTime.ToString("t").Replace(":", "");
                                    if (int.Parse(FastDateTimestr) < int.Parse(endgotowork1.Replace(":", "")))
                                    {
                                        reData.ISLeaveearly = true;
                                        reData.Leaveearly = DateDiff(FastDateTimestr, endgotowork1.Replace(":", ""));
                                    }//是否加班
                                    //else if (int.Parse(FastDateTimestr) > int.Parse(endgotowork1.Replace(":", "")))
                                    //{
                                    //    reData.workOvertime = DateDiff(endgotowork1.Replace(":", ""), FastDateTimestr);
                                    //}
                                }

                                //计算二段考勤
                                if (liststa2.Count > 0)
                                {
                                    DateTime FastDateTime = liststa2[0].captureTime;
                                    for (int min = 0; min < liststa2.Count; min++)
                                    {
                                        if (DateTime.Compare(FastDateTime, liststa2[min].captureTime) >= 0)
                                        {
                                            FastDateTime = liststa2[min].captureTime;
                                        }
                                    }
                                    if (string.IsNullOrEmpty(reData.temperature))
                                    {
                                        reData.temperature = Math.Round(liststa2[0].temperature, 2).ToString();
                                    }
                                    reData.closeup = liststa2[0].closeup;
                                    reData.Punchinformation2 = FastDateTime.ToString("t");
                                    //计算是否迟到
                                    string FastDateTimestr = FastDateTime.ToString("t").Replace(":", "");
                                    if (int.Parse(FastDateTimestr) > int.Parse(stagotowork2.Replace(":", "")))
                                    {
                                        if (!string.IsNullOrEmpty(reData.late))
                                        {
                                            reData.late = (int.Parse(DateDiff(FastDateTimestr, stagotowork2.Replace(":", ""))) +int.Parse(reData.late)).ToString();
                                        }
                                        else
                                        {
                                            reData.late = DateDiff(FastDateTimestr, stagotowork2.Replace(":", ""));
                                        }
                                    }
                                }
                                //找到最晚的时间记录
                                if (listend2.Count > 0)
                                {
                                    DateTime FastDateTime = listend2[0].captureTime;
                                    for (int min = 0; min < listend2.Count; min++)
                                    {
                                        if (DateTime.Compare(FastDateTime, listend2[min].captureTime) <= 0)
                                        {
                                            FastDateTime = listend2[min].captureTime;
                                        }
                                        //else
                                        //{
                                        //    FastDateTime = FastDateTime;
                                        //}
                                    }
                                    reData.temperature1 = Math.Round(listend2[0].temperature, 2).ToString();
                                    reData.closeup = listend2[0].closeup;
                                    reData.Punchinformation22 = FastDateTime.ToString("t");
                                    string FastDateTimestr = FastDateTime.ToString("t").Replace(":", "");
                                    if (int.Parse(FastDateTimestr) < int.Parse(endgotowork2.Replace(":", "")))
                                    {
                                        reData.ISLeaveearly = true;
                                        
                                        if (!string.IsNullOrEmpty(reData.Leaveearly))
                                        {
                                            reData.Leaveearly =(int.Parse(DateDiff(FastDateTimestr, endgotowork2.Replace(":", ""))) +int.Parse(reData.Leaveearly)).ToString();
                                        }
                                        else
                                        {
                                            reData.Leaveearly = DateDiff(FastDateTimestr, endgotowork2.Replace(":", ""));
                                        }
                                    }//是否加班
                                    //else if (int.Parse(FastDateTimestr) > int.Parse(endgotowork2.Replace(":", "")))
                                    //{
                                    //    if (!string.IsNullOrEmpty(reData.workOvertime))
                                    //    {
                                    //        reData.workOvertime =(int.Parse(DateDiff(endgotowork2.Replace(":", ""), FastDateTimestr))+int.Parse(reData.workOvertime) ).ToString();
                                    //    }
                                    //    else
                                    //        reData.workOvertime = DateDiff(endgotowork2.Replace(":", ""), FastDateTimestr);
                                    //}
                                }
                                //把考勤数据存到数据库
                                if (!string.IsNullOrEmpty(reData.personId))
                                {
                                    setAttendance_Data2(reData, stagotowork1.Replace(":", ""), endgotowork1.Replace(":", ""), stagotowork2.Replace(":", ""), endgotowork2.Replace(":", ""));
                                }
                            }
                            //三时段
                            //else if (!string.IsNullOrEmpty(gotowork1) && !string.IsNullOrEmpty(gotowork2)
                            //    && !string.IsNullOrEmpty(gotowork3))
                            //{

                            //}
                        }
                    }
                }
            }
        }


        

        public static void setAttendance_Data(reData reData,string stagotowork1,string endgotowork1)
        {
            try
            {
                GetData.setAttendance_Data(reData, stagotowork1,endgotowork1);
            }
            catch(Exception ex)
            {}
        }

        public static void setAttendance_Data2(reData reData, string stagotowork1, string endgotowork1, string stagotowork2, string endgotowork2)
        {
            try
            {
                GetData.setAttendance_Data(reData, stagotowork1, endgotowork1, stagotowork2, endgotowork2);
            }
            catch (Exception ex)
            { }
        }
        public static string DateDiff(string DateTime1, string DateTime2)
        {

            int re = 0;
            try
            {
                if (DateTime1.Length > 2 && DateTime2.Length > 2)
                {
                    string mm1 = DateTime1.Substring(DateTime1.Length - 2, 2);
                    string mm2 = DateTime2.Substring(DateTime2.Length - 2, 2);

                    string hh1 = DateTime1.Substring(0, DateTime1.Length - 2);
                    string hh2 = DateTime2.Substring(0, DateTime2.Length - 2);


                    int h = int.Parse(hh1) - int.Parse(hh2);
                    int m = int.Parse(mm1) - int.Parse(mm2);

                    re = h*60 + m;
                    re = Math.Abs(re);
                }
            }
            catch(Exception ex)
            {
            }
            return re.ToString();
        }
        //获取当前周几
        private static string GetWeek(string datetime)
        {
            string week = string.Empty;
            switch ((int)DateTime.Parse(datetime).DayOfWeek)
            {
                case 1:
                    week = "Monday";
                    break;
                case 2:
                    week = "Tuesday";
                    break;
                case 3:
                    week = "Wednesday";
                    break;
                case 4:
                    week = "Thursday";
                    break;
                case 5:
                    week = "Friday";
                    break;
                case 6:
                    week = "Saturday";
                    break;
                default:
                    week = "Sunday";
                    break;
            }
            return week;
        }

        private static string GetWeek()
        {
            string week = string.Empty;
            switch ((int)DateTime.Now.DayOfWeek)
            {
                case 1:
                    week = "Monday";
                    break;
                case 2:
                    week = "Tuesday";
                    break;
                case 3:
                    week = "Wednesday";
                    break;
                case 4:
                    week = "Thursday";
                    break;
                case 5:
                    week = "Friday";
                    break;
                case 6:
                    week = "Saturday";
                    break;
                default:
                    week = "Sunday";
                    break;
            }
            return week;
        }
    }
    class data
    {
        public DateTime captureTime { get; set; }
        public int matchScore { get; set; }
        public string personName { get; set; }
        public float temperature { get; set; }
        public string personId { get; set; }
        public string Employee_code { get; set; }//closeup
        public string closeup { get; set; }
    }

    class reData
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string stagotowork1 { get; set; }//姓名
        /// <summary>
        /// 姓名
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string name { get; set; }//姓名

        /// <summary>
        /// 部门
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string department { get; set; }//姓名

        /// <summary>
        /// 员工id
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string personId { get; set; }//员工ID
        /// <summary>
        /// 员工编号
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Employee_code { get; set; }//员工ID
        /// <summary>
        /// 考勤日期
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Date { get; set; }//考勤日期
        /// <summary>
        /// 上班打卡信息
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Punchinformation { get; set; }//打卡信息
        /// <summary>
        /// 上班体温
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string temperature { get; set; }//体温

        /// <summary>
        /// 下班体温
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string temperature1 { get; set; }//体温
        /// <summary>
        /// 下班打卡信息
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Punchinformation1 { get; set; }//打卡信息
        /// <summary>
        /// 二时段下班打卡信息
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Punchinformation2 { get; set; }//二时段打卡信息
        /// <summary>
        /// 二时段下班打卡信息
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Punchinformation22 { get; set; }//二时段打卡信息
        /// <summary>
        /// 班次信息
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Shiftinformation { get; set; }//班次信息
        /// <summary>
        /// 班次时长
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Duration { get; set; }//班次信息
        /// <summary>
        /// 迟到
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string late { get; set; }//迟到
        /// <summary>
        /// 加班
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string workOvertime { get; set; }//迟到
        /// <summary>
        /// 早退
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Leaveearly { get; set; }//早退
        /// <summary>
        /// 是否早退
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public bool ISLeaveearly { get; set; }//早退
        /// <summary>
        /// 是否旷工 0是 1否
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string isAbsenteeism { get; set; }//是否旷工
        /// <summary>
        /// 图片路径
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string closeup { get; set; }//是否旷工
        /// <summary>
        /// 是否跨夜
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public bool IsAcrossNight { get; set; }//是否跨夜
    }
}

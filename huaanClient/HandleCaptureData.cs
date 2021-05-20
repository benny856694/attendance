using DBUtility.SQLite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using static huaanClient.CameraConfigPort;

namespace huaanClient
{
    class HandleCaptureData
    {
        public static void setCaptureDataToDatabase(CaptureDataEventArgs CaptureData, string DeviceNo,string DeviceName)
        {
            if (string.IsNullOrEmpty(DeviceNo))
            {
                DeviceNo = CaptureData.device_sn;
            }
            if (string.IsNullOrEmpty(DeviceName))
            {
                DeviceName = CaptureData.addr_name;
            }

            string time = CaptureData.time.ToString("yyyy-MM-dd") + " " +CaptureData.time.TimeOfDay;
            //先根据设备编号和编号去查询是否重复time
            string spl = "SELECT COUNT(*) as len FROM Capture_Data WHERE time=='"+ time.TrimEnd('0') + "' AND device_sn='" + DeviceNo.Trim() + "'";
            string quIPsr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, spl);
            if (!string.IsNullOrEmpty(quIPsr))
            {
                JArray jo = (JArray)JsonConvert.DeserializeObject(quIPsr);
                string reint = jo[0]["len"].ToString();
                if (int.Parse(reint) > 0)
                    return;
            }

            //string connectionString = "Data Source=" + Application.StartupPath + @"\huaanDatabase.sqlite;Version=3;";
            string connectionString = ApplicationData.connectionString;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Capture_Data(");
            strSql.Append("sequnce,device_id,addr_name,time,match_status,person_id,person_name,hatColor,wg_card_id,match_failed_reson,exist_mask,body_temp,device_sn,idcard_number,idcard_name,closeup,match_type,QRcodestatus,QRcode,trip_infor)");
            strSql.Append(" values (");
            strSql.Append("@sequnce,@device_id,@addr_name,@time,@match_status,@person_id,@person_name,@hatColor,@wg_card_id,@match_failed_reson,@exist_mask,@body_temp,@device_sn,@idcard_number,@idcard_name,@closeup,@match_type,@QRcodestatus,@QRcode,@trip_infor)");

            //如果遇到健康码版本相机 身份证号码带*号  历史记录存放在ID里面的
            if (string.IsNullOrEmpty(CaptureData.idcard_number))
            {
                CaptureData.idcard_number = "";
            }
            string wg_card_id = string.Empty;
            //获取对应人员的 韦根卡号
            if (!string.IsNullOrEmpty(CaptureData.person_id)&& !CaptureData.person_id.Contains("*"))
            {
                string wg_card_iddata = GetData.getwg_card_id(CaptureData.person_id.Trim());
                JArray jo = (JArray)JsonConvert.DeserializeObject(wg_card_iddata);
                if (jo.Count > 0)
                {
                    wg_card_id = jo[0]["face_idcard"].ToString();
                }
            }
            else if (!string.IsNullOrEmpty(CaptureData.person_id) && CaptureData.person_id.Contains("*") && string.IsNullOrEmpty(CaptureData.idcard_number))
            {
                if (SubstringCount(CaptureData.person_id, "*") > 2)
                {
                    CaptureData.idcard_number = CaptureData.person_id;
                }
                //else
                //{
                //    CaptureData.person_id = "0";
                //}
            }
            //else
            //{
            //    CaptureData.person_id = "0";
            //}


            if (string.IsNullOrEmpty(CaptureData.person_name))
            {
                if (!string.IsNullOrEmpty(CaptureData.person_name_ext))
                {
                    CaptureData.person_name = CaptureData.person_name_ext;
                }
                else
                {
                    CaptureData.person_name = "";
                }  
            }
            SQLiteParameter[] parameters = {
            new SQLiteParameter("@sequnce", CaptureData.sequnce),
            new SQLiteParameter("@device_id", CaptureData.device_id),
            new SQLiteParameter("@addr_name", DeviceName),
            new SQLiteParameter("@time", CaptureData.time),
            new SQLiteParameter("@match_status", CaptureData.match_status),
            new SQLiteParameter("@person_id", CaptureData.person_id),
            new SQLiteParameter("@person_name", CaptureData.person_name),
            new SQLiteParameter("@hatColor", CaptureData.hatColor),
            //new SQLiteParameter("@wg_card_id", CaptureData.wg_card_id),
            new SQLiteParameter("@wg_card_id", wg_card_id),
            new SQLiteParameter("@match_failed_reson", CaptureData.match_failed_reson),
            new SQLiteParameter("@exist_mask", CaptureData.exist_mask),
            new SQLiteParameter("@body_temp", CaptureData.body_temp),
            new SQLiteParameter("@device_sn", DeviceNo),
            //new SQLiteParameter("@device_sn", DeviceNo),
            new SQLiteParameter("@idcard_number", CaptureData.idcard_number),
            new SQLiteParameter("@idcard_name", CaptureData.idcard_name),
            new SQLiteParameter("@closeup", CaptureData.closeup),
            new SQLiteParameter("@match_type", CaptureData.match_type),
            new SQLiteParameter("@QRcodestatus", CaptureData.customer_text),
            new SQLiteParameter("@QRcode", CaptureData.QRcode),
            new SQLiteParameter("@trip_infor", CaptureData.trip_infor),
            };
            SQLiteHelper.ExecSQL(connectionString, strSql.ToString(), parameters);
        }
        /// <summary>
        /// 计算字符串中子串出现的次数
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="substring">子串</param>
        /// <returns>出现的次数</returns>
        static int SubstringCount(string str, string substring)
        {
            try
            {
                if (str.Contains(substring))
                {
                    string strReplaced = str.Replace(substring, "");
                    return (str.Length - strReplaced.Length) / substring.Length;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        //获取line管理员邮箱
        public static void getstaffforlineAdminEmail()
        {
            //先移除所有的元素
            if (ApplicationData.lineadmin!=null)
            {
                ApplicationData.lineadmin.Clear();
            }
            else
            {
                ApplicationData.lineadmin = new List<string>();
                ApplicationData.lineadmin.Clear();
            }
            string linedmindata = GetData.getstaffforlineAdminEmail();
            if (linedmindata.Length > 2)
            {
                JArray linejArray = (JArray)JsonConvert.DeserializeObject(linedmindata);
                if (linejArray.Count > 0)
                {
                    foreach (var l in linejArray)
                    {
                        ApplicationData.lineadmin.Add(l["Email"].ToString());
                    }        
                }
            }
        }
        public static void httptoline(reData reData)
        {
            if (!string.IsNullOrEmpty(reData.personId))
            {
                string line_userid = string.Empty;
                string data = GetData.getline_useridforstaff(reData.personId);
                if (data.Length > 2)
                {
                    JArray jArray = (JArray)JsonConvert.DeserializeObject(data);
                    if (jArray.Count > 0)
                    {
                        if(jArray[0]["line_type"].ToString().Trim()=="1"|| string.IsNullOrEmpty(jArray[0]["line_type"].ToString().Trim()))
                        {
                            line_userid = jArray[0]["line_userid"].ToString().Trim() + "&";
                        }
                        else if(jArray[0]["line_type"].ToString().Trim() == "2")
                        {
                            line_userid = "mailTo:"+jArray[0]["Email"].ToString().Trim() + "&";
                        }
                        else if (jArray[0]["line_type"].ToString().Trim() == "3")
                        {
                            line_userid = jArray[0]["line_userid"].ToString().Trim()+"&"+ "mailTo:" + jArray[0]["Email"].ToString().Trim();
                        }
                    }
                }
                if (string.IsNullOrEmpty(line_userid))
                {
                    return;
                }
                string[] line_userids = line_userid.Split('&');
                if (line_userids.Length>1)
                {
                    foreach (var line_useridstr in line_userids)
                    {
                        if (line_useridstr.Length<8) continue;
                        httpToLineorEmail(reData,line_useridstr);
                    }
                }

                if (IsBetweenTime(reData.Date, DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00", DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59"))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(reData.Punchinformation)&& !string.IsNullOrEmpty(reData.Punchinformation1))
                        {
                            return;
                        }
                        //发送到管理员
                        setFtpImge(reData, false, "");
                    }
                    catch { }
                }
            }
        }

        public static void httpToLineorEmail(reData reData,string line_userid) {
            if (!IsBetweenTime(reData.Date, DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00", DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59"))
            {
                return;
            }
            string messge = string.Empty;

            

            //正常上学打卡
            if (!string.IsNullOrEmpty(reData.Punchinformation) && string.IsNullOrEmpty(reData.late) && string.IsNullOrEmpty(reData.Punchinformation1))
            {
                if (float.Parse(reData.temperature) > 0)
                {
                    if (float.Parse(reData.temperature) > float.Parse(ApplicationData.temperature))
                        messge = ApplicationData.lineMessage3;
                    else
                        messge = ApplicationData.lineMessage2;
                }
                else
                    messge = ApplicationData.lineMessage;
                messge = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + reData.name + string.Format(messge, reData.temperature);


                if (!GetData.getLine_list_data(reData.Date, "Attendance", line_userid))
                {
                    //发送到line
                    if (!string.IsNullOrEmpty(ApplicationData.line_url))
                    {
                        string url = ApplicationData.line_url + "/sendmessage.php?userid=" + line_userid + "&message=" + messge;
                        string INUserIDs_str = HttpGet(url);
                        try
                        {
                            JObject jObject = (JObject)JsonConvert.DeserializeObject(INUserIDs_str);
                            if (jObject["result"].ToString().Contains("OK"))
                            {
                                GetData.setLine_list(reData, messge, "Attendance", reData.temperature, reData.Date, reData.Punchinformation, line_userid, "1");
                            }
                            else
                                GetData.setLine_list(reData, messge, "Attendance", reData.temperature, reData.Date, reData.Punchinformation, line_userid, "0");
                        }
                        catch
                        {
                            GetData.setLine_list(reData, messge, "Attendance", reData.temperature, reData.Date, reData.Punchinformation, line_userid, "0");
                        }
                    }
                }

                //if (IsBetweenTime(reData.Date, DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00", DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59"))
                //{
                //    try
                //    {
                //        //发送到管理员
                //        setFtpImge(reData, true, messge);
                //    }
                //    catch { }
                //}
            }
            //正常放学打卡
            if (!string.IsNullOrEmpty(reData.Punchinformation1) && !reData.ISLeaveearly)
            {
                if (float.Parse(reData.temperature1) > 0)
                {
                    if (float.Parse(reData.temperature1) > float.Parse(ApplicationData.temperature))
                        messge = ApplicationData.lineMessage12;
                    else
                        messge = ApplicationData.lineMessage11;
                }
                else
                    messge = ApplicationData.lineMessage10;
                messge = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + reData.name + string.Format(messge, reData.temperature1);

                //发送到line
                if (!GetData.getLine_list_data(reData.Date, "Offduty", line_userid))
                {
                    //发送到line
                    if (!string.IsNullOrEmpty(ApplicationData.line_url))
                    {
                        string url = ApplicationData.line_url + "/sendmessage.php?userid=" + line_userid + "&message=" + messge;
                        string INUserIDs_str = HttpGet(url);
                        try
                        {
                            JObject jObject = (JObject)JsonConvert.DeserializeObject(INUserIDs_str);
                            if (jObject["result"].ToString().Contains("OK"))
                            {
                                GetData.setLine_list(reData, messge, "Offduty", reData.temperature1, reData.Date, reData.Punchinformation1, line_userid, "1");
                            }
                            else
                                GetData.setLine_list(reData, messge, "Offduty", reData.temperature1, reData.Date, reData.Punchinformation1, line_userid, "0");
                        }
                        catch
                        {
                            GetData.setLine_list(reData, messge, "Offduty", reData.temperature1, reData.Date, reData.Punchinformation1, line_userid, "0");
                        }
                    }

                    
                }
                //if (IsBetweenTime(reData.Date, DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00", DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59"))
                //{
                //    try
                //    {
                //        //发送到管理员
                //        setFtpImge(reData, false, messge);
                //    }
                //    catch { }
                //}
            }
            //迟到
            else if (!string.IsNullOrEmpty(reData.late) && string.IsNullOrEmpty(reData.Punchinformation1))
            {
                if (float.Parse(reData.temperature) > 0)
                {
                    if (float.Parse(reData.temperature) > float.Parse(ApplicationData.temperature))
                        messge = ApplicationData.lineMessage6;
                    else
                        messge = ApplicationData.lineMessage5;
                }
                else
                    messge = ApplicationData.lineMessage4;
                messge = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + reData.name + string.Format(messge, reData.temperature);
                //保存到数据库 经过客户要求于20201027修改为直接发送
                //GetData.setLine_list(reData,messge,"late",reData.temperature, reData.Date, reData.Punchinformation, line_userid,"0");
                //发送到line
                if (!GetData.getLine_list_data(reData.Date, "late", line_userid))
                {
                    //发送到line
                    if (!string.IsNullOrEmpty(ApplicationData.line_url))
                    {
                        string url = ApplicationData.line_url + "/sendmessage.php?userid=" + line_userid + "&message=" + messge;
                        string INUserIDs_str = HttpGet(url);
                        try
                        {
                            JObject jObject = (JObject)JsonConvert.DeserializeObject(INUserIDs_str);
                            if (jObject["result"].ToString().Contains("OK"))
                            {
                                GetData.setLine_list(reData, messge, "late", reData.temperature, reData.Date, reData.Punchinformation, line_userid, "1");
                            }
                            else
                                GetData.setLine_list(reData, messge, "late", reData.temperature, reData.Date, reData.Punchinformation, line_userid, "0");
                        }
                        catch
                        {
                            GetData.setLine_list(reData, messge, "late", reData.temperature, reData.Date, reData.Punchinformation, line_userid, "0");
                        }
                    }
                    
                }
                //if (IsBetweenTime(reData.Date, DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00", DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59"))
                //{
                //    try
                //    {
                //        //发送到管理员
                //        setFtpImge(reData, false, messge);
                //    }
                //    catch { }
                //}

            }
            //早退
            else if (reData.ISLeaveearly)
            {
                if (float.Parse(reData.temperature1) > 0)
                {
                    if (float.Parse(reData.temperature1) > float.Parse(ApplicationData.temperature))
                        messge = ApplicationData.lineMessage9;
                    else
                        messge = ApplicationData.lineMessage8;
                }
                else
                    messge = ApplicationData.lineMessage7;
                messge = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + reData.name + string.Format(messge, reData.temperature1);
                GetData.setLine_list(reData, messge, "Leave", reData.temperature1, reData.Date, reData.Punchinformation1, line_userid, "0");      
            }
        }

        public static bool IsBetweenTime(string timeStr, string startTime, string endTime)
        {
            //判断当前时间是否在工作时间段内
            try
            {
                timeStr = timeStr + " 01:00:00";//2020-11-10 11:15:08 00:00:00
                TimeSpan startSpan = DateTime.Parse(startTime).TimeOfDay;
                TimeSpan endSpan = DateTime.Parse(endTime).TimeOfDay;


                //string time1 = "2017-2-17 8:10:00";
                DateTime t1 = Convert.ToDateTime(timeStr);


                TimeSpan dspNow = t1.TimeOfDay;
                if (dspNow > startSpan && dspNow < endSpan)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }


        }
        /// <summary>
        /// 发送请求的方法
        /// </summary>
        /// <param name="Url">地址</param>
        /// <param name="postDataStr">数据</param>
        /// <returns></returns>
        private static string post(string url, string param)
        {
            try
            {
                //win7以上
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            }
            catch (Exception)
            {
                //xp系统
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            }
            string cookie = "";
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = url,//URL     必需项    
                Method = "post",//URL     可选项 默认为Get   
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                //Cookie = "PHPSESSID=fa0dafe4dc9f10f930ff247f7ef207dc",//字符串Cookie     可选项   
                Referer = "",//来源URL     可选项   
                Postdata = param,//Post数据     可选项GET时不需要写   
                Timeout = 100000,//连接超时时间     可选项默认为100000    
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.122 Safari/537.36",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                Allowautoredirect = true,//是否根据301跳转     可选项   
                //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数    
                //ProxyPwd = "123456",//代理服务器密码     可选项    
                //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                ResultType = ResultType.String
            };
            HttpResult result = http.GetHtml(item);
            string html = result.Html;
            string[] values = result.Header.GetValues("set-cookie");
            if (values.Length > 0)
                ApplicationData.Cookiek = values[0].ToString();
            return html;
        }
        public static string HttpGet(string url)
        {
            if (ApplicationData.Isfirsthttptoline)
            {
                //登录
                string re = post(ApplicationData.line_url + "/login.php", "loginid=heatcheck&pass=mizuma4649");
                JObject jObject = (JObject)JsonConvert.DeserializeObject(re);
                if (jObject["result"].ToString().Contains("OK") && !string.IsNullOrEmpty(ApplicationData.Cookiek))
                {
                    ApplicationData.Isfirsthttptoline = false;
                }
            }
            try
            {
                //win7以上
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            }
            catch (Exception)
            {
                //xp系统
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            }
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {

                URL = url,
                Encoding = System.Text.Encoding.GetEncoding("utf-8"),//URL     可选项 默认为Get 
                Method = "get",//URL     可选项 默认为Get   
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                Cookie = ApplicationData.Cookiek,
                Referer = "",//来源URL     可选项   
                Postdata = "",//Post数据     可选项GET时不需要写   
                Timeout = 100000,//连接超时时间     可选项默认为100000    
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                Allowautoredirect = true,//是否根据301跳转     可选项   
                //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                //ProxyIp = MyApplication.CurInsCompInfo.Proxy,//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数    
                //ProxyPwd = "123456",//代理服务器密码     可选项    
                //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                ResultType = ResultType.String
            };
            HttpResult result = http.GetHtml(item);
            string html = result.Html;
            return html;
        }

        //发送数据到ftp服务器  修改成发送到STP邮箱服务器
        public static bool setFtpImge(reData reData, bool IsAtt, string Messge)
        {
            string temperatureforDta = string.Empty;
            string temperatureforDta2 = string.Empty;
            if (IsAtt)
            {
                temperatureforDta = reData.temperature;
            }
            else
            {
                temperatureforDta = reData.temperature;
                temperatureforDta2 = reData.temperature1;
            }
            if (string.IsNullOrEmpty(ApplicationData.ftpserver) ||
                string.IsNullOrEmpty(ApplicationData.ftppassword) ||
                string.IsNullOrEmpty(ApplicationData.ftpusername))
            {
                return false;
            }
            bool re = false;
            if (string.IsNullOrEmpty(reData.personId) || string.IsNullOrEmpty(reData.closeup)
                )
            {
                return re;
            }


            string ti = string.Empty;
            string temperature = ApplicationData.temperature;
            if (string.IsNullOrEmpty(temperature))
            {
                return re;
            }
            if (!string.IsNullOrEmpty(temperatureforDta))
            {
                if (float.Parse(temperatureforDta) <= float.Parse(temperature.Trim()))
                {
                    return re;
                }
                ti = temperatureforDta;
            }
            else if (!string.IsNullOrEmpty(temperatureforDta2))
            {
                if (float.Parse(temperatureforDta2) <= float.Parse(temperature.Trim()))
                {
                    return re;
                }
                ti = temperatureforDta2;
            }
            else
            {
                return re;
            }
            
            //先发送缩略图
            //if (FTPHelper.UploadFile(reData.closeup, "preview"))
            //{
            //    //再次发送正式图到更目录下
            //    if (!FTPHelper.UploadFile(reData.closeup, ""))
            //        return false;
            //}


            if (ApplicationData.lineadmin.Count==0)
                return re;

            foreach (var Email in ApplicationData.lineadmin)
            {
                if (string.IsNullOrEmpty(Email)||Email=="null")
                {
                    continue;
                }
                //发送邮件
                FTPHelper.Page_Load(ApplicationData.ftpserver,
                    ApplicationData.ftpusername,
                    ApplicationData.ftppassword,
                    Email,
                    "体温が異常である",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"体温異常が検出されました。体温：”" + ti + "”℃",
                    reData.closeup
                    );


                //string url = ApplicationData.line_url + "/sendmessage.php?userid=mailTo:" + Email + "&message="+ "体温異常が検出されました。体温：”"+ temperatureforDta + "”℃"+" & image=" + Path.GetFileName(reData.closeup);
                //string INUserIDs_str = HttpGet(url);
                //JObject jObject = (JObject)JsonConvert.DeserializeObject(INUserIDs_str);

                //if (jObject["result"].ToString().Contains("OK"))
                //{
                //    re = true;
                //}
            }

            return re;
        }
        public static string generatecode(string id)
        {
            string re = "ERROR";
            try
            {
                if (string.IsNullOrEmpty(id))
                    return re;
                string code = string.Empty;
                //先获取是否已经生成code
                string linecodedata = GetData.getstaffline_code(id);
                if (!string.IsNullOrEmpty(linecodedata))
                {
                    JArray jo = (JArray)JsonConvert.DeserializeObject(linecodedata);
                    string line_userid = jo[0]["line_userid"].ToString();
                    if (line_userid.Trim().Length > 0)
                    {
                        return "success";
                    }
                    string line_code = jo[0]["line_code"].ToString();
                    if (!string.IsNullOrEmpty(line_code) && line_code.Trim().Length == 6)
                    {
                        code = line_code;
                    }
                }

                //生成六位数的随机数，然后到数据库查询是否重复
                if (string.IsNullOrEmpty(code))
                {
                    code = GetRandomString(6);
                    //只能循环
                    for (int i = 0; i < 999999; i++)
                    {
                        string commandTextdepartmentid = "SELECT COUNT(*) as len FROM staff WHERE line_code=" + code ;
                        string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
                        if (!string.IsNullOrEmpty(sr))
                        {
                            JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                            string reint = jo[0]["len"].ToString();
                            if (int.Parse(reint) > 0)
                            {
                                code = GetRandomString(6);
                            }
                            else
                            {
                                string url = ApplicationData.line_url + "/addUser.php?id=" + id + "&code=" + code;
                                string INUserIDs_str = HttpGet(url);
                                JObject jObject = (JObject)JsonConvert.DeserializeObject(INUserIDs_str);

                                if (jObject["result"].ToString().Contains("OK"))
                                {
                                    GetData.setstaffline_code(id, code);
                                    re = "success";
                                }

                                //将生成的可用code写进数据库
                                break;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return re;
            }
            return re;
        }

        public static string generatecodemail(string id)
        {
            string re = "ERROR";
            try
            {
                if (string.IsNullOrEmpty(id))
                    return re;
                string code = string.Empty;
                //先获取是否已经生成code
                string linecodedata = GetData.getstaffline_code(id);
                if (!string.IsNullOrEmpty(linecodedata))
                {
                    JArray jo = (JArray)JsonConvert.DeserializeObject(linecodedata);
                    string line_codemail = jo[0]["line_codemail"].ToString();
                    if (!string.IsNullOrEmpty(line_codemail) && line_codemail.Trim().Length == 6)
                    {
                        code = line_codemail;
                    }
                }

                //生成六位数的随机数，然后到数据库查询是否重复
                if (string.IsNullOrEmpty(code))
                {
                    code = GetRandomString(6);
                    //只能循环
                    for (int i = 0; i < 999999; i++)
                    {
                        string commandTextdepartmentid = "SELECT COUNT(*) as len FROM staff WHERE  line_codemail=" + code;
                        string sr = SQLiteHelper.SQLiteDataReader(ApplicationData.connectionString, commandTextdepartmentid);
                        if (!string.IsNullOrEmpty(sr))
                        {
                            JArray jo = (JArray)JsonConvert.DeserializeObject(sr);
                            string reint = jo[0]["len"].ToString();
                            if (int.Parse(reint) > 0)
                            {
                                code = GetRandomString(6);
                            }
                            else
                            {
                                string url = ApplicationData.line_url + "/addMail.php?id=" + id + "&codemail=" + code;
                                string INUserIDs_str = HttpGet(url);
                                JObject jObject = (JObject)JsonConvert.DeserializeObject(INUserIDs_str);

                                if (jObject["result"].ToString().Contains("OK"))
                                {
                                    GetData.setstaffline_codemail(id, code);
                                    re = "success";
                                }

                                //将生成的可用code写进数据库
                                break;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return re;
            }
            return re;
        }


        public static string getLINUserID(string id)
        {
            string re = "ERROR";
            try
            {
                if (string.IsNullOrEmpty(id))
                    return re;
                string code = string.Empty;
                //先获取是否已经生成code
                string linecodedata = GetData.getstaffline_code(id);
                if (!string.IsNullOrEmpty(linecodedata))
                {
                    JArray jo = (JArray)JsonConvert.DeserializeObject(linecodedata);
                    string line_userid = jo[0]["line_userid"].ToString();
                    if (line_userid.Trim().Length > 0)
                    {
                        return "success-" + line_userid.Trim();
                    }
                    string line_code = jo[0]["line_code"].ToString();
                    if (!string.IsNullOrEmpty(line_code) || line_code.Trim().Length == 6)
                    {
                        code = line_code;
                    }
                }
                if (!string.IsNullOrEmpty(code))
                {
                    string url = ApplicationData.line_url + "/getUserID.php?id=" + id;

                    string INUserIDs_str = HttpGet(url);
                    JObject jObject = (JObject)JsonConvert.DeserializeObject(INUserIDs_str);
                    string sss = jObject["LINEUserID"].ToString();

                    if (!jObject["LINEUserID"].ToString().Contains("ERROR") && !string.IsNullOrEmpty(jObject["LINEUserID"].ToString()))
                    {
                        if (!jObject["LINEUserID"].ToString().Contains("LINEID is empty"))
                        {
                            string LINUserID = jObject["LINEUserID"].ToString();
                            bool ru = GetData.setline_userid(id, LINUserID);
                            if (ru)
                                re = "success-" + LINUserID.Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return re;
        }


        public static string getLINEEmail(string id)
        {
            string re = "ERROR";
            try
            {
                if (string.IsNullOrEmpty(id))
                    return re;
                string code = string.Empty;
                //先获取是否已经生成code
                string linecodedata = GetData.getstaffline_code(id);
                if (!string.IsNullOrEmpty(linecodedata))
                {
                    JArray jo = (JArray)JsonConvert.DeserializeObject(linecodedata);
                    //string Email = jo[0]["Email"].ToString();
                    //if (Email.Trim().Length > 0)
                    //{
                    //    return "success-" + Email.Trim();
                    //}
                    string line_code = jo[0]["line_codemail"].ToString();
                    if (!string.IsNullOrEmpty(line_code) || line_code.Trim().Length == 6)
                    {
                        code = line_code;
                    }
                }
                if (!string.IsNullOrEmpty(code))
                {
                    string url = ApplicationData.line_url + "/getUserID.php?id=" + id;

                    string Emails_str = HttpGet(url);
                    JObject jObject = (JObject)JsonConvert.DeserializeObject(Emails_str);
                    string sss = jObject["mail"].ToString();

                    if (!jObject["mail"].ToString().Contains("ERROR") && !string.IsNullOrEmpty(jObject["mail"].ToString()))
                    {
                        if (!jObject["mail"].ToString().Contains("mail is empty")&&!jObject["mail"].ToString().Contains("found"))
                        {
                            string Email = jObject["mail"].ToString();
                            bool ru = GetData.setline_Email(id, Email);
                            if (ru)
                                re = "success-" + Email.Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return re;
        }
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，不然打不开
            return true;
        }


        /// <summary>
        /// 生成6位随机数
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomString(int len)
        {
            //string s = "123456789abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ";
            string s = "0123456789";
            string reValue = string.Empty;
            Random rd = new Random();
            while (reValue.Length < len)
            {
                string s1 = s[rd.Next(0, s.Length)].ToString();
                if (reValue.IndexOf(s1) == -1)
                    reValue += s1;
            }
            return reValue;
        }
    }
}

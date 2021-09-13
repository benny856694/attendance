﻿using CefSharp.WinForms;
using huaanClient.Database;
using InsuranceBrowser;
using InsuranceBrowserLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace huaanClient
{
    static class Program
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            DapperExtensions.DapperExtensions.SetMappingAssemblies(new[] { typeof(StaffMapper).Assembly });

            if (System.Diagnostics.Process.GetProcessesByName("FaceRASystem").ToList().Count > 1)
            {
                //获取地区自动显示中英日文
                string ss = System.Globalization.CultureInfo.InstalledUICulture.Name;
                if (ss.Contains("ja-JP"))
                {
                    MessageBox.Show("Face recognition attendance system is on!", "HEAT CHECK");
                    return;
                }
                else
                {
                    MessageBox.Show("Face recognition attendance system is on!", "提示");
                    return;
                }
            }

            TestMethod();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            LoginNew loginNew = new LoginNew();
            loginNew.ShowDialog();

            //LoginForm f = new LoginForm();
            //f.ShowDialog();
            Cef_Initialize();

            //todo: 考勤计算
            //AttendanceAlgorithm.getpersonnel("2021-07-01 00:00:00", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);
            //return;
            
            //连接设备
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        GetDevinfo.getinfoToMyDev();
                        
                        Thread.Sleep(6 * 1000);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "连接设备线程异常");
                    }
                }
            });
            //下发人脸
            Thread thread1 = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(10 * 1000);
                        DistributeToequipment.distrbute();
                        Thread.Sleep(10 * 1000);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "下发人像异常");
                    }
                }
            });
            thread.IsBackground = true;
            thread.Start();

            thread1.IsBackground = true;
            thread1.Start();
            //时间同步线程
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(10 * 1000);
                        GetDevinfo.timeSynchronization();
                        Thread.Sleep(60 * 1000 * 2);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "时间同步异常");
                    }
                }
            })
            {
                IsBackground = true
            }.Start();
            //同步设备人员
            new Thread(() =>
            {
                try
                {
                    Thread.Sleep(10 * 1000);
                    DataSynchronization.DataSynchronizationtask();
                    Thread.Sleep(60 * 1000 * 5);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "设备人员同步异常");
                }
            })
            {
                IsBackground = true
            }.Start();
            //抓拍记录下载
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(10 * 1000);
                        TimingGet.Timingquery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "获取抓拍记录异常");
                    }
                }
            })
            {
                IsBackground = true
            }.Start();



            string url = Application.StartupPath + @"\detached\index.html"; 
            string isZn = "Zh";
            if (ApplicationData.LanguageSign.Contains("English"))
            {
                isZn = "US";
            }
            else if (ApplicationData.LanguageSign.Contains("日本語"))
            {
                isZn = "JPN";
                //获取linefor_表中的message参数和userid参数 然后保存到本地内存中
                new Thread(() =>
                {
                    try
                    {
                        string data = GetData.getline();
                        if (data.Length > 2)
                        {
                            JArray jArray = (JArray)JsonConvert.DeserializeObject(data);
                            if (jArray.Count > 0)
                            {
                                //体温阀值
                                ApplicationData.temperature = jArray[0]["temperature"].ToString().Trim();

                                //上学不检测体温
                                ApplicationData.lineMessage = jArray[0]["Message"].ToString().Trim();
                                //上学体温未达阀值
                                ApplicationData.lineMessage2 = jArray[0]["Message2"].ToString().Trim();
                                //上学体温达阀值
                                ApplicationData.lineMessage3 = jArray[0]["Message3"].ToString().Trim();

                                //迟到不检测体温
                                ApplicationData.lineMessage4 = jArray[0]["Message4"].ToString().Trim();
                                //迟到体温未达阀值
                                ApplicationData.lineMessage5 = jArray[0]["Message5"].ToString().Trim();
                                //迟到体温达阀值
                                ApplicationData.lineMessage6 = jArray[0]["Message6"].ToString().Trim();

                                //早退不检测体温
                                ApplicationData.lineMessage7 = jArray[0]["Message7"].ToString().Trim();
                                //早退体温未达阀值
                                ApplicationData.lineMessage8 = jArray[0]["Message8"].ToString().Trim();
                                //早退体温达阀值
                                ApplicationData.lineMessage9 = jArray[0]["Message9"].ToString().Trim();

                                //放学不检测体温
                                ApplicationData.lineMessage10 = jArray[0]["Message10"].ToString().Trim();
                                //放学体温未达阀值
                                ApplicationData.lineMessage11 = jArray[0]["Message11"].ToString().Trim();
                                //放学体温达阀值
                                ApplicationData.lineMessage12 = jArray[0]["Message12"].ToString().Trim();
                                //line服务地址
                                ApplicationData.line_url = jArray[0]["line_url"].ToString().Trim();


                                //FTP服务器地址
                                ApplicationData.ftpserver = jArray[0]["ftpserver"].ToString().Trim();
                                //FTP服务器账号
                                ApplicationData.ftpusername = jArray[0]["ftpusername"].ToString().Trim();
                                //FTP服务器密码
                                ApplicationData.ftppassword = jArray[0]["ftppassword"].ToString().Trim();
                            }
                        }

                        //将line管理员写到本地静态数据
                        HandleCaptureData.getstaffforlineAdminEmail();
                    }
                    catch (Exception ex) { }
                })
                {
                    IsBackground = true
                }.Start();

                new Thread(() =>
                {
                    try
                    {
                        string data = GetData.getPdfconfiguration();
                        if (data.Length > 2)
                        {
                            JArray jArray = (JArray)JsonConvert.DeserializeObject(data);
                            if (jArray.Count > 0)
                            {
                                //体温阀值
                                ApplicationData.pdftitle = jArray[0]["pdftitle"].ToString().Trim();

                                //上学不检测体温
                                ApplicationData.rows1 = jArray[0]["rows1"].ToString().Trim();
                                //上学体温未达阀值
                                ApplicationData.rows2 = jArray[0]["rows2"].ToString().Trim();
                                //上学体温达阀值
                                ApplicationData.rows3 = jArray[0]["rows3"].ToString().Trim();

                                //迟到不检测体温
                                ApplicationData.rows4 = jArray[0]["rows4"].ToString().Trim();
                                //迟到体温未达阀值
                                ApplicationData.rows5 = jArray[0]["rows5"].ToString().Trim();
                                //迟到体温达阀值
                                ApplicationData.rows6 = jArray[0]["rows6"].ToString().Trim();

                                //早退不检测体温
                                ApplicationData.rows7 = jArray[0]["rows7"].ToString().Trim();
                                //早退体温未达阀值
                                ApplicationData.rows8 = jArray[0]["rows8"].ToString().Trim();
                                //早退体温达阀值
                                ApplicationData.rows9 = jArray[0]["rows9"].ToString().Trim();

                                //放学不检测体温
                                ApplicationData.rows10 = jArray[0]["rows10"].ToString().Trim();
                                //放学体温未达阀值
                                ApplicationData.rows11 = jArray[0]["rows11"].ToString().Trim();
                                ApplicationData.rows12 = jArray[0]["rows12"].ToString().Trim();
                            }
                        }

                        //将line管理员写到本地静态数据
                        //HandleCaptureData.getstaffforlineAdminEmail();
                    }
                    catch (Exception ex) { }
                })
                {
                    IsBackground = true
                }.Start();
            }
            else if (ApplicationData.LanguageSign == Constants.LANG_NAME_FRENCH)
            {
                isZn = "FR";
            }
            else if (ApplicationData.LanguageSign == Constants.LANG_NAME_VIETNAMESE)
            {
                isZn = "vi";
            }
            ChromiumForm chromiumForm = new ChromiumForm(url);
            chromiumForm.Text = ". . .";
            Application.Run(new MainForm(chromiumForm, isZn));
            /**
             * 当前用户是管理员的时候，直接启动应用程序
             * 如果不是管理员，则使用启动对象启动程序，以确保使用管理员身份运行
             */
            //获得当前登录的Windows用户标示
            //System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            //System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
            //Application.Run(new MainForm(chromiumForm, isZn));
            ////判断当前登录用户是否为管理员
            //if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            //{
            //    //如果是管理员，则直接运行
            //    Application.Run(new MainForm(chromiumForm, isZn));
            //}
            //else
            //{
            //    //创建启动对象
            //    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //    startInfo.UseShellExecute = true;
            //    startInfo.WorkingDirectory = Environment.CurrentDirectory;
            //    startInfo.FileName = Application.ExecutablePath;
            //    //设置启动动作,确保以管理员身份运行
            //    startInfo.Verb = "runas";
            //    try
            //    {
            //        System.Diagnostics.Process.Start(startInfo);
            //    }
            //    catch
            //    {
            //        return;
            //    }
            //    //退出
            //    Application.Exit();
            //}   
        }

        static void Cef_Initialize()
        {
            CefSettings setting = new CefSettings();

            if (ApplicationData.LanguageSign.Contains("日本語"))
            {
                setting.Locale = "ja";//日文
            }
            else
            {
                //设置中文
                setting.Locale = "zh-CN";
            }

            //gpu设置, 防止高分辨率下显示问题
            setting.CefCommandLineArgs.Add("disable-gpu", "1");

            CefSharp.Cef.Initialize(setting);

        }


        static void TestMethod()
        {
            var rule = GetData.AddAccessRule("rule1", RepeatType.RepeatByDay);
            GetData.AddTimeSegmentToDay(rule.Days[0].Id, "18:00", "19:00");

            rule = GetData.AddAccessRule("rule2", RepeatType.RepeatByWeek);
            GetData.AddTimeSegmentToDay(rule.Days[0].Id, "19:00", "20:00");

            rule = GetData.AddAccessRule("rule1", RepeatType.RepeatByDay);

            var rules = GetData.GetAllAccessRules();

            var rd = GetData.AddRuleDistribution("dist1");
            GetData.AddDeviceToRuleDistribution(rd.Id, 1);
            GetData.AddStaffToRuleDistribution(rd.Id, "1631008646026");

            var rd2 = GetData.AddRuleDistribution("dist2");
            GetData.AddDeviceToRuleDistribution(rd2.Id, 1);
            

            var rd3 = GetData.AddRuleDistribution("dist3");
            GetData.AddStaffToRuleDistribution(rd3.Id, "1631008646026");

            rd3 = GetData.AddRuleDistribution("dist4");
            var rds = GetData.GetAllRuleDistribution();


        }
    }
}

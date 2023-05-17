using CefSharp.WinForms;
using Dapper.Contrib.Extensions;
using DBUtility.SQLite;
using huaanClient.Database;
using InsuranceBrowser;
using InsuranceBrowserLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Dapper;
using System.Diagnostics;
using WebSocketSharp.Server;
using System.Net;
using System.Threading.Tasks;
using WebSocketSharp;

namespace huaanClient
{
    static class Program
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static WebSocketServer _wssv;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            DapperExtensions.DapperExtensions.SetMappingAssemblies(new[] { typeof(StaffMapper).Assembly });

            if (Process.GetProcessesByName("FaceRASystem").ToList().Count > 1)
            {
                MessageBox.Show(Properties.Strings.AppIsRunningInBackground);
                if (!Debugger.IsAttached)
                {
                    InsuranceBrowserLib.NativeMethods.NativeMethods.PostMessage(
                    (IntPtr)InsuranceBrowserLib.NativeMethods.NativeMethods.HWND_BROADCAST,
                    InsuranceBrowserLib.NativeMethods.NativeMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero);
                    return;
                }
            }

            //检查sqlserver是否在运行
            if (ApplicationData.CopyData)
            {
                //var p = Process.GetProcessesByName("SQL2000");
                //if (p.Length == 0)
                //{
                //    MessageBox.Show(
                //        "已经打开了复制数据到SqlServer功能, 但是SqlServer没有运行, 请确保SqlServer已经启动, 本程序即将退出。",
                //        null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
            }

            //test
            //var count = 0;
            //var client = new Api.Client("192.168.0.38");

            //client.OnRecordReceived += (s, e) =>
            //{
            //    count+=e.count;

            //    Debug.WriteLine("接收到数据：" + String.Join(", ", e.records.Select(x=>x.sequence.ToString())));

            //    if (e.count < 5)
            //    {
            //        Debug.WriteLine("count：" + count);

            //    }
            //};

            //client.QueryCaptureRecordAsync(5, DateTime.Parse("2018-01-01 00:00:00"), DateTime.Parse("2022-06-07 10:53:43"));


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                ApplicationData.FaceRASystemToolUrl = args[0];
                ApplicationData.cmdLineSpecifiedDirectory = true;
            }

            LoginNew loginNew = new LoginNew();
            loginNew.ShowDialog();

            //init dapper
            SqlMapper.RemoveTypeMap(typeof(DateTime));
            SqlMapper.AddTypeHandler(new DateTimeHandler());
            DapperExtensions.DapperExtensions.SqlDialect = new DapperExtensions.Sql.SqliteDialect();


            //LoginForm f = new LoginForm();
            //f.ShowDialog();
            Cef_Initialize();

            var cts = new CancellationTokenSource();
            var exitProgramEvent = new ManualResetEvent(false);

            //连接设备
            Thread thread = new Thread(ct =>
            {
                var token = (CancellationToken)ct;
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        GetDevinfo.getinfoToMyDev(token);
                        exitProgramEvent.WaitOne(6 * 1000);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "连接设备线程异常");
                    }
                }
            });
            //下发人脸
            Thread thread1 = new Thread(ct =>
            {
                var token = (CancellationToken)ct;
                exitProgramEvent.WaitOne(10 * 1000);
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        DistributeToequipment.Wait();
                        DistributeToequipment.distrbute(token);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "下发人像异常");
                    }
                }
            });

            thread.IsBackground = true;
            thread.Start(cts.Token);

            thread1.Name = "人脸下发线程";
            thread1.IsBackground = true;
            thread1.Start(cts.Token);
            //时间同步线程
            new Thread(ct =>
            {
                var token = (CancellationToken)ct;
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        exitProgramEvent.WaitOne(10 * 1000);
                        GetDevinfo.timeSynchronization();
                        exitProgramEvent.WaitOne(60 * 1000 * 2);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "时间同步异常");
                    }
                }
            }).Start(cts.Token);
            //同步设备人员
            if (ChromiumForm.userSettings.AutoDataSyn)
            {
                new Thread(ct =>
                {
                    var token = (CancellationToken)ct;
                    try
                    {
                        exitProgramEvent.WaitOne(10 * 1000);
                        Console.WriteLine("自动同步设备人员开始...");
                        DataSynchronization.DataSynchronizationtask(token);
                        exitProgramEvent.WaitOne(60 * 1000 * 5);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "设备人员同步异常");
                    }

                }).Start(cts.Token);
            }

            //抓拍记录下载
            if (ChromiumForm.userSettings.AutoCaptureSyn)
            {
                new Thread(ct =>
                {
                    var token = (CancellationToken)ct;
                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            exitProgramEvent.WaitOne(10 * 1000);
                            var hasData = TimingGet.Timingquery(token);
                            //exitProgramEvent.WaitOne(hasData ? 10 * 1000 : 60 * 1000);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "获取抓拍记录异常");
                        }
                    }
                    Logger.Debug("抓拍记录下载线程退出");
                }).Start(cts.Token);
            }



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
            else if (ApplicationData.LanguageSign == Constants.LANG_NAME_RUSSIAN)
            {
                isZn = "ru";
            }


            StartWebsocketServer();

            ChromiumForm chromiumForm = new ChromiumForm(url);
            chromiumForm.Text = ". . .";
            var mainForm = new MainForm(chromiumForm, isZn);
            mainForm.FormClosed += (sender, e) =>
            {
                cts.Cancel();
                exitProgramEvent.Set();
                DistributeToequipment.Wakeup();
            };

            Application.Run(mainForm);
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

        private static void StartWebsocketServer()
        {
            _wssv = new WebSocketServer(IPAddress.Loopback, 3006, false);
            //_wssv.Log.Level = WebSocketSharp.LogLevel.Trace;
            _wssv.AddWebSocketService<BroadcastBehavior>("/messaging");
            _wssv.Start();

            if (_wssv.IsListening)
            {
                Console.WriteLine("Listening on port {0}, and providing WebSocket services:", _wssv.Port);
                foreach (var path in _wssv.WebSocketServices.Paths)
                    Console.WriteLine("- {0}", path);
            }
        }


        static void Cef_Initialize()
        {
            CefSettings setting = new CefSettings();
            setting.LocalesDirPath = Application.StartupPath + @"\locales";

            var lng = CultureInfo.CurrentCulture;
            switch (lng.Name)
            {
                case Constants.LANG_LOCALE_VIETNAMESE:
                    setting.Locale = "vi";
                    break;
                case Constants.LANG_LOCALE_CHINESE:
                    setting.Locale = "zh-CN";
                    break;
                case Constants.LANG_LOCALE_JAPANESE:
                    setting.Locale = "ja";
                    break;
                case Constants.LANG_LOCALE_ENGLISH:
                    setting.Locale = "en-US";
                    break;
                case Constants.LANG_LOCALE_FRENCH:
                    setting.Locale = "fr";
                    break;
                default:
                    setting.Locale = "zh-CN";
                    break;
            }

            setting.AcceptLanguageList = setting.Locale;

            //gpu设置, 防止高分辨率下显示问题
            setting.CefCommandLineArgs.Add("disable-gpu", "1");

            CefSharp.Cef.Initialize(setting);

        }

    }
}

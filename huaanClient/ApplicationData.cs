using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace huaanClient
{
    class ApplicationData
    {
        //是否为测试
        public static bool IsTest => Properties.Settings.Default.debug;

        public static string MyAppVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        
        //默认语言 0 中文 1 英文 2日文
        public static int defaultLanguage = 0;

        public static int DefaultLanguage { get; set; }
        //记录请求是否多次发送
        public static bool isRepeatRequest { get; set; } = false;
        //是否为第一次请求
        public static bool Isfirsthttptoline { get; set; } = true;
        //记录line接口返回的Cookiek
        public static string Cookiek { get; set; }
        public static string LanguageSign { get; set; }
        public static string lineuserid { get; set; }
        public static string temperature { get; set; }
        public static string lineMessage { get; set; }
        public static string lineMessage2 { get; set; }
        public static string lineMessage3 { get; set; }
        public static string lineMessage4 { get; set; }
        public static string lineMessage5 { get; set; }
        public static string lineMessage6 { get; set; }
        public static string lineMessage7 { get; set; }
        public static string lineMessage8 { get; set; }
        public static string lineMessage9 { get; set; }
        public static string lineMessage10 { get; set; }
        public static string lineMessage11 { get; set; }
        public static string lineMessage12 { get; set; }

        public static string ftpserver { get; set; }
        public static string ftppassword { get; set; }
        public static string ftpusername { get; set; }

        public static string line_url { get; set; }


        public static string pdftitle { get; set; }
        public static string rows1 { get; set; }
        public static string rows2 { get; set; }
        public static string rows3 { get; set; }
        public static string rows4 { get; set; }
        public static string rows5 { get; set; }
        public static string rows6 { get; set; }
        public static string rows7 { get; set; }
        public static string rows8 { get; set; }
        public static string rows9 { get; set; }
        public static string rows10 { get; set; }
        public static string rows11 { get; set; }
        public static string rows12 { get; set; }
        public static List<string> lineadmin { get; set; }

        public static bool isrealtime { get; set; }

        private static string faceRASystemToolUrl;

        private static object _locker = new object();
        private static bool? _copyData;
        public static string sqlServerConnectionString;

        public static bool CopyData
        {
            get
            {
                lock (_locker)
                {
                    if (_copyData == null)
                    {
                        var json = Tools.GetBrandObjectInJson();
                        var obj = JObject.Parse(json)["copyData"];
                        _copyData = obj["enabled"].Value<bool>();
                        sqlServerConnectionString = obj["connectionString"].Value<string>();
                    }
                    return _copyData.Value;
                }
            }
        }


        public static string FaceRASystemToolUrl
        {
            get
            {
                return faceRASystemToolUrl;
            }
            set
            {
                connectionString = "Data Source=" + value + "\\huaanDatabase.sqlite;Version=3;Pooling=True;Max Pool Size=100;";
                faceRASystemToolUrl = value;
            }
        }

        //public static string connectionString = "Data Source=" + Application.StartupPath + @"\huaanDatabase.sqlite;Version=3;";
        public static string connectionString = "Data Source=D:\\FaceRASystemTool\\huaanDatabase.sqlite;Version=3;Pooling=True;Max Pool Size=100;";
    }
}

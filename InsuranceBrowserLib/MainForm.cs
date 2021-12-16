
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Windows.Forms;
using ZXCL.WinFormUI;

namespace InsuranceBrowserLib
{
    public partial class MainForm : ZFormMdi
    {
        string args;
        ZFormChild insAppForm;
        ZFormChild chromeForm;
        HttpClient httpClient;
        string iszn;
        //用于chrome浏览器控件
        public MainForm(ZFormChild chromeForm,string isZn)
        {
            iszn = isZn;
            InitializeComponent();
            this.chromeForm = chromeForm;

            if (File.Exists("./branding/logo.ico"))
            {
                this.Icon = new Icon("./branding/logo.ico");
            }

        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_MINIMIZEBOX = 0x00020000; 
                CreateParams cp = base.CreateParams;
                cp.Style = cp.Style | WS_MINIMIZEBOX; 
                return cp;
            }
        }

        //用于c++启动程序;
        public MainForm(string args)
        {
            InitializeComponent();
            this.args = args;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if(iszn.Contains("Zh"))
            {
                notifyIcon1.Text = "智慧人脸考勤门禁系统";
            }
            else if (iszn.Contains("JPN"))
            {
                notifyIcon1.Text = "HEAT CHECK";
            }
            else if (iszn.Contains("US"))
            {
                notifyIcon1.Text = "Smart face access control system";
            }
            else if (iszn.Contains("FR"))
            {
                notifyIcon1.Text = "Système intelligent de contrôle d'accès facial";
            }
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Top = 0;
            this.Left = 0;
            this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            //这里先等待控件加载完成
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 500;
            timer.Tick += (a, b) =>
            {
                timer.Stop();
                onFormLoad();
                timer.Dispose();
                timer = null;
            };
            timer.Start();
            //语言初始化
            language();
        }
        private void onFormLoad()
        {
            this.AddTableForm(chromeForm);
            //
            //this.Location = new Point(100, 100);
            if (Debugger.IsAttached)
            {
                this.Resizable = true;
            }

        }
        private void language()
        {
            if (iszn.Contains("US"))
            {
                this.退出ToolStripMenuItem1.Text = "Exit";
            }
            else if (iszn.Contains("JPN"))
            {
                this.退出ToolStripMenuItem1.Text = "終了";
            }
        }
        //调整工作栏长度
        private void panelTopSizeChange()
        {
            
        }

        /// <summary>
        /// 添加IE控件页面
        /// </summary>
        /// <param name="url"></param>
        public void SetButtons(bool canBack, bool canForward)
        {
            

        }
        public void SetToolBarVisible(bool visible)
        {
            
        }
        private void MainForm_Resize(object sender, EventArgs e)
        {
            panelTopSizeChange();
        }


       

        private void btnClearHistory_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确定清除缓存?", "提示", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                
            }
        }
        public bool CanClose = true;
        //bool isExit;
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Close close = new Close(iszn);
            DialogResult dr= close.ShowDialog();
            if (dr==DialogResult.OK)
            {
                e.Cancel = true;
                if (iszn.Contains("US"))
                {
                    this.Hide();
                    this.WindowState = FormWindowState.Minimized;
                    this.notifyIcon1.Visible = true;
                    this.notifyIcon1.ShowBalloonTip(3000, "Running in the background", "The icon has been reduced to the tray. To open the window, double-click the icon or right-click [display]。", ToolTipIcon.Info);
                }
                else if (iszn.Contains("JPN"))
                {
                    this.Hide();
                    this.WindowState = FormWindowState.Minimized;
                    this.notifyIcon1.Visible = true;
                    this.notifyIcon1.ShowBalloonTip(3000, "バックグラウンド実行中", "アイコンをパレットに縮小しました。ウィンドウを開けたらアイコンまたは右ボタンをダブルクリックしてください。。", ToolTipIcon.Info);
                }
                else if (iszn.Contains("FR"))
                {
                    this.Hide();
                    this.WindowState = FormWindowState.Minimized;
                    this.notifyIcon1.Visible = true;
                    this.notifyIcon1.ShowBalloonTip(3000, "Background Running", "L'icône a été réduite au plateau. Pour ouvrir la fenêtre, double - cliquez sur l'icône ou faites un clic droit sur [afficher].", ToolTipIcon.Info);
                }
                else
                {
                    this.Hide();
                    this.WindowState = FormWindowState.Minimized;
                    this.notifyIcon1.Visible = true;
                    this.notifyIcon1.ShowBalloonTip(3000, "后台运行中", "图标已经缩小到托盘，打开窗口请双击图标或者右键【显示】即可。", ToolTipIcon.Info);
                }
                
            }
            else if(dr == DialogResult.No)
            {
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                e.Cancel = true;
            }

            //if (iszn.Contains("US"))
            //{
            //    dr = MessageBox.Show(this, "Confirm to exit the clock in system?", "提示",
            //                             MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            //}
            //else if (iszn.Contains("JPN"))
            //{
            //    dr = MessageBox.Show(this, "勤務評定カードシステムを終了しますか？", "提示",
            //                             MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            //}
            //else
            //{
            //    dr = MessageBox.Show(this, "确定退出 考勤打卡系统?", "提示",
            //                                 MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            //}


            //this.TopMost = false;
            //if (dr == DialogResult.OK)
            //{
            //    Process.GetCurrentProcess().Kill();
            //}
            //else
            //{
            //    e.Cancel = true;
            //    if (iszn.Contains("US"))
            //    {
            //        this.Hide();
            //        this.WindowState = FormWindowState.Minimized;
            //        this.notifyIcon1.Visible = true;
            //        this.notifyIcon1.ShowBalloonTip(3000, "Running in the background", "The icon has been reduced to the tray. To open the window, double-click the icon or right-click [display]。", ToolTipIcon.Info);
            //    }
            //    else if (iszn.Contains("JPN"))
            //    {
            //        this.Hide();
            //        this.WindowState = FormWindowState.Minimized;
            //        this.notifyIcon1.Visible = true;
            //        this.notifyIcon1.ShowBalloonTip(3000, "バックグラウンド実行中", "アイコンをパレットに縮小しました。ウィンドウを開けたらアイコンまたは右ボタンをダブルクリックしてください。。", ToolTipIcon.Info);
            //    }
            //    else
            //    {
            //        this.Hide();
            //        this.WindowState = FormWindowState.Minimized;
            //        this.notifyIcon1.Visible = true;
            //        this.notifyIcon1.ShowBalloonTip(3000, "后台运行中", "图标已经缩小到托盘，打开窗口请双击图标或者右键【显示】即可。", ToolTipIcon.Info);
            //    }
            //}
            //if (e.CloseReason == CloseReason.UserClosing)//当用户点击窗体右上角X按钮或(Alt + F4)时 发生          
            //{
            //    e.Cancel = true;
            //    if (iszn.Contains("US"))
            //    {
            //        this.Hide();
            //        this.WindowState = FormWindowState.Minimized;
            //        this.notifyIcon1.Visible = true;
            //        this.notifyIcon1.ShowBalloonTip(3000, "Running in the background", "The icon has been reduced to the tray. To open the window, double-click the icon or right-click [display]。", ToolTipIcon.Info);
            //    }
            //    else if (iszn.Contains("JPN"))
            //    {
            //        this.Hide();
            //        this.WindowState = FormWindowState.Minimized;
            //        this.notifyIcon1.Visible = true;
            //        this.notifyIcon1.ShowBalloonTip(3000, "バックグラウンド実行中", "アイコンをパレットに縮小しました。ウィンドウを開けたらアイコンまたは右ボタンをダブルクリックしてください。。", ToolTipIcon.Info);
            //    }
            //    else
            //    {
            //        this.Hide();
            //        this.WindowState = FormWindowState.Minimized;
            //        this.notifyIcon1.Visible = true;
            //        this.notifyIcon1.ShowBalloonTip(3000, "后台运行中", "图标已经缩小到托盘，打开窗口请双击图标或者右键【显示】即可。", ToolTipIcon.Info);
            //    }
            //}

            //DialogResult dr ;
            //if (iszn)
            //{
            //    dr = MessageBox.Show(this, "确定退出 考勤打卡系统?", "提示",
            //                             MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            //}
            //else
            //{
            //    dr = MessageBox.Show(this, "Confirm to exit the clock in system?", "提示",
            //                             MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            //}


            //this.TopMost = false;
            //if (dr == DialogResult.OK)
            //{
            //    Process.GetCurrentProcess().Kill();
            //}
            //else
            //{
            //    e.Cancel = true;
            //}
        }

        private void MainForm_OnCloseBoxClick()
        {
            CanClose = true;
        }



        private bool existProcess(string processName)
        {
            System.Diagnostics.Process[] myProcesses;
            myProcesses = System.Diagnostics.Process.GetProcessesByName(processName);
            if (myProcesses.Count() > 0)
                return true;
            return false;
        }



        private void btnProxy_Click(object sender, EventArgs e)
        {
            
        }

        private void MainForm_OnDropDownClick(object obj)
        {
            Control c = (Control)obj;
            contextMenuStrip.Show(c, -30, c.Bottom + 1);
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            
        }

        public void SetDropDownVisible(bool visible)
        {
            this.DropDwon = visible;
        }

        private void menubtnClearHistory_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确定清除缓存?", "提示", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                
            }
        }

        private void munubtnRestoreConfig_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("这将会清除所有默认设置(包括所有保险公司的默认渠道,网址,密码)\r\n\r\n如果只是想清除部分默认设置,请到欢迎界面选择需要修改的保险公司图标,点击鼠标右键进行相关操作\r\n\r\n确定清除?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
               
            }
        }

        private void menubtnInternetOption_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = new System.Diagnostics.ProcessStartInfo("rundll32.exe", "shell32.dll,Control_RunDLL   inetcpl.cpl");
            p.Start();
        }

        private void menubtnProxy_Click(object sender, EventArgs e)
        {
            
        }

        private void menubtnCheckUpdate_Click(object sender, EventArgs e)
        {

            

        }

        

        private void menubtnAbout_Click(object sender, EventArgs e)
        {
            
        }

        private void menubtnRepair_Click(object sender, EventArgs e)
        {
            
        }


        private bool Duibi(string str)
        {
            bool ruest = true;
            string[] s = GetCompatibilitySites();
            if (str != null && str != "")
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (str.Contains(s[i].ToString()))
                    {
                        ruest = false;
                    }
                }
            }
            return ruest;
        }

        /// <summary>
        /// 默认设置
        /// </summary>

        #region ---设置主页---
        private const string ID_IEKEY = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";
        private const string ID_IEHome = @"Software\Microsoft\Internet Explorer\Main";

        /// <summary>
        /// 设置主页
        /// </summary>
        /// <param name="url"></param>
        public static void SetHomePage(string url)
        {
            var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(ID_IEHome, true);

            if (key != null)
            {
                key.SetValue("Start Page", url);
            }
        }

        /// <summary>
        /// 读取主页
        /// </summary>
        /// <returns></returns>
        //public static string GetHomePage()
        //{
        //    var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(ID_IEHome, true);
        //    if (key != null)
        //    {
        //        return Conversion.ToString(key.GetValue("Start Page"), "");
        //    }
        //    return "";
        //}

        #endregion

        #region ---添加可信站点---
        private const string ID_ZoneMap = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap";
        private const string ID_ZoneMap_Domains = ID_ZoneMap + @"\Domains";
        private const string ID_ZoneMap_Ranges = ID_ZoneMap + @"\Ranges";
        /// <summary>
        /// 添加可信站点
        /// </summary>
        /// <param name="url"></param>
        public static void AddTrustSite(string url)
        {
            UriDomain uriDomain = new UriDomain(url);
            AddTrustSite(uriDomain);
        }

        public static void AddTrustSite(UriDomain uriDomain)
        {
            switch (uriDomain.HostType)
            {
                case UriHostType.Dns:
                    AddTrustSiteByDns(uriDomain);
                    break;
                case UriHostType.IPv4:
                    AddTrustSiteByIPv4(uriDomain);
                    break;
                default:
                    throw new Exception("AddTrustSite:无效的地址类型！");
            }
        }

        /// <summary>
        /// 通过域名添加可信站点
        /// </summary>
        /// <param name="uriDomain"></param>
        private static void AddTrustSiteByDns(UriDomain uriDomain)
        {
            /////////////如果不是IP则在Domains节点下操作///////////////
            RegistryKey domains = Registry.CurrentUser.OpenSubKey(ID_ZoneMap_Domains, true);

            RegistryKey node = domains.OpenSubKey(uriDomain.SubDomain, true);

            //URL没有添加过
            if (node == null)
            {
                node = domains.CreateSubKey(uriDomain.SubDomain);
                RegistryKey pnode = node.CreateSubKey(uriDomain.TopDomain);
                pnode.SetValue(uriDomain.Protocol, 2, RegistryValueKind.DWord);//添加协议对应的值
            }
            else
            {
                RegistryKey pnode = node.OpenSubKey(uriDomain.TopDomain, true);
                //如果协议节点不存在
                if (pnode == null)
                {
                    pnode = node.CreateSubKey(uriDomain.TopDomain);
                    pnode.SetValue(uriDomain.Protocol, 2, RegistryValueKind.DWord);//添加协议对应的值
                }
            }
        }

        /// <summary>
        /// 添加可信站点IPV4
        /// </summary>
        /// <param name="uriDomain"></param>
        private static void AddTrustSiteByIPv4(UriDomain uriDomain)
        {
            ///判断是普通URL还是IP，普通URL在Domains节点下操作，IP在Ranges节点下操作////////////////
            RegistryKey ranges = Registry.CurrentUser.OpenSubKey(ID_ZoneMap_Ranges, true);

            bool has = false;//标识IP是否已经添加
            List<string> numbers = new List<string>();//用来存放Range编号
                                                      //循环所有的range节点，如果要添加的IP已经存在则不操作，否则添加
            foreach (string u in ranges.GetSubKeyNames())
            {
                numbers.Add(u.Substring("Range".Length));//截取Range1后面的数字1
                RegistryKey range = ranges.OpenSubKey(u, true);//以可写的权限打开子节点

                //如果该IP地址已经存在
                if (range.GetValue(":Range").Equals(uriDomain.Address))
                {
                    has = true;
                    if (range.GetValue(uriDomain.Protocol) != null) break;//如果协议也正确说明IP已经是信任节点
                    range.SetValue(uriDomain.Protocol, 2, RegistryValueKind.DWord);//添加协议对应的值
                }
            }
            //如果该IP没有在信任列表则重新添加
            if (!has)
            {
                RegistryKey range = ranges.CreateSubKey("Range" + FindMinNumber(numbers));
                range.SetValue(":Range", uriDomain.Address, RegistryValueKind.String);
                range.SetValue(uriDomain.Protocol, 2, RegistryValueKind.DWord);
            }
        }

        #endregion

        #region ---移除可信站点---
        public static void RemoveTrustSite(string url)
        {
            UriDomain uriDomain = new UriDomain(url);
            RemoveTrustSite(uriDomain);
        }
        /// <summary>
        /// 在注册表删除信任站点
        /// </summary>
        /// <param name="url"></param>
        public static void RemoveTrustSite(UriDomain uriDomain)
        {
            switch (uriDomain.HostType)
            {
                case UriHostType.Dns:
                    RemoveTrustSiteByDns(uriDomain);
                    break;
                case UriHostType.IPv4:
                    RemoveTrustSiteByIPv4(uriDomain);
                    break;
                default:
                    throw new Exception("DeleteTrustSite:无效的地址类型！");
            }
        }

        /// <summary>
        /// 通过可信站点移除
        /// </summary>
        /// <param name="uriDomain"></param>
        private static void RemoveTrustSiteByIPv4(UriDomain uriDomain)
        {
            RegistryKey ranges = Registry.CurrentUser.OpenSubKey(ID_ZoneMap_Ranges, true);

            //循环所有的range节点，如果要添加的IP已经存在则不操作，否则添加
            foreach (string u in ranges.GetSubKeyNames())
            {
                RegistryKey range = ranges.OpenSubKey(u, true);//以可写的权限打开子节点

                //如果该IP地址已经存在
                if (range.GetValue(":Range").Equals(uriDomain.Address))
                {
                    if (range.GetValue(uriDomain.Protocol) != null) range.DeleteValue(uriDomain.Protocol);//如果协议节点存在则先删除
                    if (range.GetValueNames().Length == 1) ranges.DeleteSubKey(u);//只剩下“:Range”则将该节点删除
                }
            }
        }

        /// <summary>
        /// 通过DNS移除
        /// </summary>
        /// <param name="uriDomain"></param>
        private static void RemoveTrustSiteByDns(UriDomain uriDomain)
        {
            RegistryKey domains = Registry.CurrentUser.OpenSubKey(ID_ZoneMap_Domains, true);

            RegistryKey node = domains.OpenSubKey(uriDomain.SubDomain);
            RegistryKey pnode = node.OpenSubKey(uriDomain.TopDomain);
            if (pnode != null)
                node.DeleteSubKey(uriDomain.TopDomain);//删除协议节点
            if (node.GetSubKeyNames().Length == 0)
                domains.DeleteSubKey(uriDomain.SubDomain);//如果没有协议节点则删除该URL节点
        }

        /// <summary>
        /// 获取数字列表里从小到大排列最先却少的那个数字，比如numbers里包含2，3时返回1
        /// </summary>
        /// <param name="numbers">已经使用的数字列表</param>
        /// <returns></returns>
        private static string FindMinNumber(List<string> numbers)
        {
            for (int i = 1; i <= numbers.Count; i++)
            {
                if (numbers.IndexOf(i.ToString()) == -1) return i.ToString();
            }
            return (numbers.Count + 1).ToString();
        }

        #endregion

        #region ---设置兼容视图---
        //兼容性列表在注册表中的位置，注意：不同位的操作系统可能不同，请注意测试。
        private const string ID_CLEARABLE = @"Software\Microsoft\Internet Explorer\BrowserEmulation\ClearableListData";
        private const string ID_USERFILTER = "UserFilter";

        /// <summary>
        /// 得到已经存在的所有兼容网站列表，如果没有，则返回空数组。
        /// </summary>
        /// <returns></returns>
        public static string[] GetCompatibilitySites()
        {
            string[] domains = { };
            try
            {
                using (RegistryKey regkey = Registry.CurrentUser.OpenSubKey(ID_CLEARABLE))
                {
                    //可能不存在此key.
                    Object filterData = regkey.GetValue(ID_USERFILTER);
                    if (filterData != null)
                    {
                        byte[] filter = filterData as byte[];
                        domains = GetCompatibilitySites(filter);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return domains;
        }

        /// <summary>
        /// 从byte数组中分析所有网站名称
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static string[] GetCompatibilitySites(byte[] filter)
        {
            List<string> domains = new List<string>();
            int length;
            int offset_filter = 24;
            int totalSize = filter.Length;
            while (offset_filter < totalSize)
            {
                length = BitConverter.ToUInt16(filter, offset_filter + 16);
                domains.Add(System.Text.Encoding.Unicode.GetString(filter, 16 + 2 + offset_filter, length * 2));
                offset_filter += 16 + 2 + length * 2;
            }
            return domains.ToArray();
        }

        /// <summary>
        /// 从兼容性列表中删除一个网站。
        /// </summary>
        /// <param name="domain">要删除网站</param>
        private static void RemoveUserFilter(UriDomain uriDomain)
        {
            String[] domains = GetCompatibilitySites();
            if (!domains.Contains(uriDomain.TopDomain))
            {
                return;
            }
            using (RegistryKey regkey = Registry.CurrentUser.OpenSubKey(ID_CLEARABLE, true))
            {
                object oldData = regkey.GetValue(ID_USERFILTER);
                if (oldData != null)
                {
                    byte[] filter = oldData as byte[];
                    byte[] newReg = GetRemovedValue(uriDomain.TopDomain, filter);

                    if (GetCompatibilitySites(newReg).Length == 0)
                        regkey.DeleteValue(ID_USERFILTER);
                    else
                        regkey.SetValue(ID_USERFILTER, newReg, RegistryValueKind.Binary);
                }
            }
        }

        public static void RemoveUserFilter(string url)
        {
            UriDomain uriDomain = new UriDomain(url);
            RemoveUserFilter(uriDomain);
        }

        /// <summary>
        /// 得到一个网站的存储的数据
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static byte[] GetRemovedValue(string domain, byte[] filter)
        {
            byte[] newReg;
            int length;
            int offset_filter = 24;
            int offset_newReg = 0;
            int totalSize = filter.Length;

            newReg = new byte[totalSize];
            Array.Copy(filter, 0, newReg, 0, offset_filter);
            offset_newReg += offset_filter;

            while (offset_filter < totalSize)
            {
                length = BitConverter.ToUInt16(filter, offset_filter + 16);
                if (domain != System.Text.Encoding.Unicode.GetString(filter, offset_filter + 16 + 2, length * 2))
                {
                    Array.Copy(filter, offset_filter, newReg, offset_newReg, 16 + 2 + length * 2);
                    offset_newReg += 16 + 2 + length * 2;
                }
                offset_filter += 16 + 2 + length * 2;
            }
            Array.Resize(ref newReg, offset_newReg);
            byte[] newSize = BitConverter.GetBytes((UInt16)(offset_newReg - 12));
            newReg[12] = newSize[0];
            newReg[13] = newSize[1];

            return newReg;
        }

        class BinClass
        {
            public byte[] header = new byte[] { 0x41, 0x1F, 0x00, 0x00, 0x53, 0x08, 0xAD, 0xBA };
            public byte[] delim_a = new byte[] { 0x01, 0x00, 0x00, 0x00 };
            public byte[] delim_b = new byte[] { 0x0C, 0x00, 0x00, 0x00 };
            public byte[] checksum = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
            public byte[] filler = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 };
        }

        private static void AddCompatibilityView(UriDomain uriDomain)
        {
            byte[] regbinary = new byte[] { };
            BinClass regbinclass = new BinClass();

            String[] domains = GetCompatibilitySites();
            if (domains.Length > 0)
            {
                if (domains.Contains(uriDomain.TopDomain))
                    return;
                else
                    domains = domains.Concat(new String[] { uriDomain.TopDomain }).ToArray();
            }
            else
            {
                domains = domains.Concat(new String[] { uriDomain.TopDomain }).ToArray();
            }

            int count = domains.Length;
            byte[] entries = new byte[0];
            foreach (String d in domains)
            {
                entries = Combine(entries, GetDomainEntry(d, regbinclass));
            }
            regbinary = regbinclass.header;
            regbinary = Combine(regbinary, BitConverter.GetBytes(count));
            regbinary = Combine(regbinary, regbinclass.checksum);
            regbinary = Combine(regbinary, regbinclass.delim_a);
            regbinary = Combine(regbinary, BitConverter.GetBytes(count));
            regbinary = Combine(regbinary, entries);
            Registry.CurrentUser.OpenSubKey(ID_CLEARABLE, true).SetValue(ID_USERFILTER, regbinary, RegistryValueKind.Binary);

            regbinclass = null;
            regbinary = null;
        }

        /// <summary>
        /// 向兼容性列表中添加一个网站
        /// </summary>
        /// <param name="domain"></param>
        public static void AddCompatibilityView(String url)
        {
            UriDomain uriDomain = new UriDomain(url);
            AddCompatibilityView(uriDomain);
        }

        /// <summary>
        /// 得到一个网站在兼容性列表中的数据，跟GetRemovedValue类似
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        private static byte[] GetDomainEntry(String domain, BinClass regbinclass)
        {
            byte[] tmpbinary = new byte[0];
            byte[] length = BitConverter.GetBytes((UInt16)domain.Length);
            byte[] data = System.Text.Encoding.Unicode.GetBytes(domain);
            tmpbinary = Combine(tmpbinary, regbinclass.delim_b);
            tmpbinary = Combine(tmpbinary, regbinclass.filler);
            tmpbinary = Combine(tmpbinary, regbinclass.delim_a);
            tmpbinary = Combine(tmpbinary, length);
            tmpbinary = Combine(tmpbinary, data);
            return tmpbinary;
        }

        //把两个byte[]数组合并在一起
        private static byte[] Combine(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];
            System.Buffer.BlockCopy(a, 0, c, 0, a.Length);
            System.Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
            return c;
        }

        #endregion

        #region ---调整安全策略---
        private const string ID_Zones = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones";

        /// <summary>
        /// 设置当前策略的值
        /// </summary>
        /// <param name="policys"></param>
        /// <param name="code"></param>
        /// <param name="value"></param>
        public static void SetZonesPolicy(ZonesPolicys policys, string code, uint value)
        {
            var keyname = ID_Zones + "\\" + policys;
            RegistryKey regKeys = Registry.CurrentUser.OpenSubKey(keyname, true);
            if (regKeys != null)
            {
                regKeys.SetValue(code, value);
            }
        }

        /// <summary>
        /// 设置ActiveX的策略，自动打开ActiveX策略
        /// </summary>
        /// <param name="value"></param>
        public static void SetActiveXPolicy(ZonesPolicys policys = ZonesPolicys.Trustedsiteszone, uint value = 0)
        {
            SetZonesPolicy(policys, ZonesPolicyCode.ID_1001, value);
            SetZonesPolicy(policys, ZonesPolicyCode.ID_1004, value);
            SetZonesPolicy(policys, ZonesPolicyCode.ID_1200, value);
            SetZonesPolicy(policys, ZonesPolicyCode.ID_1201, value);
            SetZonesPolicy(policys, ZonesPolicyCode.ID_1405, value);
            SetZonesPolicy(policys, ZonesPolicyCode.ID_2201, value);
        }
        #endregion

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Minimized;

                HideMainForm();
            }
            else if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = true;
                ShowMainForm();
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr;
            if (iszn.Contains("US"))
            {
                dr = MessageBox.Show(this, "Confirm to exit the clock in system?", "提示",
                                         MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            }
            else if (iszn.Contains("JPN"))
            {
                dr = MessageBox.Show(this, "勤務評定カードシステムを終了しますか？", "提示",
                                         MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            }
            else if (iszn.Contains("FR"))
            {
                dr = MessageBox.Show(this, "勤務評定カードシステムを終了しますか？", "提示",
                                         MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            }
            else
            {
                dr = MessageBox.Show(this, "Confirmer la sortie du système de pointage de présence?", "Conseils",
                                             MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            }


            this.TopMost = false;
            if (dr == DialogResult.OK)
            {
                Process.GetCurrentProcess().Kill();
            }
        }
        private void HideMainForm()
        {
            this.Hide();
        }

        private void ShowMainForm()
        {
            this.notifyIcon1.Visible = false; //不显示托盘图标
            this.ShowInTaskbar = true;//图标显示在任务栏
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void 退出ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult dr;
            if (iszn.Contains("US"))
            {
                dr = MessageBox.Show(this, "Confirm to exit the clock in system?", "提示",
                                         MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            }
            else if (iszn.Contains("JPN"))
            {
                dr = MessageBox.Show(this, "「HEATCHECKを終了しますか」？", "提示",
                                         MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            }
            else if (iszn.Contains("FR"))
            {
                dr = MessageBox.Show(this, "Confirmer la sortie du système de pointage de présence？", "提示",
                                         MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            }
            else
            {
                dr = MessageBox.Show(this, "确定退出 考勤打卡系统?", "提示",
                                             MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            }


            this.TopMost = false;
            if (dr == DialogResult.OK)
            {
                notifyIcon1.Dispose();
                Process.GetCurrentProcess().Kill();
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.notifyIcon1.Visible = false; //不显示托盘图标
                this.ShowInTaskbar = true;//图标显示在任务栏
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            //当窗体为最小化状态时
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.notifyIcon1.Visible = false; //不显示托盘图标
                this.ShowInTaskbar = true;//图标显示在任务栏
            }
        }
    }

    #region ---地址域名类---

    public class UriDomain
    {
        protected string uriString;

        public Uri FullUri { get; protected set; }
        public UriHostType HostType { get; protected set; }
        public bool IsHttps { get; protected set; }
        public string Protocol { get; protected set; }
        public string Address { get; protected set; }
        public string TopDomain { get; protected set; }
        public string SubDomain { get; protected set; }

        public override string ToString()
        {
            return this.uriString;
        }

        public UriDomain(String uriString)
        {
            this.uriString = uriString;
            this.FullUri = new Uri(uriString);
            this.IsHttps = String.Equals(FullUri.Scheme, "https", StringComparison.OrdinalIgnoreCase);
            this.Protocol = FullUri.Scheme;

            this.Address = FullUri.Host;
            this.HostType = GetHostType();
            this.TopDomain = GetTopDomain(Address);
            this.SubDomain = GetSubDomain(TopDomain);
        }

        /// <summary>
        /// 添加二级域名
        /// </summary>
        /// <param name="sDomain"></param>
        /// <returns></returns>
        protected string GetSubDomain(string sDomain)
        {
            var s = sDomain.IndexOf(this.TopDomain);
            if (s > 0)
                return sDomain.Substring(0, s);
            else
                return "";
        }

        /// <summary>
        /// 获得顶级域名
        /// </summary>
        /// <param name="sDomain"></param>
        /// <returns></returns>
        protected string GetTopDomain(string sDomain)
        {
            string[] topList = new string[] { ".com.cn", ".net.cn", ".org.cn", ".gov.cn",
                    ".ac.cn", ".bj.cn", ".sh.cn", ".tj.cn", ".cq.cn", ".he.cn", ".sx.cn",
                    ".nm.cn", ".ln.cn", ".jl.cn", ".hl.cn", ".js.cn", ".zj.cn", ".ah.cn",
                    ".fj.cn", ".jx.cn", ".sd.cn", ".ha.cn", ".hb.cn", ".hn.cn", ".gd.cn",
                    ".gx.cn", ".hi.cn", ".sc.cn", ".gz.cn", ".yn.cn", ".xz.cn", ".sn.cn",
                    ".gs.cn", ".qh.cn",  ".nx.cn", ".xj.cn", ".tw.cn", ".hk.cn", ".mo.cn",
                    ".com", ".net", ".org",".biz", ".info", ".cc", ".tv", ".cn" };

            for (int i = 0; i < topList.Length; i++)
            {
                var toppart = sDomain.Substring(sDomain.Length - topList[i].Length, topList[i].Length).ToLower();
                if (toppart == topList[i])
                {

                    sDomain = sDomain.Substring(0, sDomain.Length - topList[i].Length);//去除域名后缀   
                    if (sDomain.LastIndexOf(".") > 0)
                    {
                        //二级域名，提取顶级域，顺便组合   
                        sDomain = sDomain.Substring(sDomain.LastIndexOf("."), sDomain.Length - sDomain.LastIndexOf(".")) + topList[i];
                        if (sDomain.IndexOf(".") == 0) //第一个为小数点，去掉   
                        {
                            sDomain = sDomain.Substring(1, sDomain.Length - 1);
                        }
                    }
                    else
                    {
                        sDomain += topList[i];//已是顶级域，组合返回   
                    }
                    break;
                }
            }

            return sDomain;

        }

        /// <summary>
        /// 获得域名类型
        /// </summary>
        protected UriHostType GetHostType()
        {
            if (this.FullUri.HostNameType == UriHostNameType.IPv4)
                return UriHostType.IPv4;
            if (this.FullUri.HostNameType == UriHostNameType.Dns)
                return UriHostType.Dns;

            return UriHostType.Invalid;
        }
    }

    public enum UriHostType : int
    {
        Invalid = 0,
        Dns = 1,
        IPv4 = 2,
    }
    #endregion

    #region ---参数类定义---
    public enum ZonesPolicys : uint
    {
        MyComputer = 0,
        LocalIntranetzone = 1,
        Trustedsiteszone = 2,
        Internetzone = 3,
        RestrictedSites = 4
    }

    public class ZonesPolicyCode
    {
        public const string ID_1001 = "1001";//下载已签名的 ActiveX 控件
        public const string ID_1004 = "1004";//下载未签名的 ActiveX 控件
        public const string ID_1200 = "1200";//运行 ActiveX 控件和插件
        public const string ID_1201 = "1201";//对没有标记为安全的 ActiveX 控件进行初始化和脚本运行
        public const string ID_1206 = "1206";//允许 Internet Explorer Webbrowser 控件的脚本
        public const string ID_1400 = "1400";//活动脚本
        public const string ID_1402 = "1402";//Java 小程序脚本
        public const string ID_1405 = "1405";//对标记为可安全执行脚本的 ActiveX 控件执行脚本
        public const string ID_1406 = "1406";//通过域访问数据资源
        public const string ID_1407 = "1407";//允许通过脚本进行粘贴操作
        public const string ID_1601 = "1601";//提交非加密表单数据
        public const string ID_1604 = "1604";//字体下载
        public const string ID_1605 = "1605";//运行 Java
        public const string ID_1606 = "1606";//持续使用用户数据
        public const string ID_1607 = "1607";//跨域浏览子框架
        public const string ID_1608 = "1608";//允许 META REFRESH *
        public const string ID_1609 = "1609";//显示混合内容 *
        public const string ID_1800 = "1800";//桌面项目的安装
        public const string ID_1802 = "1802";//拖放或复制和粘贴文件
        public const string ID_1803 = "1803";//文件下载
        public const string ID_1804 = "1804";//在 IFRAME 中加载程序和文件
        public const string ID_1805 = "1805";//在 Web 视图中加载程序和文件
        public const string ID_1806 = "1806";//加载应用程序和不安全文件
        public const string ID_1807 = "1807";//";//";//  保留 **
        public const string ID_1808 = "1808";//";//";//  保留 **
        public const string ID_1809 = "1809";//使用弹出窗口阻止程序 **
        public const string ID_1A00 = "1A00";//登录
        public const string ID_1A02 = "1A02";//允许持续使用存储在计算机上的 Cookie
        public const string ID_1A03 = "1A03";//允许使用每个会话的 Cookie（未存储）
        public const string ID_1A04 = "1A04";//没有证书或只有一个证书时不提示选择客户证书 *
        public const string ID_1A05 = "1A05";//允许持续使用第三方 Cookie *
        public const string ID_1A06 = "1A06";//允许使用第三方会话 Cookie *
        public const string ID_1A10 = "1A10";//隐私设置 *
        public const string ID_1C00 = "1C00";//Java 权限
        public const string ID_1E05 = "1E05";//软件频道权限
        public const string ID_1F00 = "1F00";//保留 **
        public const string ID_2000 = "2000";//二进制和脚本行为
        public const string ID_2001 = "2001";//运行已用 Authenticode 签名的 .NET 组件
        public const string ID_2004 = "2004";//运行未用 Authenticode 签名的 .NET 组件
        public const string ID_2100 = "2100";//基于内容打开文件，而不是基于文件扩展名 **
        public const string ID_2101 = "2101";//在低特权 Web 内容区域中的网站可以导航到此区域 **
        public const string ID_2102 = "2102";//允许由脚本初始化的窗口，没有大小和位置限制 **
        public const string ID_2200 = "2200";//文件下载自动提示 **
        public const string ID_2201 = "2201";//ActiveX 控件自动提示 **
        public const string ID_2300 = "2300";//允许网页为活动内容使用受限制的协议 **

        #endregion
    }
}

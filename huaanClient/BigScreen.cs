using CefSharp;
using CefSharp.WinForms;
using huaanClient;
using InsuranceBrowser.CefHanderCommon;
using InsuranceBrowser.CefHanderForChromiumFrom;
using InsuranceBrowserLib;
using VideoHelper;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXCL.WinFormUI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace InsuranceBrowser
{
    public partial class BigScreen : ZXCL.WinFormUI.ZFormChild, InsuranceBrowserLib.IWebOperation, InsuranceBrowserLib.management
    {
        private ChromiumWebBrowser webBrowser = null;
        private string url = "about:blank";
        private bool isHide = false;
        InsuranceBrowserLib.MainForm mainForm;

        public BigScreen()
        {
            InitializeComponent();
        }

        public BigScreen(string url)
        {
            InitializeComponent();
            this.url = url;
        }


        public ChromiumWebBrowser WebBrowser
        {
            get { return webBrowser; }
            set { webBrowser = value; }
        }

        private void ChromiumForm_Load(object sender, EventArgs e)
        {
            this.url = Application.StartupPath + @"\LargeScreenDisplay\index.html";
            initWeb();
            ShowLayer();

            Thread t = new Thread(() =>
            {
                Thread.Sleep(1500);
                HideLayer();
            });
            t.Start();
        }



        private void ChromiumForm_OnFormActive()
        {
            if (mainForm == null)
                mainForm = this.MdiForm as InsuranceBrowserLib.MainForm;

            if (mainForm != null)
            {
                this.BeginInvoke(new Action(() =>
                {
                    mainForm.SetToolBarVisible(true);
                    mainForm.SetButtons(webBrowser.CanGoBack, webBrowser.CanGoForward);
                    if (ApplicationData.LanguageSign.Contains("日本語"))
                    {
                        mainForm.Text = " HEAT CHECK";
                    }
                    else if(ApplicationData.LanguageSign.Contains("中文"))
                    {
                        mainForm.Text = " 智慧人脸考勤门禁系统";
                    }
                    else
                    {
                        mainForm.Text = " Face Recognition System";
                    }
                    
                }));

            }
        }

        private void ChromiumForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //对于弹出的子窗体，必须先关闭，不然程序会卡很久
            //如果父窗体关闭了，子页面的某些功能将不能正常使用
            //这里采用隐藏父窗体的办法来处理
            if (!webBrowser.GetBrowser().IsPopup)
            {
                int popCount = 0;  //弹出子窗体数量

                foreach (var item in mainForm.MdiChildren)
                {
                    if (item is BigScreen)
                    {
                        BigScreen form = item as BigScreen;
                        if (form != this && form.webBrowser.GetBrowser().IsPopup)
                        {
                            popCount++;
                        }
                    }
                }

                if (!isHide && popCount > 0)
                {
                    e.Cancel = true;
                    MdiForm.HideForm(this);
                    isHide = true;
                }
            }
            Cef.Shutdown();

        }

        private void initWeb()
        {
            //全屏处理
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            }

            if (webBrowser == null)
                webBrowser = new ChromiumWebBrowser(url);

            webBrowser.Dock = DockStyle.Fill;
            this.toolStripContainer.ContentPanel.Controls.Add(webBrowser);
            //addAddressBar();
            webBrowser.DownloadHandler = new DownloadHandler();
            webBrowser.MenuHandler = new MenuHandler();
            JavaScriptBound1 jsBound = new JavaScriptBound1(this);
            //webBrowser.RegisterJsObject("myExtension", jsBound, false);
            var binder = BindingOptions.DefaultBinder;
            binder.CamelCaseJavascriptNames = false;
            webBrowser.JavascriptObjectRepository.Register("myExtension", jsBound, isAsync: false, options: binder );
            //webBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
            webBrowser.TitleChanged += WebBrowser_TitleChanged;
            webBrowser.StatusMessage += WebBrowser_StatusMessage;
            //webBrowser.LifeSpanHandler = new LifeSpanHandler(this);
            if (huaanClient.ApplicationData.IsTest)
                webBrowser.KeyboardHandler = new KeyboardHandler();
            //webBrowser.LoadError += OnLoadError;
            //webBrowser.FrameLoadEnd += WebBrowser_FrameLoadEnd;
        }

        private void WebBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            //new InsuranceCompany.CompanyBase().WebCompleteForCef(e);
        }

        private void WebBrowser_StatusMessage(object sender, CefSharp.StatusMessageEventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {
                //throw;
            }

        }

        private void WebBrowser_TitleChanged(object sender, CefSharp.TitleChangedEventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                this.Text = e.Title;
            }));

        }

        private void WebBrowser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {

        }





        private void Security_policyForm()
        {

        }


        void InsuranceBrowserLib.IWebOperation.ClearCache()
        {

        }

        void InsuranceBrowserLib.IWebOperation.GoBack()
        {
            webBrowser.Back();
        }

        void InsuranceBrowserLib.IWebOperation.GoForward()
        {
            webBrowser.Forward();
        }

        void InsuranceBrowserLib.IWebOperation.Navigate(string url)
        {
            webBrowser.Load(url);
        }

        void InsuranceBrowserLib.IWebOperation.Refresh()
        {
            webBrowser.Reload();
        }

        void InsuranceBrowserLib.IWebOperation.RunScript()
        {

        }

        void InsuranceBrowserLib.IWebOperation.Stop()
        {

        }

        void InsuranceBrowserLib.IWebOperation.Submit()
        {

        }

        void InsuranceBrowserLib.IWebOperation.SubmitForGongbao()
        {

        }


        public string runJSReturnStr { get; set; }
        string InsuranceBrowserLib.IWebOperation.GetString()
        {
            return runJSReturnStr;
        }

        void InsuranceBrowserLib.IWebOperation.RunScript(string path)
        {
            runJSReturnStr = null;
            //string path = @"D:\script\script.js";
            if (!File.Exists(path))
            {
                MessageBox.Show("脚本文件不存在\r\n目录:" + path);
                return;
            }
            StreamReader sr = new StreamReader(path);
            string script = sr.ReadToEnd();
            sr.Close();
            object r = webBrowser.EvaluateScriptAsync(script).Result.Result;
            if (r != null)
                runJSReturnStr = webBrowser.EvaluateScriptAsync(script).Result.Result.ToString();


        }

        bool IWebOperation.IsChrome { get { return true; } }
        bool IWebOperation.IsChromePopup { get { return webBrowser.GetBrowser().IsPopup; } }



        private OpaqueLayer m_OpaqueLayer = null;//半透明蒙板层
        bool isLoading = false;
        /// <summary>
        /// 显示遮罩层
        /// </summary>
        /// <param name="control"></param>
        /// <param name="alpha"></param>
        /// <param name="showLoadingImage"></param>
        public void ShowLayer()
        {
            isLoading = true;
            Control control = this;
            int alpha = 125;
            bool showLoadingImage = true;
            this.Invoke(new Action(() =>
            {
                if (this.m_OpaqueLayer == null)
                {
                    this.m_OpaqueLayer = new OpaqueLayer(alpha, showLoadingImage);
                    control.Controls.Add(this.m_OpaqueLayer);
                    this.m_OpaqueLayer.Dock = DockStyle.Fill;
                    this.m_OpaqueLayer.BringToFront();
                }
                this.m_OpaqueLayer.Enabled = true;
                this.m_OpaqueLayer.Visible = true;
            }));
        }

        /// <summary>
        /// 隐藏遮罩层
        /// </summary>
        public void HideLayer()
        {
            isLoading = false;
            this.Invoke(new Action(() =>
            {
                if (this.m_OpaqueLayer != null)
                {
                    this.m_OpaqueLayer.Enabled = false;
                    this.m_OpaqueLayer.Visible = false;
                }
            }));

        }


        string IWebOperation.GetUrl()
        {
            try
            {
                throw new NotImplementedException();
            }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
            catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
            {
                MessageBox.Show("谷歌内核站点暂时不支持兼容性操作!", "提示");

            }
            return "guge";
        }

        public bool save_staf(string name, string staff_no)
        {
            string ssss;
            return true;
        }

        private void BigScreen_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Escape)
            {
                this.Close();
            }
        }



        //private void button1_Click(object sender, EventArgs e)
        //{
        //    webBrowser.ShowDevTools();
        //}
    }


}


namespace InsuranceBrowser.CefHanderForChromiumFrom
{


    class JavaScriptBound1
    {
        BigScreen form;
        public JavaScriptBound1(BigScreen form)
        {
            this.form = form;
        }
    }
}
namespace InsuranceBrowser.CefHanderCommon
{
    
}

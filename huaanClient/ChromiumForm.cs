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
using System.Security.Cryptography.X509Certificates;
using HaSdkWrapper;
using CefSharp.ModelBinding;
using huaanClient.Database;
using huaanClient.Properties;
using DBUtility.SQLite;
using DapperExtensions;
using Dapper;
using System.Collections.Generic;
using huaanClient.Services;
using huaanClient.Worker;

namespace InsuranceBrowser
{
    public partial class ChromiumForm : ZXCL.WinFormUI.ZFormChild, InsuranceBrowserLib.IWebOperation, InsuranceBrowserLib.management
    {
        private ChromiumWebBrowser webBrowser = null;
        private string url = "about:blank";
        private bool isHide = false;
        InsuranceBrowserLib.MainForm mainForm;
        public static UserSettings userSettings = new UserSettings();

        
        public ChromiumForm()
        {
            InitializeComponent();
        }

        public ChromiumForm(string url)
        {
            //判断是否合格
            HaCamera.InitEnvironment();
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
            initWeb();
            Services.Tracker.Track(userSettings);
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
                        mainForm.Text = " HEAT CHECK" + " " + ApplicationData.MyAppVersion;
                    }
                    else if(ApplicationData.LanguageSign.Contains("中文"))
                    {
                        mainForm.Text = " 智慧人脸考勤门禁系统" + " " + ApplicationData.MyAppVersion;
                    }
                    else
                    {
                        mainForm.Text = " Face Recognition System" + " " + ApplicationData.MyAppVersion;
                    }

                    if (userSettings.EnableTitleLong && !string.IsNullOrEmpty(userSettings.TitleLong))
                    {
                        mainForm.Text = userSettings.TitleLong;
                    }

                }));

            }

            //规则下发
            this._manager = AccessRuleDeployManager.Instance;
            _manager.DefaultAccess = userSettings.DefaultAccess;
            _manager.LoadTasks();
            this._manager.Start();

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
                    if (item is ChromiumForm)
                    {
                        ChromiumForm form = item as ChromiumForm;
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

        }

        private void initWeb()
        {
            if (webBrowser == null)
                webBrowser = new ChromiumWebBrowser(url);

            webBrowser.Dock = DockStyle.Fill;
            this.toolStripContainer.ContentPanel.Controls.Add(webBrowser);
            //addAddressBar();
            webBrowser.DownloadHandler = new DownloadHandler();
            webBrowser.MenuHandler = new MenuHandler();
            JavaScriptBound jsBound = new JavaScriptBound(this, this.skinPanel1);
            //webBrowser.RegisterJsObject("myExtension", jsBound, false);
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            CefSharpSettings.WcfEnabled = true; 
            webBrowser.JavascriptObjectRepository.Register("myExtension1", jsBound, isAsync: false, options: new BindingOptions() { CamelCaseJavascriptNames = false });
            //webBrowser.JavascriptObjectRepository.Register("myExtension", jsBound, isAsync: false, options: BindingOptions.DefaultBinder);
            webBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
            webBrowser.TitleChanged += WebBrowser_TitleChanged;
            webBrowser.StatusMessage += WebBrowser_StatusMessage;
            webBrowser.LifeSpanHandler = new LifeSpanHandler(this);
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
                //this.Text = e.Title;
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
        private AccessRuleDeployManager _manager;

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

        public void setText(string text)
        {
            if (mainForm != null)
            {
                mainForm.Text = text;
            }
        }
    }


}


namespace InsuranceBrowser.CefHanderForChromiumFrom
{


    class JavaScriptBound
    {
        //显示大屏
        public void showBigScreen()
        {
            form.Invoke(new Action(() =>
            {
                BigScreen bigScreen = new BigScreen();
                bigScreen.Show();
            }));      
        }
        public string getlanguage()
        {
            string language = "zh_CN";
            if (ApplicationData.LanguageSign.Contains("English"))
            {
                language = "en_US";
            }
            else if (ApplicationData.LanguageSign.Contains("日本語"))
            {
                language = "Jan_JPN";
            }
            else if (ApplicationData.LanguageSign == Constants.LANG_NAME_FRENCH)
            {
                language = "Fr_fr";
            }
            else if (ApplicationData.LanguageSign == Constants.LANG_NAME_VIETNAMESE)
            {
                language = Constants.LANG_LOCALE_VIETNAMESE;
            }
            return language;
        }
        //System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
        OpenFileDialog openFile = new OpenFileDialog();
        ChromiumForm form;
        MultiPlayerPanel skinPanel;
        public JavaScriptBound(ChromiumForm form, MultiPlayerPanel skinPanel)
        {
            this.form = form;
            this.skinPanel = skinPanel;
        }
        //更新用户
        public bool save_staff(string name, string staff_no)
        {
            string ssss;
            return true;
        }
        //获取机构信息列表
        public string getDepartmentData()
        {
            string data = GetData.getDepartmentDataI();
            return data;
        }

        public bool delDepartmentData(string no, string sedata)
        {
            var dep = JsonConvert.DeserializeObject<Department>(sedata);
            bool data = GetData.delDepartmentData(no, sedata);
            using (var c = SQLiteHelper.GetConnection())
            {
                c.Execute($"DELETE FROM RuleDistributionItem WHERE GroupId = {dep.id} AND GroupType = 1");
            }
            return data;
        }

        public string updatDepartmentData(string name, string explain, string phone, string address, string no)
        {
            string data = GetData.updateDepartmentData(name, explain, phone, address, no);
            return data;
        }

        public string AddDepartmentData(string name, string explain, string phone, string address, string no, string ParentId)
        {
            string data = GetData.AddDepartmentData(name, explain, phone, address, no, ParentId);
            return data;
        }

        //返回部门编码
        public string getDepartmentNo()
        {
            string data = GetData.getDepartmentNo();
            return data;
        }
        public string getstaffforlineAdminData()
        {
            string data = GetData.getstaffforlineAdminData();
            return data;
        }
        public string getstaffforlineAdmin()
        {
            string data = GetData.getstaffforlineAdmin();
            return data;
        }
        //批量导出门禁记录
        public void BatchXportforCapture(string statime, string endtime, string name, string devname, string selectedPersonTypes, string HealthCodeType, string type, string tempFrom, string tempTo)
        {

            string result = GetData.getCapture_Datacuont(statime, endtime, name, devname, selectedPersonTypes, HealthCodeType, tempFrom.toFloat(), tempTo.toFloat());
            form.Invoke(new Action(() =>
            {
                JArray jo = (JArray)JsonConvert.DeserializeObject(result);
                string reint = jo[0]["count"].ToString();
                if (int.Parse(reint) > 10000)
                {
                    MessageBox.Show("选择条件内超过不能超过10000行数据！");
                    return;
                }else if (int.Parse(reint)==0)
                {
                    MessageBox.Show("未查询到数据！");
                    return;
                }
                else
                {
                    var data = GetData.getCapture_Data1(statime, endtime, name, devname, selectedPersonTypes, HealthCodeType, tempFrom.toFloat(), tempTo.toFloat());
                    var propertyNames = Tools.GetPropertyNames(nameof(Capture_Data));
                    Func<Capture_Data, string, object, string> convertProperty = (d, pname, v) =>
                    {
                        switch (pname)
                        {
                            case nameof(Capture_Data.body_temp):
                                if (v is string val)
                                {
                                    return val.Length > 3 ? val.Substring(0, 4) : val;
                                }
                                return "";
                                break;
                            case nameof(Capture_Data.exist_mask):
                                return v?.ToString() == "1" ? Strings.Yes : Strings.No;
                                break;
                            case nameof(Capture_Data.QRcodestatus):
                                switch (v)
                                {
                                    case "0"://绿
                                        return Strings.GreenCode;
                                        break;
                                    case "1"://红
                                        return Strings.RedCode;
                                        break;
                                    case "2"://黄
                                        return Strings.YellowCode;
                                        break;
                                    default:
                                        return v?.ToString().Split(';')[0] ?? "";
                                        break;
                                }
                                break;
                            default:
                                return v?.ToString() ?? "";
                                break;
                        }
                    };

                    DataToCsv.ExportDataToXlsx<Capture_Data>(
                        Strings.CaptureDataExportDefaultFileName,
                        data,
                        propertyNames,
                        convertProperty
                        );
                    //exportToCsv.exportFor(type,re, statime.Split(' ')[0] +"-"+ endtime.Split(' ')[0]);
                }
                //选择路径进行导出

            }));
            
        }

        public void export()
        {
            form.Invoke(new Action(() =>
            {
                exportToCsv.exportForstaff();

            }));

        }

        public bool setstaffforlineAdmin(string ids)
        {
            return GetData.setstaffforlineAdmin(ids);
        }

        //返回line的userid
        public void addUserforline(IJavascriptCallback callback, string id)
        {
            Task.Factory.StartNew(() =>
            {
                string data = HandleCaptureData.getLINUserID(id);
                callback.ExecuteAsync(data);
            });
        }
        //先获取code
        public void generatecode(IJavascriptCallback callback, string id)
        {
            Task.Factory.StartNew(() =>
            {
                string data = HandleCaptureData.generatecode(id);
                callback.ExecuteAsync(data);
            });
        }

        //返回line的Email
        public void addEmailforline(IJavascriptCallback callback, string id)
        {
            Task.Factory.StartNew(() =>
            {
                string data = HandleCaptureData.getLINEEmail(id);
                callback.ExecuteAsync(data);
            });
        }
        //先获取Email_code
        public void generatecodemail(IJavascriptCallback callback, string id)
        {
            Task.Factory.StartNew(() =>
            {
                string data = HandleCaptureData.generatecodemail(id);
                callback.ExecuteAsync(data);
            });
        }


        //获取员工分类信息
        public string getEmployetype()
        {
            string data = GetData.getEmployetype();
            return data;
        }
        //删除某个员工分类
        public void deleteEmployetype(string val)
        {
            GetData.deleteEmployetype(val);
        }
        public void addEmployetype(string val)
        {
            GetData.addEmployetype(val);
        }



        //获取默认line_userid
        public string getline_userid()
        {
            string data = GetData.getline_userid();
            return data;
        }
        //获取line参数信息
        public string getline()
        {
            string data = GetData.getline();
            return data;
        }

        public string getPdfconfiguration()
        {
            string data = GetData.getPdfconfiguration();
            return data;
        }


        public string EdPdfconfiguration(string pdftitle, string rows1, string rows2, string rows3, string rows4
            , string rows5
            , string rows6
            , string rows7, string rows8, string rows9, string rows10, string rows11,string rows12)
        {
            string data = GetData.EdPdfconfiguration(pdftitle, rows1, rows2, rows3, rows4
            , rows5
            , rows6
            , rows7, rows8, rows9, rows10, rows11, rows12);
            return data;
        }
        public string getStaffDataQuey(string name, string no, string qu_phone, string pageint, string limt)
        {
            string data = GetData.getStaffData(name, no, qu_phone, pageint, limt);
            return data;
        }
        public string getStaffDataQueyforcount(string name, string no, string qu_phone)
        {
            string data = GetData.getStaffDataforcount(name, no, qu_phone);
            return data;
        }

        //获取用户信息列表
        public string getStaffData(string pageint, string limt)
        {
            string data = GetData.getStaffData(pageint, limt);
            return data;
        }
        public string getStaffDatacount()
        {
            string data = GetData.getStaffDatacount();
            return data;
        }

        //添加新员工
        public string setStaff(string name, string staff_no, string phone, string email, string department, string Employetype, string imgeurl, string lineType, string line_userid, string face_idcard,string idcardtype,string customer_text)
        {
            string data = GetData.setStaf(name.Trim(), staff_no, phone.Trim(), email.Trim(), department, Employetype, imgeurl, lineType.Trim(), line_userid, face_idcard.Trim(), idcardtype.Trim(), Staff.STAFF_SOURCE_MANUAL_ADD, customer_text.Trim());
            return data;
        }
        public string setStaffForsynchronization(string ID,string name, string staff_no, string phone, string email, string department, string Employetype, string imgeurl, string lineType, string line_userid, string face_idcard, string idcardtype, string source)
        {
            string data = GetData.setStaf(ID.ToString(),name.Trim(), staff_no, phone.Trim(), email.Trim(), department, Employetype, imgeurl, lineType.Trim(), line_userid, face_idcard.Trim(), idcardtype.Trim(), source.Trim());
            return data;
        }
        //编辑员工
        public string EditStaff(string name, string staff_no, string phone, string email, string department, string Employetype, string imgeurl, string line_userid,string lineType, string id,string face_idcard, string idcardtype)
        {
            string data = GetData.eidStaf(name.Trim(), staff_no, phone.Trim(), email.Trim(), department, Employetype, imgeurl, line_userid.Trim(), lineType.Trim(), id, face_idcard.Trim(), idcardtype.Trim()) ;
            return data;
        }

        //添加访客
        public string setVisitor(string name,  string phone,  string imgeurl, string statime, string endtime)
        {
            string data = GetData.setVisitor(name.Trim(), phone, imgeurl.Trim(), statime.Trim(), endtime.Trim());
            return data;
        }
        //编辑访客
        public string EditVisitor(string name, string phone, string imgeurl, string statime, string endtime,string id)
        {
            string data = GetData.editVisitor(name.Trim(), phone, imgeurl.Trim(), statime.Trim(), endtime.Trim(), id);
            return data;
        }
        //下发访客
        public bool downVisitorForid(string name, string imgeurl, string statime, string endtime, string id)
        {
            bool re = false;
            form.Invoke(new Action(() =>
            {
                try
                {
                    form.ShowLayer();
                    re = DistributeToequipment.distrbute(name.Trim(), imgeurl.Trim(), statime.Trim(), endtime.Trim(), id);
                    form.HideLayer();
                }
                catch
                {
                    form.HideLayer();
                }
            }));

            return re;
        }

        public void delVisitorforId(string id)
        {
            DistributeToequipment.delVisitorforId(id.Trim());
        }

        public bool delVisitorForid(string id)
        {
            bool re = GetData.delVisitorForid(id);
            return re;
        }

        //添加考勤组
        public string setGroup(string attribute, string name, string isdefault, string ids, string timestamp)
        {
            string data = GetData.setGroup(attribute, name, isdefault, ids, timestamp);
            return data;
        }

        //修改考勤组
        public string editGroup(string attribute, string name, string isdefault, string ids, string id)
        {
            string data = GetData.editGroup(attribute, name, isdefault, ids, id);
            return data;
        }


        //获取员工分类列表
        public string getlEmployetypedata()
        {
            string data = GetData.getlEmployetypedata();
            return data;
        }
        //删除用户
        public bool DeleteUser(string userid)
        {
            bool data = GetData.DeleteUser(userid);
            return data;
        }

        public bool DeleteGroup(string id)
        {
            bool data = GetData.DeleteGroup(id);
            return data;
        }
        //设置默认班组
        public bool set_default(string id)
        {
            bool data = GetData.set_default(id);
            return data;
        }


        public string openImgeUrl()
        {
            string re = "";
            string reurl = "";
            form.Invoke(new Action(() =>
            {
                re = copyfile.openImge();
            }));
            if (!string.IsNullOrEmpty(re))
            {
                if (re.Length > 3)
                {
                    reurl = copyfile.copyimge(re, copyfile.GetTimeStamp());
                }
                else
                {
                    reurl = re;
                }
            }
            return reurl;
        }

        public string openImgeforRcodeUrl()
        {
            string re = "";
            string reurl = "";
            form.Invoke(new Action(() =>
            {
                re = copyfile.openImgeforRcode();
            }));
            if (!string.IsNullOrEmpty(re))
            {
                if (re.Length > 3)
                {
                    reurl = copyfile.copyimge(re, copyfile.GetTimeStamp());
                }
                else
                {
                    reurl = re;
                }
            }
            return reurl;
        }

        //获取所有设备列表
        public void getAllDeviceDiscover(IJavascriptCallback callback)
        {
            Task.Factory.StartNew(() =>
            {
                string data = GetData.getallDeviceDiscover().Result;
                callback.ExecuteAsync(data);
            });

        }
        //获取当前设备列表
        public string getDeviceDiscover()
        {
            string data = GetData.getDeviceDiscover();
            return data;
        }

        public string AddIPtoMydevice(string IP, string DeviceName, int inout)
        {
            string data = GetData.AddIPtoMydevice(IP, DeviceName, inout);
            return data;
        }
        public string UpdatIPtoMydevice(string oldIp, string IP, string DeviceName, int inout)
        {
            string data = GetData.UpdatIPtoMydevice(oldIp, IP, DeviceName, inout);
            return data;
        }
        public bool DeleteIPtoMydevice(string IP)
        {
            bool result = GetData.DeleteIPtoMydevice(IP);
            return result;
        }

        public bool UpdateDeviceName(string newname, string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return false;
            if (string.IsNullOrEmpty(newname))
                newname = "";
            bool result = GetData.UpdateDeviceName(newname, ip);
            return result;
        }

        public void Open(IJavascriptCallback callback, string ip)
        {
            Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(ip))
                    callback.ExecuteAsync(false);
                bool result = GetData.Open(ip);
                callback.ExecuteAsync(result);
            });
        }

        //下发人脸数据
        //public void AddPerson(IJavascriptCallback callback,string ip, string name, string imgeurl,string code)
        //{
        //    Task.Factory.StartNew(() =>
        //    {
        //        string data = GetData.setAddPerson(ip, name, imgeurl, code);
        //        callback.ExecuteAsync(data);
        //    });
        //}
        public string AddPerson(string ip, string name, string imgeurl, string code)
        {
            string data = GetData.setAddPerson(ip, name, imgeurl, code);
            return data;
        }

        //一键下发
        public void One_click_distribution(IJavascriptCallback callback)
        {
            Task.Factory.StartNew(() =>
            {
                bool data = GetData.setAddPersonToEquipment_distribution(); ;
                callback.ExecuteAsync(data);
            });
        }

        public void AddPersonToEquipment_distribution(string datajson)
        {
            GetData.setAddPersonToEquipment_distribution(datajson);
        }

        public string queryPerson(string id)
        {
            string data = GetData.queryPerson(id);
            return data;
        }

        //获取考勤班组管理列表
        public string getShiftData()
        {
            string data = GetData.getShiftData();
            return data;
        }
        //获取考勤组列表
        public string getGroup()
        {
            string data = GetData.getGroup();
            return data;
        }
        public string getGroupForId(string id)
        {
            string data = GetData.getGroup(id);
            return data;
        }
        public string getStaffIdsInAttendanceGroup(int groupId)
        {
            return GetData.getStaffIdsInAttendanceGroup(groupId);
        }
        public string getEffectiveTime(string Shift_id)
        {
            string data = GetData.getEffectiveTime(Shift_id);
            return data;
        }
        public string setShiftData(string shifdata)
        {
            string data = GetData.setShiftData(shifdata);
            return data;
        }
        public string setShiftData_edit(string shifdata, string id)
        {
            string data = GetData.setShiftData(shifdata, id);
            return data;
        }
        //DeleteShifr
        public bool DeleteShift(string id)
        {
            bool data = GetData.DeleteShift(id);
            return data;
        }
        //获取考勤信息
        public void getAttendanceinformation(IJavascriptCallback callback, string starttime, string endtime)
        {
            starttime = starttime.Replace('/', '-');
            endtime = endtime.Replace('/', '-');
            Task.Factory.StartNew(() =>
            {
                using (callback)
                {
                    string data = AttendanceAlgorithm.getpersonnel(starttime + " 00:00:00", endtime + " 23:59:59", 1);
                    callback.ExecuteAsync(data);
                }

                //string data = AttendanceAlgorithm.getpersonnel(starttime+" 00:00:00", endtime + " 23:59:59");
                //callback.ExecuteAsync(data);
            });
        }
        //生成考勤信息   主动从相机去获取数据然后写进数据库
        public void getAttendanceinfordevice(IJavascriptCallback callback, string starttime, string endtime)
        {
            starttime = starttime.Replace('/', '-');
            endtime = endtime.Replace('/', '-');
            Task.Factory.StartNew(() =>
            {
                using (callback)
                {
                    string data = AttendanceAlgorithm.getpersonnel(starttime + " 00:00:00", endtime + " 23:59:59", 0);
                    callback.ExecuteAsync(data);
                }

                //string data = AttendanceAlgorithm.getpersonnel(starttime+" 00:00:00", endtime + " 23:59:59");
                //callback.ExecuteAsync(data);
            });
        }
        //获取月度考勤信息
        public string getMonthlyData(string date, string name)
        {
            date = date.Replace(@"/", "-");
            var data = GetData.getMonthlyData(date, name);
            return JsonConvert.SerializeObject(data);
        }
        //导出月度考勤报表
        public void exportMonthlyData(string date, string name)
        {
            form.Invoke(new Action(() =>
            {
                date = date.Replace(@"/", "-");
                var data = GetData.getMonthlyData(date, name);
                var pnames = Tools.GetPropertyNames(nameof(AttendanceDataMonthly));
                var selectedProperty = new string[] { "name", "department", "Employee_code", "nowdate", "Attendance", "latedata", "Leaveearlydata", "AbsenteeismCount", "LeaveCount"};
                Func<AttendanceDataMonthly, string, object, string> convertPropertyToString = (obj, pname, pvalue) =>
                {
                    switch (pname)
                    {
                        case nameof(AttendanceDataMonthly.LeaveCount)://请假天数
                            var LeaveCountforint = string.Empty;
                            LeaveCountforint = ((obj.LeaveCount + obj.LeaveCount1) / 2).ToString();
                            return LeaveCountforint;
                            break;
                        default:
                            return pvalue?.ToString() ?? "";
                            break;
                    }

                };

                
                DataToCsv.ExportDataToXlsx(
                    $"{Strings.AttendanceDataMonthlyExportFileName}({date})",
                    data,
                    pnames,
                    convertPropertyToString,
                    selectedProperty
                    );
                //exportToCsv.export(data, date);
            }));

        }

        public string queryAttendanceinformationcount(string starttime, string endtime, string name, string late, string Leaveearly, string isAbsenteeism)
        {
            starttime = starttime.Replace(@"/", "-");
            endtime = endtime.Replace(@"/", "-");
            string data = GetData.queryAttendanceinformationcount(starttime, endtime, name, late, Leaveearly, isAbsenteeism);
            return data;
        }

        public void queryAttendanceinformation(IJavascriptCallback callback, string starttime, string endtime, string name, string late, string Leaveearly, string isAbsenteeism, string page, string limt)
        {
            starttime = starttime.Replace(@"/", "-");
            endtime = endtime.Replace(@"/", "-");
            Task.Factory.StartNew(() =>
            {
                form.ShowLayer();
                form.Invoke(new Action(() =>
                {
                    string data = GetData.queryAttendanceinformation(starttime, endtime, name, late, Leaveearly, isAbsenteeism,  page,  limt);
                    form.HideLayer();
                    callback.ExecuteAsync(data);
                }));

            });
        }

        public void queryAttendanceinByid(IJavascriptCallback callback,string personId)
        {
            Task.Factory.StartNew(() =>
            {
                string data = GetData.queryAttendanceinformation(personId);
                callback.ExecuteAsync(data);
            });
        }

        //写入自定义导出每日考勤数据
        public bool setCsvSettings(string key, string values)
        {
            bool data = GetData.setCsvSettings(key,values);
            return data;
        }

        //获取自定义导出每日考勤数据
        public string  getCsvSettings()
        {
            string data = GetData.getCsvSettings();
            return data;
        }

        //导出每日考勤数据
        public void exportAttendanceinformation(string starttime, string endtime, string name, string late, string Leaveearly, string isAbsenteeism)
        {
            form.Invoke(new Action(() =>
            {
                starttime = starttime.Replace(@"/", "-");
                endtime = endtime.Replace(@"/", "-");


                string[] userSelProp  = null;
                using (var conn = SQLiteHelper.GetConnection())
                {
                    var csvSetting = (IDictionary<string, object>) conn.QueryFirstOrDefault("select * from CsvSettings");
                    object keyStr = null;
                    if (csvSetting?.TryGetValue("keyStr", out keyStr) == true)
                    {
                        userSelProp = keyStr.ToString().Split(',');
                    }
                }
                

                var selectedProperties = new[] { "name", "department", "Employee_code", "Date", "Punchinformation", "Punchinformation1", "Shiftinformation", "Duration", "late", "Leaveearly", "workOvertime", "isAbsenteeism", "temperature" };
                var attData = GetData.queryAttendanceinformation(starttime, endtime, name, late, Leaveearly, isAbsenteeism);
                exportToCsv.exportForDay(attData, starttime + endtime, userSelProp ?? selectedProperties);
            }));

        }
        //修改IP地址
        public void changeIP(string mac, string ip, string mask, string gateway)
        {
            DeviceDiscover.SetIpByMac(mac, ip, mask, gateway);
        }

        //判断本地文件是否存在
        public bool IsExis(string rul)
        {
            if (System.IO.File.Exists(@rul))
                return true;
            else return false;
        }

        //修改line参数
        public string EditLine(string temperature, string Message, string Message2, string Message3, string Message4
            , string Message5
            , string Message6
            , string Message7, string Message8, string Message9, string Message10, string Message11, string Message12, string line_url
            , string ftpserver, string ftppassword, string ftpusername)
        {
            string data = GetData.EditLine(temperature, Message, Message2, Message3, Message4
            , Message5
            , Message6
            , Message7, Message8, Message9, Message10, Message11, Message12, line_url,  ftpserver,  ftppassword,  ftpusername);
            return data;
        }

        //获取下发记录总数
        public string getcountforEquipment_distribution(string name, string ip, string status, string DeviceName)
        {
            string data = GetData.getcountforEquipment_distribution(name, ip, status, DeviceName);
            return data;
        }

        public string getforEquipment_distribution(string page, string limt, string name, string ip, string status, string DeviceName)
        {
            string data = GetData.getEquipment_distribution(page, limt, name, ip, status, DeviceName);
            return data;
        }

        //获取line接口迟到早退总人数
        public string getcountforLineFor_list(string name, string stadate, string endate, string type)
        {
            stadate = stadate.Replace('/', '-');
            endate = endate.Replace('/', '-');
            string data = GetData.getcountforLineFor_list(name, stadate, endate, type);
            return data;
        }

        public string getforLineFor_list(string name, string stadate, string endate, string type, string page, string limt)
        {
            stadate = stadate.Replace('/', '-');
            endate = endate.Replace('/', '-');
            string data = GetData.getforLineFor_list(name, stadate, endate, type, page, limt);
            return data;
        }
        //下发迟到早退人员到Line接口
        public void sendOutforLine(IJavascriptCallback callback, string id)
        {
            Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(id))
                    callback.ExecuteAsync(false);
                bool result = GetData.sendOutforLine(id);
                callback.ExecuteAsync(result);
            });
        }

        //获取不打卡日期或者打卡日期 type   0 不打卡 1代表必须打卡
        public string getSpecial_datecount(string editId, string type)
        {
            string data = GetData.getSpecial_datecount(editId, type);
            return data;
        }
        //获取不打卡日期或者打卡日期
        public string getSpecial_date(string editId, string type, string page, string limt)
        {
            string data = GetData.getSpecial_date(editId, type, page, limt);
            return data;
        }

        public bool delSpecial_date(string ids)
        {
            bool data = GetData.delSpecial_date(ids);
            return data;
        }

        public bool saveSpecial_date(string editId, string type, string date, string Shiftid)
        {
            bool data = GetData.saveSpecial_date(editId, type, date, Shiftid);
            return data;
        }

        //批量导入
        public void BatchIimport(IJavascriptCallback callback)
        {
            form.ShowLayer();
            form.BeginInvoke(new Action(async () =>
            {
                string result = "";
                try
                {
                    Task<string> task = BatchImport.batchImport();
                    result = await task;
                }
                catch { }

                form.HideLayer();
                await callback.ExecuteAsync(result);
            }));
        }
        public void Download()
        {
            form.Invoke(new Action(() =>
            {
                BatchImport.Download();
            }));

        }

        //getCapture_Data(string statime,string endtime,string name,string devname,string pageint,string limt)
        public string getCapture_Data(string statime, string endtime, string name, string devname, string stranger,string HealthCodeType, string tempFrom, string tempTo, string pageint, string limt)
        {
            //Task.Factory.StartNew(() =>
            //{
            //    form.Invoke(new Action(() =>
            //    {
            //        form.ShowLayer();
            //        string result = GetData.getCapture_Data(statime,endtime,name,devname,pageint,limt);
            //        form.HideLayer();
            //        callback.ExecuteAsync(result);
            //    }));
            //});
            string result = string.Empty;
            form.Invoke(new Action(() =>
            {
                try
                {
                    form.ShowLayer();
                    result = GetData.getCapture_Data(statime, endtime, name, devname, stranger,HealthCodeType, tempFrom.toFloat(), tempTo.toFloat(), pageint, limt);
                    form.HideLayer();
                }
                catch
                {

                }
            }));
            return result;
        }
        //首页调用
        public void getCapture_Dataforindex(IJavascriptCallback callback,string statime, string endtime, string name, string devname, string selectedPersonTypes, string HealthCodeType, string tempFrom, string tempTo, string pageint, string limt)
        {
            //Task.Factory.StartNew(() =>
            //{
            //    form.Invoke(new Action(() =>
            //    {
            //        form.ShowLayer();
            //        string result = GetData.getCapture_Data(statime,endtime,name,devname,pageint,limt);
            //        form.HideLayer();
            //        callback.ExecuteAsync(result);
            //    }));
            //});
            string result = string.Empty;
            Task.Factory.StartNew(() =>
            {
                string data = GetData.getCapture_Data(statime, endtime, name, devname, selectedPersonTypes, HealthCodeType, tempFrom.toFloat(), tempTo.toFloat(), pageint, limt);
                callback.ExecuteAsync(data);
            });


            //form.Invoke(new Action(() =>
            //{
            //    try
            //    {
                    
            //        result = 
            //    }
            //    catch
            //    {

            //    }
            //}));
            //return result;
        }
        public string getVisitor(string statime, string statime1, string endtime, string endtime1, string name, string phone, string isDown, string pageint, string limt)
        {
            //Task.Factory.StartNew(() =>
            //{
            //    form.Invoke(new Action(() =>
            //    {
            //        form.ShowLayer();
            //        string result = GetData.getCapture_Data(statime,endtime,name,devname,pageint,limt);
            //        form.HideLayer();
            //        callback.ExecuteAsync(result);
            //    }));
            //});
            string result = string.Empty;
            form.Invoke(new Action(() =>
            {
                try
                {
                    form.ShowLayer();
                    result = GetData.getVisitor(statime, statime1, endtime, endtime1, name, phone, isDown, pageint, limt);
                    form.HideLayer();
                }
                catch
                {
                    form.HideLayer();
                }
            }));
            return result;
        }
        public bool delCapture_DataForid(string id)
        {
            bool re = GetData.delCapture_DataForid(id);
            return re;
        }
        public string getCapture_Datacuont(string statime, string endtime, string name, string devname, string stranger,string HealthCodeType, string tempFrom, string tempTo)
        {
            
            string result = GetData.getCapture_Datacuont(statime, endtime, name, devname, stranger, HealthCodeType,
                tempFrom.toFloat(), tempTo.toFloat());

            return result;
        }

        public string getVisitorcuont(string statime, string statime1, string endtime, string endtime2, string name, string phone, string isDown)
        {

            string result = GetData.getVisitorcuont(statime, statime1, endtime, endtime2,name, phone, isDown);

            return result;
        }

        public void Prinpdf(string id,string linetype)
        {
            form.Invoke(new Action(() =>
            {
                if (linetype=="2")
                {
                    PrinPDF.prinpdfforlineEmail(id);
                }
                else
                {
                    PrinPDF.prinpdf(id);
                } 
            }));

        }
        //上传line二维码图片
        public string setlineQRcode(string imgeurl)
        {
            string data = GetData.setlineQRcode(imgeurl);
            return data;
        }

        public string setlineQRcodeEmail(string imgeurl)
        {
            string data = GetData.setlineQRcodeEmail(imgeurl);
            return data;
        }

        public string getlineQRcode()
        {
            string data = GetData.getlineQRcode();
            return data;
        }

        public string getlineQRcodeEmail()
        {
            string data = GetData.getlineQRcodeEmail();
            return data;
        }

        //调用摄像头
        public string OpenCamera()
        {
            string imgeUrl = "";
            form.Invoke(new Action(() =>
            {
                Camera camera = new Camera();
                camera.ShowDialog();
                if (!string.IsNullOrEmpty(camera.imgurl))
                {
                    imgeUrl = camera.imgurl;
                }
            }));
            return imgeUrl;
        }

        public void displayPanel(string width, string height, string locationW, string locationH)
        {
            form.Invoke(new Action(() =>
            {
                if (skinPanel.Visible == false)
                {
                    if (width.Contains("."))
                    {

                        width = width.Split('.')[0];
                    }
                    if (height.Contains("."))
                    {
                        height = height.Split('.')[0];
                    }
                    if (locationW.Contains("."))
                    {
                        locationW = locationW.Split('.')[0];
                    }
                    if (locationH.Contains("."))
                    {
                        locationH = locationH.Split('.')[0];
                    }
                    int ss = Convert.ToInt32(locationW);
                    int ssss = Convert.ToInt32(locationH);
                    skinPanel.Location = new System.Drawing.Point(Convert.ToInt32(locationW), Convert.ToInt32(locationH));
                    skinPanel.Size = new Size(Convert.ToInt32(width), Convert.ToInt32(height));
                    skinPanel.Visible = true;
                }
            }));
        }

        public void NodisplayPanel()
        {
            form.Invoke(new Action(() =>
            {
                if (skinPanel.Visible == true)
                {
                    for (int i=0;i< VideoHelper.VideoHelper.DevicelistForVideo.Count;i++)
                    {
                        MultiPlayerControl s = (MultiPlayerControl)VideoHelper.VideoHelper.DevicelistForVideo[i].Tag;
                        s.MPCStop();
                    }
                    //VideoHelper.VideoHelper.DevicelistForVideo.ForEach(c =>
                    //{
                    //    MultiPlayerControl s = (MultiPlayerControl)c.Tag;
                    //    s.MPCStop();
                    //});
                    VideoHelper.VideoHelper.DevicelistForVideo.Clear();
                    skinPanel.Visible = false;
                }
            }));
        }

        public void AppIp(string ip)
        {
            form.Invoke(new Action(() =>
            {
                VideoHelper.VideoHelper.getinfoToMyDev(ip);
            }));

        }

        public bool CardReplacement(string type, string id, string staTime, string endTime, string timeInterval, string number)
        {
            bool re = GetData.CardReplacement(type, id,  staTime,  endTime, timeInterval, number);
            return re;
        }

        public string getindexforAttendanceToday()
        {
            string re = GetData.getindexforAttendanceToday();
            return re;
        }
        public string getindexforNumberRegist()
        {
            string re = GetData.getindexforNumberRegist();
            return re;
        }
        public string getindexforNumberequipments()
        {
            string re = GetData.getindexforNumberequipments();
            return re;
        }
        public string getindexforNumberequipment()
        {
            string re = GetData.getindexforNumberequipment();
            return re;
        }
        public string getindexforlate()
        {
            string re = GetData.getindexforlate();
            return re;
        }
        public string getindexforLeaveEarly()
        {
            string re = GetData.getindexforLeaveEarly();
            return re;
        }
        public string getindexforleave()
        {
            string re = GetData.getindexforleave();
            return re;
        }

        public string getCameraParameters(string ip)
        {
            string re = GetData.getCameraParameters(ip);
            return re;
        }

        public bool setCameraParameters(string ip, string dereplication,
            string enable_alive,
            string enable,
            string limit,
            string led_mode,
            string led_brightness,
            string led_sensitivity,
            string screensaver_mode,
            string output_not_matched,
            string volume)
        {
            bool re = GetData.setCameraParameters(ip, dereplication, enable_alive, enable, limit, led_mode, led_brightness, led_sensitivity, screensaver_mode, output_not_matched, volume);
            return re;
        }
        //修改ip
        public bool SetNetworkInfo(string ip,string oldip, string gateway, string netmask, string dns)
        {
            bool re = GetData.SetNetworkInfo(ip, oldip, gateway, netmask, dns);
            return re;
        }
        //获取ip
        public string GetNetworkInfo(string ip)
        {
            return GetData.GetNetworkInfo(ip);
        }
        //写入软件开关
        public bool setSystem_setting(string nooff)
        {
            return GetData.setIsNtpSync(nooff);
        }
        //获取开关
        public bool gettime_syn()
        {
            return GetData.getIsNtpSync();
        }


        public  bool getIscode_syn()
        {
            return GetData.getIscode_syn();
        }
        public  bool setIscode_syn(string nooff)
        {
            return GetData.setIscode_syn(nooff);
        }

        //是否提示新手引导 如果返回true则不显示新手提示
        public bool IsNoNoviceGuide()
        {
            return GetData.IsNoNoviceGuide();
        }

        //获取本机IP
        public string GetIpforPC()
        {
            return GetData.GetIpforPC();
        }

        public string getDataSyn(string name, string role, string stutas, string addr_name, string page, string limt)
        {
            return GetData.getDataSyn( name,  role,  stutas,addr_name,  page,  limt);
        }

        public string getDataSynCount(string name, string role, string stutas, string addr_name)
        {
            return GetData.getDataSynCount(name, role, stutas,addr_name);
        }

        public bool deleteDataSyn(string personid, string device_sn)
        {
            return GetData.deleteDataSyn(personid, device_sn);
        }

        //一键注册  数据同步——>staff
        public void registDataSynTostaff(string name, string role, string stutas, string addr_name)
        {
            GetData.registDataSynTostaff(name, role, stutas, addr_name);
        }

        //获取前七天的数据
        public string getCapture_Data7day()
        {
            return GetData.getCapture_Data7day();
        }

        public string getLogo()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"branding\logo.bmp");
            if (File.Exists(path))
            {
                return path;
            }

            return "";
        }

        public void enableLongTitle(string enable)
        {
            ChromiumForm.userSettings.EnableTitleLong = enable == "true" || enable == "1";
            Services.Tracker.Persist(ChromiumForm.userSettings);
        }


        public void setLongTitle(string title)
        {
            ChromiumForm.userSettings.TitleLong = title;
            Services.Tracker.Persist(ChromiumForm.userSettings);

            if (ChromiumForm.userSettings.EnableTitleLong)
            {
                form.BeginInvoke((Action)(()=>form.setText(title)));
            }
            
        }

        public void enableShortTitle(string enable)
        {
            ChromiumForm.userSettings.EnableTitleShort = enable == "true" || enable == "1";
            Services.Tracker.Persist(ChromiumForm.userSettings);
        }

        public void setShortTitle(string title)
        {
            ChromiumForm.userSettings.TitleShort = title;
            Services.Tracker.Persist(ChromiumForm.userSettings);
        }

        public void hideAttendanceManagementPage(string hide)
        {
            ChromiumForm.userSettings.HideAttendanceManagementPage = hide == "true" || hide == "1";
            Services.Tracker.Persist(ChromiumForm.userSettings);
        }

        public void hideAttendanceConfigPage(string hide)
        {
            ChromiumForm.userSettings.HideAttendanceConfigPage = hide == "true" || hide == "1";
            Services.Tracker.Persist(ChromiumForm.userSettings);
        }

        public void setShowCelsius(bool showCelsius)
        {
            ChromiumForm.userSettings.ShowTemperatureInCelsius = showCelsius;
            Services.Tracker.Persist(ChromiumForm.userSettings);
        }

        public string getUserConfigObject()
        {
            var json = JsonConvert.SerializeObject(ChromiumForm.userSettings);
            return json;
        }

        //获取数据库中的设备列表
        public string getAllMyDevices()
        {
            var json = JsonConvert.SerializeObject(GetData.getAllMyDevice());
            return json;
        }

        public string getInOutCount()
        {
            var data = GetData.getInOutCount(DateTime.Today);
            return JsonConvert.SerializeObject( new { data.In, data.Out } );
        }

        public string getCaptureDataByIdForDate(string id, string date)
        {
            var data = GetData.getCaptureDataByIdForDate(id, DateTime.Parse(date));
            var json = JsonConvert.SerializeObject(data);
            return json;
        }

        public string getAllAccessRules()
        {
            var data = GetData.GetAllAccessRules();
            var json = JsonConvert.SerializeObject(data);
            return json;
        }

        public string addTimeSegment(int parentId, string from, string to)
        {
            var data = GetData.AddTimeSegmentToDay(parentId, from, to);
            var json = JsonConvert.SerializeObject(data);
            return json;
        }

        public void removeTimeSegment(int id)
        {
            GetData.RemoveTimeSegmentById(id);
        }

        public void removeAccessRule(int id)
        {
            GetData.RemoveAccessRuleById(id);
        }

        public string addWeekAccessRule(string name)
        {
            var data = GetData.AddAccessRule(name, RepeatType.RepeatByWeek);
            return JsonConvert.SerializeObject(data);
        }

        public string addDayAccessRule(string name)
        {
            var data = GetData.AddAccessRule(name, RepeatType.RepeatByDay);
            return JsonConvert.SerializeObject(data);
        }

        public string getAllRuleDistribution()
        {
            var data = GetData.GetAllRuleDistribution();
            return JsonConvert.SerializeObject(data);
        }

        public string getAllEmployeeType()
        {
            var data = GetData.getAllEmployeeType();
            return JsonConvert.SerializeObject(data);
        }

        public string getAllDepartment()
        {
            var data = GetData.getAllDepartment();
            return JsonConvert.SerializeObject(data);
        }

        public void removeRuleDistributionItem(int Id)
        {
            GetData.RemoveRuleDistributionItem(Id);
        }
       
        public void removeRuleDistributionDevice(int id)
        {
            GetData.RemoveRuleDistributionDevice(id);
        }

        public void setAccessRuleForRuleDistribution(int distributionId, int accessRuleId)
        {
            GetData.SetAccessRuleToDistribution(distributionId, accessRuleId);
        }

        public string addEmployeeTypeDistribution(string name)
        {
            var data = GetData.AddRuleDistribution(name, DistributionItemType.EmployeeType);
            return JsonConvert.SerializeObject(data);
        }

        public string addDepartmentDistribution(string name)
        {
            var data = GetData.AddRuleDistribution(name, DistributionItemType.Department);
            return JsonConvert.SerializeObject(data);
        }

        public string addStaffDistribution(string name)
        {
            var data = GetData.AddRuleDistribution(name, DistributionItemType.Staff);
            return JsonConvert.SerializeObject(data);
        }

        public void removeDistribution(int id)
        {
            GetData.RemoveDistribution(id);
        }

        public string addGroupIdToDistribution(int distId, int groupId, GroupIdType groupIdType)
        {
            var data = GetData.AddGroupToRuleDistribution(distId, groupId, groupIdType);
            return JsonConvert.SerializeObject(data);

        }

        public string addStaffIdToDistribution(int distributionId, string staffId)
        {
            var data = GetData.AddStaffToRuleDistribution(distributionId, staffId);
            return JsonConvert.SerializeObject(data);
        }

        public string getStaffByNameFuzzy(string query)
        {
            var data = GetData.GetStaffByNameFuzzy(query);
            return JsonConvert.SerializeObject(data);
        }

        public string addDeviceIdToDistribution(int distId, int deviceId)
        {
            var data = GetData.AddDeviceToRuleDistribution(distId, deviceId);
            return JsonConvert.SerializeObject(data);
        }

        public void buildRuleDeploymentTask()
        {
            AccessRuleDeployManager.Instance.AddDeployTaskAsync();
        }

        public string getAllAccessRuleDeployTasks()
        {
            var data = AccessRuleDeployManager.Instance.GetAllTasks();
            return JsonConvert.SerializeObject(data);
        }

        public bool canAddAccessControlDeployTask()
        {
            return AccessRuleDeployManager.Instance.CanAddTask;
        }

        public void  removeAccessControlDeployTask(int id)
        {
            AccessRuleDeployManager.Instance.removeTask(id);
        }

        public void setDefaultAccess(Access access)
        {
            ChromiumForm.userSettings.DefaultAccess = access;
            AccessRuleDeployManager.Instance.DefaultAccess = access;
            Services.Tracker.Persist(ChromiumForm.userSettings);
        }
    }

    class KeyboardHandler : IKeyboardHandler
    {
        /// <inheritdoc/>>
        public bool OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            if (KeyType.RawKeyDown == type)
            {
                if (windowsKeyCode == (int)Keys.F12)
                    browser.ShowDevTools();
                else if (windowsKeyCode == (int)Keys.F5)
                    browser.Reload();
            }


            return false;
        }

        /// <inheritdoc/>>
        public bool OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            var result = false;
            Debug.WriteLine("OnKeyEvent: KeyType: {0} 0x{1:X} Modifiers: {2}", type, windowsKeyCode, modifiers);
            // TODO: Handle MessageNeeded cases here somehow.
            return result;
        }
    }

    class MenuHandler : IContextMenuHandler
    {
        private const int ShowDevTools = 26501;
        private const int CloseDevTools = 26502;
        private const int RefreshTools = 26503;
        private const int ShowAddressBar = 26504;

        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            //string s = browser.FocusedFrame.Name;
            //string Url = browser.FocusedFrame.Url;
            //if (browser != null && browser.FocusedFrame != null && browser.FocusedFrame.Url != null && browser.FocusedFrame.Url.Contains("insCompanyEntryList.action"))
            //{
            //    model.Clear();
            //}

            //只有当选择文字的时候才会弹出菜单
            if (string.IsNullOrEmpty(parameters.SelectionText))
            {
                model.Clear();
            }
            if (!string.IsNullOrEmpty(parameters.SourceUrl))
            {
                model.AddItem(CefMenuCommand.Copy, "复制图片");
            }

            //   model.AddItem((CefMenuCommand)ShowDevTools, "复制图片");

            //To disable the menu then call clear
            // model.Clear();
            //Removing existing menu item
            //bool removed = model.Remove(CefMenuCommand.ViewSource); // Remove "View Source" option

            //Add new custom menu items
            //model.AddItem((CefMenuCommand)ShowDevTools, "Show DevTools");
            //model.AddItem((CefMenuCommand)CloseDevTools, "Close DevTools");

            ////测试环境添加调试, 生产则禁用右键菜单(文本框除外)
            //if (MyApplication.CustomizeInfo.IsTest)
            //{
            //    model.AddItem((CefMenuCommand)ShowDevTools, "调试");
            //    //model.AddItem((CefMenuCommand)CloseDevTools, "关闭调试");
            //    model.AddItem((CefMenuCommand)RefreshTools, "刷新");
            //    model.AddItem((CefMenuCommand)ShowAddressBar, "地址栏");
            //    if (parameters.TypeFlags.HasFlag(ContextMenuType.Media))
            //        model.Clear();
            //}
            //else
            //{
            //    if (!parameters.TypeFlags.HasFlag(ContextMenuType.Editable)
            //    && string.IsNullOrEmpty(parameters.SelectionText))
            //        model.Clear();
            //}

            //只有当选择文字的时候才会弹出菜单
            //if (!parameters.TypeFlags.HasFlag(ContextMenuType.Editable)
            //    && string.IsNullOrEmpty(parameters.SelectionText))
            //    model.Clear();
            //else
            //{
            //    //model.Clear();
            //    model.AddItem((CefMenuCommand)ShowDevTools, "调试");
            //    //model.AddItem((CefMenuCommand)CloseDevTools, "关闭调试");
            //    model.AddItem((CefMenuCommand)RefreshTools, "刷新");
            //    if (MyApplication.CustomizeInfo.IsTest)
            //        model.AddItem((CefMenuCommand)ShowAddressBar, "地址栏");
            //}

        }

        bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            return false;
        }

        private Image ImageFromWeb(String url)
        {
            if (url != null && url != "")
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Image img = Image.FromStream(response.GetResponseStream());
                return img;
            }
            else
            {
                return null;
            }
        }

        void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {

            //绘制了一遍菜单栏  所以初始化的时候不必绘制菜单栏，再此处绘制即可
            var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;

            //chromiumWebBrowser.Invoke(new Action() =>
            //{
            //    var menu = new ContextMenu
            //    {

            //    };

            //    //R handler = null;

            //    //handler = (s, e) =>
            //    //{
            //    //    menu.Closed -= handler;

            //    //    //If the callback has been disposed then it's already been executed
            //    //    //so don't call Cancel
            //    //    if (!callback.IsDisposed)
            //    //    {
            //    //        callback.Cancel();
            //    //    }
            //    //};

            //    //menu.Closed += handler;

            //    //menu.Items.Add(new MenuItem
            //    //{
            //    //    Header = "最小化",
            //    //    Command = new CustomCommand(MinWindow)
            //    //});
            //    //menu.Items.Add(new MenuItem
            //    //{
            //    //    Header = "关闭",
            //    //    Command = new CustomCommand(CloseWindow)
            //    //});
            //    chromiumWebBrowser.ContextMenu = menu;

            //});
            //chromiumWebBrowser.Invoke(new Action(() =>
            //{
            //    var menu = new ContextMenu
            //    {
            //        Name = "菜单"
            //    };
            //    //var menu = chromiumWebBrowser.ContextMenu;
            //    menu.MenuItems.Add(new MenuItem
            //    {
            //        Name = "sada"
            //    });

            //    chromiumWebBrowser.ContextMenu= menu;
            //}));
            return false;
        }
    }

    class LifeSpanHandler : ILifeSpanHandler
    {
        ChromiumForm chromiumForm = null;
        public LifeSpanHandler(ChromiumForm chromiumForm)
        {
            this.chromiumForm = chromiumForm;
        }

        bool ILifeSpanHandler.OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {

            var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;
            ChromiumForm chromiumForm = null;  //承接ChromiumWebBrowser的窗体
            chromiumWebBrowser.Invoke(new Action(() =>
            {
                chromiumForm = new ChromiumForm() { Text = "加载中..." };
                chromiumForm.WebBrowser = new ChromiumWebBrowser("about:blank");
                chromiumForm.WebBrowser.SetAsPopup();
                ChromiumForm owner = chromiumWebBrowser.FindForm() as ChromiumForm;
                owner.MdiForm.AddTableForm(chromiumForm);
            }));

            newBrowser = chromiumForm.WebBrowser;

            return false;
        }

        void ILifeSpanHandler.OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {

        }

        bool ILifeSpanHandler.DoClose(IWebBrowser browserControl, IBrowser browser)
        {
            new Thread(() =>
            {
                Thread.Sleep(500);
                chromiumForm.BeginInvoke(new Action(() =>
                {
                    if (!chromiumForm.IsDisposed && !chromiumForm.Disposing)
                    {
                        chromiumForm.Close();
                    }
                }));
            })
            {
                IsBackground = true
            }.Start();

            return true;
        }

        public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {

        }
    }

    public class RequestHandler : IRequestHandler
    {
        public bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            callback.Dispose();
            return false;
        }

        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            throw new NotImplementedException();
        }

        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            throw new NotImplementedException();
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return null;
        }


        public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect)
        {

            //var m = request.Method;
            //if (request.Method == "POST")
            //{
            //    using (var postData = request.PostData)
            //    {
            //        if (postData != null)
            //        {
            //            var elements = postData.Elements;

            //            var charSet = request.GetCharSet();

            //            foreach (var element in elements)
            //            {
            //                if (element.Type == PostDataElementType.Bytes)
            //                {
            //                    var body = element.GetBody(charSet);
            //                }
            //            }
            //        }
            //    }
            //}

            return false;
        }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            return true;
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            if (request.Url.Contains("checkMacAddress") && request.Method == "POST")
            {
                using (var postData = request.PostData)
                {
                    if (postData != null)
                    {
                        var elements = postData.Elements;
                        foreach (var element in elements)
                        {
                            if (element.Type == PostDataElementType.Bytes)
                            {
                                var body = System.Text.Encoding.Default.GetString(element.Bytes);
                                if (body.Contains("macAddress"))
                                {
                                    element.Bytes = System.Text.Encoding.Default.GetBytes("_");
                                }
                            }
                        }
                    }
                }
            }

            return CefReturnValue.Continue;
        }

        public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            return false;
        }

        public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            throw new NotImplementedException();
        }

        public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
        {

        }

        public bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
        {
            return false;
        }

        public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            return false;
        }

        public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
        {

        }

        public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
        {

        }

        public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {

        }

        public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, ref string newUrl)
        {

        }

        public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
        {

        }

        public bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return false; ;

        }

        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            throw new NotImplementedException();
        }
    }
}
namespace InsuranceBrowser.CefHanderCommon
{
    class DownloadHandler : IDownloadHandler
    {
        public void OnBeforeDownload(IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            callback.Continue(string.Empty, true);
        }

        public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            throw new NotImplementedException();
        }

        public void OnDownloadUpdated(IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            if (downloadItem.IsComplete)
            {
                //MessageBox.Show("下载成功");
                //System.Diagnostics.Process.Start("explorer.exe", downloadItem.FullPath.Replace(System.IO.Path.GetFileName(downloadItem.FullPath),""));
                string fileToSelect = downloadItem.FullPath;
                string args = string.Format("/Select, {0}", fileToSelect);

                ProcessStartInfo pfi = new ProcessStartInfo("Explorer.exe", args);
                System.Diagnostics.Process.Start(pfi);
            }
        }

        public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            throw new NotImplementedException();
        }
    }

    class CookieMonster : ICookieVisitor
    {
        // readonly List<Tuple<string, string>> cookies = new List<Tuple<string, string>>();
        readonly ManualResetEvent getAllCookies = new ManualResetEvent(false);
        public string Cookies = "";

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Visit(CefSharp.Cookie cookie, int count, int total, ref bool deleteCookie)
        {
            //cookies.Add(new Tuple<string, string>(cookie.Name, cookie.Value));
            Cookies += cookie.Name + "=" + cookie.Value + ";";

            if (count == total - 1)
                getAllCookies.Set();

            return true;
        }

        public void WaitForAllCookies()
        {
            getAllCookies.WaitOne(1000);  //等待5秒
        }

        //public IEnumerable<Tuple<string, string>> NamesValues
        //{
        //    get { return cookies; }
        //}


        /*
        var visitor = new CookieMonster();
            if (Cef.GetGlobalCookieManager().VisitAllCookies(visitor))
                visitor.WaitForAllCookies();
            var sb = new StringBuilder();
            foreach (var nameValue in visitor.NamesValues)
                sb.AppendLine(nameValue.Item1 + " = " + nameValue.Item2);

        */
    }
}

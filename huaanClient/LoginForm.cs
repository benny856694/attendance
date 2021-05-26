using CSharp_SQLite;
using huaanClient.DatabaseTool;
using InsuranceBrowser;
using InsuranceBrowserLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ZXCL.WinFormUI;

namespace huaanClient
{
    public partial class LoginForm : ZForm
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
    (
        int nLeftRect, // x-coordinate of upper-left corner
        int nTopRect, // y-coordinate of upper-left corner
        int nRightRect, // x-coordinate of lower-right corner
        int nBottomRect, // y-coordinate of lower-right corner
        int nWidthEllipse, // height of ellipse
        int nHeightEllipse // width of ellipse
     );

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        private bool m_aeroEnabled;                     // variables for box shadow
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;

        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public struct MARGINS                           // struct for box shadow
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        private const int WM_NCHITTEST = 0x84;          // variables for dragging the form
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();

                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW;

                return cp;
            }
        }

        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0;
                DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCPAINT:                        // box shadow
                    if (m_aeroEnabled)
                    {
                        var v = 2;
                        DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
                        MARGINS margins = new MARGINS()
                        {
                            bottomHeight = 1,
                            leftWidth = 1,
                            rightWidth = 1,
                            topHeight = 1
                        };
                        DwmExtendFrameIntoClientArea(this.Handle, ref margins);

                    }
                    break;
                default:
                    break;
            }
            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)     // drag the form
                m.Result = (IntPtr)HTCAPTION;

        }
        public LoginForm()
        {
            InitializeComponent();
            SetWindowRegion();
        }

        private void pbClose_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void pbClose_MouseEnter(object sender, EventArgs e)
        {
            pbClose.BackgroundImage = Properties.Resources.sysbtn_close_hover;
            toolTip1.SetToolTip(pbClose, "关闭");
        }

        private void pbClose_MouseLeave(object sender, EventArgs e)
        {
            pbClose.BackgroundImage = Properties.Resources.sysbtn_close_normal;
        }

        private void pbClose_MouseDown(object sender, MouseEventArgs e)
        {
            pbClose.BackgroundImage = Properties.Resources.sysbtn_close_down;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            ApplicationData.IsTest = ApplicationData.istest;
            ApplicationData.DefaultLanguage = ApplicationData.defaultLanguage;
            try
            {
                Language_Selection1.SelectedIndex = 2;
                //获取地区自动显示中英日文
                string ss = System.Globalization.CultureInfo.InstalledUICulture.Name;
                if (ss.Contains("zh-CN"))
                {
                    Language_Selection1.SelectedIndex = 0;
                    pictureBox1.Visible = false;
                    this.CaptionTextColor = Color.Black;
                    this.CaptionColorEnd = Color.WhiteSmoke;
                    this.CaptionColorStart = Color.WhiteSmoke;
                }
                else if (ss.Contains("ja-JP"))
                {
                    pictureBox1.Visible = true;
                    this.CaptionColorEnd = Color.WhiteSmoke;
                    this.CaptionColorStart = Color.WhiteSmoke;
                    Language_Selection1.SelectedIndex = 1;
                }
                else
                {
                    pictureBox1.Visible = false;
                    this.CaptionTextColor = Color.Black;
                    this.CaptionColorEnd = Color.WhiteSmoke;
                    this.CaptionColorStart = Color.WhiteSmoke;
                    Language_Selection1.SelectedIndex = 2;
                }

                Language_Selection1.SelectedIndex = int.Parse(ConfigurationManager.AppSettings["select"]);
                tbusername.Text = ConfigurationManager.AppSettings["name"];
                tbPassword.Text = ConfigurationManager.AppSettings["password"];

            }
            catch
            {
                Language_Selection1.SelectedIndex = 0;
            }



            this.AcceptButton = btnLogin;
            //string dbPath = @"d:\NBA.db3";
            //CSQLiteHelper.NewDbFile("huaanDatabase.sqlite");
            ////创建一个数据库db文件
            //CSQLiteHelper.NewDbFile(dbPath);
            ////创建一个表
            //string tableName = "user";
            //CSQLiteHelper.NewTable(dbPath, tableName);
            AddStars();
        }
        private static void AddStars()
        {
            //SQLiteConnection conn = new SQLiteConnection("Data Source=huaanDatabase.sqlite;Version=3;");
            //conn.Open();
            //string query = "insert into user (username,password) values('admin','123456')";
            //SQLiteCommand cmd = new SQLiteCommand(query, conn);
            //cmd.ExecuteNonQuery();
            //conn.Close();
            //cmd.Dispose();
        }
        public void SetWindowRegion()
        {

            System.Drawing.Drawing2D.GraphicsPath FormPath;

            FormPath = new System.Drawing.Drawing2D.GraphicsPath();

            Rectangle rect = new Rectangle(-1, -1, this.Width + 1, this.Height);

            FormPath = GetRoundedRectPath(rect, 10);

            this.Region = new Region(FormPath);

        }
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            int diameter = radius;

            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

            GraphicsPath path = new GraphicsPath();

            //   左上角 
            path.AddArc(arcRect, 185, 90);

            //   右上角 
            arcRect.X = rect.Right - diameter;

            path.AddArc(arcRect, 275, 90);

            //   右下角 
            arcRect.Y = rect.Bottom - diameter;

            path.AddArc(arcRect, 356, 90);

            //   左下角 
            arcRect.X = rect.Left;

            arcRect.Width += 2;

            arcRect.Height += 2;

            path.AddArc(arcRect, 90, 90);

            path.CloseFigure();

            return path;
        }
        public static bool DriverExists(string DriverName)
        {
            return System.IO.Directory.GetLogicalDrives().Contains(DriverName);
        }
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                //先判断是否存在
                bool bc = DriverExists(@"D:\");
                if (bc)
                {
                    //先创建基础文件夹
                    var imgPath = "D:\\FaceRASystemTool";
                    if (!Directory.Exists(imgPath))
                    {
                        Directory.CreateDirectory(imgPath);
                    }

                    ApplicationData.FaceRASystemToolUrl = "D:\\FaceRASystemTool";
                }
                //D盘不存在 直接创建到C盘
                else
                {
                    //先创建基础文件夹
                    var imgPath = "C:\\FaceRASystemTool";
                    if (!Directory.Exists(imgPath))
                    {
                        Directory.CreateDirectory(imgPath);
                    }

                    ApplicationData.FaceRASystemToolUrl = "C:\\FaceRASystemTool";
                }
            }
            catch
            {
                MessageBox.Show("Error, Please contact customer service");
                return;
            }


            bool isZn;
            if (Language_Selection1.SelectedIndex == 0)
            {
                isZn = true;
            }
            else
            {
                isZn = false;
            }

            if (isZn)
            {
                lbStatus.Text = "正在初始化数据库";
            }
            else
            {
                lbStatus.Text = "Initializing database";
            }
            try
            {
                //初始化数据库
                Logger.Debug($"begin init database");
               var suc = await AddDataTtables.addData();
                if (!suc)
                {
                    Logger.Debug($"init database failed");
                    if (isZn)
                    {
                        MessageBox.Show(lbStatus.Text = "数据库初始化失败。");
                        return;
                    }
                    else
                    {
                        MessageBox.Show(lbStatus.Text = "Database initialization failed。");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Initialize database");

                if (isZn)
                {
                    MessageBox.Show(lbStatus.Text = "数据库初始化失败。");
                    return;
                }
                else
                {
                    MessageBox.Show(lbStatus.Text = "Database initialization failed。");
                    return;
                }
            }


            //登录检测
            try
            {
                if (isZn)
                {
                    lbStatus.Text = "正在登陆。";
                }
                else
                {
                    lbStatus.Text = "Landing in progress.";
                }
                //Application.StartupPath Directory.GetCurrentDirectory()
                string dbPath = ApplicationData.connectionString;
                using (SQLiteConnection conn = new SQLiteConnection(dbPath))
                {
                    //查询是否有相应的数据
                    string uname = tbusername.Text.Trim();
                    string upwd = tbPassword.Text.Trim();
                    if (string.IsNullOrEmpty(uname))
                    {
                        if (isZn)
                        {
                            lbStatus.Text = "请输入账号";
                        }
                        else
                        {
                            lbStatus.Text = "Please enter your account number";
                        }
                        return;
                    }
                    if (string.IsNullOrEmpty(upwd))
                    {
                        if (isZn)
                        {
                            lbStatus.Text = "请输入密码";
                        }
                        else
                        {
                            lbStatus.Text = "Please input a password";
                        }
                        return;
                    }
                    string sql_select = "select count(0) from user where username =@uname and password =@upwd ";
                    //连接数据库
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SQLiteCommand cmd = new SQLiteCommand(sql_select, conn);
                    //cmd.Parameters.Add(new SQLiteParameter("@uname", uname));
                    //cmd.Parameters.Add(new SQLiteParameter("@upwd", upwd));
                    SQLiteParameter[] paras = new SQLiteParameter[] { new SQLiteParameter("@uname", uname), new SQLiteParameter("@upwd", upwd) };
                    cmd.Parameters.AddRange(paras);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        int result = Convert.ToInt16(reader.GetValue(0));
                        if (result == 0)
                        {
                            if (isZn)
                            {
                                lbStatus.Text = "用户名或者密码错误，请重新输入";
                            }
                            else
                            {
                                lbStatus.Text = "Wrong user name or password, please re-enter";
                            }
                        }
                        else if (result == 1)
                        {
                            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                            //Language_Selection1.SelectedIndex
                            cfa.AppSettings.Settings["select"].Value = Language_Selection1.SelectedIndex.ToString();
                            cfa.AppSettings.Settings["name"].Value = uname;
                            cfa.AppSettings.Settings["password"].Value = upwd;
                            cfa.Save();

                            ApplicationData.LanguageSign = Language_Selection1.Text;
                            if (isZn)
                            {
                                lbStatus.Text = "登录成功";
                            }
                            else
                            {
                                lbStatus.Text = "Login successfu";
                            }
                            this.Close();
                        }
                        else
                        {
                            if (isZn)
                            {
                                lbStatus.Text = "登录失败";
                            }
                            else
                            {
                                lbStatus.Text = "Login failed, please contact the customer";
                            }

                        }
                    }
                    //关闭数据库连接
                    //reader.Close();
                    //conn.Close();
                }
            }
            catch (Exception ex)
            {
                lbStatus.Text = "";
                if (isZn)
                {
                    MessageBox.Show("连接数据库失败：" + ex.Message);
                }
                else
                {
                    MessageBox.Show("Failed to connect to database：" + ex.Message);
                }
            }
        }

        private void zComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Language_Selection1.SelectedIndex == 0)
            {
                tbusername.WaterText = "账号";
                tbPassword.WaterText = "密码";
                btnLogin.Text = "登  录";
                this.CaptionLeftSpacing = 55;
                this.Text = "智慧人脸考勤门禁系统";

                this.CaptionFont = new Font("微软雅黑", 22, FontStyle.Bold);
                this.CaptionTextColor = Color.Black;
                pictureBox1.Visible = false;
                this.CaptionColorEnd = Color.WhiteSmoke;
                this.CaptionColorStart = Color.WhiteSmoke;
            }
            else if (Language_Selection1.SelectedIndex == 1)
            {
                tbusername.WaterText = "Account Number";
                tbPassword.WaterText = "Password";
                btnLogin.Text = "Login";
                this.CaptionTextColor = Color.Black;
                this.CaptionLeftSpacing = 55;
                this.Text = " Face Recognition System";

                this.CaptionFont = new Font("微软雅黑", 17, FontStyle.Bold);
                pictureBox1.Visible = false;
                this.CaptionColorEnd = Color.WhiteSmoke;
                this.CaptionColorStart = Color.WhiteSmoke;
            }
            else if (Language_Selection1.SelectedIndex == 2)
            {
                tbusername.WaterText = "アカウント";
                tbPassword.WaterText = "パスワード";
                btnLogin.Text = "ログイン";
                this.CaptionLeftSpacing = 115;
                this.Text = "管理画面ログイン";
                this.CaptionTextColor = Color.WhiteSmoke;
                pictureBox1.Visible = true;
                this.CaptionColorEnd = Color.WhiteSmoke;
                this.CaptionColorStart = Color.WhiteSmoke;
            }
        }
    }
}

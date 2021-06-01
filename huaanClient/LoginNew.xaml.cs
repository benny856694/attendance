using huaanClient.DatabaseTool;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace huaanClient
{
    /// <summary>
    /// LoginNew.xaml 的交互逻辑
    /// </summary>
    public partial class LoginNew : Window
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public LoginNew()
        {
            InitializeComponent();
            Dictionary<int, string> mydic = new Dictionary<int, string>() { { 0, "中文" }, { 1, "English" }, { 2, "日本語" }, { 3, "French" } };
            Language_Selection1.ItemsSource = mydic;
            Language_Selection1.SelectedValuePath = "Key";
            Language_Selection1.DisplayMemberPath = "Value";
        }

        public static bool DriverExists(string DriverName)
        {
            return System.IO.Directory.GetLogicalDrives().Contains(DriverName);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoginForm();
        }

        public  async void LoginForm()
        {
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
                changeLable("正在初始化数据库");
            }
            else
            {
                changeLable("Initializing database");
            }


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
            catch (Exception ex)
            {
                Logger.Error(ex, "Init error");
                MessageBox.Show($"Error:({ex.Message})\r\nPlease contact customer service");
                return;
            }


            try
            {
                //初始化数据库
                Task<bool> task = AddDataTtables.addData();
                bool count = await task;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "init db error");
                if (isZn)
                {
                    changeLable("数据库初始化失败。");
                    return;
                }
                else
                {
                    changeLable("Database initialization failed");
                    return;
                }
            }

            //登录检测
            try
            {
                if (isZn)
                {
                    changeLable("正在登陆....");
                }
                else
                {
                    changeLable("Landing in progress");
                }
                //Application.StartupPath Directory.GetCurrentDirectory()
                string dbPath = ApplicationData.connectionString;
                using (SQLiteConnection conn = new SQLiteConnection(dbPath))
                {
                    //查询是否有相应的数据
                    string uname = username.Text.Trim();
                    string upwd = password.Password.Trim();
                    if (string.IsNullOrEmpty(uname))
                    {
                        if (isZn)
                        {
                            changeLable("请输入账号");
                        }
                        else
                        {
                            changeLable("Please enter your account number");
                        }
                        return;
                    }
                    if (string.IsNullOrEmpty(upwd))
                    {
                        if (isZn)
                        {
                            changeLable("请输入密码");
                        }
                        else
                        {
                            changeLable("Please input a password");
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
                                changeLable("用户名或者密码错误，请重新输入");
                            }
                            else
                            {
                                changeLable("Wrong user name or password, please re-enter");
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
                                changeLable("登录成功");

                            }
                            else
                            {
                                changeLable("Login successfu");
                            }
                            this.Close();
                        }
                        else
                        {
                            if (isZn)
                            {
                                changeLable("登录失败");
                            }
                            else
                            {
                                changeLable("Login failed, please contact the customer");
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
                lbStatus.Content = "";
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
        public  void changeLable(string value)
        {
            this.Dispatcher.BeginInvoke(new Action(() => {
                lbStatus.Content = value;
            }));

            //this.Dispatcher.BeginInvoke((Action)delegate ()
            //{
            //    lbStatus.Content = value;
            //});
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationData.DefaultLanguage = ApplicationData.defaultLanguage;
            try
            {
                Language_Selection1.SelectedIndex = 2;
                //获取地区自动显示中英日文
                string ss = System.Globalization.CultureInfo.InstalledUICulture.Name;
                if (ss.Contains("zh-CN"))
                {
                    jPlogo.Visibility = Visibility.Collapsed;
                    title.Visibility = Visibility.Visible;
                    title.Margin = new Thickness(97, 52, 95, 0);
                    Language_Selection1.SelectedIndex = 0;
                }
                else if (ss.Contains("ja-JP"))
                {
                    jPlogo.Visibility = Visibility.Visible;
                    title.Visibility = Visibility.Collapsed;
                    jPlogo.Margin = new Thickness(124, 52, 95, 0);
                    Language_Selection1.SelectedIndex = 1;
                }
                else
                {
                    title.Visibility = Visibility.Visible;
                    jPlogo.Visibility = Visibility.Collapsed;
                    title.Margin = new Thickness(97, 52, 95, 0);
                    Language_Selection1.SelectedIndex = 2;
                }

                Language_Selection1.SelectedIndex = int.Parse(ConfigurationManager.AppSettings["select"]);
                username.Text = ConfigurationManager.AppSettings["name"];
                password.Password = ConfigurationManager.AppSettings["password"];

            }
            catch
            {
                Language_Selection1.SelectedIndex = 0;
            }



            //this.AcceptButton = btnLogin;
            //string dbPath = @"d:\NBA.db3";
            //CSQLiteHelper.NewDbFile("huaanDatabase.sqlite");
            ////创建一个数据库db文件
            //CSQLiteHelper.NewDbFile(dbPath);
            ////创建一个表
            //string tableName = "user";
            //CSQLiteHelper.NewTable(dbPath, tableName);
        }

        private void Language_Selection1_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }


        private void Language_Selection1_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (Language_Selection1.SelectedIndex == 0)
            {
                usernamelable.Content = "账号";
                passwordlable.Content = "密码";
                login.Content = "登 录";
                title.Content = "智慧人脸考勤门禁系统";

                jPlogo.Visibility = Visibility.Collapsed;
                title.Visibility = Visibility.Visible;
                title.Margin = new Thickness(97, 52, 95, 0);
            }
            else if (Language_Selection1.SelectedIndex == 1)
            {
                usernamelable.Content = "Account Number";
                passwordlable.Content = "Password";
                login.Content = "Login";
                title.Content = "Face Recognition System";

                title.Visibility = Visibility.Visible;
                jPlogo.Visibility = Visibility.Collapsed;
                title.Margin = new Thickness(97, 52, 95, 0);
            }
            else if (Language_Selection1.SelectedIndex == 2)
            {
                usernamelable.Content = "アカウント";
                passwordlable.Content = "パスワード";
                login.Content = "ログイン";
                title.Content = "管理画面ログイン";

                jPlogo.Visibility = Visibility.Visible;
                title.Visibility = Visibility.Collapsed;
                jPlogo.Margin = new Thickness(124, 52, 95, 0);
            }
            else if (Language_Selection1.SelectedIndex == 3)
            {
                usernamelable.Content = "Numéro de compte";
                passwordlable.Content = "Mot de passe";
                login.Content = "Connexion";
                title.Content = "Système de contrôle d'accès facial";

                title.Visibility = Visibility.Visible;
                jPlogo.Visibility = Visibility.Collapsed;
                title.Margin = new Thickness(10,52,10,0);
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            colseBtn.Source = new BitmapImage(new Uri("pack://application:,,,/close_hover.png"));
        }

        private void colseBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            colseBtn.Source = new BitmapImage(new Uri("pack://application:,,,/close_norm.png"));
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}

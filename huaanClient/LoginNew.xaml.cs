using Dapper;
using DBUtility.SQLite;
using huaanClient.DatabaseTool;
using huaanClient.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

        private double? height;


        IList<Control> _heightControls = new List<Control>();

        private bool ShowChangePassword
        {
            set
            {
                var visible = value ? Visibility.Visible : Visibility.Collapsed;
                NewPasswordlable.Visibility = visible;
                NewPassword.Visibility = visible;
                ConfirmNewPasswordlable.Visibility = visible;
                confirmNewPassword.Visibility = visible;
                if (!height.HasValue)
                {
                    height = 0;
                    foreach (var item in _heightControls)
                    {
                        height += item.ActualHeight + item.Margin.Top + item.Margin.Bottom;
                    }
                }

                this.Height += value ? height.Value : -height.Value;
            }
        }

        public LoginNew()
        {
            InitializeComponent();
            _heightControls.Add(NewPasswordlable);
            _heightControls.Add(NewPassword);
            _heightControls.Add(ConfirmNewPasswordlable);
            _heightControls.Add(confirmNewPassword);

            Dictionary<int, string> mydic = new Dictionary<int, string>() {
                { 0, Constants.LANG_NAME_CHINESE }, 
                { 1, Constants.LANG_NAME_ENGLISH }, 
                { 2, Constants.LANG_NAME_JAPANESE }, 
                { 3, Constants.LANG_NAME_FRENCH }, 
                { 4, Constants.LANG_NAME_VIETNAMESE } 
            };
            Language_Selection1.ItemsSource = mydic;
            Language_Selection1.SelectedValuePath = "Value";
            Language_Selection1.DisplayMemberPath = "Value";
        }

        public static bool DriverExists(string DriverName)
        {
            return System.IO.Directory.GetLogicalDrives().Contains(DriverName);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                changePassword.IsEnabled = false;
                NewPassword.IsEnabled = false;
                var valid = ValidateInput();
                if (valid)
                {
                    Login();
                }
            }
            finally
            {
                changePassword.IsEnabled = true;
                NewPassword.IsEnabled = true;
            }
            
        }

        private bool ValidateInput()
        {
            if (changePassword.IsChecked == true)
            {
                if (string.IsNullOrEmpty(NewPassword.Password))
                {
                    ShowStatus(Strings.PasswordEmpty);
                    return false;
                }

                if(NewPassword.Password.Contains(" "))
                {
                    ShowStatus(Strings.PasswordSpace);
                    return false;
                }

                if (string.IsNullOrEmpty(confirmNewPassword.Password))
                {
                    ShowStatus(Strings.PasswordEmpty);
                    return false;
                }

                if (confirmNewPassword.Password.Contains(""))
                {
                    ShowStatus(Strings.PasswordSpace);
                    return false;
                }

                if (!string.Equals(NewPassword.Password, confirmNewPassword.Password, StringComparison.InvariantCulture))
                {
                    ShowStatus(Strings.ConfirmNewPasswordMismatch);
                    return false;
                }

                if (string.Equals(NewPassword.Password, password.Password, StringComparison.InvariantCulture))
                {
                    ShowStatus(Strings.NewPasswordCanotBeSameAsOld);
                    return false;
                }
            }

            return true;
        }

        public  async void Login()
        {

            ShowStatus(Strings.InitializingDb);


            try
            {
                //先判断是否存在
                var drives = new List<DriveInfo>() { new DriveInfo("d:"), new DriveInfo("c:") };
                foreach (var d in drives)
                {
                    if (d.IsReady && d.DriveType == DriveType.Fixed)
                    {
                        var path = System.IO.Path.Combine(d.RootDirectory.FullName, "FaceRASystemTool");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        ApplicationData.FaceRASystemToolUrl = path;
                        break;

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Init error");
                MessageBox.Show(string.Format(Strings.ExceptionOccurred, ex.Message));
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
                
                ShowStatus(Strings.InitializeDbFailed);
                return;
                
            }

            //登录检测
            try
            {
                ShowStatus(Strings.LoginInProgress);
                //Application.StartupPath Directory.GetCurrentDirectory()
                string dbPath = ApplicationData.connectionString;
                using (SQLiteConnection conn = new SQLiteConnection(dbPath))
                {
                    //查询是否有相应的数据
                    string uname = username.Text.Trim();
                    string upwd = password.Password.Trim();
                    if (string.IsNullOrEmpty(uname))
                    {
                        ShowStatus(Strings.UsernameEmpty);
                        return;
                    }
                    if (string.IsNullOrEmpty(upwd))
                    {
                        ShowStatus(Strings.PasswordEmpty);
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
                            ShowStatus(Strings.UsernameOrPasswordIncorrect);
                        }
                        else if (result == 1)
                        {
                            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                            cfa.AppSettings.Settings["rememberPassword"].Value = RememberPassword.IsChecked.ToString();
                            cfa.AppSettings.Settings["select"].Value = Language_Selection1.SelectedIndex.ToString();
                            cfa.AppSettings.Settings["name"].Value = uname;
                            if (RememberPassword.IsChecked == true)
                            {
                                cfa.AppSettings.Settings["password"].Value = upwd;
                            }
                            
                            cfa.Save();

                            if (changePassword.IsChecked == true)
                            {
                                conn.Execute($"UPDATE user SET password = '{NewPassword.Password}' WHERE id = 1");

                            }

                            ApplicationData.LanguageSign = Language_Selection1.Text;
                            ShowStatus(Strings.LoginSucceed);
                            this.Close();
                        }
                        else
                        {
                            ShowStatus(Strings.LoginFailed);
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                lbStatus.Content = "";
                MessageBox.Show(string.Format(Strings.ConnectDbFailed, ex.Message));
            }
        }

        private void SetCurrentLanguage()
        {
            CultureInfo culture = null;

            switch (Language_Selection1.SelectedValue)
            {
                case Constants.LANG_NAME_CHINESE: //chinese
                    culture = CultureInfo.GetCultureInfo(Constants.LANG_LOCALE_CHINESE);
                    break;
                case Constants.LANG_NAME_ENGLISH: //english
                    culture = CultureInfo.GetCultureInfo(Constants.LANG_LOCALE_ENGLISH);
                    break;
                case Constants.LANG_NAME_JAPANESE: //japaness
                    culture = CultureInfo.GetCultureInfo(Constants.LANG_LOCALE_JAPANESE);
                    break;
                case Constants.LANG_NAME_FRENCH: //french
                    culture = CultureInfo.GetCultureInfo(Constants.LANG_LOCALE_FRENCH);
                    break;
                case Constants.LANG_NAME_VIETNAMESE: //french
                    culture = CultureInfo.GetCultureInfo(Constants.LANG_LOCALE_VIETNAMESE);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            //Culture for any thread
            CultureInfo.DefaultThreadCurrentCulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            //Culture for UI in any thread
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        public  void ShowStatus(string value)
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
                    //title.Margin = new Thickness(97, 52, 95, 0);
                    Language_Selection1.SelectedIndex = 0;
                }
                else if (ss.Contains("ja-JP"))
                {
                    jPlogo.Visibility = Visibility.Visible;
                    title.Visibility = Visibility.Collapsed;
                    //jPlogo.Margin = new Thickness(124, 52, 95, 0);
                    Language_Selection1.SelectedIndex = 1;
                }
                else
                {
                    title.Visibility = Visibility.Visible;
                    jPlogo.Visibility = Visibility.Collapsed;
                    //title.Margin = new Thickness(97, 52, 95, 0);
                    Language_Selection1.SelectedIndex = 2;
                }

                Language_Selection1.SelectedIndex = int.Parse(ConfigurationManager.AppSettings["select"]);
                username.Text = ConfigurationManager.AppSettings["name"];
                var rememberPassword = string.Equals(ConfigurationManager.AppSettings["rememberPassword"], "true", StringComparison.InvariantCultureIgnoreCase);
                RememberPassword.IsChecked = rememberPassword;
                
                password.Password = rememberPassword ?
                    ConfigurationManager.AppSettings["password"] : "";

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

        


        private void Language_Selection1_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            SetCurrentLanguage();

            ShowStatus("");
            if (Language_Selection1.SelectedIndex == 0)
            {
                usernamelable.Content = "账号";
                passwordlable.Content = "密码";
                login.Content = "登 录";
                title.Content = "智慧人脸考勤门禁系统";

                jPlogo.Visibility = Visibility.Collapsed;
                title.Visibility = Visibility.Visible;
                //title.Margin = new Thickness(97, 52, 95, 0);
            }
            else if (Language_Selection1.SelectedIndex == 1)
            {
                usernamelable.Content = "Account Number";
                passwordlable.Content = "Password";
                login.Content = "Login";
                title.Content = "Face Recognition System";

                title.Visibility = Visibility.Visible;
                jPlogo.Visibility = Visibility.Collapsed;
                //title.Margin = new Thickness(97, 52, 95, 0);
            }
            else if (Language_Selection1.SelectedIndex == 2)
            {
                usernamelable.Content = "アカウント";
                passwordlable.Content = "パスワード";
                login.Content = "ログイン";
                title.Content = "管理画面ログイン";

                jPlogo.Visibility = Visibility.Visible;
                title.Visibility = Visibility.Collapsed;
                //jPlogo.Margin = new Thickness(124, 52, 95, 0);
            }
            else if (Language_Selection1.SelectedIndex == 3)
            {
                usernamelable.Content = "Numéro de compte";
                passwordlable.Content = "Mot de passe";
                login.Content = "Connexion";
                title.Content = "Système de contrôle d'accès facial";

                title.Visibility = Visibility.Visible;
                jPlogo.Visibility = Visibility.Collapsed;
                //title.Margin = new Thickness(10,52,10,0);
            }
            else if (Language_Selection1.SelectedIndex == 4)
            {
                usernamelable.Content = "Tên Người Dùng";
                passwordlable.Content = "Mật Khẩu";
                login.Content = "Đăng Nhập";
                title.Content = "Hệ Thống Nhận Dạng Khuôn Mặt";

                title.Visibility = Visibility.Visible;
                jPlogo.Visibility = Visibility.Collapsed;
                //title.Margin = new Thickness(10, 52, 10, 0);
            }

            changePassword.Content = Strings.ChangePassword;
            NewPasswordlable.Content = Strings.NewPassword;
            ConfirmNewPasswordlable.Content = Strings.ConfirmNewPassword;
            RememberPassword.Content = Strings.RememberPassword;
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

        private void changePassword_Click(object sender, RoutedEventArgs e)
        {
            ShowChangePassword = changePassword.IsChecked == true;
        }
    }
}

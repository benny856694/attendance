using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace huaanClient
{
    class Inihelper
    {
        #region 声明读写配置文件的API函数
        /// <summary>
        /// 写配置文件 www.jbxue.com
        /// </summary>
        /// <param name="section">节(如果该节不存在，则创建它)</param>
        /// <param name="key">键(该键不存在于指定的部分，它被创建。 如果这个参数为NULL，则整节被删除)</param>
        /// <param name="val">值(如果这个参数为NULL,则删除该键)</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>写入成功，则返回非零值。如果函数失败或者只是刷新最近访问的缓冲区版本，返回值为零</returns>
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);

        /// <summary>
        /// 读配置文件
        /// </summary>
        /// <param name="section">节（当值为NULL时，读取全部节）</param>
        /// <param name="key">键（当值为NULL时，读取全部键）</param>
        /// <param name="defaultVal">默认值（键不存在时，返回该值。当值为NULL时，则往缓冲区中写入空字符串""，避免该值为""）</param>
        /// <param name="returnVal">缓冲区，它接收检索到的字符串的指针</param>
        /// <param name="size">缓冲区的大小</param>
        /// <param name="filePath">文件的路径</param>
        /// <returns>复制到缓冲区，不包括终止空字符的字符数</returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defaultVal, StringBuilder returnVal, int size, string filePath);
        #endregion

        #region 成员变量
        private static string fileName; //INI文件名
        #endregion

        #region 属性
        /// <summary>
        /// INI文件名
        /// </summary>
        public static string FileName
        {
            set
            {
                fileName = string.IsNullOrEmpty(value) ? "tool.ini" : value;

                // 判断文件是否存在
                FileInfo fileInfo = new FileInfo(fileName);
                if ((!fileInfo.Exists))
                {
                    //文件不存在，建立文件
                    StreamWriter sw = new StreamWriter(fileName, false, Encoding.Default);
                    try
                    {
                        sw.Close();
                    }
                    catch (Exception)
                    {
                        throw (new ApplicationException("Ini文件不存在"));
                    }
                }

                //必须是完全路径，不能是相对路径
                fileName = fileInfo.FullName;
            }
        }
        #endregion

        #region 构造函数
        private Inihelper()
        {
            fileName = string.Empty;
        }
        #endregion

        #region 限制只能有一个对象
        private static readonly Inihelper instance = new Inihelper();//菜单操作对象
        public static Inihelper Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region 写字符串
        /// <summary>
        /// 写写字符串 www.jbxue.com
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void WriteString(string section, string key, string value)
        {
            try
            {
                WritePrivateProfileString(section, key, value, fileName);
            }
            catch (Exception)
            {
                throw (new Exception("写入Ini文件出错"));
            }
        }
        #endregion

        #region 读字符串
        /// <summary>
        /// 读字符串（最大可以读500个字符）
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="defaultVal">值</param>
        /// <returns>返回指定节指定键的值</returns>
        public static string ReadString(string section, string key, string defaultVal)
        {
            StringBuilder temp = new StringBuilder(500);
            try
            {
                GetPrivateProfileString(section, key, defaultVal, temp, 500, fileName);
            }
            catch (Exception)
            {
                throw (new Exception("读取Ini文件出错"));
            }

            return temp.ToString();
        }
        #endregion

        #region 读整数
        /// <summary>
        /// 读整数
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="defaultVal">默认值</param>
        /// <returns>返回指定节指定键的值</returns>
        public static int ReadInteger(string section, string key, int defaultVal)
        {

            string result = ReadString(section, key, Convert.ToString(defaultVal));
            try
            {
                return Convert.ToInt32(result);
            }
            catch (Exception)
            {
                throw (new Exception("该键的值不是整数！"));
            }
        }
        #endregion

        #region 写整数
        /// <summary>
        /// 写整数
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void WriteInteger(string section, string key, int value)
        {
            WriteString(section, key, value.ToString());
        }
        #endregion

        #region 读布尔
        /// <summary>
        /// 读布尔
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="defaultVal">默认值</param>
        /// <returns></returns>
        public static bool ReadBool(string section, string key, bool defaultVal)
        {
            string result = ReadString(section, key, Convert.ToString(defaultVal));
            try
            {
                return Convert.ToBoolean(result);
            }
            catch (Exception)
            {
                throw (new Exception("该键的值不是布尔值!"));
            }
        }
        #endregion

        #region 写Bool
        /// <summary>
        /// 写Bool
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void WriteBool(string section, string key, bool value)
        {
            WriteString(section, key, Convert.ToString(value));
        }
        #endregion

        #region 删除节
        /// <summary>
        /// 删除节
        /// </summary>k
        /// <param name="section">节</param>
        public void DeleteSection(string section)
        {
            try
            {
                WritePrivateProfileString(section, null, null, fileName);
            }
            catch (Exception)
            {
                throw (new Exception("无法清除Ini文件中的节"));
            }
        }
        #endregion

        #region 删除节下的键
        /// <summary>
        /// 删除节下的键
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        public void DeleteKey(string section, string key)
        {
            WritePrivateProfileString(section, key, null, fileName);
        }
        #endregion

        #region 更新缓冲区
        /// <summary>
        /// 更新缓冲区(1、对于Win9X，需要实现UpdateFile方法将缓冲中的数据写入文件
 //2、在Win NT, 2000和XP上，都是直接写文件，没有缓冲，无须实现UpdateFile3、执行完对Ini文件的修改之后，应该调用本方法更新缓冲区。)
        /// </summary>
        public void UpdateFile()
        {
            WritePrivateProfileString(null, null, null, fileName);
        }
        #endregion

        #region 析构函数
        /// <summary>
        /// 更新缓冲区
        /// </summary>
        ~Inihelper()
        {
            UpdateFile();
        }
        #endregion
    }

    class Inihelper1
    {
        #region 声明读写配置文件的API函数
        /// <summary>
        /// 写配置文件 www.jbxue.com
        /// </summary>
        /// <param name="section">节(如果该节不存在，则创建它)</param>
        /// <param name="key">键(该键不存在于指定的部分，它被创建。 如果这个参数为NULL，则整节被删除)</param>
        /// <param name="val">值(如果这个参数为NULL,则删除该键)</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>写入成功，则返回非零值。如果函数失败或者只是刷新最近访问的缓冲区版本，返回值为零</returns>
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);

        /// <summary>
        /// 读配置文件
        /// </summary>
        /// <param name="section">节（当值为NULL时，读取全部节）</param>
        /// <param name="key">键（当值为NULL时，读取全部键）</param>
        /// <param name="defaultVal">默认值（键不存在时，返回该值。当值为NULL时，则往缓冲区中写入空字符串""，避免该值为""）</param>
        /// <param name="returnVal">缓冲区，它接收检索到的字符串的指针</param>
        /// <param name="size">缓冲区的大小</param>
        /// <param name="filePath">文件的路径</param>
        /// <returns>复制到缓冲区，不包括终止空字符的字符数</returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defaultVal, StringBuilder returnVal, int size, string filePath);
        #endregion

        #region 成员变量
        private static string fileName; //INI文件名
        #endregion

        #region 属性
        /// <summary>
        /// INI文件名
        /// </summary>
        public static string FileName
        {
            set
            {
                fileName = string.IsNullOrEmpty(value) ? "tool.ini" : value;

                // 判断文件是否存在
                FileInfo fileInfo = new FileInfo(fileName);
                if ((!fileInfo.Exists))
                {
                    //文件不存在，建立文件
                    StreamWriter sw = new StreamWriter(fileName, false, Encoding.Default);
                    try
                    {
                        sw.Close();
                    }
                    catch (Exception)
                    {
                        throw (new ApplicationException("Ini文件不存在"));
                    }
                }

                //必须是完全路径，不能是相对路径
                fileName = fileInfo.FullName;
            }
        }
        #endregion

        #region 构造函数
        private Inihelper1()
        {
            fileName = string.Empty;
        }
        #endregion

        #region 限制只能有一个对象
        private static readonly Inihelper1 instance = new Inihelper1();//菜单操作对象
        public static Inihelper1 Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region 写字符串
        /// <summary>
        /// 写写字符串 www.jbxue.com
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void WriteString(string section, string key, string value)
        {
            try
            {
                WritePrivateProfileString(section, key, value, fileName);
            }
            catch (Exception)
            {
                throw (new Exception("写入Ini文件出错"));
            }
        }
        #endregion

        #region 读字符串
        /// <summary>
        /// 读字符串（最大可以读500个字符）
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="defaultVal">值</param>
        /// <returns>返回指定节指定键的值</returns>
        public static string ReadString(string section, string key, string defaultVal)
        {
            StringBuilder temp = new StringBuilder(500);
            try
            {
                GetPrivateProfileString(section, key, defaultVal, temp, 500, fileName);
            }
            catch (Exception)
            {
                throw (new Exception("读取Ini文件出错"));
            }

            return temp.ToString();
        }
        #endregion

        #region 读整数
        /// <summary>
        /// 读整数
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="defaultVal">默认值</param>
        /// <returns>返回指定节指定键的值</returns>
        public static int ReadInteger(string section, string key, int defaultVal)
        {

            string result = ReadString(section, key, Convert.ToString(defaultVal));
            try
            {
                return Convert.ToInt32(result);
            }
            catch (Exception)
            {
                throw (new Exception("该键的值不是整数！"));
            }
        }
        #endregion

        #region 写整数
        /// <summary>
        /// 写整数
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void WriteInteger(string section, string key, int value)
        {
            WriteString(section, key, value.ToString());
        }
        #endregion

        #region 读布尔
        /// <summary>
        /// 读布尔
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="defaultVal">默认值</param>
        /// <returns></returns>
        public static bool ReadBool(string section, string key, bool defaultVal)
        {
            string result = ReadString(section, key, Convert.ToString(defaultVal));
            try
            {
                return Convert.ToBoolean(result);
            }
            catch (Exception)
            {
                throw (new Exception("该键的值不是布尔值!"));
            }
        }
        #endregion

        #region 写Bool
        /// <summary>
        /// 写Bool
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void WriteBool(string section, string key, bool value)
        {
            WriteString(section, key, Convert.ToString(value));
        }
        #endregion

        #region 删除节
        /// <summary>
        /// 删除节
        /// </summary>k
        /// <param name="section">节</param>
        public void DeleteSection(string section)
        {
            try
            {
                WritePrivateProfileString(section, null, null, fileName);
            }
            catch (Exception)
            {
                throw (new Exception("无法清除Ini文件中的节"));
            }
        }
        #endregion

        #region 删除节下的键
        /// <summary>
        /// 删除节下的键
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        public void DeleteKey(string section, string key)
        {
            WritePrivateProfileString(section, key, null, fileName);
        }
        #endregion

        #region 更新缓冲区
        /// <summary>
        /// 更新缓冲区(1、对于Win9X，需要实现UpdateFile方法将缓冲中的数据写入文件
 //2、在Win NT, 2000和XP上，都是直接写文件，没有缓冲，无须实现UpdateFile3、执行完对Ini文件的修改之后，应该调用本方法更新缓冲区。)
        /// </summary>
        public void UpdateFile()
        {
            WritePrivateProfileString(null, null, null, fileName);
        }
        #endregion

        #region 析构函数
        /// <summary>
        /// 更新缓冲区
        /// </summary>
        ~Inihelper1()
        {
            UpdateFile();
        }
        #endregion
    }
}

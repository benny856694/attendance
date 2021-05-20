using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace huaanClient
{
    class FTPHelper
    {
        // <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileinfo">需要上传的文件</param>
        /// <param name="targetDir">目标路径</param>
        /// <param name="hostname">ftp地址</param>
        /// <param name="username">ftp用户名</param>
        /// <param name="password">ftp密码</param>
        public void UploadFileForNow(FileInfo fileinfo, string targetDir, string hostname, string username, string password)
        {
            //1. check target
            string target;
            if (targetDir.Trim() == "")
            {
                return;
            }
            target = Guid.NewGuid().ToString();  //使用临时文件名
            string URI = "FTP://" + hostname + "/" + target;
            FtpWebRequest ftp = GetRequest(URI, username, password);

            //设置FTP命令 设置所要执行的FTP命令，
            ftp.Method = WebRequestMethods.Ftp.UploadFile;
            //指定文件传输的数据类型
            ftp.UseBinary = true;
            ftp.UsePassive = true;

            //告诉ftp文件大小
            ftp.ContentLength = fileinfo.Length;
            //缓冲大小设置为2KB
            const int BufferSize = 2048;
            byte[] content = new byte[BufferSize - 1 + 1];
            int dataRead;
            //打开一个文件流 (System.IO.FileStream) 去读上传的文件
            using (FileStream fs = fileinfo.OpenRead())
            {
                try
                {
                    //把上传的文件写入流
                    using (Stream rs = ftp.GetRequestStream())
                    {
                        do
                        {
                            //每次读文件流的2KB
                            dataRead = fs.Read(content, 0, BufferSize);
                            rs.Write(content, 0, dataRead);
                        } while (!(dataRead < BufferSize));
                        rs.Close();
                    }

                }
                catch (Exception ex) { }
                finally
                {
                    fs.Close();
                }

            }

            ftp = null;
            //设置FTP命令
            ftp = GetRequest(URI, username, password);
            ftp.Method = WebRequestMethods.Ftp.Rename; //改名
            ftp.RenameTo = fileinfo.Name;
            try
            {
                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftp.GetResponse();

            }
            catch (Exception ex)
            {
                ftp = GetRequest(URI, username, password);
                ftp.Method = WebRequestMethods.Ftp.DeleteFile; //删除
                ftp.GetResponse();
                throw ex;
            }
            finally
            {
                fileinfo.Delete();
            }

        }
        private static FtpWebRequest GetRequest(string URI, string username, string password)
        {
            //根据服务器信息FtpWebRequest创建类的对象
            FtpWebRequest result = (FtpWebRequest)FtpWebRequest.Create(URI);
            //提供身份验证信息
            result.Credentials = new NetworkCredential(username, password);
            //设置请求完成之后是否保持到FTP服务器的控制连接，默认值为true
            result.KeepAlive = false;

            return result;
        }

        /// <summary>
        /// 上传文件
        /// 不修改名字及后缀名
        /// </summary>
        /// <param name="originalFilePath">上传文件的绝对路径</param>
        /// <returns></returns>
        public static bool UpFile(string originalFilePath,string ftpPath,string ipaddress,string Username,string Password)
        {
            if (string.IsNullOrEmpty(originalFilePath))
                throw new ArgumentException("参数错误！");
            bool filepathsql = false;
            try
            {
                //检查是否存在此文件
                if (!File.Exists(originalFilePath))
                    throw new Exception("文件不存在！");
                //Stream sr = upfile.PostedFile.InputStream;
                //byte[] file = new byte[sr.Length];
                //sr.Read(file, 0, file.Length);
                // file.SaveAs(HttpContext.Current.Server.MapPath(filePathstr));//把文件上传到服务器的绝对路径上
                bool check;
                string uri = @"ftp://" + ipaddress + "/";
                //检查是否存在此目录文件夹
                if (!string.IsNullOrEmpty(ftpPath))
                {
                    if (CheckDirectoryExist(uri, ftpPath, Username, Password))
                    {
                        //存在此文件夹就直接上传
                        check = Upload(originalFilePath, ipaddress, ftpPath, Username, Password);
                    }
                    else
                    {
                        MakeDir(uri, ftpPath, Username, Password);//创建
                        check = Upload(originalFilePath, ipaddress, ftpPath, Username, Password);
                    }
                }
                else
                {
                    check = Upload(originalFilePath, ipaddress, ftpPath, Username, Password);
                }
                //成功就更新
                if (check)
                {
                    filepathsql = true;
                }
                ////检查是否存在此文件
                //if (File.Exists(originalFilePath))
                //{
                //    File.Delete(originalFilePath);
                //}
                return filepathsql;
            }
            catch (Exception ex)
            {
                //File.Delete(originalFilePath);
                throw ex;
            }
        }
        /// <summary>
        /// FTP上传文件
        /// </summary>
        /// <param name="filename">上传文件路径</param>
        /// <param name="ftpServerIP">FTP服务器的IP和端口</param>
        /// <param name="ftpPath">FTP服务器下的哪个目录</param>
        /// <param name="ftpUserID">FTP用户名</param>
        /// <param name="ftpPassword">FTP密码</param>
        public static bool Upload(string filename, string ftpServerIP, string ftpPath, string ftpUserID, string ftpPassword)
        {
            FileInfo fileInf = new FileInfo(filename);
            string uri = "ftp://" + ftpServerIP + "/" + ftpPath + "/" + fileInf.Name;
            if (string.IsNullOrEmpty(ftpPath))
            {
                uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
            }
            try
            {
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                // ftp用户名和密码
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                // 指定执行什么命令
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                // 指定数据传输类型
                reqFTP.UseBinary = true;
                // 上传文件时通知服务器文件的大小
                reqFTP.ContentLength = fileInf.Length;
                //this.Invoke(InitUProgress, fileInf.Length);
                // 缓冲大小设置为2kb
                int buffLength = 4096;
                byte[] buff = new byte[buffLength];
                int contentLen;
                // 打开一个文件流 (System.IO.FileStream) 去读上传的文件
                FileStream fs = fileInf.OpenRead();
                // 把上传的文件写入流
                Stream strm = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // 关闭两个流
                strm.Close();
                strm.Dispose();
                fs.Close();
                fs.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 新建目录
        /// </summary>
        /// <param name="ftpPath"></param>
        /// <param name="dirName"></param>
        public static void MakeDir(string ftpPath, string dirName, string username, string password)
        {
            try
            {
                //实例化FTP连接
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath + dirName));
                // ftp用户名和密码
                request.Credentials = new NetworkCredential(username, password);
                // 默认为true，连接不会被关闭
                request.KeepAlive = false;
                //指定FTP操作类型为创建目录
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                //获取FTP服务器的响应
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                //Respons
            }
        }
        /// <summary>
        /// 检查目录是否存在
        /// </summary>
        /// <param name="ftpPath">要检查的目录的路径</param>
        /// <param name="dirName">要检查的目录名</param>
        /// <returns>存在返回true，否则false</returns>
        public static bool CheckDirectoryExist(string ftpPath, string dirName, string username, string password)
        {
            bool result = false;
            try
            {
                //实例化FTP连接
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath));
                // ftp用户名和密码
                request.Credentials = new NetworkCredential(username, password);
                request.KeepAlive = false;
                //指定FTP操作类型为创建目录
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                //获取FTP服务器的响应
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
                StringBuilder str = new StringBuilder();
                string line = sr.ReadLine();
                while (line != null)
                {
                    str.Append(line);
                    str.Append("|");
                    line = sr.ReadLine();
                }
                string[] datas = str.ToString().Split('|');
                for (int i = 0; i < datas.Length; i++)
                {
                    if (datas[i].Contains("<DIR>"))
                    {
                        int index = datas[i].IndexOf("<DIR>");
                        string name = datas[i].Substring(index + 5).Trim();
                        if (name == dirName)
                        {
                            result = true;
                            break;
                        }
                    }
                }
                sr.Close();
                sr.Dispose();
                response.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return result;
        }
        public static bool UploadFile(string filePathNow,string ftpPath)
        {
            return UpFile(filePathNow, ftpPath, ApplicationData.ftpserver, ApplicationData.ftpusername, ApplicationData.ftppassword);
        }

        public static void Page_Load(string Host,string fromAddr, string psw, string toAddr, string subject, string body,string ImgeUrl)
        {
            try
            {
                MailMessage mm = new MailMessage();
                MailAddress Fromma = new MailAddress(fromAddr);
                mm.From = Fromma;
                //收件人
                mm.To.Add(new MailAddress(toAddr));
                //邮箱标题
                mm.Subject = subject;
                //邮件内容
                mm.Body = body;
                //内容的编码格式
                mm.BodyEncoding = Encoding.UTF8;
                mm.SubjectEncoding = Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                if (!string.IsNullOrEmpty(ImgeUrl))
                {
                    //添加附件
                    mm.Attachments.Add(new Attachment(ImgeUrl));
                    //设置附件类型
                    mm.Attachments[0].ContentType.Name = "image/jpg";
                    //设置附件 Id
                    mm.Attachments[0].ContentId = "1";
                    //设置附件为 inline-内联
                    mm.Attachments[0].ContentDisposition.Inline = true;
                    //设置附件的编码格式
                    mm.Attachments[0].TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                }
                
                SmtpClient sc = new SmtpClient();
                NetworkCredential nc = new NetworkCredential();
                nc.UserName = fromAddr;
                nc.Password = psw;
                sc.EnableSsl = true;
                sc.UseDefaultCredentials = true;
                sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                sc.Credentials = nc;
                sc.Host = Host;
                sc.Port = 587;
                sc.Send(mm);
            }
            catch (Exception ex)
            {
            }

        }
    }
}

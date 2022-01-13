using AForge.Imaging.Filters;
using CCWin.SkinControl;
using HaSdkWrapper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;

namespace huaanClient
{
    class copyfile
    {
        public static string copyimge(string path, string filename)
        {
            string ext = Path.GetExtension(path);//截取后缀名
            if (!Constants.AllowedImageFileFormats.Contains(ext.ToLowerInvariant()))
            {
                return null;
            }

            var imgeurl = "";
            try
            {
                //新建一个文件夹
                var targetFolder = ApplicationData.FaceRASystemToolUrl+"\\imgefile";
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }
                
                if(Tools.TryDownscaleImage(path, out var array))
                {
                    var targetFilePath = Path.Combine(targetFolder, filename + ".jpg");
                    File.WriteAllBytes(targetFilePath, array);
                    imgeurl = targetFilePath;
                }
                else
                {
                    var targetFilePath = Path.Combine(targetFolder, filename + ext);
                    File.Copy(path, targetFilePath);
                    imgeurl = targetFilePath;
                }
                return imgeurl;
            }
            catch
            {
                return null;
            }
            
        }


        public static bool Copyfile(string path, string newpath, string filename)
        {
            try
            {
                var nf = Path.Combine(newpath);//将新的文件名称路径字符串结合成路径。
                if (!nf.ToString().IsNullOrEmpty())
                {
                    File.Copy(path, nf,true);//进行文件复制，第一个参数是需要复制的文件路径，第二个参数是目标文件夹中文件路径
                }
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }

        }

        public static bool Copyfile1(string path, string newpath, string filename)
        {
            try
            {
                var nf = Path.Combine(newpath);//将新的文件名称路径字符串结合成路径。
                if (!nf.ToString().IsNullOrEmpty())
                {
                    File.Copy(path, nf, true);//进行文件复制，第一个参数是需要复制的文件路径，第二个参数是目标文件夹中文件路径
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }

        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        public static string openImge()
        {
            string reimgeurl = string.Empty;
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "jpg (*.jpg)|*.jpg|png (*.png)|*.png|jpeg (*.png)|*.jpeg";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string PicFileName = openFile.FileName;

                //if (getImageSize(PicFileName)>5)
                //{
                //    return "100";
                //}
                byte[] imgData;
                imgData = SaveImage(PicFileName);
                //判断图片大小
                //Bitmap bitmap= IsQualified(PicFileName,600,800);
                //if (bitmap!=null)
                //{
                //    imgData = SaveImage(bitmap);
                //}
                //else
                //{
                //    imgData = SaveImage(PicFileName);
                //}


                byte[][] re = HaCamera.HA_GetJpgFeatureImageNew(imgData);

                if (re[2][0] != 0)
                {
                    reimgeurl = "0";
                }
                else
                {
                    //byte[] twis = HaCamera.HA_FeatureConvert(re[1], "HV10");
                    reimgeurl = PicFileName;
                }
            }
            return reimgeurl;
        }

        public static string openImgeforRcode()
        {
            string reimgeurl = string.Empty;
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "jpg (*.jpg)|*.jpg|png (*.png)|*.png|jpeg (*.jpeg)|*.jpeg";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string PicFileName = openFile.FileName;
                reimgeurl = PicFileName;
            }
            return reimgeurl;
        }

        public static Bitmap IsQualified(string path,int x, int y)
        {
            Bitmap bitmap = null;
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                float height = image.Height;
                float width = image.Width;

                if (height > y || width > x)
                {
                    Bitmap map = new Bitmap(image);
                    bitmap = GetThumbnail(map, x, y);
                }
            }
            catch
            {
            }
            return bitmap;
        }
        /// <summary>
        /// 将图片以二进制流
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] SaveImage(String path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
                BinaryReader br = new BinaryReader(fs);
                byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
                return imgBytesIn;
            }
            catch
            {
                return null;
            }
            
        }

        /// <summary>
        /// 将图片以二进制流
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] SaveImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }

        /// <summary>
        /// 得到路径下文件的大小 MB
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static double getImageSize(string path)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                double length = Convert.ToDouble(fileInfo.Length);
                double Size = length / 1024 / 1024;
                return Size;
            }
            catch
            {
                return 100;
            }
        }

        /// <summary>
        /// 修改图片的大小
        /// </summary>
        /// <param name="b"></param>
        /// <param name="destHeight">高</param>
        /// <param name="destWidth">宽</param>
        /// <returns></returns>
        public static Bitmap GetThumbnail(Bitmap b, int destHeight, int destWidth)
        {
            System.Drawing.Image imgSource = b;
            System.Drawing.Imaging.ImageFormat thisFormat = imgSource.RawFormat;
            int sW = 0, sH = 0;
            // 按比例缩放           
            int sWidth = imgSource.Width;
            int sHeight = imgSource.Height;
            if (sHeight > destHeight || sWidth > destWidth)
            {
                if ((sWidth * destHeight) > (sHeight * destWidth))
                {
                    sW = destWidth;
                    sH = (destWidth * sHeight) / sWidth;
                }
                else
                {
                    sH = destHeight;
                    sW = (sWidth * destHeight) / sHeight;
                }
            }
            else
            {
                sW = sWidth;
                sH = sHeight;
            }
            Bitmap outBmp = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage(outBmp);
            g.Clear(Color.Transparent);
            // 设置画布的描绘质量         
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(imgSource, new Rectangle((destWidth - sW) / 2, (destHeight - sH) / 2, sW, sH), 0, 0, imgSource.Width, imgSource.Height, GraphicsUnit.Pixel);
            g.Dispose();
            // 以下代码为保存图片时，设置压缩质量     
            EncoderParameters encoderParams = new EncoderParameters();
            long[] quality = new long[1];
            quality[0] = 100;
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            imgSource.Dispose();
            return outBmp;
        }
    }
}

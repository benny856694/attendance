using System;
using ZXCL.WinFormUI;
using AForge;
using AForge.Controls;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using HaSdkWrapper;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace huaanClient
{
    public partial class Camera : ZForm
    {
        FilterInfoCollection videoDevices;
        VideoCaptureDevice videoSource;
        public int selectedDeviceIndex = 0;
        HaCamera haCamera;
        public string imgurl = string.Empty; 
        public Camera()
        {
            InitializeComponent();
        }

        //public delegate void VideoParmReceivedArgs(string sender);
        //public event VideoParmReceivedArgs Alarm;//声明事件 
        public int myWidth { get; set; }
        public int myHeight { get; set; }
        string newip = "";
        public bool ischange = false;
        public bool isMyDevice = false;
        void getWidthAndHeight(string ip)
        {
            if (haCamera != null)
            {
                haCamera.VideoParmReceived -= HaCamera_VideoParmReceived;
                haCamera.DisConnect();
            }
            haCamera = new HaCamera();
            haCamera.Ip = ip;
            newip= ip;
            haCamera.Port = 9527;
            haCamera.VideoParmReceived += HaCamera_VideoParmReceived;
            haCamera.Connect();
        }

        private void HaCamera_VideoParmReceived(object sender, VideoParmReceivedArgs e)
        {
            myWidth = e.Width;
            myHeight = e.Height;

            //if (e.Width> e.Height)
            //{
            //    if (myWidth > 350)
            //    {
            //        myHeight = 350 * myHeight / myWidth;
            //        myWidth = 350;
            //    }
            //}
            //else
            //{
            //    if (myHeight > 350)
            //    {
            //        myWidth = 350 * myWidth / myHeight;
            //        myHeight = 350;
            //    }
            //}

            //if (myHeight> Screen.PrimaryScreen.WorkingArea.Height/2)
            //{
            //    myWidth = myWidth / 2;
            //    myHeight = myHeight / 2;

            //    if (myHeight > Screen.PrimaryScreen.WorkingArea.Height / 2)
            //    {
            //        myWidth =(int) Math.Round((double)myWidth / 3)*2 ;
            //        myHeight = (int)Math.Round((double)myHeight / 3) * 2;
            //    }
            //}
           
            haCamera.VideoParmReceived -= HaCamera_VideoParmReceived;
            haCamera.DisConnect();
            setwidthAndheight();
            haCamera.Ip = newip;
            haCamera.Port = 9527;
            BeginInvoke(new Action(() => {
                haCamera.Connect(this.videoSourcePlayer1.Handle);
            }));
        }

        public void chanvideo(string ip)
        {
            ischange = false;
            videoSourcePlayer1.Text = "正在切换...";
            videoSourcePlayer1.Stop();
            if (ip.Length<5)
            {
                if (haCamera!=null)
                {
                    haCamera.DisConnect();
                } 
                SwitchLocalCamera(ip);
            }
            else
            {
                ischange = true;
                getWidthAndHeight(ip);   
            } 
        }

        //切换本地相机
        public void SwitchLocalCamera(string No)
        {
            selectedDeviceIndex = int.Parse(No);
            videoSource = new VideoCaptureDevice(videoDevices[selectedDeviceIndex].MonikerString);//连接摄像头。
            videoSource.VideoResolution = videoSource.VideoCapabilities[selectedDeviceIndex];

            myWidth = videoSource.VideoResolution.FrameSize.Width;
            myHeight = videoSource.VideoResolution.FrameSize.Height;

            setwidthAndheight();

            videoSourcePlayer1.VideoSource = videoSource;
            videoSourcePlayer1.Start();
        }

        private void Camera_Load(object sender, EventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count != 0)
            {
                for (int i = 0; i < videoDevices.Count; i++)
                {
                    Iplist iplist = new Iplist();
                    var v = i;
                    iplist.Controls["btn"].Click += (object sender_, EventArgs e_) => chanvideo((v).ToString());
                    iplist.Controls["Iplabel"].Text = "摄像头"+(i+1);
                    plView.Controls.Add(iplist);
                }
                
                selectedDeviceIndex = 0;
                videoSource = new VideoCaptureDevice(videoDevices[selectedDeviceIndex].MonikerString);//连接摄像头。
                videoSource.VideoResolution = videoSource.VideoCapabilities[selectedDeviceIndex];

                myWidth = videoSource.VideoResolution.FrameSize.Width;
                myHeight = videoSource.VideoResolution.FrameSize.Height;

                

                setwidthAndheight();

                videoSourcePlayer1.VideoSource = videoSource;
                videoSourcePlayer1.Start();
            }
            //查询所有的IP地址
            string data = GetData.getDeviceDiscover();
            JArray jArray = (JArray)JsonConvert.DeserializeObject(data);

            if (jArray.Count > 0)
            {
                foreach (var j in jArray)
                {
                    Iplist iplist = new Iplist();
                    // j["IP"].ToString().Trim()+ j["DeviceName"].ToString().Trim()

                    if (Database.Deviceinfo.MyDevicelist.Find(a=>a.IP== j["IP"].ToString().Trim()).IsConnected)
                    {
                        iplist.Controls["Iplabel"].Text = j["IP"].ToString().Trim();

                        iplist.Controls["btn"].Click += (object sender_, EventArgs e_) => chanvideo(j["IP"].ToString().Trim());

                        plView.Controls.Add(iplist);
                    }
                }
            }
        }

        void setwidthAndheight()
        {
            if (myWidth > myHeight)
            {
                if (myWidth > 350)
                {
                    myHeight = 350 * myHeight / myWidth;
                    myWidth = 350;
                }
            }
            else
            {
                if (myHeight > 350)
                {
                    myWidth = 350 * myWidth / myHeight;
                    myHeight = 350;
                }
            }
            this.BeginInvoke(new Action(()=> {
                //this.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
                //this.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;

                //this.Size = new System.Drawing.Size(myWidth + 140+170, myHeight + 140);
                //this.plView.Size= new System.Drawing.Size(170, myHeight);
                //this.plView.Location = new System.Drawing.Point(20, 50);
                videoSourcePlayer1.Size = new System.Drawing.Size(myWidth, myHeight);
                //this.Location = new System.Drawing.Point(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width
                //    / 2 - (myWidth + 140 + 170) / 2, System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 2 - (myHeight + 140) / 2);

                //this.btnLogin.Location = new System.Drawing.Point((myWidth + 140 + 190 + 140) / 2 - 115, myHeight + 70);
                if (myWidth==350)
                {
                    this.videoSourcePlayer1.Location = new System.Drawing.Point(212, 50);
                }
                else
                {
                    this.videoSourcePlayer1.Location = new System.Drawing.Point(197 + myWidth / 2, 50);
                }
            }) );    
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = null;
            string fileName = string.Empty;
            if (ischange)
            {
                //截图
                System.Drawing.Image image = haCamera.Snapshot(2000);
                fileName = copyfile.GetTimeStamp();
                if (image!=null)
                    bitmap = new Bitmap(image);
                
            }
            else
            {
                if (videoSource == null)
                    return;
                bitmap = videoSourcePlayer1.GetCurrentVideoFrame();
                fileName = copyfile.GetTimeStamp();//DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ff") + ".jpg";
                if (bitmap == null)
                {
                    return;
                }
            }
            if (bitmap == null)
            {
                return;
            }
            byte[][] re = HaCamera.HA_GetJpgFeatureImageNew(copyfile.SaveImage(bitmap));

            if (re[2][0] != 0)
            {
                bitmap.Dispose();
                this.DialogResult = DialogResult.No;

                string mes = "图片不合法，图片必须满足正面且只有唯一人脸";
                if (ApplicationData.LanguageSign.Contains("English"))
                { 
                    mes = "The picture is illegal. The picture must be positive and have only one face";
                }
                else if (ApplicationData.LanguageSign.Contains("日本語"))
                {
                    mes = "写真は合法的ではなく、正面を満たす必要があります。しかも唯一の顔しかありません";
                }
                if (videoSourcePlayer1 != null && videoSourcePlayer1.IsRunning)
                {
                    videoSourcePlayer1.SignalToStop();
                    videoSourcePlayer1.WaitForStop();
                }
                MessageBox.Show(mes);
                return;
            }
            else
            {
                var imgPath = ApplicationData.FaceRASystemToolUrl + "\\imgefile";
                if (!Directory.Exists(imgPath))
                {
                    Directory.CreateDirectory(imgPath);
                }
                bitmap.Save(ApplicationData.FaceRASystemToolUrl+"\\imgefile\\" + fileName+".jpg", ImageFormat.Jpeg);
                imgurl = ApplicationData.FaceRASystemToolUrl + "\\imgefile\\" + fileName+ ".jpg";
                bitmap.Dispose();
                if (videoSourcePlayer1 != null && videoSourcePlayer1.IsRunning)
                {
                    videoSourcePlayer1.SignalToStop();
                    videoSourcePlayer1.WaitForStop();
                }

                this.DialogResult = DialogResult.OK;
                if (haCamera != null)
                {
                    haCamera.DisConnect();
                }
                return;
            }
            
        }

        private void Camera_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (haCamera != null)
            {
                haCamera.DisConnect();
            }
            if (e.CloseReason == CloseReason.UserClosing)//当用户点击窗体右上角X按钮或(Alt + F4)时 发生          
            {
                this.DialogResult = DialogResult.Cancel;
                e.Cancel = false;
            }
        }

        private byte[] imageToByte(System.Drawing.Image _image)
        {
            MemoryStream ms = new MemoryStream();
            _image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
    }
}

using huaanClient;
using System.Collections.Generic;
namespace VideoHelper
{
    public  class VideoHelper
    {
        //视频连接需要的设备
        public static List<TLVClientHolder> DevicelistForVideo { get; set; }

        static VideoHelper()
        {
            DevicelistForVideo = new List<TLVClientHolder>();
        }

        public static void getinfoToMyDev(string ip)
        {
            if (DevicelistForVideo.Count==0)
            {
                CameraStreamPort cam=new CameraStreamPort(ip);
                cam.Username = "123";
                cam.Password = "123";
                cam.Connect();
                TLVClientHolder holder = new TLVClientHolder(ip, cam, "", false);
                DevicelistForVideo.Add(holder);   
                holder.Tag= MultiPlayerPanel.ConnectCamera(holder);
            }
            else
            {
                var deviceForVideo = DevicelistForVideo.Find(c =>c.CameraStreamPort.IP.Trim()==ip.Trim());
                if (deviceForVideo == null)
                {
                    CameraStreamPort cam = new CameraStreamPort(ip);
                    cam.Username = "123";
                    cam.Password = "123";
                    cam.Connect();

                    TLVClientHolder holder = new TLVClientHolder(ip, cam, "", false);
                    DevicelistForVideo.Add(holder);
                    holder.Tag =MultiPlayerPanel.ConnectCamera(holder);
                }
            }     
        }
    }
}

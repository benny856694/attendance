using huaanClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoHelper
{
    public  class TLVClientHolder : Holder
    {
        public string Name { get; }
        public object Tag { get; set; }
        

        public CameraStreamPort CameraStreamPort { get;set; }

        public event DisconnectEventHandler Disconnect;
        public event H264ReceivedEventHandler H264Received;
        public event ToggleDoneEventHandler ToggleDone;


        public TLVClientHolder(string name, CameraStreamPort cameraStreamPort, string id, bool isToggleOn)
        {
            Tag = "";
            Name = name;
            CameraStreamPort = cameraStreamPort;
            IsToggleOn = isToggleOn;
            CameraStreamPort.Disconnected += CameraStreamPort_Disconnected; 
            //SubsystemParamData = subsystemParamData;
        }

        private void CameraStreamPort_Disconnected(TlvclientV sender)
        {
            Disconnect?.Invoke();
        }

        public void Start()
        {
            CameraStreamPort.LiveStreamFrameEvent += CameraStreamPort_LiveStreamFrameEvent;
        }

        private void CameraStreamPort_LiveStreamFrameEvent(object sender, LiveStreamEventArgs e)
        {
            H264Received?.Invoke(e.Data);  
        }

        public void Stop()
        {
            CameraStreamPort.LiveStreamFrameEvent -= CameraStreamPort_LiveStreamFrameEvent;
            CameraStreamPort.Disconnected -= CameraStreamPort_Disconnected;
            CameraStreamPort.DisConnect();
            VideoHelper.DevicelistForVideo.RemoveAll(s => s.Name.Trim() == CameraStreamPort.IP) ;
        }
        public bool ToggleAble => true;
        public bool IsToggleOn { get; private set; }
        private byte[] SubsystemParamData;
        public void Toggle()
        {

        }
    }
}

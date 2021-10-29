using VideoHelper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace huaanClient
{
    public sealed class LiveStreamEventArgs : EventArgs
    {
        /// <summary>
        /// 视频画面宽（像素）
        /// </summary>
        public ushort Width { get; internal set; }
        /// <summary>
        /// 视频画面高（像素）
        /// </summary>
        public ushort Height { get; internal set; }
        /// <summary>
        /// 帧序号
        /// </summary>
        public uint Seq { get; internal set; }
        /// <summary>
        /// 帧数据
        /// </summary>
        public byte[] Data { get; internal set; }
    }
    public delegate void DisconnectedEventHandler(TlvclientV sender);
    public class CameraStreamPort
    {
        private TlvclientV tlv;
        private volatile bool needReconnect = true;
        /// <summary>
        /// 设备IP
        /// </summary>
        public readonly string IP;
        /// <summary>
        /// 登录用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 登录密码
        /// 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 收到视频帧
        /// </summary>
        public event EventHandler<LiveStreamEventArgs> LiveStreamFrameEvent;
        public event DisconnectedEventHandler Disconnected;
        /// <summary>
        /// 通过设备IP地址创建逻辑设备对象
        /// </summary>
        /// <param name="ip">设备IP地址</param>
        public CameraStreamPort(string ip)
        {
            IP = ip;
        }

        private volatile bool m_isconnected;
        /// <summary>
        /// 与设备的网络连接是否正常
        /// </summary>
        public bool IsConnected { get => m_isconnected; }

        private object reconnectLocker = new object();
        private Timer reconnectTimer;
        /// <summary>
        /// <para>尝试连接到设备</para>
        /// <para>如果当前设备不可达，程序会在后台尝试重连，直至连接成功或者调用Disconnect</para>
        /// <para>后续任何时间点或线程，可使用IsConnected判定设备网络连通性；精度为15秒</para>
        /// </summary>
        /// <returns>当此连接成功与否</returns>
        public bool Connect()
        {
            tlv = new TlvclientV(IP, 20000);
            tlv.MessageReceived += Tlv_MessageReceived;
            tlv.Disconnected += Tlv_Disconnected;
            reconnectTimer = new Timer();
            reconnectTimer.Interval = 15 * 1000;
            reconnectTimer.Elapsed += ReconnectTimer_Elapsed; ;
            reconnectTimer.Start();
            m_isconnected = tlv.Connect();
            if (m_isconnected) SendAuth();
            return m_isconnected;
        }

        private void Tlv_Disconnected(TlvclientV sender)
        {
            m_isconnected = false;
        }

        private void ReconnectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (reconnectLocker)
            {
                if (needReconnect && m_isconnected == false)
                {
                    tlv?.DisConnect();
                    tlv = new TlvclientV(IP, 20000);
                    m_isconnected = tlv.Connect();
                    if (m_isconnected) SendAuth();
                }
            }
        }

        private unsafe void SendAuth()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password)) return;
            byte[] bytes = new byte[4 + 15 + 16 + 33];
            byte[] ubytes = Encoding.UTF8.GetBytes(Username);
            byte[] tbytes = BitConverter.GetBytes(Convert.ToUInt32(DateTime.Now.ToUniversalTime().Subtract(DateTime.Parse("1970-1-1")).TotalSeconds));
            Array.Copy(tbytes, bytes, 4);
            Array.Copy(ubytes, 0, bytes, 4, ubytes.Length);
            byte[] pbytes_o = new byte[Password.Length + 4];
            Array.Copy(tbytes, pbytes_o, 4);
            byte[] pbytes = Encoding.UTF8.GetBytes(Password);
            Array.Copy(pbytes, 0, pbytes_o, 4, pbytes.Length);
            MD5 md5 = MD5.Create();
            byte[] pbytes_md5 = md5.ComputeHash(pbytes_o);
            Array.Copy(pbytes_md5, 0, bytes, 19, pbytes_md5.Length);
            tlv?.Write(SysType, Marjor, Minor, 7, bytes);
        }

        /// <summary>
        /// 断开与设备的连接
        /// </summary>
        public void DisConnect()
        {
            OnDisConnect();
        }

        protected virtual void OnDisConnect()
        {
            try
            {
                if (IsConnected)
                {
                    needReconnect = false;
                    tlv?.DisConnect();
                }
            }
            catch { }
            Disconnected?.Invoke(tlv);
        }

        ~CameraStreamPort()
        {
            DisConnect();
        }

        private static int SysType = 12;
        private static int Marjor = 2;
        private static int Minor = 11;

        private unsafe void Tlv_MessageReceived(TlvclientV sender, int sysType, int majorVersion, int minorVersion, int msgType, int? ack, byte[] _v)
        {
            if(msgType == 103)
            {
                if (LiveStreamFrameEvent == default) return;
                int format = BitConverter.ToInt32(_v, 0);
                if(format == 2)//h264
                {
                    LiveStreamEventArgs e = new LiveStreamEventArgs();
                    e.Width = BitConverter.ToUInt16(_v, 4);
                    e.Height = BitConverter.ToUInt16(_v, 6);
                    e.Seq = BitConverter.ToUInt32(_v, 8);
                    e.Data = new byte[_v.Length - 20]; // format used
                    Array.Copy(_v, 20, e.Data, 0, e.Data.Length);
                    LiveStreamFrameEvent(this, e);
                }
                else if(format == 1)//jpg stream
                {
                    LiveStreamEventArgs e = new LiveStreamEventArgs();
                    e.Width = BitConverter.ToUInt16(_v, 4);
                    e.Height = BitConverter.ToUInt16(_v, 6);
                    //e.Seq = BitConverter.ToUInt32(_v, 8);
                    e.Data = new byte[_v.Length - 12]; // format used
                    Array.Copy(_v, 12, e.Data, 0, e.Data.Length);
                    LiveStreamFrameEvent(this, e);
                }
            }
        }
    }
}

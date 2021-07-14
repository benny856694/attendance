using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace huaanClient
{
    public delegate void DisconnectedEventHandler(TLVClient sender);
    public delegate void MessageReceivedEventHandler(TLVClient sender, int sysType, int majorVersion, int minorVersion, int msgType, int? ack, byte[] _v);
    public class TLVClient
    {
        public event DisconnectedEventHandler Disconnected;
        public event MessageReceivedEventHandler MessageReceived;

        private Socket _clientSocket;

        public object Tag { get; set; }
        private IPEndPoint ip;

        public Guid id { get; private set; }

        public TLVClient(string _ip, int _port)
        {
            ip = new IPEndPoint(IPAddress.Parse(_ip), _port);
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            id = Guid.NewGuid();

            Debug.WriteLine($"=====create new socket:{id}=========");
        }


        enum ReadState
        {
            ReadType,
            ReadLength,
            ReadValue
        }
        byte[] _readBuffer = new byte[1024 * 1024]; // 读取每次4k
        private ReadState readState = ReadState.ReadType;
        internal int sysType;
        internal int majorVersion;
        internal int minorVersion;
        internal int msgType;
        internal int ack;
        internal bool isReply;
        internal int length;
        internal byte[] v;
        internal int vI = 0;
        public bool Connect()
        {
            try
            {
                IAsyncResult result = _clientSocket.BeginConnect(ip, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(1000, true);
                if (!success)
                {
                    ShutDown();
                    _clientSocket?.Close();
                    return false;
                }
            }
            catch
            {
                return false;
            }
            if (_clientSocket.Connected)
            {
                OnStart();
                Interlocked.Exchange(ref _connected, 1);
                return true;
            }
            return false;
        }

        private volatile bool recvData = false;
        private System.Timers.Timer heartBeatS;
        private System.Timers.Timer heartBeatR;
        protected virtual void OnStart()
        {
            Recv();
            heartBeatR = new System.Timers.Timer();
            heartBeatR.Interval = 15 * 1000;
            heartBeatR.Elapsed += HeartBeatR_Elapsed;
            heartBeatR.Start();
            /*heartBeatS = new Timer();
            heartBeatS.Interval = 10 * 1000;
            heartBeatS.Elapsed += HeartBeatS_Elapsed;
            heartBeatS.Start();*/
        }

        /*private void HeartBeatS_Elapsed(object sender, ElapsedEventArgs e)
        {
            Write(sysType, majorVersion, minorVersion, 2, null);
        }*/

        private void HeartBeatR_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!recvData) OnDisConnect();
            recvData = false;
        }

        private void Recv()
        {
            try
            {
                _clientSocket.BeginReceive(_readBuffer, 0, _readBuffer.Length, SocketFlags.None, asyncResult =>
                {
                    try
                    {
                        Debug.WriteLine($"======beging recv {id}==========");
                        int readLen = _clientSocket.EndReceive(asyncResult);
                        if (readLen == 0)
                        {
                            ShutDown();
                            OnDisConnect();
                            _clientSocket?.Close();
                            
                            return;
                        }
                        recvData = true;
                        byte[] readBuffer = _readBuffer;
                        dealBuf:
                        switch (readState)
                        {
                            case ReadState.ReadType:
                                if (readLen >= 4)
                                {
                                    int T = BitConverter.ToInt32(readBuffer, 0);
                                    sysType = (T) >> 24;
                                    majorVersion = ((T) & 0x00FFC000) >> 14;
                                    minorVersion = ((T) & 0x3C00) >> 10;
                                    msgType = (T) & 0x3FF;
                                    isReply = msgType == 4;
                                    readState = ReadState.ReadLength;
                                }
                                if (readLen >= 8)
                                {
                                    length = BitConverter.ToInt32(readBuffer, 4);
                                    if (length > 10 * 1024 * 1024 || length < 0) goto doRecv; // 数据错乱，扔掉
                                    v = new byte[length];
                                    vI = 0;
                                    readState = ReadState.ReadValue;
                                }
                                if (readLen > 8)
                                {
                                    bool zb1 = readLen - 8 > length; // 粘包？
                                    int sb1 = Math.Min(readLen - 8, length); // sb means 'should copy bytes'
                                    Array.Copy(readBuffer, 8, v, 0, sb1);
                                    vI += sb1;
                                    if (vI == length)
                                    {
                                        OutputMessage();
                                        readState = ReadState.ReadType;
                                    }
                                    if (zb1)
                                    {
                                        readLen -= (length + 8);
                                        byte[] _readBuffer = new byte[readLen];
                                        Array.Copy(readBuffer, length + 8, _readBuffer, 0, readLen);
                                        readBuffer = _readBuffer;
                                        goto dealBuf;
                                    }
                                }
                                break;
                            case ReadState.ReadLength:
                                if (readLen >= 4)
                                {
                                    length = BitConverter.ToInt32(readBuffer, 0);
                                    if (length > 1024 * 1024 || length < 0) goto doRecv; // 数据错乱，扔掉
                                    v = new byte[length];
                                    vI = 0;
                                    readState = ReadState.ReadValue;
                                }
                                if (readLen > 4)
                                {
                                    bool zb2 = readLen - 4 > length; // 粘包？
                                    int sb2 = Math.Min(readLen - 4, length); // sb means 'should copy bytes'
                                    Array.Copy(readBuffer, 4, v, 0, sb2);
                                    vI += sb2;
                                    if (vI == length)
                                    {
                                        OutputMessage();
                                        readState = ReadState.ReadType;
                                    }
                                    if (zb2)
                                    {
                                        readLen -= (length + 4);
                                        byte[] _readBuffer = new byte[readLen];
                                        Array.Copy(readBuffer, length + 4, _readBuffer, 0, readLen);
                                        readBuffer = _readBuffer;
                                        goto dealBuf;
                                    }
                                }
                                break;
                            case ReadState.ReadValue:
                                bool zb3 = readLen > length - vI; // 粘包？
                                int sb3 = Math.Min(readLen, length - vI); // sb means 'should copy bytes'
                                Array.Copy(readBuffer, 0, v, vI, sb3);
                                vI += sb3;
                                if (vI == length)
                                {
                                    OutputMessage();
                                    readState = ReadState.ReadType;
                                }
                                if (zb3)
                                {
                                    readLen -= sb3;
                                    byte[] _readBuffer = new byte[readLen];
                                    Array.Copy(readBuffer, sb3, _readBuffer, 0, readLen);
                                    readBuffer = _readBuffer;
                                    goto dealBuf;
                                }
                                break;
                            default:
                                break;
                        }
                        doRecv:
                        Recv();
                    }
                    catch (Exception ex)
                    {
                        StringBuilder msg = new StringBuilder();
                        msg.Append($"******************{id}********************* \n");
                        msg.AppendFormat(" 异常发生时间： {0} \n", DateTime.Now);
                        msg.AppendFormat(" 导致当前异常的 Exception 实例： {0} \n", ex.InnerException);
                        msg.AppendFormat(" 导致异常的应用程序或对象的名称： {0} \n", ex.Source);
                        msg.AppendFormat(" 引发异常的方法： {0} \n", ex.TargetSite);
                        msg.AppendFormat(" 异常堆栈信息： {0} \n", ex.StackTrace);
                        msg.AppendFormat(" 异常消息： {0} \n", ex.Message);
                        msg.Append("***************************************");
                        Console.WriteLine(msg);
                        OnDisConnect();
                    }
                    //catch { OnDisConnect(); }
                }, null);
            }
            catch { OnDisConnect(); }
        }

        private void OutputMessage()
        {
            if (msgType == 2)
            {
                Write(sysType, majorVersion, minorVersion, 2, null);
                return;
            }
            int sysType_, majorVersion_, minorVersion_, msgType_;
            int? ack_ = null;
            byte[] v_;
            {
                if (isReply)
                {
                    msgType = BitConverter.ToInt32(v, 0);
                    ack_ = BitConverter.ToInt32(v, 4);
                    byte[] _v = new byte[length - 8];
                    Array.Copy(v, 8, _v, 0, length - 8);
                    v = _v;
                }
                sysType_ = sysType;
                majorVersion_ = majorVersion;
                minorVersion_ = minorVersion;
                msgType_ = msgType;
                v_ = v;
            }
            Task.Factory.StartNew(() =>
            {
                MessageReceived?.Invoke(this, sysType_, majorVersion_, minorVersion_, msgType_, ack_, v_);
            });
        }

        ~TLVClient()
        {
            Debug.WriteLine($"=========finalize {id}==============");
            ShutDown();
            OnDisConnect();
            _clientSocket.Dispose();
        }

        private void ShutDown()
        {
            if (Interlocked.Exchange(ref _shutdown, 1) == 0)
            {
                Debug.WriteLine($"========shutdown {id}===========");
                if (_clientSocket.Connected)
                {
                    _clientSocket.Shutdown(SocketShutdown.Both);
                }
            }

           
        }

        public void DisConnect()
        {
            ShutDown();
            OnDisConnect();
        }

        protected virtual void OnDisConnect()
        {
            heartBeatR?.Stop();
            heartBeatS?.Stop();
            if (Interlocked.Exchange(ref _connected, 0) == 1)
            {
                Disconnected?.Invoke(this);
            }
        }

        private object writeLocker = new object();
        private int _connected = 0;
        private int _shutdown = 0;

        public void Write(int sysType, int majorVersion, int minorVersion, int msgType, byte[] v)
        {
            try
            {
                if (_clientSocket.Connected)
                {
                    int t = 0;
                    t |= (sysType << 24);
                    t |= (majorVersion) << 14;
                    t |= (minorVersion) << 10;
                    t |= msgType;
                    int l = 0;
                    if (v != null)
                        l = v.Length;
                    byte[] sendBuf = new byte[l + 8];
                    byte[] tbs = BitConverter.GetBytes(t);
                    byte[] lbs = BitConverter.GetBytes(l);
                    Array.Copy(tbs, sendBuf, 4);
                    Array.Copy(lbs, 0, sendBuf, 4, 4);
                    if (v != null)
                        Array.Copy(v, 0, sendBuf, 8, l);
                    lock (writeLocker)
                        _clientSocket.Send(sendBuf);
                }
                else
                {
                    OnDisConnect();
                }
            }
            catch {
               
            }
        }
    }
}

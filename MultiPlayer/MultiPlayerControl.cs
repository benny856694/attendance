//#define DESIGN
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using MultiPlayer.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static VideoHelper.FFHelper;

namespace VideoHelper
{
    public unsafe partial class MultiPlayerControl : UserControl
    {
        [DllImport("msvcrt", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        static extern void memcpy(IntPtr dest, IntPtr src, int count);
        internal bool CanFullScreen = false;

        public MultiPlayerControl()
        {
            InitializeComponent();
            this.label1.Text = Strings.NotConnected;

            if (!DesignMode)
            {
                avcodec_register_all();
                frame = av_frame_alloc();
                avpkt = av_packet_alloc();
                av_init_packet(avpkt);
                nalData = Marshal.AllocHGlobal(1024 * 1024);
                codec = avcodec_find_decoder(AVCodecID.AV_CODEC_ID_H264);
                avpkt->data = (void*)nalData;
            }

        }

        private Holder m_holder;
        public Holder Holder { set
            {
                if (m_holder != null)
                    MPCStop();
                m_holder = value;
                m_holder.ToggleDone += M_holder_ToggleDone;
                if (m_holder.ToggleAble)
                {
                    btn_disconnect.Visible = false;
                    if(m_holder.IsToggleOn)
                        btn_disconnect.Image = Resources.toggleon;
                }
                else
                    btn_disconnect.Visible = false;
                Tag = m_holder.Tag;
                MPCPlay();

                MPCOnPaused = null;
                MPCOnPictureStoped = null;
                MPCOnPlaying = null;
                MPCOnRequestRotate = null;
                MPCOnResume = null;
                MPCOnStarted = null;
                MPCOnStop = null;
                MPCOnToggleEvent = null;
            }
            get => m_holder;
        }

        private void M_holder_ToggleDone(bool suc)
        {
            if (!suc) return;
            if (m_holder.IsToggleOn)
                btn_disconnect.Image = Resources.toggleon;
            else
                btn_disconnect.Image = Resources.toggleoff;
        }

        private int lastIWidth, lastIHeight;
        private Rectangle lastCBounds;
        private Rectangle lastVRect;
        private Device device;
        private Surface surface;
        private IntPtr panelHandle;
        private DateTime lastRender;
        private Thread yuvWatchDog;

        AVCodec* codec;
        AVCodecContext* ctx;
        AVPacket* avpkt;
        IntPtr nalData;
        AVFrame* frame;
        AVBufferRef* hw_ctx;
        AVPixelFormat lastFmt;
        
        private volatile bool _released = false;
        private object _codecLocker = new object();
        private void Releases()
        {
            lock (_codecLocker)
            {
                if (_released) return;
                if (null != ctx)
                    fixed (AVCodecContext** LPctx = &ctx)
                        avcodec_free_context(LPctx);
                if (null != hw_ctx)
                    fixed (AVBufferRef** LPhw_ctx = &hw_ctx)
                        av_buffer_unref(LPhw_ctx);
                surface = null;
                device = null;
                lastFmt = AVPixelFormat.AV_PIX_FMT_NONE;
                _released = true;
            }
        }

        private void M_holder_H264Received(byte[] nal)
        {
            if(nal[0] == 0xFF && nal[1] == 0xD8) //jpeg stream
            {
                var img = Image.FromStream(new MemoryStream(nal));
                var rect = CenterImage(img.Size);
                var g = Graphics.FromHwnd(panelHandle);
                g.DrawImage(img, rect);
                g.Dispose();
                img.Dispose();
                
                
                //File.WriteAllBytes(@"D:\\test.jpg", nal);
            }
            else
            {
                lock (_codecLocker)
                {
                    if (!panel3.ClientRectangle.Equals(lastCBounds))
                    {
                        lastCBounds = panel3.ClientRectangle;
                        Releases();
                    }
                    if (null == ctx)
                    {
                        ctx = avcodec_alloc_context3(codec);
                        if (null == ctx)
                        {
                            return;
                        }
                        AVDictionary* dic;
                        av_dict_set_int(&dic, "hWnd", panelHandle.ToInt64(), 0);
                        fixed (AVBufferRef** LPhw_ctx = &hw_ctx)
                        {
                            if (av_hwdevice_ctx_create(LPhw_ctx, AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2,
                                                            null, dic, 0) >= 0)
                            {
                                ctx->hw_device_ctx = av_buffer_ref(hw_ctx);
                            }
                        }
                        av_dict_free(&dic);
                        if (avcodec_open2(ctx, codec, null) < 0)
                        {
                            fixed (AVCodecContext** LPctx = &ctx)
                                avcodec_free_context(LPctx);
                            if (null != hw_ctx)
                                fixed (AVBufferRef** LPhw_ctx = &hw_ctx)
                                    av_buffer_unref(LPhw_ctx);
                            return;
                        }
                    }
                    _released = false;

                    Marshal.Copy(nal, 0, nalData, nal.Length);
                    avpkt->size = nal.Length;
                    if (avcodec_send_packet(ctx, avpkt) < 0)
                    {
                        Releases(); return;
                    }
                receive_frame:
                    int err = avcodec_receive_frame(ctx, frame);
                    if (err == -11) return; // EAGAIN
                    if (err < 0)
                    {
                        Releases(); return;
                    }
                    AVFrame s_frame = *frame;
                    if (s_frame.format != AVPixelFormat.AV_PIX_FMT_DXVA2_VLD && s_frame.format != AVPixelFormat.AV_PIX_FMT_YUV420P && s_frame.format != AVPixelFormat.AV_PIX_FMT_YUVJ420P) return;
                    try
                    {
                        int width = s_frame.width;
                        int height = s_frame.height;
                        if (lastIWidth != width || lastIHeight != height || lastFmt != s_frame.format)
                        {
                            if (s_frame.format != AVPixelFormat.AV_PIX_FMT_DXVA2_VLD)
                            //ffmpeg没有yv12，只有i420，而一般显卡又支持的是yv12，不过没关系，uv转换软件来做很快
                            {
                                PresentParameters pp = new PresentParameters();
                                pp.Windowed = true;
                                pp.SwapEffect = SwapEffect.Discard;
                                pp.BackBufferCount = 0;
                                pp.DeviceWindowHandle = panelHandle;
                                pp.BackBufferFormat = Manager.Adapters.Default.CurrentDisplayMode.Format;
                                pp.EnableAutoDepthStencil = false;
                                pp.PresentFlag = PresentFlag.Video;
                                pp.FullScreenRefreshRateInHz = 0;//D3DPRESENT_RATE_DEFAULT
                                pp.PresentationInterval = 0;//D3DPRESENT_INTERVAL_DEFAULT
                                Caps caps = Manager.GetDeviceCaps(Manager.Adapters.Default.Adapter, DeviceType.Hardware);
                                CreateFlags behaviorFlas = CreateFlags.MultiThreaded | CreateFlags.FpuPreserve;
                                if (caps.DeviceCaps.SupportsHardwareTransformAndLight)
                                {
                                    behaviorFlas |= CreateFlags.HardwareVertexProcessing;
                                }
                                else
                                {
                                    behaviorFlas |= CreateFlags.SoftwareVertexProcessing;
                                }
                                device = new Device(Manager.Adapters.Default.Adapter, DeviceType.Hardware, panelHandle, behaviorFlas, pp);
                                //(Format)842094158;//nv12
                                surface = device.CreateOffscreenPlainSurface(width, height, (Format)842094169, Pool.Default);//yv12
                            }
                            lastIWidth = width;
                            lastIHeight = height;
                            lastVRect = new Rectangle(0, 0, lastIWidth, lastIHeight);
                            lastFmt = s_frame.format;
                        }
                        if (lastFmt != AVPixelFormat.AV_PIX_FMT_DXVA2_VLD)
                        {
                            int stride;
                            var gs = surface.LockRectangle(LockFlags.DoNotWait, out stride);
                            if (gs == null) return;
                            for (int i = 0; i < lastIHeight; i++)
                            {
                                memcpy(gs.InternalData + i * stride, s_frame.data1 + i * s_frame.linesize1, lastIWidth);
                            }
                            for (int i = 0; i < lastIHeight / 2; i++)
                            {
                                memcpy(gs.InternalData + stride * lastIHeight + i * stride / 2, s_frame.data3 + i * s_frame.linesize3, lastIWidth / 2);
                            }
                            for (int i = 0; i < lastIHeight / 2; i++)
                            {
                                memcpy(gs.InternalData + stride * lastIHeight + stride * lastIHeight / 4 + i * stride / 2, s_frame.data2 + i * s_frame.linesize2, lastIWidth / 2);
                            }
                            surface.UnlockRectangle();
                        }
                        Surface _surface = lastFmt == AVPixelFormat.AV_PIX_FMT_DXVA2_VLD ? new Surface(s_frame.data4) : surface;
                        if (lastFmt == AVPixelFormat.AV_PIX_FMT_DXVA2_VLD)
                            GC.SuppressFinalize(_surface);
                        Device _device = lastFmt == AVPixelFormat.AV_PIX_FMT_DXVA2_VLD ? _surface.Device : device;
                        _device.Clear(ClearFlags.Target, Color.Black, 1, 0);
                        _device.BeginScene();
                        Surface backBuffer = _device.GetBackBuffer(0, 0, BackBufferType.Mono);
                        var dstRect = CenterImage(lastVRect.Size);
                        _device.StretchRectangle(_surface, lastVRect, backBuffer, dstRect, TextureFilter.Linear);
                        _device.EndScene();
                        _device.Present();
                        backBuffer.Dispose();
                    }
                    catch (DirectXException ex)
                    {
                        StringBuilder msg = new StringBuilder();
                        msg.Append("*************************************** \n");
                        msg.AppendFormat(" 异常发生时间： {0} \n", DateTime.Now);
                        msg.AppendFormat(" 导致当前异常的 Exception 实例： {0} \n", ex.InnerException);
                        msg.AppendFormat(" 导致异常的应用程序或对象的名称： {0} \n", ex.Source);
                        msg.AppendFormat(" 引发异常的方法： {0} \n", ex.TargetSite);
                        msg.AppendFormat(" 异常堆栈信息： {0} \n", ex.StackTrace);
                        msg.AppendFormat(" 异常消息： {0} \n", ex.Message);
                        msg.Append("***************************************");
                        Console.WriteLine(msg);
                        Releases();
                        return;
                    }
                    lastRender = DateTime.Now;
                    if (started && !playing)
                    {
                        OnPlaying();
                    }
                    goto receive_frame;
                }

            }
        }

        private Rectangle CenterImage(Size sz)
        {
            Size dst = panel3.ClientRectangle.Size;
            GetScale(sz, ref dst);
            Rectangle dstRect = panel3.ClientRectangle;
            dstRect.X += (dstRect.Width - dst.Width) / 2;
            dstRect.Y += (dstRect.Height - dst.Height) / 2;
            dstRect.Size = dst;
            return dstRect;
        }

        private void GetScale(Size src, ref Size dst)
        {
            int width = 0, height = 0;
            //按比例缩放
            int sourWidth = src.Width;
            int sourHeight = src.Height;
            if (sourHeight > dst.Height || sourWidth > dst.Width)
            {
                if ((sourWidth * dst.Height) > (sourHeight * dst.Width))
                {
                    width = dst.Width;
                    height = (dst.Width * sourHeight) / sourWidth;
                }
                else
                {
                    height = dst.Height;
                    width = (sourWidth * dst.Height) / sourHeight;
                }
            }
            else
            {
                width = sourWidth;
                height = sourHeight;
            }
            dst.Width = width;
            dst.Height = height;
        }

        private void M_holder_Disconnect()
        {
            m_holder = null; // 从holder断开时就不在OnStop里释放holder了
            OnStop();
        }
        
        private bool m_mpc_active = false;
        public bool MPCActive { get { return m_mpc_active; } set { m_mpc_active = value; Invalidate(); } }

        public event EventHandler MPCActived; // 用户点击特定播放器控件时
        public event EventHandler MPCOnStop; // 由于某种原因导致播放停止
        public event EventHandler MPCOnStarted; // 播放开始
        public event EventHandler MPCOnPlaying; // 画面开始显示

        public void MPCStop()
        {
            OnStop();
        }

        protected virtual void OnStop()
        {
            if (m_holder != null)
            {
                m_holder.Disconnect -= M_holder_Disconnect;
                m_holder.H264Received -= M_holder_H264Received;
                m_holder.Stop();
                m_holder = null;
            }
            if (InvokeRequired)
                BeginInvoke(new Action(() =>
                {
                    label1.Text = Strings.NotConnected;
                    panel3.Invalidate();
                }));
            else
            {
                label1.Text = Strings.NotConnected;
                panel3.Invalidate();
            }
            started = false;
            playing = false;
            CanFullScreen = false;
            btn_connect.Image = Resources.connect;
            Releases();
            if (MPCOnStop != null)
                MPCOnStop.Invoke(this, EventArgs.Empty);
            yuvWatchDog?.Abort();
        }
        
        public void MPCPlay()
        {
            OnStarted();
        }

        protected virtual void OnStarted()
        {
            if (m_holder != null)
            {
                m_holder.Disconnect += M_holder_Disconnect;
                m_holder.H264Received += M_holder_H264Received;
                label1.Text = String.Format(Strings.Connecting, m_holder.Name);
                m_holder.Start();
            }
            started = true;
            CanFullScreen = true;
            if (MPCOnStarted != null)
                MPCOnStarted.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnPlaying()
        {
            Invoke(new Action(() => {
                label1.Text = String.Format(Strings.Playing, m_holder.Name); 
            }));
            playing = true;
            btn_connect.Image = Resources.connecting;
            MPCOnPlaying?.Invoke(this, EventArgs.Empty);
            yuvWatchDog = new Thread(new ParameterizedThreadStart((_mpc) =>
            {
                MultiPlayerControl mpc = _mpc as MultiPlayerControl;
                while (mpc.playing)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    if (mpc.playing && DateTime.Now - mpc.lastRender > TimeSpan.FromSeconds(10))
                    {
                        //Console.WriteLine("画面很久没输出了");
                        mpc.OnPictureStop();
                    }
                }
            }));
            yuvWatchDog.IsBackground = true;
            yuvWatchDog.Start(this);
        }

        public event EventHandler MPCOnPictureStoped;

        protected virtual void OnPictureStop()
        {
            MPCOnPictureStoped?.Invoke(this, EventArgs.Empty);
        }
        
        private volatile bool started, playing;

        public bool IsStarted => started;
        public bool IsPlaying => playing;

        private void btn_connect_MouseClick(object sender, MouseEventArgs e)
        {
            if (started || playing)
                MPCStop();
        }

        private void btn_connect_MouseEnter(object sender, EventArgs e)
        {
            if (started || playing)
            {
                btn_connect.Image = Resources.disconnect;
                toolTip1.SetToolTip(btn_connect, Strings.StopPlay);
            }
            else
            {
                toolTip1.SetToolTip(btn_connect, null);
            }
        }

        private void btn_connect_MouseLeave(object sender, EventArgs e)
        {
            if(playing)
                btn_connect.Image = Resources.connecting;
            else
                btn_connect.Image = Resources.connect;
        }

        private void btn_disconnect_MouseClick(object sender, MouseEventArgs e)
        {
            OnToggle();
        }

        protected virtual void OnToggle()
        {
            if (m_holder != null)
                m_holder.Toggle();
            MPCOnToggleEvent?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler MPCOnToggleEvent;

        private void btn_disconnect_MouseEnter(object sender, EventArgs e)
        {
            if(m_holder != null && m_holder.IsToggleOn)
                btn_disconnect.Image = Resources.toggleoff;
            else
                btn_disconnect.Image = Resources.toggleon;
        }

        private void btn_disconnect_MouseLeave(object sender, EventArgs e)
        {
            if (m_holder != null && m_holder.IsToggleOn)
                btn_disconnect.Image = Resources.toggleon;
            else
                btn_disconnect.Image = Resources.toggleoff;
        }

        private void btn_fullscreen_MouseClick(object sender, MouseEventArgs e)
        {
            if (CanFullScreen && (started || playing)) OnEnterFullScreen();
        }

        private void panel3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (fs)
            {
                OnLeaveFullScreen();
                return;
            }
            if (CanFullScreen && (started || playing)) OnEnterFullScreen();
        }

        public void ExitFullScreen()
        {
            if (!fs) return;
            OnLeaveFullScreen();
        }

        public event EventHandler MPCEnterFullScreen;
        public event EventHandler MPCLeaveFullScreen;
        private bool fs;
        protected virtual void OnEnterFullScreen()
        {
            if (fs) return;
            OnPause();
            Control control = panel3;
            control.Dock = DockStyle.None;
            control.Left = 0;
            control.Top = 0;
            control.Width = Screen.PrimaryScreen.Bounds.Width;
            control.Height = Screen.PrimaryScreen.Bounds.Height;
            MultiPlayerPanel.SetParent(control.Handle, IntPtr.Zero);
            MultiPlayerPanel.SetForegroundWindow(control.Handle);
            MPCEnterFullScreen?.Invoke(this, EventArgs.Empty);
            fs = true;
            OnResume();
        }

        protected virtual void OnLeaveFullScreen()
        {
            OnPause();
            MultiPlayerPanel.SetParent(panel3.Handle, Handle);
            panel3.Dock = DockStyle.Fill;
            MPCLeaveFullScreen.Invoke(this, EventArgs.Empty);
            fs = false;
            OnResume();
        }

        private void btn_fullscreen_MouseEnter(object sender, EventArgs e)
        {
            if(CanFullScreen)
                btn_fullscreen.Image = Resources.fullscreening;
        }

        private void btn_fullscreen_MouseLeave(object sender, EventArgs e)
        {
            btn_fullscreen.Image = Resources.fullscreen;
        }

        private void MultiPlayerControl_Load(object sender, EventArgs e)
        {
            panelHandle = panel3.Handle;
            lastCBounds = panel3.ClientRectangle;
        }

        private void MultiPlayerControl_Paint(object sender, PaintEventArgs e)
        {
            if (m_mpc_active)
            {
                Pen pen = new Pen(new SolidBrush(Color.LightBlue), 2);
                e.Graphics.DrawRectangle(pen, 1, 1, Width - 4, Height - 4);
            }
        }

        private void MultiPlayerControl_MouseClick(object sender, MouseEventArgs e)
        {
            OnActive();
        }

        public void MakeActive()
        {
            OnActive();
        }

        protected virtual void OnActive()
        {
            if (MPCActived != null)
            {
                MPCActived.Invoke(this, EventArgs.Empty);
            }
        }

        public void MPCPause()
        {
            if (!started) return;
            OnPause();
        }

        protected virtual void OnPause()
        {
            if (m_holder != null)
            {
                m_holder.H264Received -= M_holder_H264Received;
            }
            playing = false;
            yuvWatchDog?.Abort();
            MPCOnPaused?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler MPCOnPaused;

        public void MPCResume()
        {
            if (!started) return;
            OnResume();
        }

        public event EventHandler MPCOnRequestRotate;

        private void btn_cam_rotate_Click(object sender, EventArgs e)
        {
            MPCOnRequestRotate?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnResume()
        {
            if (m_holder != null)
            {
                m_holder.H264Received += M_holder_H264Received;
            }
            playing = true;
            yuvWatchDog = new Thread(new ParameterizedThreadStart((_mpc) =>
            {
                MultiPlayerControl mpc = _mpc as MultiPlayerControl;
                while (mpc.playing)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    if (mpc.playing && DateTime.Now - mpc.lastRender > TimeSpan.FromSeconds(10))
                    {
                        //Console.WriteLine("画面很久没输出了");
                        mpc.OnPictureStop();
                    }
                }
            }));
            yuvWatchDog.IsBackground = true;
            yuvWatchDog.Start(this);
            MPCOnResume?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler MPCOnResume;
    }
}

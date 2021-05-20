using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static MultiPlayer.FFHelper;

namespace MultiPlayer
{
    public unsafe class FFPlayer : UserControl
    {
        [DllImport("msvcrt", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        static extern void memcpy(IntPtr dest, IntPtr src, int count);  // 用于在解码器和directx间拷贝内存的c函数


        private IntPtr contentPanelHandle;                              // 画面渲染的控件句柄，因为画面渲染时可能出于非UI线程，因此先保存句柄避免CLR报错

        private int lastIWidth, lastIHeight;                            // 上次控件大小，用于在控件大小改变时做出判定重新初始化渲染上下文
        private Rectangle lastCBounds;                                  // 临时变量，存储上次控件区域（屏幕坐标）
        private Rectangle lastVRect;                                    // 临时变量，存储上次解码出的图像大小
        private Device device;                                          // 当使用软解时，这个变量生效，它是IDirect3Device9*对象，用于绘制YUV
        private Surface surface;                                        // 当使用软解时，这个变量生效，它是IDirect3Surface9*对象，用于接受解码后的YUV数据
        AVPixelFormat lastFmt;                                          // 上次解码出的图像数据类型，这个理论上不会变

        AVCodec* codec;                                                 // ffmpeg的解码器
        AVCodecContext* ctx;                                            // ffmpeg的解码上下文
        AVBufferRef* hw_ctx;                                            // ffmpeg的解码器硬件加速上下文，作为ctx的扩展存在
        AVPacket* avpkt;                                                // ffmpeg的数据包，用于封送待解码数据
        IntPtr nalData;                                                 // 一块预分配内存，作为avpkt中真正存储数据的内存地址
        AVFrame* frame;                                                 // ffmpeg的已解码帧，用于回传解码后的图像

        private volatile bool _released = false;                        // 资源释放标识，与锁配合使用避免重复释放资源（由于底层是c/c++，多线程下double free会导致程序崩溃）
        private object _codecLocker = new object();                     // 锁，用于多线程下的互斥

        static FFPlayer()
        {
            avcodec_register_all();                                     // 静态块中注册ffmpeg解码器
        }

        public FFPlayer()
        {
            InitializeComponent();

            // 过程中，下列对象只需初始化一次
            frame = av_frame_alloc();
            avpkt = av_packet_alloc();
            av_init_packet(avpkt);
            nalData = Marshal.AllocHGlobal(1024 * 1024);
            codec = avcodec_find_decoder(AVCodecID.AV_CODEC_ID_H264);
            avpkt->data = (void*)nalData;
        }

        ~FFPlayer()
        {
            // 过程中，下列对象只需释放一次
            if (null != frame)
                fixed (AVFrame** LPframe = &frame)
                    av_frame_free(LPframe);
            if (null != avpkt)
                fixed (AVPacket** LPpkt = &avpkt)
                    av_packet_free(LPpkt);
            if (default != nalData)
                Marshal.FreeHGlobal(nalData);
        }

        // 释放资源
        // 此函数并非表示“终止”，更多的是表示“改变”和“重置”，实际上对此函数的调用更多的是发生在界面大小发生变化时和网络掉包导致硬解异常时
        private void Releases()
        {
            // 过程中，下列对象会重复创建和销毁多次
            lock (_codecLocker)
            {
                if (_released) return;
                if (null != ctx)
                    fixed (AVCodecContext** LPctx = &ctx)
                        avcodec_free_context(LPctx);
                if (null != hw_ctx)
                    fixed (AVBufferRef** LPhw_ctx = &hw_ctx)
                        av_buffer_unref(LPhw_ctx);
                // (PS:device和surface我们将其置为null，让GC帮我们调用Finalize，它则会自行释放资源）
                surface = null;
                device = null;
                lastFmt = AVPixelFormat.AV_PIX_FMT_NONE;
                _released = true;
            }
        }

        // Load事件中保存控件句柄
        private void FFPlayer_Load(object sender, EventArgs e)
        {
            contentPanelHandle = Handle; // 这个句柄也可以是你控件内真正要渲染画面的句柄
            lastCBounds = ClientRectangle; // 同理，区域也不一定是自身显示区域
        }

        // 解码函数，由外部调用，送一一个分片好的nal
        public void H264Received(byte[] nal)
        {
            lock (_codecLocker)
            {
                // 判断界面大小更改了，先重置一波
                // (因为DirectX中界面大小改变是一件大事，没得法绕过，只能推倒从来）
                // 如果你的显示控件不是当前控件本身，此处需要做修改
                if (!ClientRectangle.Equals(lastCBounds))
                {
                    lastCBounds = ClientRectangle;
                    Releases();
                    return;
                }

                if (null == ctx)
                {
                    // 第一次接收到待解码数据时初始化一个解码器上下文
                    ctx = avcodec_alloc_context3(codec);
                    if (null == ctx)
                    {
                        return;
                    }
                    // 通过参数传递控件句柄给硬件加速上下文
                    AVDictionary* dic;
                    av_dict_set_int(&dic, "hWnd", contentPanelHandle.ToInt64(), 0);
                    fixed (AVBufferRef** LPhw_ctx = &hw_ctx)
                    {
                        if (av_hwdevice_ctx_create(LPhw_ctx, AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2,
                                                        null, dic, 0) < 0)
                        {
                            av_dict_free(&dic);
                            fixed (AVCodecContext** LPctx = &ctx)
                                avcodec_free_context(LPctx);
                            return;
                        }
                    }
                    av_dict_free(&dic);
                    ctx->hw_frames_ctx = av_buffer_ref(hw_ctx);
                    if (avcodec_open2(ctx, codec, null) < 0)
                    {
                        fixed (AVCodecContext** LPctx = &ctx)
                            avcodec_free_context(LPctx);
                        fixed (AVBufferRef** LPhw_ctx = &hw_ctx)
                            av_buffer_unref(LPhw_ctx);
                        return;
                    }
                }
                _released = false;

                // 开始解码
                Marshal.Copy(nal, 0, nalData, nal.Length);
                avpkt->size = nal.Length;
                if (avcodec_send_packet(ctx, avpkt) < 0)
                {
                    Releases(); return; // 如果程序走到了这里，一般是因为网络掉包导致nal数据不连续，没办法， 推倒从来
                }
            receive_frame:
                int err = avcodec_receive_frame(ctx, frame);
                if (err == -11) return; // EAGAIN
                if (err < 0)
                {
                    Releases(); return; // 同上，一般这里很少出错，但一旦发生，只能推倒从来
                }

                // 尝试播放一帧画面
                AVFrame s_frame = *frame;
                // 这里由于我无论如何都要加速，而一般显卡最兼容的是yv12格式，因此我只对dxva2和420p做了处理，如果你的h264解出来不是这些，我建议转成rgb（那你就需要编译和使用swscale模块了）
                if (s_frame.format != AVPixelFormat.AV_PIX_FMT_DXVA2_VLD && s_frame.format != AVPixelFormat.AV_PIX_FMT_YUV420P && s_frame.format != AVPixelFormat.AV_PIX_FMT_YUVJ420P) return;
                try
                {
                    int width = s_frame.width;
                    int height = s_frame.height;
                    if (lastIWidth != width || lastIHeight != height || lastFmt != s_frame.format) // 这个if判定的是第一次尝试渲染，因为一般码流的宽高和格式不会变
                    {
                        if (s_frame.format != AVPixelFormat.AV_PIX_FMT_DXVA2_VLD)
                        {
                            // 假如硬解不成功（例如h264是baseline的，ffmpeg新版不支持baseline的dxva2硬解）
                            // 我们就尝试用directx渲染yuv，至少省去yuv转rgb，可以略微节省一丢丢cpu
                            PresentParameters pp = new PresentParameters();
                            pp.Windowed = true;
                            pp.SwapEffect = SwapEffect.Discard;
                            pp.BackBufferCount = 0;
                            pp.DeviceWindowHandle = contentPanelHandle;
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
                            device = new Device(Manager.Adapters.Default.Adapter, DeviceType.Hardware, contentPanelHandle, behaviorFlas, pp);
                            //(Format)842094158;//nv12
                            surface = device.CreateOffscreenPlainSurface(width, height, (Format)842094169, Pool.Default);//yv12，显卡兼容性最好的格式
                        }
                        lastIWidth = width;
                        lastIHeight = height;
                        lastVRect = new Rectangle(0, 0, lastIWidth, lastIHeight);
                        lastFmt = s_frame.format;
                    }
                    if (lastFmt != AVPixelFormat.AV_PIX_FMT_DXVA2_VLD)
                    {
                        // 如果硬解失败，我们还需要把yuv拷贝到surface
                        //ffmpeg没有yv12，只有i420，而一般显卡又支持的是yv12，因此下文中uv分量是反向的
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

                    // 下面的代码开始烧脑了，如果是dxva2硬解出来的图像数据，则图像数据本身就是一个surface，并且它就绑定了device
                    // 因此我们可以直接用它，如果是x264软解出来的yuv，则我们需要用上文创建的device和surface搞事情
                    Surface _surface = lastFmt == AVPixelFormat.AV_PIX_FMT_DXVA2_VLD ? new Surface(s_frame.data4) : surface;
                    if (lastFmt == AVPixelFormat.AV_PIX_FMT_DXVA2_VLD)
                        GC.SuppressFinalize(_surface);// 这一句代码是点睛之笔，如果不加，程序一会儿就崩溃了，熟悉GC和DX的童鞋估计一下就能看出门道；整篇代码，就这句折腾了我好几天，其他都好说
                    Device _device = lastFmt == AVPixelFormat.AV_PIX_FMT_DXVA2_VLD ? _surface.Device : device;
                    _device.Clear(ClearFlags.Target, Color.Black, 1, 0);
                    _device.BeginScene();
                    Surface backBuffer = _device.GetBackBuffer(0, 0, BackBufferType.Mono);
                    _device.StretchRectangle(_surface, lastVRect, backBuffer, lastCBounds, TextureFilter.Linear);
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
                goto receive_frame; // 尝试解出第二幅画面（实际上不行，因为我们约定了单次传入nal是一个，当然，代码是可以改的）
            }
        }
        
        // 外部调用停止解码以显示释放资源
        public void Stop()
        {
            Releases();
        }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace huaanClient
{
    internal class CameraConfigPort
    {
        private static long imgIdx = 1;

        private TLVClient tlv;
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
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }


        private volatile string m_DevicVersion;
        /// <summary>
        /// 设备版本号
        /// </summary>
        public string DevicVersion { get => m_DevicVersion; }

        /// <summary>
        /// 设备序列号
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 设备在数据库的ID
        /// </summary>
        public string Deviceid { get; set; }

        /// <summary>
        /// 通过设备IP地址创建逻辑设备对象
        /// </summary>
        /// <param name="ip">设备IP地址</param>
        public CameraConfigPort(string ip)
        {
            IP = ip;
        }
        /// <summary>
        /// 获取设备平台
        /// </summary>
        public string platform { get; set; }

        /// <summary>
        /// 获取固件版本时间
        /// </summary>
        public string master_buildtime { get; set; }

        private object _lockIsConnected = new object();
        private volatile bool _isconnected;
        /// <summary>
        /// 与设备的网络连接是否正常
        /// </summary>
        public bool IsConnected
        {
            get
            {
                lock(_lockIsConnected)
                    return _isconnected;
            }
            set
            {
                lock (_lockIsConnected)
                    _isconnected = value;
            }
        }

        private object reconnectLocker = new object();
        private Timer reconnectTimer;

        NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// <para>尝试连接到设备</para>
        /// <para>如果当前设备不可达，程序会在后台尝试重连，直至连接成功或者调用Disconnect</para>
        /// <para>后续任何时间点或线程，可使用IsConnected判定设备网络连通性；精度为15秒</para>
        /// </summary>
        /// <returns>当此连接成功与否</returns>
        public bool Connect()
        {
            tlv = new TLVClient(IP, 9527);
            tlv.MessageReceived += Tlv_MessageReceived;
            tlv.Disconnected += Tlv_Disconnected;
            reconnectTimer = new Timer();
            reconnectTimer.Interval = 15 * 1000;
            reconnectTimer.Elapsed += ReconnectTimer_Elapsed; 
            reconnectTimer.Start();
            var c = tlv.Connect();
            IsConnected = c;
            if (c)
            {
                SendAuth();
                //取回设备版本
                string re = JsonInteractive("{\"cmd\": \"get version\",\"version\":123}");
                if (!string.IsNullOrEmpty(re))
                {
                    try
                    {
                        JObject jObject = (JObject)JsonConvert.DeserializeObject(re);
                        JToken alg_version = jObject["alg_version"];
                        if (alg_version != null)
                        {
                            m_DevicVersion = jObject["alg_version"].ToString().Trim();
                        }
                        JToken j_platform = jObject["platform"];
                        if (j_platform != null)
                        {
                            platform = jObject["platform"].ToString();
                        }
                        master_buildtime = jObject["master_buildtime"]?.ToString();

                    }
                    catch { }
                }
                
                
            }

            return IsConnected;
        }

        private void Tlv_Disconnected(TLVClient sender)
        {
            IsConnected = false;
        }

        private void ReconnectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (reconnectLocker)
            {
                lock(_lockIsConnected)
                if (needReconnect && IsConnected == false)
                {
                    tlv?.DisConnect();
                    tlv = new TLVClient(IP, 9527);
                    tlv.MessageReceived += Tlv_MessageReceived;
                    tlv.Disconnected += Tlv_Disconnected;
                    IsConnected = tlv.Connect();
                    if (IsConnected) SendAuth();
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
            needReconnect = false;
            tlv?.DisConnect();
        }

        ~CameraConfigPort()
        {
            DisConnect();
        }

        private static int SysType = 12;
        private static int Marjor = 2;
        private static int Minor = 11;

        private object jsonInteractiveLocker = new object();
        private string jsonRet;
        /// <summary>
        /// 与设备进行http协议JSON交互
        /// </summary>
        /// <param name="json2send">向设备发送的json数据</param>
        /// <param name="timeoutms">超时时间（默认3s）</param>
        /// <returns>设备处理之后返回的json数据；可能为string.empty表示超时（一般发生在设备未连接），其他情况请解析返回的json之后自行判定</returns>
        public string JsonInteractive(string json2send, int timeoutms = 3000)
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json2send);
            byte[] jsonLenBytes = BitConverter.GetBytes(jsonBytes.Length);
            byte[] v = new byte[4 + jsonBytes.Length];
            Array.Copy(jsonLenBytes, v, 4);
            Array.Copy(jsonBytes, 0, v, 4, jsonBytes.Length);
            tlv?.Write(SysType, Marjor, Minor, 50, v);
            lock (jsonInteractiveLocker)
            {
                jsonRet = string.Empty;
                Monitor.Wait(jsonInteractiveLocker, 10000);
            }
            return jsonRet;
        }

        private object getAllPersonIdLocker = new object();
        private List<string> allPersonId = new List<string>();
        /// <summary>
        /// 获取当前设备所有人员编号
        /// </summary>
        /// <param name="timeoutms">超时时间</param>
        /// <returns>当前设备所有人员编号</returns>
        public List<string> GetAllPersonId(int timeoutms = 3000)
        {
            tlv?.Write(SysType, Marjor, Minor, 400, null);
            lock (getAllPersonIdLocker)
            {
                allPersonId.Clear();
                Monitor.Wait(getAllPersonIdLocker, timeoutms);
            }
            return allPersonId;
        }

        [DllImport("msvcrt", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        static unsafe extern void memcpy(void *dest, byte[] src, int count);
        [DllImport("msvcrt", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        static unsafe extern void memcpy(byte[] dest, void *src, int count);
        private unsafe struct ListSnapCriteria
        {
            public int page_no;
            public int page_size;
            public sbyte img_flag;
            public sbyte model_img_flag;
            public fixed byte resv[6];
            public ushort condition_flag;
            public ushort fuzzy_flag;
            public uint time_start;
            public uint time_end;
            public fixed byte id[20];
            public ListSnapCriteria(DateTime timeStart, DateTime timeEnd, string personId)
            {
                page_no = 1;
                page_size = 20;
                img_flag = 1;
                model_img_flag = 1;
                condition_flag = 0x1;
                fuzzy_flag = 0;
                time_start = Convert.ToUInt32(timeStart.ToUniversalTime().Subtract(DateTime.Parse("1970-1-1")).TotalSeconds);
                time_end = Convert.ToUInt32(timeEnd.ToUniversalTime().Subtract(DateTime.Parse("1970-1-1")).TotalSeconds);
                if (!string.IsNullOrEmpty(personId)) {
                    condition_flag = 0x11;
                    byte[] idStrBytes = Encoding.UTF8.GetBytes(personId);
                    fixed (byte* ptrId = id)
                    memcpy(ptrId, idStrBytes, Math.Min(idStrBytes.Length, 20));
                }

            }
        }
        /*private unsafe struct ListSnapResponseItem
        {
            public int total;
            public int idx;
            public uint seq;
            public uint sec;
            public uint usec;
            public short match_result;
            public byte sex;
            public byte age;
            public fixed sbyte id[20];
            public fixed sbyte name[16];
            public int faceImgLen;
            public ushort faceX;
            public ushort faceY;
            public ushort faceW;
            public ushort faceH;
            public int regImgLen;
            public byte qvalue;
            public sbyte upload;
            public sbyte role;
            public byte aes;
            public uint match_type;
            public fixed sbyte id_long[64];
            public fixed sbyte name_long[64];
            public float temp;
        }*/
        /// <summary>
        /// 结构体转byte数组
        /// </summary>
        /// <param name="structObj">要转换的结构体</param>
        /// <returns>转换后的byte数组</returns>
        public static byte[] StructToBytes(object structObj)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(structObj);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;
        }
        /// <summary>
        /// byte数组转结构体
        /// </summary>
        /// <param name="bytes">byte数组</param>
        /// <param name="type">结构体类型</param>
        /// <returns>转换后的结构体</returns>
        public static object BytesToStuct(byte[] bytes, Type type)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(type);
            //byte数组长度小于结构体的大小
            if (size > bytes.Length)
            {
                //返回空
                return null;
            }
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return obj;
        }
        public static DateTime ConvertToDateTime(uint seconds, uint useconds)
        {
            var milliseconds = useconds * 0.001;
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(seconds).AddMilliseconds(milliseconds).ToLocalTime();
            return dtDateTime;
        }

        /*private List<(DateTime captureTime, int matchScore, string personName, int personRole, float temperature)> _queriedRecords = new List<(DateTime captureTime, int matchScore, string personName, int personRole, float temperature)>();
        private object recordsLocker = new object();
       
        private static int recordsTotal;
        /// <summary>
        /// 获取当前设备特定人员在特定时间区间内的打卡记录
        /// </summary>
        /// <param name="timeStart">时间区间开始</param>
        /// <param name="timeEnd">时间区间结束</param>
        /// <param name="personId">人员编号</param>
        /// <param name="timeoutms">超时时间</param>
        /// <returns>当前设备特定人员在特定时间区间内的打卡记录</returns>
        public List<(DateTime captureTime, int matchScore, string personName, int personRole, float temperature)> GetRecords(DateTime timeStart, DateTime timeEnd, string personId, int timeoutms = 3000)
        {
            lock (this)
            {
                int page_no = 1;
                List<(DateTime captureTime, int matchScore, string personName, int personRole, float temperature)> ret = new List<(DateTime captureTime, int matchScore, string personName, int personRole, float temperature)>();
                ListSnapCriteria listSnapCriteria = new ListSnapCriteria(timeStart, timeEnd, personId);
                recordsTotal = 100;
            lblRecords:
                listSnapCriteria.page_no = page_no;
                byte[] criteriaBytes = StructToBytes(listSnapCriteria);
                tlv?.Write(SysType, Marjor, Minor, 217, criteriaBytes);
                lock (recordsLocker)
                {
                    _queriedRecords.Clear();
                    if (!Monitor.Wait(recordsLocker, timeoutms))
                    {
                        _queriedRecords.Clear();
                    }
                }
                if (_queriedRecords.Count < 1)
                {
                    return ret;
                }
                ret.AddRange(_queriedRecords);
                if (recordsTotal > ret.Count)
                {
                    page_no++;
                    goto lblRecords;
                }
                return ret;
            }
        }*/
        public List<(DateTime captureTime, int matchScore, string personName, int personRole, float temperature)> GetRecords(DateTime timeStart, DateTime timeEnd, string personId, int timeoutms = 3000)
        {
            return new List<(DateTime captureTime, int matchScore, string personName, int personRole, float temperature)>();
        }

        private List<CaptureDataEventArgs> _queriedRecords = new List<CaptureDataEventArgs>();
        private object recordsLocker = new object();

        private static int recordsTotal;
        /// <summary>
        /// 获取当前设备特定时间段的打卡记录
        /// </summary>
        /// <param name="timeStart">时间区间开始</param>
        /// <param name="timeEnd">时间区间结束</param>
        /// <param name="timeoutms">每接收100条数据超时时间</param>
        /// <param name="timeouttotal">当此查询总共的超时时间（会在每次分页完成后判定）</param>
        /// <returns>当前设备特定时间区间内的打卡记录</returns>
        public (int count, DateTime lastRecordTime) GetRecords(DateTime timeStart, DateTime timeEnd, CancellationToken token)
        {
            var recordsCount = 0;
            var lastRecordTime = timeStart;

            using (var client = new Api.Client(this.IP))
            {
                client.OnRecordReceived += (sender, e) =>
                {
                    if (e.records != null)
                    {
                        foreach (var item in e.records)
                        {
                            var r = new CaptureDataEventArgs();
                            r.person_id = item.id;
                            r.person_name = item.name ?? item.person_name_ext;
                            r.time = DateTime.Parse(item.time);
                            if (!string.IsNullOrEmpty(item.face_image))
                                r._closeup = Convert.FromBase64String(item.face_image);
                            if (!string.IsNullOrEmpty(item.reg_image))
                                r.regImage = Convert.FromBase64String(item.reg_image);
                            r.sequnce = (uint)item.sequence;
                            r.person_role = (PersonRole)item.role;
                            r.match_status = (short)item.score;
                            r.match_failed_reson = (MatchFailedReason)item.match_failed_reson;
                            r.body_temp = item.body_temp;
                            if (r._closeup != null)
                                SaveCloseup(r);
                            
                            HandleCaptureData.setCaptureDataToDatabase(r, DeviceNo, DeviceName);
                            if (r.time > lastRecordTime)
                            {
                                lastRecordTime = r.time;
                            }
                            
                            recordsCount++;
                        }
                    }
                    
                };
                client.QueryCaptureRecord(5, 1000, timeStart, timeEnd, true, true, token);
            }

            Logger.Debug($"device: {this.IP}, time: {timeStart}-{timeEnd}, count: {recordsCount}");
            return (recordsCount, lastRecordTime);
        }

        public enum PersonRole : int
        {
            /// <summary>
            /// 普通人员
            /// </summary>
            NORMAL = 0,
            /// <summary>
            /// 白名单
            /// </summary>
            WHITE,
            /// <summary>
            /// 黑名单
            /// </summary>
            BLACK
        }

        public enum HatColor : int
        {
            NONE = 0,
            /// <summary>
            /// 蓝色安全帽
            /// </summary>
            BLUE,
            /// <summary>
            /// 橙色安全帽
            /// </summary>
            ORANGE,
            /// <summary>
            /// 红色安全帽
            /// </summary>
            RED,
            /// <summary>
            /// 白色安全帽
            /// </summary>
            WHITE,
            /// <summary>
            /// 黄色安全帽
            /// </summary>
            YELLOW
        }

        public enum MatchMode : int
        {
            NULL = 0,
            /// <summary>
            /// 刷脸
            /// </summary>
            NORMAL,
            /// <summary>
            /// 刷身份证（人证模式）
            /// </summary>
            IDCARD_1TO1,
            /// <summary>
            /// 刷脸+刷身份证（白名单）
            /// </summary>
            FACE_IDCARD,
            /// <summary>
            /// 刷卡
            /// </summary>
            WGCARD,
            /// <summary>
            /// 刷脸+刷卡
            /// </summary>
            FACE_WGCARD,
            /// <summary>
            /// 过人开闸
            /// </summary>
            ANY_FACE,
            /// <summary>
            /// 刷脸或者刷卡
            /// </summary>
            NORMAL_OR_WGCARD,
            /// <summary>
            /// 刷脸或刷身份证
            /// </summary>
            NORMAL_OR_IDCARD_1TO1,
            /// <summary>
            /// 刷卡并截图
            /// </summary>
            WGCARD_SNAPSHOT,
            /// <summary>
            /// 刷脸或（刷脸+刷身份证）
            /// </summary>
            NORMAL_OR_FACE_IDCARD,
            /// <summary>
            /// 刷身份证（白名单）
            /// </summary>
            IDCARD_NOONLY,
            /// <summary>
            /// 刷身份证(不需要比对)
            /// </summary>
            IDCARD_NOT_MATCH_IN_DB,
            /// <summary>
            /// 刷脸或刷身份证(不需要比对)
            /// </summary>
            NORMAL_OR_IDCARD_NOT_MATCH_IN_DB,
            /// <summary>
            /// 刷脸或RFID全景快照
            /// </summary>
            NORMAL_OR_SNAPSHOT = 20,
            /// <summary>
            /// 只测温开闸
            /// </summary>
            ONLY_TEMPERATURE
        }

        public enum MatchFailedReason : int
        {
            NULL = 0,
            /// <summary>
            /// 限制人员
            /// </summary>
            NOT_WHITE = -2,
            /// <summary>
            /// 人员过期
            /// </summary>
            EXPIRE = -3,
            /// <summary>
            /// 不在调度时间
            /// </summary>
            UN_CHEDULES = -4,
            /// <summary>
            /// 节假日
            /// </summary>
            FESTIVAL = -5,
            /// <summary>
            /// 温度过高
            /// </summary>
            ABN_TEMPERATURE = -6,
            /// <summary>
            /// 口罩
            /// </summary>
            MASK = -7,
            /// <summary>
            /// 未带安全帽
            /// </summary>
            WITHOUT_HAT = -8,
            /// <summary>
            /// 卡号未注册
            /// </summary>
            INVAILED_CARD = -9,
            /// <summary>
            /// 人证不匹配
            /// </summary>
            UMMATCHED_ID = -10,
            /// <summary>
            /// 未授权
            /// </summary>
            NOAUTH = -11
        }

        public sealed class CaptureDataEventArgs : EventArgs
        {
            /// <summary>
            /// 序号
            /// </summary>
            public uint sequnce;
            /// <summary>
            /// 相机编号
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string device_id;
            /// <summary>
            /// 点位编号
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string addr_id;
            /// <summary>
            /// 点位名称
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string addr_name;
            /// <summary>
            /// 抓拍时间
            /// </summary>
            public DateTime time;
            /// <summary>
            /// <para>分数（大于0的表示对比成功）</para>
            /// <para>为0表示对比失败</para>
            /// <para>-1表示未对比</para>
            /// 对比失败一般是陌生人，未对比可能是人脸质量太差，角度太大等原因
            /// </summary>
            public short match_status;
            /// <summary>
            /// 人员编号
            /// </summary>
            public string person_id;
            /// <summary>
            /// 人员姓名
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string person_name;
            /// <summary>
            /// 人员角色
            /// </summary>
            public PersonRole person_role;
            /// <summary>
            /// 全景图数据，可能为null
            /// </summary>
            public byte[] overall;
            /// <summary>
            /// 人脸坐标（全景图内）
            /// </summary>
            public Rectangle face_region_overall;
            /// <summary>
            /// 特写图数据，可能为null
            /// </summary>
            public byte[] _closeup;
            /// <summary>
            /// 特写图路径
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string closeup;
            /// <summary>
            /// 注册（缩略）图数据，可能为null
            /// </summary>
            public byte[] regImage;
            /// <summary>
            /// 人脸坐标（特写图内）
            /// </summary>
            public Rectangle face_region_closeup;
            /// <summary>
            /// 安全帽颜色
            /// </summary>
            public HatColor hatColor;
            /// <summary>
            /// 对比成功类型
            /// </summary>
            public MatchMode match_type;
            /// <summary>
            /// 32位韦根卡号
            /// </summary>
            public uint wg_card_id;
            /// <summary>
            /// 64位韦根卡号
            /// </summary>
            public ulong long_card_id;
            /// <summary>
            /// 不通过原因
            /// </summary>
            public MatchFailedReason match_failed_reson;
            /// <summary>
            /// 是否佩戴口罩
            /// </summary>
            public bool exist_mask;
            /// <summary>
            /// 体温
            /// </summary>
            public float body_temp;
            /// <summary>
            /// 设备序列号
            /// </summary>
            public string device_sn;
            /// <summary>
            /// 身份证号
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string idcard_number;
            /// <summary>
            /// 身份证姓名
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string idcard_name;
            /// <summary>
            /// 身份证生日
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string idcard_birth;
            /// <summary>
            /// 身份证性别
            /// </summary>
            public byte idcard_sex;
            /// <summary>
            /// 身份证民族
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string idcard_national;
            /// <summary>
            /// 身份证居住地址
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string idcard_residence_address;
            /// <summary>
            /// 身份证签发机关
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string idcard_organ_issue;
            /// <summary>
            /// 身份证有效期起始
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string idcard_valid_date_start;
            /// <summary>
            /// 身份证有效期结束
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string idcard_valid_date_end;
            /// <summary>
            /// 自定义字段
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string customer_text;
            /// <summary>
            /// 二维码内容
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string qr_code;
            /// <summary>
            /// 长姓名
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string person_name_ext;
            /// <summary>
            /// 健康码状态
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string QRcodestatus;
            /// <summary>
            /// 健康码
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string QRcode;
            /// <summary>
            /// 行程信息
            /// </summary>
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string trip_infor;
        }

        /// <summary>
        /// 收到设备抓拍数据时
        /// </summary>
        public event EventHandler<CaptureDataEventArgs> CaptureData;

        private object snapshortLocker = new object();
        private Image[] snapshotRet;
        /// <summary>
        /// <para>从设备截图</para>
        /// <para>如果返回值为空，则截图失败</para>
        /// </summary>
        /// <param name="timeoutms">超时时间</param>
        /// <returns>null或设备采集画面。第一张图为可见光，第二张图为红外成像（或许没有）</returns>
        public Image[] Snapshot(int timeoutms = 3000)
        {
            tlv?.Write(SysType, Marjor, Minor, 210, null);
            lock (snapshortLocker)
            {
                snapshotRet = null;
                Monitor.Wait(snapshortLocker, timeoutms);
            }
            return snapshotRet;
        }

        private object ntpLocker = new object();
        private byte[] ntpData = null;
        private bool ntpSetFlag = false;
        /// <summary>
        /// 开关NTP
        /// </summary>
        /// <param name="onoff">开启标识</param>
        /// <returns>是否成功</returns>
        public bool ToggleNtpEnable(bool onoff)
        {
            UdpClient uc = new UdpClient();
            uc.Send(new byte[] { 0xe9, 0x43, 0x00, 0x64, 0x00, 0x00, 0x00, 0x00 }, 8, new IPEndPoint(IPAddress.Parse(IP), 9527));
            var asyncResult = uc.BeginReceive(null, null);
            asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
            byte[] daemonData = null;
            if (asyncResult.IsCompleted)
            {
                try
                {
                    IPEndPoint remoteEP = null;
                    daemonData = uc.EndReceive(asyncResult, ref remoteEP);
                }
                catch
                {
                }
            }
            if(daemonData == null)
            {
                uc.Send(new byte[] { 0xe9, 0x43, 0x00, 0x64, 0x00, 0x00, 0x00, 0x00 }, 8, new IPEndPoint(IPAddress.Parse(IP), 9527));
                asyncResult = uc.BeginReceive(null, null);
                asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
                if (asyncResult.IsCompleted)
                {
                    try
                    {
                        IPEndPoint remoteEP = null;
                        daemonData = uc.EndReceive(asyncResult, ref remoteEP);
                    }
                    catch
                    {
                    }
                }
            }
            if (daemonData != null)
            {
                int enable = BitConverter.ToInt32(daemonData, 0x274);
                int interval = BitConverter.ToInt32(daemonData, 0x288);
                if(enable != -1 && interval != -1) // dv300的ntp参数已不在daemon中，所以将enable/interval都置为-1，表示通过face来配置！
                {
                    byte[] header = new byte[] { 0xea, 0x43, 0x00, 0x64, 0xc0, 0x02, 0x00, 0x00, 0xbc, 0x02, 0x00, 0x00 };
                    byte[] sendData = new byte[header.Length + 0x2bc];
                    Array.Copy(header, sendData, header.Length);
                    daemonData[0x274] = (byte)(onoff ? 1 : 0);
                    Array.Copy(daemonData, 0x14, sendData, header.Length, 0x2bc);
                    // 发送两次来保险
                    uc.Send(sendData, sendData.Length, new IPEndPoint(IPAddress.Parse(IP), 9527));
                    uc.Send(sendData, sendData.Length, new IPEndPoint(IPAddress.Parse(IP), 9527));
                    return true;
                }
            }

            tlv?.Write(SysType, Marjor, Minor, 0xd, null);
            lock (ntpLocker)
            {
                ntpData = null;
                Monitor.Wait(ntpLocker, 500);
            }
            if (ntpData == null) return false;

            ntpData[0] = (byte)(onoff ? 1 : 0);
            tlv?.Write(SysType, Marjor, Minor, 0xe, ntpData);
            lock (ntpLocker)
            {
                ntpSetFlag = false;
                Monitor.Wait(ntpLocker, 500);
            }
            return ntpSetFlag;
        }

        /// <summary>
        /// 获取Ntp开关状态
        /// </summary>
        /// <returns>ntp功能是否打开</returns>
        public bool GetNtpOnoff()
        {
            UdpClient uc = new UdpClient();
            uc.Send(new byte[] { 0xe9, 0x43, 0x00, 0x64, 0x00, 0x00, 0x00, 0x00 }, 8, new IPEndPoint(IPAddress.Parse(IP), 9527));
            var asyncResult = uc.BeginReceive(null, null);
            asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
            byte[] daemonData = null;
            if (asyncResult.IsCompleted)
            {
                try
                {
                    IPEndPoint remoteEP = null;
                    daemonData = uc.EndReceive(asyncResult, ref remoteEP);
                }
                catch
                {
                }
            }
            if (daemonData == null)
            {
                uc.Send(new byte[] { 0xe9, 0x43, 0x00, 0x64, 0x00, 0x00, 0x00, 0x00 }, 8, new IPEndPoint(IPAddress.Parse(IP), 9527));
                asyncResult = uc.BeginReceive(null, null);
                asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
                if (asyncResult.IsCompleted)
                {
                    try
                    {
                        IPEndPoint remoteEP = null;
                        daemonData = uc.EndReceive(asyncResult, ref remoteEP);
                    }
                    catch
                    {
                    }
                }
            }
            if (daemonData != null)
            {
                int enable = BitConverter.ToInt32(daemonData, 0x274);
                int interval = BitConverter.ToInt32(daemonData, 0x288);
                if (enable != -1 && interval != -1) // dv300的ntp参数已不在daemon中，所以将enable/interval都置为-1，表示通过face来配置！
                {
                    return enable != 0;
                }
            }

            tlv?.Write(SysType, Marjor, Minor, 0xd, null);
            lock (ntpLocker)
            {
                ntpData = null;
                Monitor.Wait(ntpLocker, 500);
            }
            if (ntpData == null) return false;

            return ntpData[0] != 0;
        }

        /// <summary>
        /// 获取设备网络信息（有线）
        /// </summary>
        /// <param name="ip">设备ip地址</param>
        /// <param name="gateway">设备默认网关</param>
        /// <param name="netmask">设备子网掩码</param>
        /// <param name="dns">设备域名解析服务器</param>
        /// <returns>是否获取成功</returns>
        public bool GetNetworkInfo(out string ip, out string gateway, out string netmask, out string dns)
        {
            UdpClient uc = new UdpClient();
            uc.Send(new byte[] { 0xe9, 0x43, 0x00, 0x64, 0x00, 0x00, 0x00, 0x00 }, 8, new IPEndPoint(IPAddress.Parse(IP), 9527));
            var asyncResult = uc.BeginReceive(null, null);
            asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
            byte[] daemonData = null;
            if (asyncResult.IsCompleted)
            {
                try
                {
                    IPEndPoint remoteEP = null;
                    daemonData = uc.EndReceive(asyncResult, ref remoteEP);
                }
                catch
                {
                }
            }
            if (daemonData == null)
            {
                uc.Send(new byte[] { 0xe9, 0x43, 0x00, 0x64, 0x00, 0x00, 0x00, 0x00 }, 8, new IPEndPoint(IPAddress.Parse(IP), 9527));
                asyncResult = uc.BeginReceive(null, null);
                asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
                if (asyncResult.IsCompleted)
                {
                    try
                    {
                        IPEndPoint remoteEP = null;
                        daemonData = uc.EndReceive(asyncResult, ref remoteEP);
                    }
                    catch
                    {
                    }
                }
            }
            if (daemonData != null)
            {
                ip = Encoding.Default.GetString(daemonData, 0x28, 20).Trim('\0').Trim();
                netmask = Encoding.Default.GetString(daemonData, 0x28 + 20, 20).Trim('\0').Trim();
                gateway = Encoding.Default.GetString(daemonData, 0x28 + 20 + 20, 20).Trim('\0').Trim();
                dns = Encoding.Default.GetString(daemonData, 0x28 + 20 + 20 + 20 + 16 + 32 + 32 + 64 + 16 + 16
                    , 16).Trim('\0').Trim();
                return true;
            }
            ip = null;
            gateway = null;
            netmask = null;
            dns = null;
            return false;
        }

        /// <summary>
        /// 设置设备网络参数（有线）
        /// </summary>
        /// <param name="ip">设备ip地址</param>
        /// <param name="gateway">设备默认网关</param>
        /// <param name="netmask">设备子网掩码</param>
        /// <param name="dns">设备域名解析服务器地址</param>
        /// <returns>是否设置成功</returns>
        public bool SetNetworkInfo(string ip, string gateway, string netmask, string dns)
        {
            UdpClient uc = new UdpClient();
            uc.Send(new byte[] { 0xe9, 0x43, 0x00, 0x64, 0x00, 0x00, 0x00, 0x00 }, 8, new IPEndPoint(IPAddress.Parse(IP), 9527));
            var asyncResult = uc.BeginReceive(null, null);
            asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
            byte[] daemonData = null;
            if (asyncResult.IsCompleted)
            {
                try
                {
                    IPEndPoint remoteEP = null;
                    daemonData = uc.EndReceive(asyncResult, ref remoteEP);
                }
                catch
                {
                }
            }
            if (daemonData == null)
            {
                uc.Send(new byte[] { 0xe9, 0x43, 0x00, 0x64, 0x00, 0x00, 0x00, 0x00 }, 8, new IPEndPoint(IPAddress.Parse(IP), 9527));
                asyncResult = uc.BeginReceive(null, null);
                asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
                if (asyncResult.IsCompleted)
                {
                    try
                    {
                        IPEndPoint remoteEP = null;
                        daemonData = uc.EndReceive(asyncResult, ref remoteEP);
                    }
                    catch
                    {
                    }
                }
            }
            if (daemonData != null)
            {
                byte[] byte20 = new byte[20];
                byte[] byte16 = new byte[16];
                byte[] header = new byte[] { 0xea, 0x43, 0x00, 0x64, 0xc0, 0x02, 0x00, 0x00, 0xbc, 0x02, 0x00, 0x00 };
                byte[] sendData = new byte[header.Length + 0x2bc];
                Array.Copy(header, sendData, header.Length);
                Array.Copy(daemonData, 0x14, sendData, header.Length, 0x2bc);
                Array.Copy(byte20, 0, sendData, 0x20, 20);
                byte[] bip = Encoding.Default.GetBytes(ip);
                Array.Copy(bip, 0, sendData, 0x20, bip.Length);
                Array.Copy(byte20, 0, sendData, 0x20 + 20, 20);
                byte[] bnm = Encoding.Default.GetBytes(netmask);
                Array.Copy(bnm, 0, sendData, 0x20 + 20, bnm.Length);
                Array.Copy(byte20, 0, sendData, 0x20 + 20 + 20, 20);
                byte[] bgw = Encoding.Default.GetBytes(gateway);
                Array.Copy(bgw, 0, sendData, 0x20 + 20 + 20, bgw.Length);
                Array.Copy(byte16, 0, sendData, 0x20 + 20 + 20 + 20 + 16 + 32 + 32 + 64 + 16 + 16, 16);
                byte[] bdns = Encoding.Default.GetBytes(dns);
                Array.Copy(bdns, 0, sendData, 0x20 + 20 + 20 + 20 + 16 + 32 + 32 + 64 + 16 + 16, bdns.Length);
                sendData[0x20 + 20 + 20 + 20 + 16 + 32 + 32 + 64 + 16 + 16 + 16] = 0;
                uc.Send(sendData, sendData.Length, new IPEndPoint(IPAddress.Parse(IP), 9527));
                asyncResult = uc.BeginReceive(null, null);
                asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
                if (asyncResult.IsCompleted)
                {
                    try
                    {
                        IPEndPoint remoteEP = null;
                        daemonData = uc.EndReceive(asyncResult, ref remoteEP);
                        int ack = BitConverter.ToInt32(daemonData, 12);
                        return ack == 0;
                    }
                    catch
                    {
                    }
                }
                else
                {
                    uc.Send(sendData, sendData.Length, new IPEndPoint(IPAddress.Parse(IP), 9527));
                    asyncResult = uc.BeginReceive(null, null);
                    asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
                    if (asyncResult.IsCompleted)
                    {
                        try
                        {
                            IPEndPoint remoteEP = null;
                            daemonData = uc.EndReceive(asyncResult, ref remoteEP);
                            int ack = BitConverter.ToInt32(daemonData, 12);
                            return ack == 0;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return false;
        }

        private unsafe void Tlv_MessageReceived(TLVClient sender, int sysType, int majorVersion, int minorVersion, int msgType, int? ack, byte[] _v)
        {
            if(msgType == 0x0E)
            {
                ntpSetFlag = ack == 0;
                lock (ntpLocker)
                {
                    Monitor.Pulse(ntpLocker);
                }
            }
            if(msgType == 0x0D)
            {
                ntpData = _v;
                lock(ntpLocker)
                {
                    Monitor.Pulse(ntpLocker);
                }
            }
            if (msgType == 50 && _v?.Length >= 4)
            {
                lock(jsonInteractiveLocker)
                {
                    int jsonLen = BitConverter.ToInt32(_v, 0);
                    if (_v.Length >= 4 + jsonLen)
                    {
                        jsonRet = Encoding.UTF8.GetString(_v, 4, jsonLen).Trim('\0');
                    }
                    Monitor.Pulse(jsonInteractiveLocker);
                }
            }
            if (msgType == 400 && _v?.Length >= 4)
            {
                lock (getAllPersonIdLocker)
                {
                    int idCount = BitConverter.ToInt32(_v, 0);
                    if (_v.Length >= 4 + idCount * 20)
                    {
                        for (int i = 0; i < idCount; ++i)
                            allPersonId.Add(Encoding.UTF8.GetString(_v, 4 + i * 20, 20).Trim('\0'));
                    }
                    Monitor.Pulse(getAllPersonIdLocker);
                }
            }
            if (msgType == 217 && _v?.Length >= 8)
            {
                /*lock (recordsLocker)
                {
                    int idx = BitConverter.ToInt32(_v, 4);
                    if(idx == 0)
                    {
                        Monitor.Pulse(recordsLocker);
                        return;
                    }
                    ListSnapResponseItem item = (ListSnapResponseItem)BytesToStuct(_v, typeof(ListSnapResponseItem));
                    recordsTotal = item.total;
                    byte[] iname = new byte[20];
                    memcpy(iname, item.name, 20);
                    _queriedRecords.Add((
                        ConvertToDateTime(item.sec, item.usec)
                        , item.match_result
                        , Encoding.UTF8.GetString(iname).TrimEnd('\0')
                        , item.role
                        , item.temp
                        ));
                }*/
                lock (recordsLocker)
                {
                    int idx = BitConverter.ToInt32(_v, 4);
                    if (idx == 0)
                    {
                        Monitor.Pulse(recordsLocker);
                        return;
                    }
                    CaptureDataEventArgs e = new CaptureDataEventArgs();
                    idx = 0;
                    recordsTotal = BitConverter.ToInt32(_v, idx);
                    idx += 4;
                    idx += 4; // idx
                    e.sequnce = BitConverter.ToUInt32(_v, idx);
                    idx += 4;
                    uint tv_sec = BitConverter.ToUInt32(_v, idx);
                    idx += 4;
                    uint tv_usec = BitConverter.ToUInt32(_v, idx);
                    idx += 4;
                    e.time = ConvertToDateTime(tv_sec, tv_usec);
                    e.match_status = BitConverter.ToInt16(_v, idx);
                    idx += 2;
                    idx += 2; // sex age
                    e.person_id = Encoding.UTF8.GetString(_v, idx, 20).TrimEnd('\0');
                    idx += 20;
                    e.person_name = Encoding.UTF8.GetString(_v, idx, 16).TrimEnd('\0');
                    idx += 16;
                    int exist_closeup = BitConverter.ToInt32(_v, idx);
                    idx += 4;
                    if (exist_closeup > 0)
                    {
                        e._closeup = new byte[exist_closeup];
                        Array.Copy(_v, idx, e._closeup, 0, exist_closeup);
                        idx += exist_closeup;
                    }
                    e.face_region_closeup = new Rectangle();
                    e.face_region_closeup.X = BitConverter.ToUInt16(_v, idx);
                    idx += 2;
                    e.face_region_closeup.Y = BitConverter.ToUInt16(_v, idx);
                    idx += 2;
                    e.face_region_closeup.Width = BitConverter.ToUInt16(_v, idx);
                    idx += 2;
                    e.face_region_closeup.Height = BitConverter.ToUInt16(_v, idx);
                    idx += 2;
                    int exist_reg = BitConverter.ToInt32(_v, idx);
                    idx += 4;
                    if (exist_reg > 0)
                    {
                        e.regImage = new byte[exist_reg];
                        Array.Copy(_v, idx, e.regImage, 0, exist_reg);
                        idx += exist_reg;
                    }
                    idx += 2; // q值 上传状态
                    e.person_role = (PersonRole)_v[idx];
                    idx += 2; // 角色，aes
                    e.match_type = (MatchMode)BitConverter.ToUInt32(_v, idx);
                    idx += 4;
                    e.customer_text = Encoding.UTF8.GetString(_v, idx, 64).TrimEnd('\0');
                    idx += 64;
                    e.person_name_ext = Encoding.UTF8.GetString(_v, idx, 64).TrimEnd('\0');
                    idx += 64;
                    e.body_temp = BitConverter.ToSingle(_v, idx);
                    idx += 4;
                    e.match_failed_reson = (MatchFailedReason)BitConverter.ToInt32(_v, idx);
 
                    try
                    {
                        idx += 70;
                        if (_v.Length- idx>511)
                        {
                            e.trip_infor = Encoding.UTF8.GetString(_v, idx, 512).TrimEnd('\0').Trim();
                        }

                        idx += 512;
                        if (idx < _v.Length)
                        {
                            e.exist_mask = BitConverter.ToInt32(_v, idx) == 1;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {

                    }
                    _queriedRecords.Add(e);
                    if(e._closeup != null)
                    {
                        SaveCloseup(e);
                    }
                }
            }
            if (msgType == 5 && CaptureData != default)
            {
                if(Properties.Settings.Default.receiveRealTimeData)
                {
                    CaptureDataEventArgs e = new CaptureDataEventArgs();
                    int idx = 0;
                    e.sequnce = BitConverter.ToUInt32(_v, idx);
                    idx += 4;
                    e.device_id = Encoding.UTF8.GetString(_v, idx, 32).TrimEnd('\0');
                    idx += 32;
                    e.addr_id = Encoding.UTF8.GetString(_v, idx, 32).TrimEnd('\0');
                    idx += 32;
                    e.addr_name = Encoding.UTF8.GetString(_v, idx, 96).TrimEnd('\0');
                    idx += 96;
                    uint tv_sec = BitConverter.ToUInt32(_v, idx);
                    idx += 4;
                    uint tv_usec = BitConverter.ToUInt32(_v, idx);
                    idx += 4;
                    e.time = ConvertToDateTime(tv_sec, tv_usec);
                    idx += 2; // is_realtime
                    e.match_status = BitConverter.ToInt16(_v, idx);
                    idx += 2;
                    if (e.match_status > 0)
                    {
                        e.person_id = Encoding.UTF8.GetString(_v, idx, 20).TrimEnd('\0');
                        idx += 20;
                        e.person_name = Encoding.UTF8.GetString(_v, idx, 16).TrimEnd('\0');
                        idx += 16;
                        e.person_role = (PersonRole)BitConverter.ToInt32(_v, idx);
                        idx += 4;
                    }
                    int exist_overall = BitConverter.ToInt32(_v, idx);
                    idx += 4;
                    if (exist_overall > 0)
                    {
                        idx += 4;//format
                        e.overall = new byte[BitConverter.ToInt32(_v, idx)];
                        idx += 4;
                        e.face_region_overall = new Rectangle();
                        e.face_region_overall.X = BitConverter.ToUInt16(_v, idx);
                        idx += 2;
                        e.face_region_overall.Y = BitConverter.ToUInt16(_v, idx);
                        idx += 2;
                        e.face_region_overall.Width = BitConverter.ToUInt16(_v, idx);
                        idx += 2;
                        e.face_region_overall.Height = BitConverter.ToUInt16(_v, idx);
                        idx += 2;
                    }
                    int exist_closeup = BitConverter.ToInt32(_v, idx);
                    idx += 4;
                    if (exist_closeup > 0)
                    {
                        idx += 4;//format
                        e._closeup = new byte[BitConverter.ToInt32(_v, idx)];
                        idx += 4;
                        e.face_region_closeup = new Rectangle();
                        e.face_region_closeup.X = BitConverter.ToUInt16(_v, idx);
                        idx += 2;
                        e.face_region_closeup.Y = BitConverter.ToUInt16(_v, idx);
                        idx += 2;
                        e.face_region_closeup.Width = BitConverter.ToUInt16(_v, idx);
                        idx += 2;
                        e.face_region_closeup.Height = BitConverter.ToUInt16(_v, idx);
                        idx += 2;
                    }
                    idx += 4;//exist_video
                    try
                    {
                        idx += 8;//性别 年龄 表情 肤色 q值 注册来源 有效属性 活体标记
                        e.hatColor = (HatColor)_v[idx];
                        idx += 1;
                        idx += 2;//角度
                        idx += 1;//加密
                        e.match_type = (MatchMode)BitConverter.ToUInt32(_v, idx);
                        idx += 4;
                        e.wg_card_id = BitConverter.ToUInt32(_v, idx);
                        idx += 4;
                        e.long_card_id = BitConverter.ToUInt64(_v, idx);
                        idx += 8;
                        idx += 36;//gps
                        e.match_failed_reson = (MatchFailedReason)BitConverter.ToInt32(_v, idx);
                        idx += 4;
                        byte _tmp_byte = _v[idx];
                        if (_tmp_byte == 1)
                        {
                            e.QRcodestatus = "0";
                        }
                        else if (_tmp_byte == 2)
                        {
                            e.QRcodestatus = "2";
                        }
                        else if (_tmp_byte == 3)
                        {
                            e.QRcodestatus = "1";
                        }
                        else
                        {
                            e.QRcodestatus = "";
                        }
                        idx += 55;//resv
                        e.exist_mask = _v[idx] == 1;
                        idx += 1;
                        e.body_temp = BitConverter.ToSingle(_v, idx);
                        idx += 4;
                        if (exist_overall > 0)
                        {
                            Array.Copy(_v, idx, e.overall, 0, e.overall.Length);
                            idx += e.overall.Length;
                        }
                        if (exist_closeup > 0)
                        {
                            Array.Copy(_v, idx, e._closeup, 0, e._closeup.Length);
                            idx += e._closeup.Length;
                        }
                        int feature_size = BitConverter.ToInt32(_v, idx);
                        idx += 4;
                        idx += 4 * feature_size;// 特征数据
                        int face_img_len = BitConverter.ToInt32(_v, idx);
                        idx += 4;
                        if (face_img_len > 0)
                        {
                            idx += 4;//format
                            idx += face_img_len;
                        }
                        if (exist_overall > 0)
                        {
                            idx += 20; // 关键点
                        }
                        if (exist_closeup > 0)
                        {
                            idx += 20; //关键点
                        }
                        e.device_sn = Encoding.UTF8.GetString(_v, idx, 32).TrimEnd('\0');
                        idx += 32;
                        int exist_idard = BitConverter.ToInt32(_v, idx);
                        idx += 4;
                        if (exist_idard > 0)
                        {
                            e.idcard_number = Encoding.UTF8.GetString(_v, idx, 36).TrimEnd('\0');
                            idx += 36;
                            e.idcard_name = Encoding.UTF8.GetString(_v, idx, 43).TrimEnd('\0');
                            idx += 43;
                            e.idcard_birth = Encoding.UTF8.GetString(_v, idx, 17).TrimEnd('\0');
                            idx += 17;
                            e.idcard_sex = _v[idx];
                            idx += 1;
                            e.idcard_national = Encoding.UTF8.GetString(_v, idx, 19).TrimEnd('\0');
                            idx += 19;
                            e.idcard_residence_address = Encoding.UTF8.GetString(_v, idx, 103).TrimEnd('\0');
                            idx += 103;
                            e.idcard_organ_issue = Encoding.UTF8.GetString(_v, idx, 43).TrimEnd('\0');
                            idx += 43;
                            e.idcard_valid_date_start = Encoding.UTF8.GetString(_v, idx, 17).TrimEnd('\0');
                            idx += 17;
                            e.idcard_valid_date_end = Encoding.UTF8.GetString(_v, idx, 17).TrimEnd('\0');
                            idx += 17;
                        }
                        if (e.match_status > 0)
                        {
                            e.customer_text = Encoding.UTF8.GetString(_v, idx, 68).TrimEnd('\0');
                            idx += 68;
                        }
                        int exist_qr = BitConverter.ToInt32(_v, idx);
                        idx += 4;
                        if (exist_qr == 1)
                        {
                            idx += 32;//type
                            e.qr_code = Encoding.UTF8.GetString(_v, idx, 1024).TrimEnd('\0');
                            idx += 1024;
                        }
                        if (e.match_status > 0)
                        {
                            e.person_name_ext = Encoding.UTF8.GetString(_v, idx, 64).TrimEnd('\0');
                            idx += 130;
                        }
                        try
                        {
                            //区分健康码版本 取行程信息 普通版本无行程信息
                            if (_v.Length - idx > 511)
                            {
                                e.trip_infor = Encoding.UTF8.GetString(_v, idx, 512).TrimEnd('\0').Trim();
                            }
                            //e.trip_infor = Encoding.UTF8.GetString(_v, idx, 512).TrimEnd('\0');
                        }
                        catch { }
                    }
                    catch (IndexOutOfRangeException)
                    {

                    }
                    if (e._closeup != null)
                    {
                        string imgename = MD5Util.MD5Encrypt32(e._closeup);
                        string fn = $@"{ApplicationData.FaceRASystemToolUrl}\imge_timing\{DateTime.Now.Year}\{DateTime.Now.Month}\{DateTime.Now.Day}\{DateTime.Now.Hour}\{imgename}.jpg";
                        //Console.WriteLine("22222");
                        e.closeup = fn;

                        byte[] bytes = e._closeup;
                        e._closeup = null;
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(fn));
                                File.WriteAllBytes(fn, bytes);

                            }
                            catch (IOException ex)
                            {
                                Logger.Error(ex, "error write image file");
                            }
                        });
                    }
                    CaptureData?.Invoke(this, e);
                    try
                    {
                        HandleCaptureData.setCaptureDataToDatabase(e, "", "");
                        ApplicationData.isrealtime = true;
                    }
                    catch (Exception x) { }
                }
            }
            if(msgType == 210)
            {
                lock(snapshortLocker)
                {
                    byte[] jpgBytes1 = null;
                    byte[] jpgBytes2 = null;
                    try
                    {
                        jpgBytes1 = new byte[BitConverter.ToInt32(_v, 8)];
                        Array.Copy(_v, 12, jpgBytes1, 0, jpgBytes1.Length);
                        int jpg2Len = BitConverter.ToInt32(_v, 12 + jpgBytes1.Length);
                        if(jpg2Len > 0)
                        {
                            jpgBytes2 = new byte[jpg2Len];
                            Array.Copy(_v, 16 + jpgBytes1.Length, jpgBytes2, 0, jpg2Len);
                        }
                    }
                    catch
                    {

                    }
                    List<Image> rets = new List<Image>();
                    if(jpgBytes1 != null)
                    {
                        rets.Add(Image.FromStream(new MemoryStream(jpgBytes1)));
                    }
                    if (jpgBytes2 != null)
                    {
                        rets.Add(Image.FromStream(new MemoryStream(jpgBytes2)));
                    }
                    snapshotRet = rets.ToArray();
                    Monitor.Pulse(snapshortLocker);
                }
            }
        }

        private unsafe void SaveCloseup(CaptureDataEventArgs e)
        {
            string imgename = MD5Util.MD5Encrypt32(e._closeup);
            //string fn = $@"D:\FaceRASystemTool\imge_timing\{DateTime.Now.Year}\{DateTime.Now.Month}\{DateTime.Now.Day}\{DateTime.Now.Hour}\{DateTime.Now.Minute}_{DateTime.Now.Second}_{DateTime.Now.Millisecond}_{imgIdx++}.jpg";
            string fn = $@"{ApplicationData.FaceRASystemToolUrl}\imge_timing\{DateTime.Now.Year}\{DateTime.Now.Month}\{DateTime.Now.Day}\{DateTime.Now.Hour}\{imgename}.jpg";
            e.closeup = fn;
            byte[] bytes = e._closeup;
            e._closeup = null;
            
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fn));
                File.WriteAllBytes(fn, bytes);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "save image exception");
            }

            
        }
    }
}

using huaanClient.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace huaanClient
{
    class DataSynchronization
    {
        static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static void DataSynchronizationtask()
        {
            //先获取所有的人员ID和所有的人员同步表的ID
            var localIds = GetData.getIDsforstaffAndDataSyn();
            

            List<CameraConfigPort> cameraConfigPorts = Deviceinfo.MyDevicelist;
            cameraConfigPorts.ForEach(a =>
            {
                if (a.IsConnected)
                {
                    try
                    {
                        var deviceIdsNeedsSync = DevicePersonIdsNeedsSync(localIds, a);
                        SyncDevicePersonIds(deviceIdsNeedsSync, a);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, $"同步人员信息异常{a.IP}");
                    }
                    
                }
            });
        }
        /// <summary>
        /// 检测是否需要同步数据
        /// </summary>
        /// <param name="localIds">当前队列所有的ID</param>
        /// <param name="cameraConfigPort">设备</param>
        /// <returns></returns>
        public static string[] DevicePersonIdsNeedsSync(string[] localIds, CameraConfigPort cameraConfigPort)
        {
            //从相机取回所有人员的ID
            var deviceIds = cameraConfigPort.GetAllPersonId().ToArray();

            return deviceIds.Except(localIds).ToArray();
        }
        /// <summary>
        /// 获取相机的指定人员信息
        /// </summary>
        /// <param name="localIds"></param>
        /// <param name="cameraConfigPort"></param>
        public static void SyncDevicePersonIds(string[] deviceIds, CameraConfigPort cameraConfigPort)
        {

            foreach (var id in deviceIds)
            {
                try
                {
                    SyncOneId(id, cameraConfigPort);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"同步人员 id:{id} @ {cameraConfigPort.IP} 异常");
                }
            }
        }


        public static void SyncOneId(string id, CameraConfigPort cameraConfigPort)
        {
            //请求回相机数据
            string request_persons = string.Format(UtilsJson.request_persons, id);
            string data = GetDevinfo.request(cameraConfigPort, request_persons);
            //更新到数据库

            if (string.IsNullOrEmpty(data)) return;

            Console.WriteLine(data);
            JToken jToken = (JToken)JsonConvert.DeserializeObject(data);
            if (jToken["code"].ToString() == "0")
            {
                //name, imge,personid,publishtime,role," +
                //"term_start,term,wg_card_id,long_card_id,addr_name
                JArray jArray = (JArray)jToken["persons"];
                if (jArray == null) return;


                JObject jObject = new JObject();
                //todo: jArray could be null
                jObject["name"] = jArray[0]["name"].ToString();

                jObject["device_sn"] = jToken["device_sn"].ToString();
                string fn = "";
                string fn_tmp = "";
                string reg_imagesfn = "";


                byte[] closeup = null;
                byte[] reg_imagescloseup = null;
                string imgename = null;

                var person = jArray[0];

                var closeupBase64 = person["normal_images"]?[0]["image_data"].ToString();
                if (closeupBase64 != null)
                {
                    closeup = Convert.FromBase64String(closeupBase64);
                    imgename = MD5Util.MD5Encrypt32(Convert.FromBase64String(closeupBase64));
                    fn_tmp = $@"{ApplicationData.FaceRASystemToolUrl}\imge_timing\{DateTime.Now.Year}\{DateTime.Now.Month}\{DateTime.Now.Day}\{DateTime.Now.Hour}\{imgename}_tmp.jpg";
                    fn= $@"{ApplicationData.FaceRASystemToolUrl}\imge_timing\{DateTime.Now.Year}\{DateTime.Now.Month}\{DateTime.Now.Day}\{DateTime.Now.Hour}\{imgename}.jpg";
                    Directory.CreateDirectory(Path.GetDirectoryName(fn_tmp));
                    File.WriteAllBytes(fn_tmp, closeup);
                    Bitmap pic = new Bitmap(fn_tmp);
                    int width = pic.Size.Width;   // 图片的宽度
                    resizePic(fn_tmp, 10, fn); ; //经测试，不管获取的相机人脸图片是112*112，还是150*150，经过扩大外边框后都不能正常下发
                    //if (width < 150)
                    //{
                    //    fn = "";
                    //}
                    //else
                    //{
                    //    resizePic(fn_tmp, 100, fn);
                    //} 
                }

                //reg_imagesfn好像没用
                var reg_imagescloseupBase64 = person["reg_images"]?[0]["image_data"].ToString();
                if (reg_imagescloseupBase64 != null && imgename != null)
                {
                    reg_imagescloseup = Convert.FromBase64String(reg_imagescloseupBase64);
                    reg_imagesfn = $@"{ApplicationData.FaceRASystemToolUrl}\imge_timing\{DateTime.Now.Year}\{DateTime.Now.Month}\{DateTime.Now.Day}\{DateTime.Now.Hour}\{imgename + "reg_images"}.jpg";
                    File.WriteAllBytes(reg_imagesfn, reg_imagescloseup);
                }


                //device_sn
                jObject["imge"] = fn;
                jObject["personid"] = jArray[0]["id"].ToString();
                jObject["role"] = jArray[0]["role"].ToString();
                jObject["term_start"] = jArray[0]["term_start"].ToString().Replace("/", "-");
                jObject["term"] = jArray[0]["term"].ToString().Replace("/", "-");
                if (jArray[0]["wg_card_id"] == null)
                {
                    jObject["wg_card_id"] = "";
                    jObject["long_card_id"] = jArray[0]["long_card_id"].ToString();
                }
                else
                {
                    jObject["long_card_id"] = "";
                    jObject["wg_card_id"] = jArray[0]["wg_card_id"].ToString();
                }
                //model
                jObject["addr_name"] = cameraConfigPort.DeviceName;
                jObject["model"] = cameraConfigPort.DevicVersion;
                GetData.setDataSyn(jObject);
            }

        }

        /// <summary>
        /// 给图片扩展外边框，并将图片至于中间
        /// </summary>
        /// <param name="sourcePicPath">原图片路径</param>
        /// <param name="extLenth">扩展的长度，对于归一化图片建议为100</param>
        /// <param name="targetPicPath">新图片路径</param>
        public static void resizePic(string sourcePicPath, int extLenth, string targetPicPath)
        {
            Bitmap bit1 = new Bitmap(sourcePicPath);//给图片加边框									
            int w = (bit1.Width + extLenth);//边框的宽度，可取任意值
            int h = (bit1.Height + extLenth);
            Bitmap bit2 = new Bitmap(w, h);

            using (Graphics g = Graphics.FromImage(bit2))
            {
                using (Pen pen = new Pen(Color.Black, w))
                {
                    g.FillRectangle(Brushes.White, 0, 0, w, h);
                    g.DrawImage(bit1, extLenth / 2, extLenth / 2, bit1.Width, bit1.Height);
                    g.Dispose();
                }
            }
            //bit2.Dump();
            bit2.Save(targetPicPath);
        }

    }

    public class Agreement
    {

        public string id { get; set; }
    }
}

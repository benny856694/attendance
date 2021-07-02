using huaanClient.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace huaanClient
{
    class DataSynchronization
    {
        public static void DataSynchronizationtask()
        {
            //先获取所有的人员ID和所有的人员同步表的ID
            string data = GetData.getIDsforstaffAndDataSyn();
            if (data.Length < 3)
            {
                return;
            }

            List<CameraConfigPort> cameraConfigPorts = Deviceinfo.MyDevicelist;
            cameraConfigPorts.ForEach(a =>
            {
                if (a.IsConnected)
                {
                    List<string> vs = null;
                    if (IsDataConsistency(out vs, data, a))
                    {
                        setDataConsistencyforDatabase(vs, data, a);
                    }
                }
            });
        }
        /// <summary>
        /// 检测是否需要同步数据
        /// </summary>
        /// <param name="ids">当前队列所有的ID</param>
        /// <param name="cameraConfigPort">设备</param>
        /// <returns></returns>
        public static bool IsDataConsistency(out List<string> outvs, string ids, CameraConfigPort cameraConfigPort)
        {
            bool re = false;
            outvs = null;
            if (ids.Length < 3)
                return re;
            //从相机取回所有人员的ID
            List<string> vs = cameraConfigPort.GetAllPersonId();
            if (vs.Count == 0 || vs == null)
            {
                return re;
            }
            //对比取回的IDS和本地的是否完全一致。
            JArray jo = (JArray)JsonConvert.DeserializeObject(ids);

            if (jo.Count != vs.Count)
            {
                outvs = vs;
                return true;
            }

            for (var s = 0; s < jo.Count; s++)
            {
                if (vs.FindAll(a => a.ToString() == jo[s]["id"].ToString()).Count == 0)
                {
                    outvs = vs;
                    re = true;
                    break;
                }
            }

            return re;
        }
        /// <summary>
        /// 获取相机的指定人员信息
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cameraConfigPort"></param>
        public static void setDataConsistencyforDatabase(List<string> outvs, string ids, CameraConfigPort cameraConfigPort)
        {
            if (ids.Length < 3 || outvs == null)
                return;
            if (outvs.Count == 0)
            {
                return;
            }
            JArray jo = (JArray)JsonConvert.DeserializeObject(ids);
            List<Agreement> list = jo.ToObject<List<Agreement>>();

            for (var s = 0; s < outvs.Count; s++)
            {
                if (list.FindAll(a => a.id.ToString() == outvs[s].ToString()).Count == 0)
                {
                    //请求回相机数据
                    string request_persons = string.Format(UtilsJson.request_persons, outvs[s].ToString());
                    string data = GetDevinfo.request(cameraConfigPort, request_persons);
                    //更新到数据库

                    if (string.IsNullOrEmpty(data))
                        continue;
                    JToken jToken = (JToken)JsonConvert.DeserializeObject(data);
                    if (jToken["code"].ToString() == "0")
                    {
                        //name, imge,personid,publishtime,role," +
                        //"term_start,term,wg_card_id,long_card_id,addr_name
                        JArray jArray = (JArray)jToken["persons"];


                        JObject jObject = new JObject();
                        jObject["name"] = jArray[0]["name"].ToString();

                        jObject["device_sn"] = jToken["device_sn"].ToString();
                        string fn = "";
                        string reg_imagesfn = "";
                        try
                        {

                            byte[] closeup = null;
                            byte[] reg_imagescloseup = null;
                            string imgename = null;

                            var person = jArray[0];

                            var closeupBase64 = person["normal_images"]?[0]["image_data"].ToString();
                            if (closeupBase64 != null)
                            {
                                closeup = Convert.FromBase64String(closeupBase64);
                                imgename = MD5Util.MD5Encrypt32(Convert.FromBase64String(closeupBase64));
                                fn = $@"{ApplicationData.FaceRASystemToolUrl}\imge_timing\{DateTime.Now.Year}\{DateTime.Now.Month}\{DateTime.Now.Day}\{DateTime.Now.Hour}\{imgename}.jpg";
                                Directory.CreateDirectory(Path.GetDirectoryName(fn));
                                File.WriteAllBytes(fn, closeup);
                            }

                            var reg_imagescloseupBase64 = person["reg_images"]?[0]["image_data"].ToString();
                            if (reg_imagescloseupBase64 != null && imgename != null)
                            {
                                reg_imagescloseup = Convert.FromBase64String(reg_imagescloseupBase64);
                                reg_imagesfn = $@"{ApplicationData.FaceRASystemToolUrl}\imge_timing\{DateTime.Now.Year}\{DateTime.Now.Month}\{DateTime.Now.Day}\{DateTime.Now.Hour}\{imgename + "reg_images"}.jpg";
                                File.WriteAllBytes(reg_imagesfn, reg_imagescloseup);
                            }

                        }
                        catch{}
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
            }
        }
    }

    public class Agreement
    {

        public string id { get; set; }
    }
}

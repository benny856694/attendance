using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huaanClient.Model;


namespace huaanClient.Database
{
    class DbEquipmentDistribution
    {
        public int id { get; set; }
        public int  userid { get; set; }
        public int  deviceid { get; set; }
        public string status { get; set; }

        //0: 下发, 1: 删除, 2: 异常
        public string type { get; set; }
        public string date { get; set; }
        public string code { get; set; }

        public EquipmentDistribution toModel()
        {
            var m = new EquipmentDistribution
            {
                id = this.id,
                userid = this.userid,
                deviceid = this.deviceid,
                success = this.status == "success",
                action =  ConvertToActionType(this.type),
                date = this.date != null ? DateTime.ParseExact(this.date, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) : DateTime.MinValue,
                error_code = this.code
            };
            return m;
        }

        public static Model.EquipmentDistribution.ActionType ConvertToActionType(string type)
        {
            switch (type)
            {
                case "0":
                    return EquipmentDistribution.ActionType.ToBeDistributed;
                case "1":
                    return EquipmentDistribution.ActionType.ToBeDeleted;
                case "2":
                    return EquipmentDistribution.ActionType.Error;
                default:
                    throw new InvalidOperationException("type is not defined");
            }
        }
    }
}

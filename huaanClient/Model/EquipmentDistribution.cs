using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huaanClient.Database;

namespace huaanClient.Model
{
    class EquipmentDistribution
    {
        public int id { get; set; }
        public int userid { get; set; }
        public int deviceid { get; set; }
        public bool success { get; set; }
        public ActionType action { get; set; }
        public DateTime date { get; set; }
        public string error_code { get; set; }

        public enum ActionType
        {
            ToBeDistributed = 0,
            ToBeDeleted = 1,
            Error = 2
        }

        public enum Status
        {

        }

        public Database.DbEquipmentDistribution toDb()
        {
            var m = new DbEquipmentDistribution
            {
                id = this.id,
                userid = this.userid,
                deviceid = this.deviceid,
                status = this.success ? "success" : "",
                type = ConvertToType(this.action),
                date = this.date.ToString("yyyy-MM-dd HH:mm:ss"),
                code = this.error_code
            };
            return m;
        }

        public static string ConvertToType(ActionType type)
        {
            switch (type)
            {
                case ActionType.ToBeDistributed:
                    return "0";
                case ActionType.ToBeDeleted:
                    return "1";
                case ActionType.Error:
                    return "2";
                default:
                    throw new InvalidOperationException();
            }
        }
    }

}

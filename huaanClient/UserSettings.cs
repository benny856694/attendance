using Jot.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient
{
    public class UserSettings : ITrackingAware
    {
        public string TitleLong { get; set;  }
        public string TitleShort {  get; set; }
        public bool EnableTitleLong { get; set; }
        public bool EnableTitleShort { get; set; }

        public void ConfigureTracking(TrackingConfiguration configuration)
        {
            configuration.AsGeneric<UserSettings>()
                .Id(_ => "settings")
                .Properties(setting => new
                {
                    setting.TitleLong,
                    setting.TitleShort,
                    setting.EnableTitleLong,
                    setting.EnableTitleShort
                });
        }
    }
}

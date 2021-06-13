using Jot.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Model
{
    public class Settings : ITrackingAware 
    {
        public bool ShowRealtimeImage { get; set; } = true;
        public bool ShowTemplateImage { get; set; } = true;
        public void ConfigureTracking(TrackingConfiguration configuration)
        {
            configuration.AsGeneric<Settings>()
                .Id(_ => "Settings")
                .Properties(s => new { s.ShowRealtimeImage, s.ShowTemplateImage });
        }
    }
}

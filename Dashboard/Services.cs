using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jot;
using System.Windows.Forms;

namespace Dashboard
{
    static class Services
    {
        public static Tracker Tracker = new Tracker();

        static Services()
        {
            // configure tracking for all Form objects

            Tracker
                .Configure<Form>()
                // use different id for different screen configurations
                .Id(f => f.Name, SystemInformation.VirtualScreen.Size)
                .Properties(f => new { f.Top, f.Width, f.Height, f.Left, f.WindowState })
                .PersistOn(nameof(Form.Move), nameof(Form.Resize), nameof(Form.FormClosing))
                // do not track form size and location when minimized/maximized
                .WhenPersistingProperty((f, p) => p.Cancel = (f.WindowState != FormWindowState.Normal && (p.Property == nameof(Form.Height) || p.Property == nameof(Form.Width) || p.Property == nameof(Form.Top) || p.Property == nameof(Form.Left))))
                // a form should not be persisted after it is closed since properties will be empty
                .StopTrackingOn(nameof(Form.FormClosing));
        }
    }
}

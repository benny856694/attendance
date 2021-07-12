using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dashboard
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var setting = new Model.Settings();
            Services.Tracker.Track(setting);
            if (!setting.PasswordChecked)
            {
                using (var pwd = new FormPassword())
                {
                    pwd.Password = setting.DefaultPassword;
                    var dr = pwd.ShowDialog();
                    if (dr == DialogResult.Cancel)
                    {
                        return;
                    }

                }
            }

            setting.PasswordChecked = true;
            Services.Tracker.Persist(setting);



            Application.Run(new Form1());
        }
    }
}

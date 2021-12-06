using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    public class MonthlyAttendanceReporter : IReporter
    {
        public void Generate(DataContext dataContext, IXLWorkbook wb)
        {
            var master = new AttendanceMasterReporter();
            master.Generate(dataContext, wb);
            var pmaster = new PeriodicMasterReporter();
            pmaster.Generate(dataContext, wb);
        }
    }
}

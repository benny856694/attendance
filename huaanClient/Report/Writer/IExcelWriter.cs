using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    public interface IExcelWriter
    {
        (int rowConsumed, int colConsumed) Write(IXLWorksheet sheet, int startRow, int startCol, DailyAttendanceDataContext ctx);

    }
}

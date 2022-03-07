using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report.Writer
{
    internal class DailyReportRecordWriter 
    {
        public (int rowConsumed, int colConsumed) Write(IXLWorksheet ws, int startRow, int startCol, DailyAttendanceDataContext ctx)
        {
            var col = startCol;
            ws.Cell(startRow, col++).Value = ctx.StaffDetails.Department?.name;
            ws.Cell(startRow, col++).Value = ctx.StaffDetails.Employeetype?.Employetype_name;
            ws.Cell(startRow, col++).SetDataType(XLDataType.Text).SetValue(ctx.StaffDetails.Staff.Employee_code);
            ws.Cell(startRow, col++).Value = ctx.StaffDetails.Staff.name;
            ws.Cell(startRow, col++).SetValue($"{ctx.Date:d}, {ctx.Date:ddd}");
            ws.Cell(startRow, col++).Value = ctx.Shift?.name;
            var shift1 = ctx.Shift?.GetShift1();
            ws.Cell(startRow, col++).SetValue(shift1?.ShiftStart.ToString("t", CultureInfo.InvariantCulture));
            ws.Cell(startRow, col++).SetValue(shift1?.ShiftEnd.ToString("t", CultureInfo.InvariantCulture));
            if (ctx.DailyAttendanceData != null)
            {
                ws.Cell(startRow, col++).SetValue(ctx.DailyAttendanceData.CheckIn?.ToString("t", CultureInfo.InvariantCulture));
                ws.Cell(startRow, col++).SetValue(ctx.DailyAttendanceData.CheckOut?.ToString("t", CultureInfo.InvariantCulture));
                ws.Cell(startRow, col++).SetValue(ctx.DailyAttendanceData.LateHour.ToMyString());
                ws.Cell(startRow, col++).SetValue(ctx.DailyAttendanceData.EarlyHour.ToMyString());
                ws.Cell(startRow, col++).SetValue(ctx.DailyAttendanceData.WorkHour.ToMyString());
                ws.Cell(startRow, col++).SetValue(ctx.DailyAttendanceData.OverTime.ToMyString());
            }
            else
            {
                col += 5;
            }
            ws.Cell(startRow, col++).SetValue(ctx.Remark.ToDisplayText());



            return (1, col - startCol);
        }
    }
}

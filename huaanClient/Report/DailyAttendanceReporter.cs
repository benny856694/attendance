using ClosedXML.Excel;
using Dapper;
using Dapper.Contrib.Extensions;
using DBUtility.SQLite;
using huaanClient.Database;
using huaanClient.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime.Text;
using NodaTime;
using huaanClient.Report.Writer;

namespace huaanClient.Report
{
    class DailyAttendanceReporter : IReporter 
    {

        public void Generate(DataContext ctx, IXLWorkbook wb)
        {
            WriteEmployees(wb, ctx);
        }

        private void WriteStatistics(IXLWorksheet ws, int row, int col, Counter counter)
        {
            ++col;
            ws.Cell(row, col++).Value = $"{Strings.DailyReportTotalPresent}: {counter.presentCount}";
            ws.Cell(row, col++).Value = $"{Strings.DailyReportTotalAbsent}: {counter.absenceCount}";
            ws.Cell(row, col++).Value = $"{Strings.DailyReportTotalLate}: {counter.lateCount}";
            ws.Cell(row, col++).Value = $"{Strings.DailyReportTotalEarly}: {counter.earlyCount}";
        }

        private void WriteEmployees(IXLWorkbook wb, DataContext ctx)
        {


            var start = ctx.From;
            var end = ctx.To;
            var departments = ctx.Staffs.GroupBy(x => x.department_id);
            for (var d = start; d <= end; d = d.PlusDays(1))
            {
                var counter = new Counter();
                var ws = wb.AddWorksheet(d.ToString("d", CultureInfo.CurrentCulture).Replace('/','-'));
                var row = 1;
                var col = 1;
                var rowCount = 0;
                var colCount = 0;
                var (rowConsumed, colConsumed) = WriteTitle(ws, row, col);
                rowCount += rowConsumed;
                colCount += colConsumed;
                row += rowConsumed;
                col += colConsumed;
                var writer = new DailyReportRecordWriter();
                foreach (var departmentStaffs in departments)
                {
                    var departmentRowStart = row;
                    foreach (var staff in departmentStaffs)
                    {
                        var dailyAttendanceDataCtx = ctx.Extract(staff.id, d);
                        (rowConsumed, colConsumed) = writer.Write(ws, row, 1, dailyAttendanceDataCtx);
                        rowCount += rowConsumed;
                        colCount += colConsumed;
                        row += rowConsumed;
                        col += colConsumed;
                        counter.Count(dailyAttendanceDataCtx);
                    }
                    ws.Range($"A{departmentRowStart}:A{row - 1}").Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
                }
                WriteStatistics(ws, row+1, 1, counter);

                ws.Columns().AdjustToContents().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Rows("1")
                    .Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Rows($"{row+1}").Style.Font.Bold = true;
                ws.SheetView.FreezeRows(1);
            }


        }


        private (int rowConsumed, int colConsumed) WriteTitle(IXLWorksheet ws, int startRow, int startCol)
        {
            //title
            var col = startCol;
            var row = startRow;
            ws.Cell(row, col++).Value = Strings.DailyReportDepartmentTitle;
            ws.Cell(row, col++).Value = Strings.DailyReportDesignationTitle;
            ws.Cell(row, col++).Value = Strings.DailyReportEmpNumberTitle;
            ws.Cell(row, col++).Value = Strings.DailyReportEmpNameTitle;
            ws.Cell(row, col++).Value = Strings.DailyReportDateTitle;
            ws.Cell(row, col++).Value = Strings.DailyReportShiftTitle;
            ws.Cell(row, col++).Value = Strings.DailyReportShiftStartTitle;
            ws.Cell(row, col++).Value = Strings.DailyReportShiftEndTitle;
            ws.Cell(row, col++).Value = Strings.DailyReportCheckInTitle;
            ws.Cell(row, col++).Value = Strings.DailyReportCheckOutTitle;
            ws.Cell(row, col++).Value = Strings.DailyReportLateTitle;
            ws.Cell(row, col++).Value = Strings.DailyReportEarlyTitle;
            ws.Cell(row, col++).Value = Strings.DailyReportWHTitle;
            ws.Cell(row, col++).Value = Strings.DailyReportOTTitle;
            ws.Cell(row, col++).Value = Strings.DailyReportRemarksTitle;
            var titleRow = ws.Range(ws.Cell(row, 1).Address, ws.Cell(row, col - 1).Address);
            titleRow.Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);
            return (1, col - startCol);
        }
    }
}

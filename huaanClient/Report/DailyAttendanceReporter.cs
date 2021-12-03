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

namespace huaanClient.Report
{
    class DailyAttendanceReporter
    {

        public void Generate(string from, string to, string pathToXlsx)
        {

            using (var wb = new XLWorkbook())
            {
                WriteEmployees(wb, from, to);
                wb.SaveAs(pathToXlsx);
            }
        }

        private void WriteStatistics(IXLWorksheet ws, int row, int col, Counter counter)
        {
            ws.Cell(row, col++).Value = $"Total Present: {counter.presentCount}";
            ws.Cell(row, col++).Value = $"Total Absent: {counter.absenceCount}";
            ws.Cell(row, col++).Value = $"Total Late: {counter.lateCount}";
            ws.Cell(row, col++).Value = $"Total Early: {counter.earlyCount}";
        }

        private void WriteEmployees(IXLWorkbook wb, string from, string to)
        {

            var ctx = new DataContext();
            ctx.Load(from, to);

            var start = from.ToLocalDate();
            var end = to.ToLocalDate();
            var departments = ctx.Staffs.GroupBy(x => x.department_id);
            for (var d = start.Value; d <= end.Value; d = d.PlusDays(1))
            {
                var counter = new Counter();
                var ws = wb.AddWorksheet(d.ToString());
                var row = 1;
                var col = 1;
                var rowCount = 0;
                var colCount = 0;
                var (rowConsumed, colConsumed) = WriteTitle(ws, row, col);
                rowCount += rowConsumed;
                colCount += colConsumed;
                row += rowConsumed;
                col += colConsumed;
                foreach (var departmentStaffs in departments)
                {
                    var departmentRowStart = row;
                    foreach (var staff in departmentStaffs)
                    {
                        var data = ctx.AttendanceData.FirstOrDefault(x => x.Date == d && x.EmployeeId == staff.id);
                        var department = ctx.Departments.FirstOrDefault(x => x.id == staff.department_id);
                        var emplyeeType = ctx.Employetypes.FirstOrDefault(x => x.id == staff.Employetype_id);
                        if (data == null) continue;
                        (rowConsumed, colConsumed) = WriteOneRecord(ws, row, 1, department, emplyeeType, staff, d, data);
                        rowCount += rowConsumed;
                        colCount += colConsumed;
                        row += rowConsumed;
                        col += colConsumed;
                        counter.Count(data);
                    }
                    ws.Range($"A{departmentRowStart}:A{row - 1}").Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
                }
                WriteStatistics(ws, row, 1, counter);

                ws.Columns().AdjustToContents().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Rows("1")
                    .Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //ws.Rows(row.ToString(CultureInfo.InvariantCulture)).Style.Font.Bold = true;
                ws.SheetView.FreezeRows(1);


            }


        }

        private (int rowCount, int colCount) WriteOneRecord(IXLWorksheet ws, int row, int col, Department department, Employetype employetype, Staff staff, LocalDate d, AttendanceDataForDay data)
        {
            var startCol = col;
            ws.Cell(row, col++).Value = department?.name;
            ws.Cell(row, col++).Value = employetype?.Employetype_name;
            ws.Cell(row, col++).SetDataType(XLDataType.Text).SetValue(data.EmployeeCode);
            ws.Cell(row, col++).Value = staff.name;
            ws.Cell(row, col++).Value = data.ShiftName;
            ws.Cell(row, col++).SetValue(data.ShiftStart?.ToString("t", CultureInfo.InvariantCulture));
            ws.Cell(row, col++).SetValue(data.ShiftEnd?.ToString("t", CultureInfo.InvariantCulture));
            ws.Cell(row, col++).SetValue(data.CheckIn?.ToString("t", CultureInfo.InvariantCulture));
            ws.Cell(row, col++).SetValue(data.CheckOut?.ToString("t", CultureInfo.InvariantCulture));
            ws.Cell(row, col++).SetValue(data.LateHour.ToMyString());
            ws.Cell(row, col++).SetValue(data.EarlyHour.ToMyString());
            ws.Cell(row, col++).SetValue(data.WorkHour.ToMyString());
            ws.Cell(row, col++).SetValue(data.Remark.ToDisplayText());

            return (1, col - startCol);

        }

        private (int rowConsumed, int colConsumed) WriteTitle(IXLWorksheet ws, int startRow, int startCol)
        {
            //title
            var col = startCol;
            var row = startRow;
            ws.Cell(row, col++).Value = "Department";
            ws.Cell(row, col++).Value = "Designation";
            ws.Cell(row, col++).Value = "Emp No.";
            ws.Cell(row, col++).Value = "Emp Name";
            ws.Cell(row, col++).Value = "Shift";
            ws.Cell(row, col++).Value = "Shift Start";
            ws.Cell(row, col++).Value = "Shift End";
            ws.Cell(row, col++).Value = "Check In";
            ws.Cell(row, col++).Value = "Check Out";
            ws.Cell(row, col++).Value = "Late";
            ws.Cell(row, col++).Value = "Early";
            ws.Cell(row, col++).Value = "WH";
            ws.Cell(row, col++).Value = "Remarks";
            var titleRow = ws.Range(ws.Cell(row, 1).Address, ws.Cell(row, col - 1).Address);
            titleRow.Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);
            return (1, col - startCol);
        }
    }
}

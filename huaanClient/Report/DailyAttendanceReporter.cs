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
                var row = WriteEmployees(wb, from, to);
                WriteStatistics(ws, row);
                ws.Columns().AdjustToContents().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Rows("1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Rows(row.ToString(CultureInfo.InvariantCulture)).Style.Font.Bold = true;
                ws.SheetView.FreezeRows(1);
                wb.SaveAs(pathToXlsx);
            }
        }

        private void WriteStatistics(IXLWorksheet ws, int row, Counter counter)
        {
            var col = 3;
            ws.Cell(row, col++).Value = $"Total Present: {PresentCount}";
            ws.Cell(row, col++).Value = $"Total Absent: {AbsentCount}";
            ws.Cell(row, col++).Value = $"Total Late: {LateCount}";
            ws.Cell(row, col++).Value = $"Total Early: {EarlyCount}";
        }

        private int WriteEmployees(IXLWorkbook wb, string from, string to)
        {

            var ctx = new DataContext();
            ctx.Load(from, to);

            var start = from.ToLocalDate();
            var end = to.ToLocalDate();
            var departments = ctx.Staffs.GroupBy(x => x.department_id);
            for (var d = start.Value; d < end.Value; d = d.PlusDays(1))
            {
                var counter = new Counter();
                var ws = wb.AddWorksheet(d.ToString());
                var row = 1;
                var col = 1;
                
                var (rowConsumed, colConsumed) = WriteTitle(ws, row, col);
                foreach (var staff in departments)
                {
                    var data = ctx.AttendanceData.FirstOrDefault(x => x.Date == d);
                    WriteOneRecord(ws, ctx, data);
                    counter.Count(data);
                }
                WriteStatistics()
            }


            var row = 2;
            var departmentGroup = attendanceData.GroupBy(x => x.department);
            foreach (var dep in departmentGroup)
            {
                var departmentRowStart = row;
                foreach (var data in dep.Select(x=>x.ToAttendanceDataForDay()))
                {
                    var col = 1;

                    ws.Cell(row, col++).Value = dep.Key;

                    ws.Cell(row, col++).Value = Util.GetEmployeeTypeName(staffs, employeeTypes, data.EmployeeId);
                    ws.Cell(row, col++).SetDataType(XLDataType.Text).SetValue(data.EmployeeCode);
                    ws.Cell(row, col++).Value = data.EmployeeName;
                    ws.Cell(row, col++).Value = data.Date;
                    ws.Cell(row, col++).Value = data.ShiftName;
                    ws.Cell(row, col++).SetValue(data.ShiftStart?.ToString("t", CultureInfo.InvariantCulture));
                    ws.Cell(row, col++).SetValue(data.ShiftEnd?.ToString("t", CultureInfo.InvariantCulture));
                    ws.Cell(row, col++).SetValue(data.CheckIn?.ToString("t", CultureInfo.InvariantCulture));
                    ws.Cell(row, col++).SetValue(data.CheckOut?.ToString("t", CultureInfo.InvariantCulture));
                    ws.Cell(row, col++).SetValue(data.Late.ToMyString());
                    ws.Cell(row, col++).SetValue(data.Early.ToMyString());
                    ws.Cell(row, col++).SetValue(data.WorkHour.ToMyString());
                    ws.Cell(row, col++).SetValue(data.Remark.ToDisplayText());

                    switch (data.Remark)
                    {
                        case Remark.Present:
                            PresentCount++;
                            if (data.CheckOut > data.ShiftEnd)
                            {
                                EarlyCount++;
                            }
                            if (data.CheckIn > data.ShiftStart)
                            {
                                LateCount++;
                            }
                            break;
                        case Remark.SinglePunch:
                            break;
                        case Remark.Absent:
                            AbsentCount++;
                            break;
                        default:
                            break;
                    }

                    row++;
                }
                ws.Range($"A{departmentRowStart}:A{row - 1}").Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            }

            return row;

        }

        private (int rowCount, int colCount) WriteOneRecord(IXLWorksheet ws, int startRow, int startCol, DataContext ctx, AttendanceDataForDay data)
        {
            throw new NotImplementedException();
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
            ws.Cell(row, col++).Value = "Date";
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
            return (0, col - 1);
        }
    }
}

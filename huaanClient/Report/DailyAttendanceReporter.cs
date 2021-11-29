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

namespace huaanClient.Report
{
    class DailyAttendanceReporter
    {
        int PresentCount;
        int AbsentCount;
        int LateCount;
        int EarlyCount;

        public void Generate(AttendanceData[] data, string pathToXlsx)
        {

            using (var wb = new XLWorkbook())
            {
                var ws = wb.AddWorksheet();
                WriteTitle(ws);
                var row = WriteEmployees(ws, data);
                WriteStatistics(ws, row);
                ws.Columns().AdjustToContents().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Rows("1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Rows(row.ToString(CultureInfo.InvariantCulture)).Style.Font.Bold = true;
                ws.SheetView.FreezeRows(1);
                wb.SaveAs(pathToXlsx);
            }
        }

        private void WriteStatistics(IXLWorksheet ws, int row)
        {
            var col = 3;
            ws.Cell(row, col++).Value = $"Total Present: {PresentCount}";
            ws.Cell(row, col++).Value = $"Total Absent: {AbsentCount}";
            ws.Cell(row, col++).Value = $"Total Late: {LateCount}";
            ws.Cell(row, col++).Value = $"Total Early: {EarlyCount}";
        }

        private int WriteEmployees(IXLWorksheet ws, AttendanceData[] attendanceData)
        {
            List<Employetype> employeeTypes;
            List<Department> departments;
            List<Staff> staffs;
            
            using (var c = SQLiteHelper.GetConnection())
            {
                employeeTypes = c.GetAll<Employetype>().ToList();
                departments = c.GetAll<Department>().ToList();
                staffs = c.GetAll<Staff>().ToList();
            }

            var row = 2;
            foreach (var dep in attendanceData.GroupBy(x=>x.department))
            {
                foreach (var data in dep.Select(x=>x.ToAttendanceDataForDay()))
                {
                    var col = 1;

                    ws.Cell(row, col++).Value = dep.Key;
                    var employeeTypeId = staffs.FirstOrDefault(x => x.id == data.EmployeeId)?.Employetype_id;
                    ws.Cell(row, col++).Value = employeeTypes.FirstOrDefault(x => x.id == employeeTypeId)?.Employetype_name ?? "";
                    ws.Cell(row, col++).SetDataType(XLDataType.Text).SetValue(data.EmployeeCode);
                    ws.Cell(row, col++).Value = data.EmployeeName;
                    ws.Cell(row, col++).Value = data.ShiftName;
                    ws.Cell(row, col++).SetValue(data.ShiftStart?.ToString("t", CultureInfo.InvariantCulture));
                    ws.Cell(row, col++).SetValue(data.ShiftEnd?.ToString("t", CultureInfo.InvariantCulture));
                    ws.Cell(row, col++).SetValue(data.CheckIn?.ToString("t", CultureInfo.InvariantCulture));
                    ws.Cell(row, col++).SetValue(data.CheckOut?.ToString("t", CultureInfo.InvariantCulture));
                    ws.Cell(row, col++).Value = $"{data.Late.Hours}:{data.Late.Minutes}";
                    ws.Cell(row, col++).Value = $"{data.Early.Hours}:{data.Early.Minutes}";
                    ws.Cell(row, col++).Value = $"{data.WorkHour.Hours}:{data.WorkHour.Minutes}";
                    ws.Cell(row, col++).Value = data.Remark.ToDisplayText();


                    row++;
                }
                ws.Range($"A2:A{row - 1}").Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            }

            return row;

        }

        private void WriteTitle(IXLWorksheet ws)
        {
            //title
            var col = 1;
            var row = 1;
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
        }
    }
}

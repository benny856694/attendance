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

namespace huaanClient.Report
{
    class DailyAttendance
    {
        int PresentCount;
        int AbsentCount;
        int LateCount;
        int EarlyCount;

        public void Generate(List<string> employeeIds, DateTime day, string pathToXlsx)
        {

            using (var wb = new XLWorkbook())
            {
                var ws = wb.AddWorksheet();
                WriteTitle(ws);
                var row = WriteEmployees(ws, employeeIds, day);
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

        private int WriteEmployees(IXLWorksheet ws, List<string> employeeIds, DateTime day)
        {
            List<Employetype> employeeTypes;
            List<Department> departments;
            List<AttendanceData> attendanceData;
            List<Staff> staffs;
            
            using (var c = SQLiteHelper.GetConnection())
            {
                employeeTypes = c.GetAll<Employetype>().ToList();
                departments = c.GetAll<Department>().ToList();
                staffs = c.GetAll<Staff>().ToList();
                if (employeeIds == null)
                {
                    employeeIds = staffs.Select(x => x.id).ToList();
                }
                var idsString =  string.Join(",", employeeIds.Select(x => $"'{x}'"));
                attendanceData = c.Query<AttendanceData>($"SELECT * FROM  Attendance_Data WHERE personId in ({idsString}) AND strftime('%Y-%m-%d', Date) == '{day:yyyy-MM-dd}'").ToList();
            }

            var row = 2;
            foreach (var dep in attendanceData.GroupBy(x=>x.department))
            {
                foreach (var data in dep)
                {
                    var col = 1;

                    ws.Cell(row, col++).Value = dep.Key;
                    var employeeTypeId = staffs.FirstOrDefault(x => x.id == data.personId)?.Employetype_id;
                    ws.Cell(row, col++).Value = employeeTypes.FirstOrDefault(x => x.id == employeeTypeId)?.Employetype_name ?? "";
                    ws.Cell(row, col++).SetDataType(XLDataType.Text).SetValue(data.Employee_code);
                    ws.Cell(row, col++).Value = data.name;
                    var shift = data.Shiftinformation.CalcShift();
                    ws.Cell(row, col++).Value = shift.Name;
                    ws.Cell(row, col++).SetValue(shift.ShiftStart);
                    ws.Cell(row, col++).SetValue(shift.ShiftEnd);
                    ws.Cell(row, col++).SetValue(data.Punchinformation);
                    ws.Cell(row, col++).SetValue(data.Punchinformation1);
                    ws.Cell(row, col++).Value = data.late;
                    ws.Cell(row, col++).Value = data.Leaveearly;
                    ws.Cell(row, col++).Value = data.Duration;
                    var remarks = data.CalcRemarks();
                    ws.Cell(row, col++).Value = remarks.ToDisplayText();

                    switch (remarks)
                    {
                        case Remark.Present:
                            PresentCount++;
                            if (string.CompareOrdinal(data.Punchinformation1, shift.ShiftEnd) < 0)
                            {
                                EarlyCount++;
                            }
                            if (string.CompareOrdinal(data.Punchinformation, shift.ShiftStart) > 0)
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

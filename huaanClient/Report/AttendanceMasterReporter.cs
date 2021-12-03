using ClosedXML.Excel;
using huaanClient.Database;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    public class AttendanceMasterReporter
    {

        public void Generate(LocalDate from, LocalDate to, string pathToXlsx)
        {
            using (var wb = new ClosedXML.Excel.XLWorkbook())
            {
                var sheet = wb.AddWorksheet();
                WriteTitleLine(sheet, from, to);
                WriteAttendanceData(sheet, from, to);
                sheet.Columns("1").AdjustToContents();
                sheet.Rows("1").Style
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                    .Alignment.Vertical = XLAlignmentVerticalValues.Top;
                sheet.SheetView.FreezeRows(1);
                sheet.SheetView.FreezeColumns(2);

                wb.SaveAs(pathToXlsx);
            }

        }

        private void WriteAttendanceData(ClosedXML.Excel.IXLWorksheet sheet, LocalDate from, LocalDate to)
        {
            var ctx = new DataContext();
            ctx.Load(from, to);
            var row = 2;
            var col = 1;
            foreach (var person in ctx.Staffs)
            {

                var staffDetails = ctx.GetStaffDetails(person.id);
                if (staffDetails == null) continue;

                var counter = new Counter();
                var personDetail = $"EmpNo:{staffDetails.Staff.Employee_code}{Environment.NewLine}{staffDetails.Staff.name}{Environment.NewLine}Dept:{staffDetails.Department?.name}{Environment.NewLine}Desig:{staffDetails.Employeetype?.Employetype_name}";
                sheet.Cell(row, col).SetValue(personDetail).Style.Font.SetBold();
                col += 1;

                sheet.Cell(row + 0, col).Value = "In";
                sheet.Cell(row + 1, col).Value = "Out";
                sheet.Cell(row + 2, col).Value = "WH";
                sheet.Cell(row + 3, col).Value = "Late";
                sheet.Cell(row + 4, col).Value = "Status";
                sheet.Cell(row + 5, col).Value = "OT";
                col += 1;


                for (var d = from; d <= to; d = d.PlusDays(1))
                {
                    var attendanceContext = ctx.Extract(person.id, d);
                    if (attendanceContext != null)
                    {
                        var attData = attendanceContext.DailyAttendanceData;
                        sheet.Cell(row + 0, col)
                            .SetValue(attData.CheckIn?.ToString("t", CultureInfo.InvariantCulture))
                            .Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        sheet.Cell(row + 1, col)
                            .SetValue(attData.CheckOut?.ToString("t", CultureInfo.InvariantCulture))
                            .Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        sheet.Cell(row + 2, col)
                            .SetValue(attData.WorkHour.ToMyString())
                            .Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        sheet.Cell(row + 3, col)
                            .SetValue(attData.LateHour.ToMyString())
                            .Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        sheet.Cell(row + 4, col)
                            .SetValue(attData.Remark.ToDisplayText())
                            .Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        sheet.Cell(row + 5, col)
                            .SetValue(attData.OverTime.ToMyString())
                            .Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        counter.Count(attendanceContext.DailyAttendanceData);
                    }
                    col += 1;
                }

                sheet.Cell(row, col).SetValue($"PR-{counter.presentCount}"); sheet.Cell(row, col + 1).SetValue($"OT-{counter.overTimeCount}");
                sheet.Cell(row + 1, col).SetValue($"AB-{counter.absenceCount}"); sheet.Cell(row + 1, col + 1).SetValue($"LT-{counter.lateCount}");
                sheet.Cell(row + 2, col).SetValue($"WO-{counter.offDayCount}"); sheet.Cell(row + 2, col + 1).SetValue($"HO-{counter.holidayCount}");
                sheet.Cell(row + 3, col).SetValue($"OT Hour-{counter.overTimeHours.Normalize().ToMyString()}"); sheet.Cell(row + 3, col + 1).SetValue($"Late Hour-{counter.lateHours.Normalize().ToMyString()}");
                sheet.Cell(row + 4, col + 1).SetValue($"Work Hour-{counter.workHours.Normalize().ToMyString()}");
                sheet.Columns($"{col}:{col + 1}").AdjustToContents();

                sheet.Range($"A{row}:A{row + 6}").Merge()
                    .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Top);
                sheet.Row(row + 6)
                    .Style
                    .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                    .Border.SetBottomBorderColor(XLColor.LightGray);

                row += 7;
                col = 1;
            }

        }

        private void WriteTitleLine(ClosedXML.Excel.IXLWorksheet sheet, LocalDate from, LocalDate to)
        {
            var row = 1;
            var col = 1;
            sheet.Cell(row, col++).Value = "Emp Details";
            col++;
            for (var  d = from; d <= to; d = d.PlusDays(1))
            {
                sheet.Cell(row, col++).SetValue($"{d.Day}{Environment.NewLine}{d:ddd}")
                    .Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(ClosedXML.Excel.XLAlignmentHorizontalValues.Center);
            }
            
        }
    }
}

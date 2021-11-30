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
        private DateTime _minDate;
        private DateTime _maxDate;

        public void Generate(AttendanceData[] data, string pathToXlsx)
        {
            using (var wb = new ClosedXML.Excel.XLWorkbook())
            {
                var sheet = wb.AddWorksheet();
                WriteTitleLine(sheet, data);
                WriteAttendanceData(sheet, data);
                sheet.Columns("1").AdjustToContents();
                sheet.Rows("1").Style
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                    .Alignment.Vertical = XLAlignmentVerticalValues.Top;
                sheet.SheetView.FreezeRows(1);
                sheet.SheetView.FreezeColumns(2);

                wb.SaveAs(pathToXlsx);
            }

        }

        private void WriteAttendanceData(ClosedXML.Excel.IXLWorksheet sheet, AttendanceData[] data)
        {
            var (employeeTypes, departments, staffs) = Util.LoadDb();
            var personGroup = data.GroupBy(x => x.personId);
            var row = 2;
            var col = 1;
            foreach (var person in personGroup)
            {
                var presentCount = 0;
                var absenceCount = 0;
                var overTimeCount = 0;
                var lateCount = 0;
                var offDayCount = 0;
                var holidayCount = 0;
                var overTimeHours = Period.Zero;
                var lateHours = Period.Zero;
                var workHours = Period.Zero;

                var employeeTypeName = Util.GetEmployeeTypeName(staffs, employeeTypes, person.Key);
                var staff = staffs.FirstOrDefault(x => x.id == person.Key);
                if (staff == null) continue;
                var dept = departments.FirstOrDefault(x => x.id == staff.department_id);
                var personDetail = $"EmpNo:{staff.Employee_code}{Environment.NewLine}{staff.name}{Environment.NewLine}Dept:{dept?.name}{Environment.NewLine}Desig:{employeeTypeName}";
                sheet.Cell(row, col).SetValue(personDetail).Style.Font.SetBold();
                col += 1;

                sheet.Cell(row + 0, col).Value = "In";
                sheet.Cell(row + 1, col).Value = "Out";
                sheet.Cell(row + 2, col).Value = "WH";
                sheet.Cell(row + 3, col).Value = "Late";
                sheet.Cell(row + 4, col).Value = "Status";
                sheet.Cell(row + 5, col).Value = "OT";
                col += 1;

                for (var d = _minDate; d <= _maxDate; d = d.AddDays(1))
                {
                    var att = person.FirstOrDefault(x => x.Date == d);
                    if (att != null)
                    {
                        var attData = att.ToAttendanceDataForDay();
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
                            .SetValue(attData.Late.ToMyString())
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

                        switch (attData.Remark)
                        {
                            case Remark.Present:
                                presentCount++;
                                if (attData.OverTime != Period.Zero)
                                {
                                    overTimeCount++;
                                    overTimeHours += attData.OverTime;
                                }
                                if (attData.Late != Period.Zero)
                                {
                                    lateCount++;
                                    lateHours += attData.Late;
                                }
                                if (attData.WorkHour != Period.Zero)
                                {
                                    workHours += attData.WorkHour;
                                }
                                break;
                            case Remark.SinglePunch:
                                break;
                            case Remark.Absent:
                                absenceCount++;
                                break;
                            case Remark.Holiday:
                                holidayCount++;
                                break;
                            case Remark.OffWork:
                                offDayCount++;
                                break;
                            default:
                                break;
                        }
                    }
                    col += 1;
                }

                sheet.Cell(row, col).SetValue($"PR-{presentCount}"); sheet.Cell(row, col+1).SetValue($"OT-{overTimeCount}");
                sheet.Cell(row+1, col).SetValue($"AB-{absenceCount}"); sheet.Cell(row+1, col + 1).SetValue($"LT-{lateCount}");
                sheet.Cell(row+2, col).SetValue($"WO-{offDayCount}"); sheet.Cell(row+2, col + 1).SetValue($"HO-{holidayCount}");
                sheet.Cell(row+3, col).SetValue($"OT Hour-{overTimeHours.Normalize().ToMyString()}"); sheet.Cell(row+3, col + 1).SetValue($"Late Hour-{lateHours.Normalize().ToMyString()}");
                sheet.Cell(row+4, col + 1).SetValue($"Work Hour-{workHours.Normalize().ToMyString()}");
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

        private void WriteTitleLine(ClosedXML.Excel.IXLWorksheet sheet, AttendanceData[] data)
        {
            _minDate = data.Min(x=>x.Date);
            _maxDate = data.Max(x => x.Date);
            var row = 1;
            var col = 1;
            sheet.Cell(row, col++).Value = "Emp Details";
            col++;
            for (var  d = _minDate; d <= _maxDate; d = d.AddDays(1))
            {
                sheet.Cell(row, col++).SetValue($"{d.Day}{Environment.NewLine}{d:ddd}")
                    .Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(ClosedXML.Excel.XLAlignmentHorizontalValues.Center);
            }
            
        }
    }
}

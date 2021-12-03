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
    public class PeriodicMasterReporter
    {
        private DateTime _minDate;
        private DateTime _maxDate;

        public void Generate(AttendanceData[] data, string pathToXlsx)
        {
            using (var wb = new ClosedXML.Excel.XLWorkbook())
            {
                var sheet = wb.AddWorksheet();
                WriteTitle(sheet, data);
                WriteEmployees(sheet, data);
                sheet.Columns().AdjustToContents();
                sheet.Rows("1").Style
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Top)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                sheet.SheetView.FreezeRows(1);
                sheet.SheetView.FreezeColumns(2);

                wb.SaveAs(pathToXlsx);
            }

        }


        private int WriteEmployees(IXLWorksheet ws, AttendanceData[] attendanceData)
        {
            throw new NotImplementedException();
            //var (employeeTypes, departments, staffs) = Util.LoadDb();
            //var row = 2;
            //var departmentGroup = attendanceData.GroupBy(x => x.department);
            //foreach (var dep in departmentGroup)
            //{
            //    var departmentRowStart = row;
            //    foreach (var data in dep.AsEnumerable().GroupBy(x=>x.personId))
            //    {
            //        var presentCount = 0;
            //        var absenceCount = 0;
            //        var overTimeCount = 0;
            //        var lateCount = 0;
            //        var offDayCount = 0;
            //        var holidayCount = 0;
            //        var overTimeHours = Period.Zero;
            //        var lateHours = Period.Zero;
            //        var workHours = Period.Zero;


            //        var col = 1;
            //        var staff = staffs.FirstOrDefault(x => x.id == data.Key);
            //        if (staff == null) continue;

            //        ws.Cell(row, col++).Value = dep.Key;
            //        ws.Cell(row, col++).SetDataType(XLDataType.Text).SetValue(Util.GetEmployeeTypeName(staffs, employeeTypes, data.Key));
            //        ws.Cell(row, col++).SetValue(staff.Employee_code);
            //        ws.Cell(row, col++).Value = staff.name;

            //        for (var d = _minDate; d <= _maxDate; d = d.AddDays(1))
            //        {
            //            var att = attendanceData.FirstOrDefault(x => x.Date == d && x.personId == staff.id);
            //            if (att != null)
            //            {
            //                var attData = att.ToAttendanceDataForDay();
            //                ws.Cell(row, col++)
            //                    .SetValue(attData.Remark.ToDisplayText())
            //                    .Style
            //                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                          
            //                switch (attData.Remark)
            //                {
            //                    case Remark.Present:
            //                        presentCount++;
            //                        if (attData.OverTime != Period.Zero)
            //                        {
            //                            overTimeCount++;
            //                            overTimeHours += attData.OverTime;
            //                        }
            //                        if (attData.LateHour != Period.Zero)
            //                        {
            //                            lateCount++;
            //                            lateHours += attData.LateHour;
            //                        }
            //                        if (attData.WorkHour != Period.Zero)
            //                        {
            //                            workHours += attData.WorkHour;
            //                        }
            //                        break;
            //                    case Remark.SinglePunch:
            //                        presentCount++;
            //                        break;
            //                    case Remark.Absent:
            //                        absenceCount++;
            //                        break;
            //                    case Remark.Holiday:
            //                        holidayCount++;
            //                        break;
            //                    case Remark.OffWork:
            //                        offDayCount++;
            //                        break;
            //                    default:
            //                        break;
            //                }
            //            }
            //            else
            //            {
            //                col++;
            //            }


            //        }

            //        ws.Cell(row, col++).SetValue(presentCount);
            //        ws.Cell(row, col++).SetValue(absenceCount);
            //        ws.Cell(row, col++).SetValue(holidayCount);
            //        ws.Cell(row, col++).SetValue(offDayCount);


            //        row++;
            //    }

            //    if (row-1 != departmentRowStart)
            //    {
            //        ws.Range($"A{departmentRowStart}:A{row - 1}").Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            //    }

            //}

            //return row;

        }


        private void WriteTitle(IXLWorksheet ws, AttendanceData[] data)
        {
            _minDate = data.Min(x => x.Date);
            _maxDate = data.Max(x => x.Date);
            //title
            var col = 1;
            var row = 1;
            ws.Cell(row, col++).Value = "Department";
            ws.Cell(row, col++).Value = "Designation";
            ws.Cell(row, col++).Value = "Emp No.";
            ws.Cell(row, col++).Value = "Emp Name";

            for (var d = _minDate; d <= _maxDate; d = d.AddDays(1))
            {
                ws.Cell(row, col++).SetValue($"{d.Day}{Environment.NewLine}{d:ddd}")
                    .Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(ClosedXML.Excel.XLAlignmentHorizontalValues.Center);
            }

            ws.Cell(row, col++).Value = "PR";
            ws.Cell(row, col++).Value = "AB";
            ws.Cell(row, col++).Value = "HO";
            ws.Cell(row, col++).Value = "WO";

            var titleRow = ws.Range(ws.Cell(row, 1).Address, ws.Cell(row, col - 1).Address);
            titleRow.Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);
        }
    }
}

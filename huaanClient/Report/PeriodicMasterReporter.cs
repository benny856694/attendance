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

        public void Generate(LocalDate from, LocalDate to, string pathToXlsx)
        {
            using (var wb = new ClosedXML.Excel.XLWorkbook())
            {
                var sheet = wb.AddWorksheet();
                WriteTitle(sheet, from, to);
                WriteEmployees(sheet, from, to);
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


        private int WriteEmployees(IXLWorksheet ws, LocalDate from, LocalDate to)
        {
            var ctx = new DataContext();
            ctx.Load(from, to);
            var row = 2;
            foreach (var deparment in ctx.Staffs.GroupBy(x=>x.department_id))
            {
                var departmentRowStart = row;
                foreach (var staff in deparment)
                {
                    var col = 1;
                    var detail = ctx.GetStaffDetails(staff.id);
                    if (detail == null) continue;

                    var counter = new Counter();
                    ws.Cell(row, col++).Value = detail.Department?.name;
                    ws.Cell(row, col++).SetDataType(XLDataType.Text).SetValue(detail.Employeetype?.Employetype_name);
                    ws.Cell(row, col++).SetValue(staff.Employee_code);
                    ws.Cell(row, col++).Value = staff.name;

                    for (var d = from; d <= to; d = d.PlusDays(1))
                    {
                        var att = ctx.Extract(staff.id, d);
                        if (att.DailyAttendanceData.Remark != Remark.OffDuty)
                        {
                            ws.Cell(row, col++)
                                .SetValue(att.DailyAttendanceData.Remark.ToDisplayText())
                                .Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            counter.Count(att.DailyAttendanceData);
                        }
                        else
                        {
                            col++;
                        }


                    }

                    ws.Cell(row, col++).SetValue(counter.presentCount);
                    ws.Cell(row, col++).SetValue(counter.absenceCount);
                    ws.Cell(row, col++).SetValue(counter.holidayCount);
                    ws.Cell(row, col++).SetValue(counter.offDayCount);

                    row++;
                }

                if (row - 1 != departmentRowStart)
                {
                    ws.Range($"A{departmentRowStart}:A{row - 1}").Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
                }

            }

            return row;

        }


        private void WriteTitle(IXLWorksheet ws, LocalDate from, LocalDate to)
        {
            //title
            var col = 1;
            var row = 1;
            ws.Cell(row, col++).Value = "Department";
            ws.Cell(row, col++).Value = "Designation";
            ws.Cell(row, col++).Value = "Emp No.";
            ws.Cell(row, col++).Value = "Emp Name";

            for (var d = from; d <= to; d = d.PlusDays(1))
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

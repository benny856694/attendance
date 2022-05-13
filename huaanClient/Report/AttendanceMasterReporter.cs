using ClosedXML.Excel;
using huaanClient.Database;
using huaanClient.Properties;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    public class AttendanceMasterReporter : IReporter
    {

        public void Generate(DataContext ctx, IXLWorkbook wb)
        {
            var sheet = wb?.AddWorksheet($"AttendanceMaster({ctx.From.ToYearMonth()})");
            WriteTitleLine(sheet, ctx.From, ctx.To);
            WriteAttendanceData(sheet, ctx);
            if(sheet != null)
            {
                sheet.Columns("1").AdjustToContents();
                sheet.Rows("1").Style
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                    .Alignment.Vertical = XLAlignmentVerticalValues.Top;
                sheet.SheetView.FreezeRows(1);
                sheet.SheetView.FreezeColumns(2);
            }
        }

        public void WriteAttendanceData(IXLWorksheet sheet, DataContext ctx)
        {
            var row = 2;
            var col = 1;
            var rowsPerEmployee = 10;
            var blue = XLColor.FromArgb(0, 138, 224);
            var green = XLColor.Green;
            var red = XLColor.Red;
            var black = XLColor.Black;
            var orange = XLColor.FromArgb(226, 107, 10);

            foreach (var person in ctx.Staffs)
            {
                
                var staffDetails = ctx.GetStaffDetails(person.id);
                if (staffDetails == null) continue;

                var personDetail =
                $"{Strings.AttendanceMasterEmpNo}:{staffDetails.Staff.Employee_code}{Environment.NewLine}" +
                $"{staffDetails.Staff.name}{Environment.NewLine}" +
                $"{Strings.AttendanceMasterDept}:{staffDetails.Department?.name}{Environment.NewLine}" +
                $"{Strings.AttendanceMasterDesig}:{staffDetails.Employeetype?.Employetype_name}";
                sheet.Cell(row, col).SetValue(personDetail).Style.Font.SetBold();
                col += 1;

                var rowInc = 0;
                //leading titles
                sheet.Cell(row + rowInc++, col).SetValue(Strings.DailyReportShiftTitle).Style.Font.FontColor = blue;
                sheet.Cell(row + rowInc++, col).SetValue(Strings.AttendanceMasterIn+"1").Style.Font.FontColor = green; 
                sheet.Cell(row + rowInc++, col).SetValue(Strings.AttendanceMasterOut+"1").Style.Font.FontColor = red;
                sheet.Cell(row + rowInc++, col).SetValue(Strings.AttendanceMasterIn+"2").Style.Font.FontColor = green;
                sheet.Cell(row + rowInc++, col).SetValue(Strings.AttendanceMasterOut+"2").Style.Font.FontColor = red;
                sheet.Cell(row + rowInc++, col).SetValue(Strings.AttendanceMasterWH).Style.Font.FontColor = black;
                sheet.Cell(row + rowInc++, col).SetValue(Strings.AttendanceMasterEarly).Style.Font.FontColor = black;
                sheet.Cell(row + rowInc++, col).SetValue(Strings.AttendanceMasterLate).Style.Font.FontColor = black;
                sheet.Cell(row + rowInc++, col).SetValue(Strings.AttendanceMasterOT).Style.Font.FontColor = black;
                sheet.Cell(row + rowInc++, col).SetValue(Strings.AttendanceMasterStatus).Style.Font.FontColor = blue;
                col += 1;
               

                var counter = new Counter();
                for (var d = ctx.From; d <= ctx.To; d = d.PlusDays(1))
                {
                    var context = ctx.Extract(person.id, d);
                    if (context != null)
                    {
                        var attData = context.DailyAttendanceData;
                        if (attData != null && sheet!=null)
                        {
                            sheet.Cell(row + 0, col)
                                   .SetValue(attData.ShiftName ?? attData.Shift?.name)
                                   .Style.Font.SetFontColor(blue)
                                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            sheet.Cell(row + 1, col)
                                   .SetValue(attData.CheckIn1?.ToString("t", CultureInfo.InvariantCulture))
                                   .Style.Font.SetFontColor(green)
                                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            var out2OffSet = attData.CheckIn2.HasValue ? 2 : 4;
                            sheet.Cell(row + out2OffSet, col)
                                .SetValue(attData.CheckOut1?.ToString("t", CultureInfo.InvariantCulture))
                                .Style.Font.SetFontColor(red)
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            sheet.Cell(row + 3, col)
                                   .SetValue(attData.CheckIn2?.ToString("t", CultureInfo.InvariantCulture))
                                   .Style.Font.SetFontColor(green)
                                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            if (attData.CheckOut2.HasValue)
                            {
                                sheet.Cell(row + 4, col)
                                .SetValue(attData.CheckOut2?.ToString("t", CultureInfo.InvariantCulture))
                                .Style.Font.SetFontColor(red)
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            }
                            
                            sheet.Cell(row + 5, col)
                                .SetValue(attData.WorkHour.ToMyString())
                                .Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            sheet.Cell(row + 6, col)
                                .SetValue(attData.EarlyHour.ToMyString())
                                .Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            sheet.Cell(row + 7, col)
                                .SetValue(attData.LateHour.ToMyString())
                                .Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            sheet.Cell(row + 8, col)
                               .SetValue(attData.OverTime.ToMyString())
                               .Style
                               .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        }
                        else//shift name
                        {
                            var shiftNameColor = black;
                            shiftNameColor = context.Shift?.name != null ? blue : orange;
                            sheet.Cell(row + 0, col)
                                   .SetValue(context.Shift?.name ?? 
                                   (context.Date.DayOfWeek == IsoDayOfWeek.Saturday || context.Date.DayOfWeek == IsoDayOfWeek.Sunday ? Strings.ReportRemarkHO : "") )
                                   .Style.Font.SetFontColor(shiftNameColor)
                                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }

                        var statusColor = black;
                        switch (context.Remark)
                        {
                            case Remark.Present:
                                statusColor = green;
                                break;
                            case Remark.SinglePunch:
                                statusColor = blue;
                                break;
                            case Remark.Absence:
                                statusColor = red;
                                break;
                            case Remark.Holiday:
                                statusColor = orange;
                                break;
                            case Remark.Leave:
                                break;
                            case Remark.OffDuty:
                                statusColor = orange;
                                break;
                        }

                        sheet?.Cell(row + 9, col)
                            .SetValue(context.Remark.ToDisplayText())
                            .Style.Font.SetFontColor(statusColor)
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                       
                        counter.Count(context);
                    }
                    col += 1;
                }

                //summary
                rowInc = 0;
                sheet.Cell(row + rowInc++, col).SetValue($"{Strings.AttendanceMasterSumPR}-{counter.presentCount}").Style.Font.SetFontColor(green);
                sheet.Cell(row + rowInc++, col).SetValue($"{Strings.AttendanceMasterSumAB}-{counter.absenceCount}").Style.Font.SetFontColor(red);
                sheet.Cell(row + rowInc++, col).SetValue($"{Strings.AttendanceMasterSumWO}-{counter.leaveDayCount}").Style.Font.SetFontColor(orange);
                sheet.Cell(row + rowInc++, col).SetValue($"{Strings.ReportRemarkHO}-{counter.holidayCount}").Style.Font.SetFontColor(orange);
                sheet.Cell(row + rowInc++, col).SetValue($"{Strings.AttendanceMasterSumWorkHour}-{counter.workHours.Normalize().ToMyString()}");
 
                sheet.Cell(row + rowInc++, col).SetValue($"{Strings.AttendanceMasterSumLateHour}-{counter.lateHours.Normalize().ToMyString()}");
                sheet.Cell(row + rowInc++, col).SetValue($"{Strings.AttendanceMasterSumLT}-{counter.lateCount}");
  
                    
                sheet.Cell(row + rowInc++, col).SetValue($"{Strings.AttendanceMasterSumEarlyHour}-{counter.earlyHours.Normalize().ToMyString()}"); 
                sheet.Cell(row + rowInc++, col).SetValue($"{Strings.AttendanceMasterSumEarly}-{counter.earlyCount}");

                sheet.Cell(row + rowInc++, col).SetValue($"{Strings.AttendanceMasterSumOTHour}-{counter.overTimeHours.Normalize().ToMyString()}"); 
                //sheet.Cell(row + 1, col).SetValue($"{Strings.AttendanceMasterSumOT}-{counter.overTimeCount}");
                    
                sheet.Columns($"{col}:{col + 1}").AdjustToContents();

                sheet.Range($"A{row}:A{row + rowsPerEmployee - 1}").Merge()
                    .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Top);

                //top line
                if (row != 2)
                {
                    sheet.Row(row)
                        .Style
                        .Border.SetTopBorder(XLBorderStyleValues.Thin)
                        .Border.SetTopBorderColor(XLColor.Black);

                }

                //bottom line
                sheet.Row(row + rowsPerEmployee - 1)
                    .Style
                    .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                    .Border.SetBottomBorderColor(XLColor.Black);

                //blank separate row
                sheet.Row(row + rowsPerEmployee)
                    .Merge();
                

                row += rowsPerEmployee + 1; //add a empty row
                col = 1;
            }
        }

        private void WriteTitleLine(ClosedXML.Excel.IXLWorksheet sheet, LocalDate from, LocalDate to)
        {
            if(sheet != null)
            {
                var row = 1;
                var col = 1;
                sheet.Cell(row, col++).Value = Strings.AttendanceMasterReportEmpDetailsTitle;
                sheet.Cell(row, col++).Value = Strings.DailyReportDateTitle;
                for (var d = from; d <= to; d = d.PlusDays(1))
                {
                    sheet.Cell(row, col++).SetValue($"{d.Day}{Environment.NewLine}{d:ddd}")
                        .Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(ClosedXML.Excel.XLAlignmentHorizontalValues.Center);
                }
                sheet.Cell(row, col++).Value = Strings.Summary;
            }
        }
    }
}

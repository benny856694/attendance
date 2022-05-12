﻿using ClosedXML.Excel;
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
            foreach (var person in ctx.Staffs)
            {
                
                var staffDetails = ctx.GetStaffDetails(person.id);
                if (staffDetails == null) continue;


                if (sheet != null)
                {
                    var personDetail =
                   $"{Strings.AttendanceMasterEmpNo}:{staffDetails.Staff.Employee_code}{Environment.NewLine}" +
                   $"{staffDetails.Staff.name}{Environment.NewLine}" +
                   $"{Strings.AttendanceMasterDept}:{staffDetails.Department?.name}{Environment.NewLine}" +
                   $"{Strings.AttendanceMasterDesig}:{staffDetails.Employeetype?.Employetype_name}";
                    sheet.Cell(row, col).SetValue(personDetail).Style.Font.SetBold();
                    col += 1;

                    sheet.Cell(row + 0, col).Value = Strings.DailyReportShiftTitle;
                    sheet.Cell(row + 1, col).Value = Strings.AttendanceMasterIn+"1";
                    sheet.Cell(row + 2, col).Value = Strings.AttendanceMasterOut+"1";
                    sheet.Cell(row + 3, col).Value = Strings.AttendanceMasterIn + "2";
                    sheet.Cell(row + 4, col).Value = Strings.AttendanceMasterOut + "2";
                    sheet.Cell(row + 5, col).Value = Strings.AttendanceMasterWH;
                    sheet.Cell(row + 6, col).Value = Strings.AttendanceMasterEarly;
                    sheet.Cell(row + 7, col).Value = Strings.AttendanceMasterLate;
                    sheet.Cell(row + 8, col).Value = Strings.AttendanceMasterOT;
                    sheet.Cell(row + 9, col).Value = Strings.AttendanceMasterStatus;
                    col += 1;
                }
               

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
                                   .Style
                                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            sheet.Cell(row + 1, col)
                                   .SetValue(attData.CheckIn1?.ToString("t", CultureInfo.InvariantCulture))
                                   .Style
                                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            var out2OffSet = attData.CheckIn2.HasValue ? 2 : 4;
                            sheet.Cell(row + out2OffSet, col)
                                .SetValue(attData.CheckOut1?.ToString("t", CultureInfo.InvariantCulture))
                                .Style
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            sheet.Cell(row + 3, col)
                                   .SetValue(attData.CheckIn2?.ToString("t", CultureInfo.InvariantCulture))
                                   .Style
                                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            if (attData.CheckOut2.HasValue)
                            {
                                sheet.Cell(row + 4, col)
                                .SetValue(attData.CheckOut2?.ToString("t", CultureInfo.InvariantCulture))
                                .Style
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
                        else
                        {
                            sheet.Cell(row + 0, col)
                                   .SetValue(context.Shift?.name ?? 
                                   (context.Date.DayOfWeek == IsoDayOfWeek.Saturday || context.Date.DayOfWeek == IsoDayOfWeek.Sunday ? Strings.ReportRemarkHO : "") )
                                   .Style
                                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }
                       
                        sheet?.Cell(row + 9, col)
                            .SetValue(context.Remark.ToDisplayText())
                            .Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                       

                        counter.Count(context);
                    }
                    col += 1;
                }


                if(sheet != null)
                {
                    sheet.Cell(row + 0, col).SetValue($"{Strings.AttendanceMasterSumPR}-{counter.presentCount}");
                    sheet.Cell(row + 1, col).SetValue($"{Strings.AttendanceMasterSumAB}-{counter.absenceCount}");
                    sheet.Cell(row + 2, col).SetValue($"{Strings.AttendanceMasterSumWO}-{counter.leaveDayCount}");
                    sheet.Cell(row + 3, col).SetValue($"{Strings.ReportRemarkHO}-{counter.holidayCount}");
                    sheet.Cell(row + 4, col).SetValue($"{Strings.AttendanceMasterSumWorkHour}-{counter.workHours.Normalize().ToMyString()}");
 
                    sheet.Cell(row + 5, col).SetValue($"{Strings.AttendanceMasterSumLateHour}-{counter.lateHours.Normalize().ToMyString()}");
                    sheet.Cell(row + 6, col).SetValue($"{Strings.AttendanceMasterSumLT}-{counter.lateCount}");
  
                    
                    sheet.Cell(row + 7, col).SetValue($"{Strings.AttendanceMasterSumEarlyHour}-{counter.earlyHours.Normalize().ToMyString()}"); 
                    sheet.Cell(row + 8, col).SetValue($"{Strings.AttendanceMasterSumEarly}-{counter.earlyCount}");

                    sheet.Cell(row + 9, col).SetValue($"{Strings.AttendanceMasterSumOTHour}-{counter.overTimeHours.Normalize().ToMyString()}"); 
                    //sheet.Cell(row + 1, col).SetValue($"{Strings.AttendanceMasterSumOT}-{counter.overTimeCount}");
                    
                    sheet.Columns($"{col}:{col + 1}").AdjustToContents();

                    sheet.Range($"A{row}:A{row + 9}").Merge()
                        .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Top);
                    sheet.Row(row + 6)
                        .Style
                        .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                        .Border.SetBottomBorderColor(XLColor.LightGray);
                }
                

                row += 11;
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

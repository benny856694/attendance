using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    public class Counter
    {
        public int presentCount = 0; //出勤
        public int absenceCount = 0; //缺勤
        public int overTimeCount = 0; //加班
        public int lateCount = 0; //迟到
        public int earlyCount = 0; //早退
        public int offDayCount = 0; //休息，请假
        public int holidayCount = 0; //节假日
        public Period overTimeHours = Period.Zero;
        public Period lateHours = Period.Zero;
        public Period workHours = Period.Zero;
        public Period earlyHours = Period.Zero;

        public void Count(DailyAttendanceDataContext dataContext)
        {
            var attData = dataContext.DailyAttendanceData;
            switch (dataContext.Remark)
            {
                case Remark.Present:
                    presentCount++;
                    if (attData.OverTime != Period.Zero)
                    {
                        overTimeCount++;
                        overTimeHours += attData.OverTime;
                    }
                    if (attData.EarlyHour != Period.Zero)
                    {
                        earlyCount++;
                        earlyHours += attData.EarlyHour;

                    }
                    if (attData.LateHour != Period.Zero)
                    {
                        lateCount++;
                        lateHours += attData.LateHour;
                    }
                    if (attData.WorkHour != Period.Zero)
                    {
                        workHours += attData.WorkHour;
                    }
                    break;
                case Remark.SinglePunch:
                    presentCount++;
                    break;
                case Remark.Absence:
                    absenceCount++;
                    break;
                case Remark.Holiday:
                    holidayCount++;
                    break;
                case Remark.Leave:
                    offDayCount++;
                    break;
                case Remark.OffDuty:
                    offDayCount++;
                    break;
                default:
                    break;
            }
        }

    }
}

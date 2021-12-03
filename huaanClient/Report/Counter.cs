﻿using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    public class Counter
    {
        public int presentCount = 0;
        public int absenceCount = 0;
        public int overTimeCount = 0;
        public int lateCount = 0;
        public int earlyCount = 0;
        public int offDayCount = 0;
        public int holidayCount = 0;
        public Period overTimeHours = Period.Zero;
        public Period lateHours = Period.Zero;
        public Period workHours = Period.Zero;
        public Period earlyHours = Period.Zero;

        public void Count(DailyAttendanceData attData)
        {
            switch (attData.Remark)
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
                default:
                    break;
            }
        }

    }
}

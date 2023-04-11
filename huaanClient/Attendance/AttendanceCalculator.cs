using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace huaanClient.Attendance
{
    internal class AttendanceCalculator: Progress<int>
    {
        public void CalculateForPeriod(DateTime from, DateTime to, CancellationToken token)
        {
            new AttendanceAlgorithm().getpersonnel(from.ToAppTimeString(), to.ToAppTimeString(), 1, token);
        }

        public Task CalculateForDateAsync(DateTime date, CancellationToken token) 
        {
            var dateOnly = date.Date;
            var stepsInMinutes = 10;
            var progress = 0;
            var total = 24 * 60;
            var endTime = dateOnly.AddDays(1);
            return Task.Run(() =>
            {
                for (var d = dateOnly; d < endTime; d = d.AddMinutes(stepsInMinutes))
                {
                    var from = d;
                    var to = d + TimeSpan.FromMinutes(10);
                    Debug.WriteLine($"beging calc: {from} -> {to}");
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    CalculateForPeriod(from, to, token);
                    progress += stepsInMinutes;
                    var percent = progress * 100.0 / total;
                    base.OnReport(Convert.ToInt32(percent));
                }

            });
        }

    }
}

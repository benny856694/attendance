using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace huaanClient.Attendance
{
    internal class AttendanceCalculator: Progress<ProgressValue>
    {
        public void CalculateForPeriod(DateTime from, DateTime to, CancellationToken token)
        {
            new AttendanceAlgorithm().getpersonnel(from.ToAppTimeString(), to.ToAppTimeString(), 1, token);
        }

        public async Task CalculateForDateAsync(DateTime date, CancellationToken token) 
        {
            var startDate = date.Date;
            var stepsInMinutes = Constants.StepInMinutes;
            var currentStep = 0;
            var steps = 24 * 60 / Constants.StepInMinutes;
            var endTime = startDate.AddDays(1);
            await Task.Factory.StartNew(() =>
            {
                for (var d = startDate; d < endTime; d = d.AddMinutes(stepsInMinutes))
                {
                    var from = d;
                    var to = d + TimeSpan.FromMinutes(10);
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    CalculateForPeriod(from, to, token);
                    currentStep++;
                    var percent = currentStep * 100.0 / steps;
                    var msg = $"{from} -> {to}";
                    var v = new ProgressValue
                    {
                        Percent = (int)percent,
                        Message = msg
                    };
                    base.OnReport(v);
                }
            });
        }

    }
}

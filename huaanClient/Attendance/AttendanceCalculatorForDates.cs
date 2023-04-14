using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace huaanClient.Attendance
{
    internal class AttendanceCalculatorForDates: Progress<ProgressValue>
    {
        private AttendanceCalculatorForDates() {}
        private static object locker = new object();
        private static AttendanceCalculatorForDates _instance;
        private ProgressValue _state = new ProgressValue();

        public static AttendanceCalculatorForDates Instance
        {
            get
            {
                lock (locker)
                {
                    if (_instance == null)
                    {
                        _instance = new AttendanceCalculatorForDates();
                    }
                    return _instance;
                }
            }
        }

        public ProgressValue Status
        {
            get
            {
                lock(locker)
                {
                    return _state; 
                }
            }
            set
            {
                lock (locker)
                {
                    _state = value;
                }
            }
        }



        public async Task CalcForDatesAsync(IList<DateTime> dates, CancellationToken token)
        {
            var sortedDates = dates.OrderBy(d => d.Date);
            var totalSteps = dates.Count() * 24 * 60 / Constants.StepInMinutes;
            var steps = 0;
            foreach (var date in sortedDates)
            {
                var calc = new AttendanceCalculator();
                calc.ProgressChanged += (s, e) =>
                {
                    steps++;
                    var percent = steps * 100.0 / totalSteps;
                    var arg = new ProgressValue
                    {
                        busy = true,
                        Percent = (int)percent,
                        Message = e.Message
                    };
                    Status = arg;
                    OnReport(arg);
                };
                await calc.CalculateForDateAsync(date, token);
            }
            var e1 = new ProgressValue { Percent = 100, done = true };
            Status = e1;
            OnReport(e1);

        }
      
    }
}

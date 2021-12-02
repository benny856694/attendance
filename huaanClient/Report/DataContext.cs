using Dapper.Contrib.Extensions;
using DBUtility.SQLite;
using huaanClient.Database;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    public class DataContext
    {
        public Department[] Departments { get; private set; }
        public Employetype[] Employetypes { get; private set; }
        public Shift[] Shifts { get; private set; }
        public Staff[] Staffs { get; private set; }
        public AttendanceGroup[] AttendanceGroups { get; private set; }
        public AttendanceDataForDay[] AttendanceData { get; private set; }

        public void Load(string from, string to)
        {
            using (var c = SQLiteHelper.GetConnection())
            {
                Departments = c.GetAll<Department>().ToArray();
                Employetypes = c.GetAll<Employetype>().ToArray();
                Shifts = c.GetAll<Shift>().ToArray();
                Staffs = c.GetAll<Staff>().ToArray();
                AttendanceGroups = c.GetAll<AttendanceGroup>().ToArray();
            }

            AttendanceData = GetData.queryAttendanceinformation(from, to, null, null, null, null, null, null)
                .Select(x=>x.ToAttendanceDataForDay())
                .ToArray();
        }
    }
}

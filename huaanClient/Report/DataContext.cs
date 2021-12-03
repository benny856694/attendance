using Dapper.Contrib.Extensions;
using DBUtility.SQLite;
using huaanClient.Database;
using NodaTime;
using NodaTime.Extensions;
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
        public Employeetype[] Employetypes { get; private set; }
        public Shift[] Shifts { get; private set; }
        public Staff[] Staffs { get; private set; }
        public AttendanceGroup[] AttendanceGroups { get; private set; }
        public DailyAttendanceData[] AttendanceData { get; private set; }

        public void Load(string from, string to)
        {
            using (var c = SQLiteHelper.GetConnection())
            {
                Departments = c.GetAll<Department>().ToArray();
                Employetypes = c.GetAll<Employeetype>().ToArray();
                Shifts = c.GetAll<Shift>().ToArray();
                Staffs = c.GetAll<Staff>().ToArray();
                AttendanceGroups = c.GetAll<AttendanceGroup>().ToArray();
            }

            AttendanceData = GetData.queryAttendanceinformation(from, to, null, null, null, null, null, null)
                .Select(x=>x.ToAttendanceDataForDay())
                .ToArray();
        }

        //0: 表示休息，其他：对应的shift id
        public int GetShiftIdForDay(string staffId, LocalDate date)
        {
            var staff = Staffs.FirstOrDefault(x => x.id == staffId);
            if (staff == null) return 0;
            return GetShiftIdForDay(staff, date);
        }

        public int GetShiftIdForDay(Staff staff, LocalDate date)
        {
            var attendanceGroup = AttendanceGroups.FirstOrDefault(x => x.id == staff.AttendanceGroup_id);
            if (attendanceGroup == null) return 0;
            var shiftId = attendanceGroup.GetShiftIdForDay(date.DayOfWeek.ToDayOfWeek());
            return shiftId;
        }

        public DailyAttendanceDataContext Extract(string staffId, LocalDate date)
        {
            var staff = Staffs.FirstOrDefault(x => x.id == staffId);
            var department = Departments.FirstOrDefault(x => x.id == staff.department_id);

            DailyAttendanceData data = null;
            Shift shift = null;
            var shiftId = GetShiftIdForDay(staff, date);
            if (shiftId == 0)
            {
                data = DailyAttendanceData.OffDuty;
            }
            else
            {
                shift = Shifts.FirstOrDefault(x => x.id == shiftId);
                var d = AttendanceData.FirstOrDefault(x => x.EmployeeId == staffId && x.Date == date) ?? DailyAttendanceData.Absense;
                data = d;
            }
            data.Date = date;

            var employeeType = Employetypes.FirstOrDefault(x => x.id == staff.Employetype_id);

            return new DailyAttendanceDataContext
            {
                Staff = staff,
                DailyAttendanceData = data,
                Shift = shift,
                Date = date,
                Department = department,
                Employeetype = employeeType
            };

        }
    }
}

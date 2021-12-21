using Dapper.Contrib.Extensions;
using DBUtility.SQLite;
using huaanClient.Database;
using NodaTime;
using NodaTime.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public Holiday[] Holidays { get; private set; }
        public DailyAttendanceData[] AttendanceData { get; private set; }
        public LocalDate From { get; private set; }
        public LocalDate To { get; private set; }

        public void Load(LocalDate from, LocalDate to)
        {
            From = from;
            To = to;
            using (var c = SQLiteHelper.GetConnection())
            {
                Departments = c.GetAll<Department>().ToArray();
                Employetypes = c.GetAll<Employeetype>().ToArray();
                Shifts = c.GetAll<Shift>().ToArray();
                Staffs = c.GetAll<Staff>().ToArray();
                AttendanceGroups = c.GetAll<AttendanceGroup>().ToArray();
                Holidays = c.GetAll<Holiday>().ToArray();
            }

            AttendanceData = GetData.queryAttendanceinformation(
                from.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), 
                to.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),  null, null, null, null, null, null)
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

        public bool? IsHolidayForDay(Staff staff, LocalDate date)
        {
            var ag = AttendanceGroups.FirstOrDefault(x => x.id == staff.AttendanceGroup_id);
            if (ag == null) return null;

            var holiday = Holidays.FirstOrDefault(x=>x.AttendanceGroupid == ag.id && date == x.date.ToLocalDateTime().Date);
            return holiday != null;
            
        }

        public StaffDetails GetStaffDetails(string staffId)
        {
            var staff = Staffs.FirstOrDefault(x => x.id == staffId);
            if (staff == null) return null;

            var department = Departments.FirstOrDefault(x => x.id == staff.department_id);
            var employeeType = Employetypes.FirstOrDefault(x => x.id == staff.Employetype_id);
            return new StaffDetails
            {
                Department = department,
                Employeetype = employeeType,
                Staff = staff
            };
        }


        public DailyAttendanceDataContext Extract(string staffId, LocalDate date)
        {
            var staffDetails = GetStaffDetails(staffId);

            DailyAttendanceData data = null;
            Shift shift = null;
            var shiftId = GetShiftIdForDay(staffDetails.Staff, date);

            var remark = Remark.Present;
            if (shiftId == 0)
            {
                data = DailyAttendanceData.OffDuty;
                remark = Remark.OffDuty;
            }
            else
            {
                shift = Shifts.FirstOrDefault(x => x.id == shiftId);
                var d = AttendanceData.FirstOrDefault(x => x.EmployeeId == staffId && x.Date == date); 
                data = d;
                remark = d == null ? Remark.Absence : d.Remark;
            }

            var isHoliday = IsHolidayForDay(staffDetails.Staff, date);
            if (isHoliday.Value == true)
            {
                remark = Remark.Holiday;
            }

            return new DailyAttendanceDataContext
            {
                StaffDetails = staffDetails,
                DailyAttendanceData = data,
                Shift = shift,
                Date = date,
                Remark = remark,
            };

        }
    }
}

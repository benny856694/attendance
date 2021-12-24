using Dapper;
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
        public Department[] AllDepartments { get; private set; }
        public Employeetype[] AllEmployetypes { get; private set; }
        public Shift[] AllShifts { get; private set; }
        public Staff[] AllStaffs { get; private set; }
        public AttendanceGroup[] AllAttendanceGroups { get; private set; }
        public Holiday[] AllHolidays { get; private set; }
        public Staff[] Staffs { get; private set; }
        public DailyAttendanceData[] AttendanceData { get; private set; }
        public LocalDate From { get; private set; }
        public LocalDate To { get; private set; }

        public void Load(QueryCriteria criteria)
        {
            From = criteria.From;
            To = criteria.To;
            using (var c = SQLiteHelper.GetConnection())
            {
                AllDepartments = c.GetAll<Department>().ToArray();
                AllEmployetypes = c.GetAll<Employeetype>().ToArray();
                AllShifts = c.GetAll<Shift>().ToArray();
                AllStaffs = c.GetAll<Staff>().ToArray();
                AllAttendanceGroups = c.GetAll<AttendanceGroup>().ToArray();
                AllHolidays = c.GetAll<Holiday>().ToArray();
            }


            Staffs = GetStaffs(criteria);

            AttendanceData = GetData.queryAttendanceinformation(
                From.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), 
                To.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),  
                name:  criteria.Name,
                late: criteria.IsLate,
                Leaveearly: criteria.LeaveEarly,
                isAbsenteeism: criteria.IsAbsense, 
                departments: criteria.DepartmentNames, 
                null)
                .Select(x=>x.ToAttendanceDataForDay())
                .ToArray();
        }

        private Staff[] GetStaffs(QueryCriteria criteria)
        {
            IEnumerable<Staff> staffs  = AllStaffs.AsEnumerable();
            if (!string.IsNullOrEmpty(criteria.Name))
            {
                staffs = staffs.Where(el => el.name?.ToUpperInvariant().Contains(criteria.Name.ToUpperInvariant()) == true);
            }
            if (!string.IsNullOrEmpty(criteria.DepartmentNames))
            {
                var staffWithDepartment = from s in AllStaffs
                                          join d in AllDepartments on s.department_id equals d.id
                                          select new { Staff = s, Department = d };
                var selectedDepartments = criteria.DepartmentNames.Split(',');
                staffs = staffWithDepartment.Where(el => selectedDepartments.Any(dep => dep == el.Department.name)).Select(el => el.Staff);
            }

            return staffs.ToArray();

        }

        //0: 表示休息，其他：对应的shift id
        public int GetShiftIdForDay(string staffId, LocalDate date)
        {
            var staff = AllStaffs.FirstOrDefault(x => x.id == staffId);
            if (staff == null) return 0;
            return GetShiftIdForDay(staff, date);
        }

        public int GetShiftIdForDay(Staff staff, LocalDate date)
        {
            var attendanceGroup = AllAttendanceGroups.FirstOrDefault(x => x.id == staff.AttendanceGroup_id);
            if (attendanceGroup == null) return 0;
            var shiftId = attendanceGroup.GetShiftIdForDay(date.DayOfWeek.ToDayOfWeek());
            return shiftId;
        }

        public bool? IsHolidayForDay(Staff staff, LocalDate date)
        {
            var ag = AllAttendanceGroups.FirstOrDefault(x => x.id == staff.AttendanceGroup_id);
            if (ag == null) return null;

            var holiday = AllHolidays.FirstOrDefault(x=>x.AttendanceGroupid == ag.id && date == x.date.ToLocalDateTime().Date);
            return holiday != null;
            
        }

        public StaffDetails GetStaffDetails(string staffId)
        {
            var staff = AllStaffs.FirstOrDefault(x => x.id == staffId);
            if (staff == null) return null;

            var department = AllDepartments.FirstOrDefault(x => x.id == staff.department_id);
            var employeeType = AllEmployetypes.FirstOrDefault(x => x.id == staff.Employetype_id);
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
                shift = AllShifts.FirstOrDefault(x => x.id == shiftId);
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

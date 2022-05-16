using Dapper;
using Dapper.Contrib.Extensions;
using DBUtility.SQLite;
using huaanClient.Database;
using huaanClient.Properties;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Text;
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

            int? pageIndex = null;
            int? pageSize = null;
            if (criteria.PageIndex != null && criteria.PageSize != null)
            {
                pageIndex = int.Parse(criteria.PageIndex);
                pageSize = int.Parse(criteria.PageSize);
            }

            AttendanceData = 
                GetData.queryAttendanceinformation(
                From.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                To.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                name: criteria.Name,
                late: criteria.IsLate,
                Leaveearly: criteria.LeaveEarly,
                isAbsenteeism: criteria.IsAbsense,
                departments: criteria.DepartmentNames,
                null,
                pageIndex: pageIndex,
                pageSize: pageSize)
                .Select(x => x.ToAttendanceDataForDay(this))
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

        //0: 数据库中表示休息，其他：对应的shift id
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
                remark = Remark.OffDuty;
            }
            else
            {
                var d = AttendanceData.FirstOrDefault(x => x.EmployeeId == staffId && x.Date == date); 
                data = d;
                shift = d?.ToShift() ?? AllShifts.FirstOrDefault(x => x.id == shiftId);
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

        public List<DailyAttendance> ToDailyAttendance()
        {
            var timePattern = LocalTimePattern.CreateWithInvariantCulture("HH:mm");
            var result = new List<DailyAttendance>();
            foreach (var data in AttendanceData)
            {
                var details = GetStaffDetails(data.EmployeeId);
                var dailyAttendance = new DailyAttendance()
                {
                    Name = details?.Staff?.name ?? $"{data.EmployeeName}({Strings.Deleted})",
                    Id = data.EmployeeId,
                    Department = details?.Department?.name ?? data.EmployeeDepartment,
                    PersonalNo = details?.Staff?.Employee_code ?? data.EmployeeCode,
                    Date = data.Date,
                    Shift = BuildShiftDesc(timePattern, data),
                    CheckIn1 = data.CheckIn1,
                    CheckOut1 = data.CheckOut1,
                    CheckIn2 = data.CheckIn2,
                    CheckOut2 = data.CheckOut2,
                    Temperature = data.Temperature,
                    LateMinutes = data.LateHour.Normalize(),
                    EarlyMinutes = data.EarlyHour.Normalize(),
                    WorkHour = data.WorkHour.Normalize(),
                    Status = data.Remark,
                };

                result.Add(dailyAttendance);
            }
            return result;
        }

        private static string BuildShiftDesc(LocalTimePattern timePattern, DailyAttendanceData data)
        {
            var s = $"{data.ShiftName}-{timePattern.Format(data.ShiftStart1.GetValueOrDefault())}-{timePattern.Format(data.ShiftEnd1.GetValueOrDefault())}";
            if (data.ShiftStart2.HasValue && data.ShiftEnd2.HasValue)
            {
                s += $";{timePattern.Format(data.ShiftStart2.GetValueOrDefault())}-{timePattern.Format(data.ShiftEnd2.GetValueOrDefault())}";
            }
            return s;
        }

        public List<MonthlyAttendance> ToMonthlyAttendance()
        {
            var result = new List<MonthlyAttendance>();
            foreach (var person in Staffs)
            {
                var monthlyAttendance = new MonthlyAttendance();

                var staffDetails = this.GetStaffDetails(person.id);
                if (staffDetails == null) continue;
                monthlyAttendance.Department = staffDetails?.Department?.name;
                monthlyAttendance.Designation = staffDetails?.Employeetype?.Employetype_name;
                monthlyAttendance.EmployeeNo = staffDetails?.Staff.Employee_code;
                monthlyAttendance.EmployeeName = staffDetails?.Staff.name;
                monthlyAttendance.YearMonth = this.From.ToYearMonth();

                
                
                var counter = new Counter();
                for (var d = this.From; d <= this.To; d = d.PlusDays(1))
                {
                    var context = this.Extract(person.id, d);
                    if (context != null)
                    {
                        counter.Count(context);
                    }
                }

                monthlyAttendance.PresentDaysCount = counter.presentCount;
                monthlyAttendance.AbsentDaysCount = counter.absenceCount;
                monthlyAttendance.LeaveDaysCount = counter.leaveDayCount;
                monthlyAttendance.HolidaysCount = counter.holidayCount;
                monthlyAttendance.TotalLateHours = counter.lateHours.Normalize();
                monthlyAttendance.TotalLateDays = counter.lateCount;
                monthlyAttendance.TotalEarlyHours = counter.earlyHours.Normalize();
                monthlyAttendance.TotalEarlyDays = counter.earlyCount;
                result.Add(monthlyAttendance);
            }

            return result.OrderBy(x => x.Department).ToList();
        }
    }
}

using ClosedXML.Excel;
using Dapper;
using Dapper.Contrib.Extensions;
using DBUtility.SQLite;
using huaanClient.Database;
using huaanClient.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    class DailyAttendance
    {
        public void Generate(List<string> employeeIds, DateTime day, string pathToXlsx)
        {
            List<Employetype> employeeTypes;
            List<Department> departments;
            List<AttendanceData> attendanceData;
            List<Staff> staffs;
            var idsString = string.Join(",", employeeIds.Select(x => $"'x'"));
            using (var c = SQLiteHelper.GetConnection())
            {
                employeeTypes = c.GetAll<Employetype>().ToList();
                departments = c.GetAll<Department>().ToList();
                staffs = c.GetAll<Staff>().ToList();
                attendanceData = c.Query<AttendanceData>( $"SELECT * FROM  Attendance_Data WHERE personId in ({idsString}) AND strftime('%Y-%m-%d', Date) == '{day:yyyy-MM-dd}'" ).ToList();
            }


            using (var wb = new XLWorkbook())
            {
                var ws = wb.AddWorksheet();



            }
        }
    }
}

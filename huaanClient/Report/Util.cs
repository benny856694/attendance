using Dapper.Contrib.Extensions;
using DBUtility.SQLite;
using huaanClient.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    public static class Util
    {
        public static (List<Employetype> employeeTypes, List<Department> departments, List<Staff> staffs) LoadDb()
        {
            List<Employetype> employeeTypes;
            List<Department> departments;
            List<Staff> staffs;

            using (var c = SQLiteHelper.GetConnection())
            {
                employeeTypes = c.GetAll<Employetype>().ToList();
                departments = c.GetAll<Department>().ToList();
                staffs = c.GetAll<Staff>().ToList();
            }

            return (employeeTypes, departments, staffs);
        }

        public static string GetEmployeeTypeName(List<Staff> staffs, List<Employetype> employeeTypes, string employeeId)
        {
            var employeeTypeId = staffs.FirstOrDefault(x => x.id == employeeId)?.Employetype_id;
            return employeeTypes.FirstOrDefault(x => x.id == employeeTypeId)?.Employetype_name ?? "";
        }
    }
}

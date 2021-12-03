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
        public static List<T> LoadFromDb<T>() where T: class
        {
            List<T> results;

            using (var c = SQLiteHelper.GetConnection())
            {
                results = c.GetAll<T>().ToList();
            }

            return results;
        }

        public static string GetEmployeeTypeName(List<Staff> staffs, List<Employeetype> employeeTypes, string employeeId)
        {
            var employeeTypeId = staffs.FirstOrDefault(x => x.id == employeeId)?.Employetype_id;
            return employeeTypes.FirstOrDefault(x => x.id == employeeTypeId)?.Employetype_name ?? "";
        }
    }
}

using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    public class QueryCriteria
    {
       public LocalDate From;
       public LocalDate To;
       public string Name;
       public int? DepartmentId;
       public bool? IsAbsense;
       public bool? IsLate;
       public bool? LeaveEarly;
    }
}

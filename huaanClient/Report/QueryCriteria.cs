﻿using NodaTime;
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
       public string DepartmentNames;
       public string IsAbsense;
       public string IsLate;
       public string LeaveEarly;
        public string PageIndex;
        public string PageSize;
    }
}

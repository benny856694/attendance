using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    public interface IReporter
    {
        void Generate(DataContext dataContext, string fileName);
    }
}

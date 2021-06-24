using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
    class StaffMapper : AutoClassMapper<Staff>
    {
        public override void Table(string tableName)
        {
            base.Table("staff");
        }
    }

    class EquipmentDistributionMapper : AutoClassMapper<EquipmentDistribution>
    {
        public override void Table(string tableName)
        {
            base.Table("Equipment_distribution");
        }
    }


    class AttendanceDataMapper : AutoClassMapper<AttendanceData>
    {
        public override void Table(string tableName)
        {
            base.Table("Attendance_Data");
        }
    }
}

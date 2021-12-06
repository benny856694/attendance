using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    public enum Remark
    {
        Present, //全勤
        SinglePunch,  //单次打卡
        Absence,  //旷工
        Holiday, //节假日
        Leave, //请假
        OffDuty, //休息日
    }
}

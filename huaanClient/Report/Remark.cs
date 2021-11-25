using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Report
{
    public enum Remark
    {
        Present, //出勤
        SinglePunch,  //单次打卡
        Absent,  //旷工
        Early, // 早退
        Late, //迟到
        Holiday, //节假日
        OffWork, //请假
    }
}

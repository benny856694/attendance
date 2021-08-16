using CCWin.SkinControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient
{
    public static class Extensions
    {
        public static float? toFloat(this string s) => s.IsNullOrEmpty() ? null : (float?)float.Parse(s);
    }
}

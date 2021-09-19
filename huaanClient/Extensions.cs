using CCWin.SkinControl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient
{
    public static class Extensions
    {
        public static float? toFloat(this string s) => s.IsNullOrEmpty() ? null : (float?)float.Parse(s, CultureInfo.InvariantCulture);

        public static string toFahreinheit(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            
            float.TryParse(s, out var celsius);
            return celsius == 0.0 ? "" : (celsius * 9 / 5 + 32).ToString();
        }

        public static float toCelsius(this float fahreinheit)
        {
            return  (fahreinheit - 32) * 5 / 9;
        }
    }
}

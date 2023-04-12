using CCWin.SkinControl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace huaanClient
{
    public static class Extensions
    {
        public static float? toFloat(this string s) => s.IsNullOrEmpty() ? null : (float?)float.Parse(s, CultureInfo.InvariantCulture);

        public static int? toIntOrNull(this string s)
        {
            if (int.TryParse(s, out var v))
            {
                return v;
            }
            return null;
        }

        public static string toFahreinheit(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            
            float.TryParse(s, out var celsius);
            return celsius.toFahreinheit().ToString("f2");
        }

        public static float toFahreinheit(this float celsius)
        {
            return  (celsius * 9 / 5 + 32);
        }

        public static float toCelsius(this float fahreinheit)
        {
            return  (fahreinheit - 32) * 5 / 9;
        }
    
        public static (int hour, int minute) toHourMinute(this string s)
        {
            var sections = s.Split(':');
            var h = int.Parse(sections[0]);
            var m = int.Parse(sections[1]);
            return (h, m);
        }

        public static bool IsGrayScale(this Image img)
        {

            if (img is Bitmap bmp)
            {
                var sz = new Size(bmp.Width, bmp.Height);
                var rnd = new Random(DateTime.Now.Second);
                var grayCount = 0;
                for (int i = 0; i < 10; i++)
                {
                    var x = rnd.Next(0, sz.Width);
                    var y = rnd.Next(0, sz.Height);
                    var c = bmp.GetPixel(x, y);
                    var isGray = c.R == c.G && c.R == c.B;
                    grayCount += isGray ? 1 : 0;
                }
                return grayCount > 5;
            }
            else
            {
                throw new InvalidOperationException("image is not bitmap");
            }
        }

        public static string Format(this string template, params object[] args) => string.Format(template, args);

        public static long ToUnixTimestamp(this DateTime dt) => (long)(dt - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;

        public static string ToAppTimeString(this DateTime dt) => dt.ToString(Constants.DateTimeFormat);

        public static DateTime ToDate(this string dt) => DateTime.ParseExact(dt, "yyyy-MM-dd", CultureInfo.InvariantCulture);

    }
}

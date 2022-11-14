using System.Linq;
using System.Reflection;

namespace huaanClient
{
    internal static class StaffRecorExtension
    {
        public static PropertyInfo[] propertyInfos = typeof(StaffRecord).GetProperties().Where(x => x.PropertyType == typeof(string) && x.CanRead && x.CanWrite).ToArray();


        public static StaffRecord TrimAllProperties(this StaffRecord staffRecord)
        {
            foreach (var propertyInfo in propertyInfos)
            {
                var v = propertyInfo.GetValue(staffRecord) as string;
                if(v!=null)
                {
                    propertyInfo.SetValue(staffRecord, v.Trim());
                }
            }

            return staffRecord;
        }
    }
}
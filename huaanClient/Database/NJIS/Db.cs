using System;
using System.Configuration;

namespace huaanClient.Database.NJIS
{
    internal class Db
    {
        static Lazy<IFreeSql> sqliteLazy = new Lazy<IFreeSql>(() =>
        {
            var cs = ConfigurationManager.ConnectionStrings["njis"].ConnectionString;
            return new FreeSql.FreeSqlBuilder()
                     .UseConnectionString(FreeSql.DataType.SqlServer, cs)
                     .UseAutoSyncStructure(false)
                     .Build();
        });
        public static IFreeSql Njis => sqliteLazy.Value;
    }
}

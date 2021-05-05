using grate.Configuration;
using grate.unittests.TestInfrastructure;

namespace grate.unittests
{
    public static class GrateTestContext
    {
        public static SqlTestContext SqlServer = new();
        public static SqlTestContext Oracle = new();
        public static IGrateTestContext PostgreSql = new PostgreSqlGrateTestContext();

        public static IGrateTestContext GetTestContext(DatabaseType databaseType) => databaseType switch
        {
            //DatabaseType.oracle => Oracle;
            //DatabaseType.sqlserver => SqlServer;
            DatabaseType.postgresql => PostgreSql
        };
    }

    public class SqlTestContext
    {
        public string? AdminPassword { get; set; }
        public int? Port { get; set; }
    }
   
}
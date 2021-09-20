using grate.unittests.TestInfrastructure;

namespace grate.unittests
{
    public static class GrateTestContext
    {
        public static readonly IGrateTestContext SqlServer = new SqlServerGrateTestContext();
        public static readonly SqlTestContext Oracle = new();
        public static readonly IGrateTestContext PostgreSql = new PostgreSqlGrateTestContext();
        public static readonly IGrateTestContext MariaDB = new MariaDbGrateTestContext();

        // public static IGrateTestContext GetTestContext(DatabaseType databaseType) => databaseType switch
        // {
        //     //DatabaseType.oracle => Oracle;
        //     DatabaseType.sqlserver => SqlServer,
        //     DatabaseType.postgresql => PostgreSql
        // };
    }

    public class SqlTestContext
    {
        public string? AdminPassword { get; set; }
        public int? Port { get; set; }
    }

}

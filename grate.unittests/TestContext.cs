using grate.unittests.TestInfrastructure;

namespace grate.unittests
{
    public static class GrateTestContext
    {
        internal static readonly SqlServerGrateTestContext SqlServer = new SqlServerGrateTestContext();
        internal static readonly SqlTestContext Oracle = new();
        internal static readonly PostgreSqlGrateTestContext PostgreSql = new PostgreSqlGrateTestContext();
        internal static readonly MariaDbGrateTestContext MariaDB = new MariaDbGrateTestContext();
        internal static readonly SqliteGrateTestContext Sqlite = new SqliteGrateTestContext();

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

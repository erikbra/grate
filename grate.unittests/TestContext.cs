using NUnit.Framework;
using Unit_tests.TestInfrastructure;

// There are some parallelism issues, but this does not solve it
//[assembly:LevelOfParallelism(1)]

namespace Unit_tests;

public static class GrateTestContext
{
    internal static readonly SqlServerGrateTestContext SqlServer = new();
    internal static readonly SqlServerGrateTestContext SqlServerCaseSensitive = new("Latin1_General_CS_AS"); //CS == Case Sensitive
    internal static readonly OracleGrateTestContext Oracle = new();
    internal static readonly PostgreSqlGrateTestContext PostgreSql = new();
    // ReSharper disable once InconsistentNaming
    internal static readonly MariaDbGrateTestContext MariaDB = new();
    internal static readonly SqliteGrateTestContext Sqlite = new();
}

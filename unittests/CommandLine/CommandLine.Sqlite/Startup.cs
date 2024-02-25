using grate.Configuration;
using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace CommandLine.Sqlite;

// ReSharper disable once UnusedType.Global
public class Startup: CommandLine.Common.Startup
{
    protected override DatabaseType DatabaseType => DatabaseType.SQLite;
    protected override Type TestContainerType => typeof(SqliteTestContainer);
    protected override Type TestContextType => typeof(SqliteGrateTestContext);
}

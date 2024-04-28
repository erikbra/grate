using grate.Configuration;
using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace CommandLine.Sqlite;

// ReSharper disable once UnusedType.Global
public class Startup: CommandLine.Common.Startup<SqliteTestDatabase, SqliteTestDatabase, SqliteGrateTestContext>
{
    protected override DatabaseType DatabaseType => DatabaseType.SQLite;
}

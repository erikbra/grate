using grate.Configuration;
using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Docker.Sqlite;

// ReSharper disable once UnusedType.Global
public class Startup: Docker.Common.Startup<SqliteTestDatabase, SqliteTestDatabase, SqliteGrateTestContext>
{
    protected override DatabaseType DatabaseType => DatabaseType.SQLite;
}

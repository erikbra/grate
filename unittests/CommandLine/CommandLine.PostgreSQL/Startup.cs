using grate.Configuration;
using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace CommandLine.PostgreSQL;

// ReSharper disable once UnusedType.Global
public class Startup: Common.Startup<PostgreSqlTestContainerDatabase, PostgreSqlExternalDatabase, PostgreSqlGrateTestContext>
{
    protected override DatabaseType DatabaseType => DatabaseType.PostgreSQL;
}

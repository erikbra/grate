using grate.Configuration;
using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Docker.PostgreSQL;

// ReSharper disable once UnusedType.Global
public class Startup: Docker.Common.Startup<PostgreSqlTestContainerDatabase, PostgreSqlExternalDatabase, PostgreSqlGrateTestContext>
{
    protected override DatabaseType DatabaseType => DatabaseType.PostgreSQL;
}

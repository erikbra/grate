using grate.Configuration;
using SqlServer.TestInfrastructure;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Docker.SqlServer;

// ReSharper disable once UnusedType.Global
public class Startup: Docker.Common.Startup<SqlServerTestContainerDatabase, SqlServerExternalDatabase, SqlServerGrateTestContext>
{
    protected override DatabaseType DatabaseType => DatabaseType.SQLServer;
}

using grate.Configuration;
using SqlServer.TestInfrastructure;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace CommandLine.SqlServer;

// ReSharper disable once UnusedType.Global
public class Startup: Common.Startup<SqlServerTestContainerDatabase, SqlServerExternalDatabase, SqlServerGrateTestContext>
{
    protected override DatabaseType DatabaseType => DatabaseType.SQLServer;
}

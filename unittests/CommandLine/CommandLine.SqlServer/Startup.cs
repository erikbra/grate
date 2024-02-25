using grate.Configuration;
using SqlServer.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace CommandLine.SqlServer;

// ReSharper disable once UnusedType.Global
public class Startup: CommandLine.Common.Startup
{
    protected override DatabaseType DatabaseType => DatabaseType.SQLServer;
    protected override Type TestContainerType => typeof(SqlServerTestContainer);
    protected override Type TestContextType => typeof(SqlServerGrateTestContext);
}

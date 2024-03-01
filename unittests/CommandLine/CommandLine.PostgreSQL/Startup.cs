using grate.Configuration;
using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace CommandLine.PostgreSQL;

// ReSharper disable once UnusedType.Global
public class Startup: CommandLine.Common.Startup
{
    protected override DatabaseType DatabaseType => DatabaseType.PostgreSQL;
    protected override Type TestContainerType => typeof(PostgreSqlTestContainer);
    protected override Type TestContextType => typeof(PostgreSqlGrateTestContext);
}

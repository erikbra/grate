using grate.Configuration;
using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace CommandLine.Oracle;

// ReSharper disable once UnusedType.Global
public class Startup: CommandLine.Common.Startup
{
    protected override DatabaseType DatabaseType => DatabaseType.Oracle;
    protected override Type TestContainerType => typeof(OracleTestContainer);
    protected override Type TestContextType => typeof(OracleGrateTestContext);
}

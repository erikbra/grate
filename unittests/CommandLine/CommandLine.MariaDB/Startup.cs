using grate.Configuration;
using MariaDB.TestInfrastructure;

namespace CommandLine.MariaDB;

// ReSharper disable once UnusedType.Global
public class Startup: CommandLine.Common.Startup
{
    protected override DatabaseType DatabaseType => DatabaseType.MariaDB;
    protected override Type TestContainerType => typeof(MariaDbTestContainer);
    protected override Type TestContextType => typeof(MariaDbGrateTestContext);
}

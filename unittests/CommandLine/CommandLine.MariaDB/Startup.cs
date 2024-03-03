using grate.Configuration;
using MariaDB.TestInfrastructure;

namespace CommandLine.MariaDB;

// ReSharper disable once UnusedType.Global
public class Startup: CommandLine.Common.Startup<MariaDbTestContainerDatabase, MariaDBExternalDatabase, MariaDbGrateTestContext>
{
    protected override DatabaseType DatabaseType => DatabaseType.MariaDB;
}

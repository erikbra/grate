using grate.Configuration;
using MariaDB.TestInfrastructure;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Docker.MariaDB;

// ReSharper disable once UnusedType.Global
public class Startup: Docker.Common.Startup<MariaDbTestContainerDatabase, MariaDBExternalDatabase, MariaDbGrateTestContext>
{
    protected override DatabaseType DatabaseType => DatabaseType.MariaDB;
}

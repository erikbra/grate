using grate.Configuration;
using Oraclde.TestInfrastructure;
using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Docker.Oracle;

// ReSharper disable once UnusedType.Global
public class Startup: Common.Startup<OracleTestContainerDatabase, OracleExternalDatabase, OracleGrateTestContext>
{
    protected override DatabaseType DatabaseType => DatabaseType.Oracle;
}

using grate.DependencyInjection;
using grate.oracle.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oraclde.TestInfrastructure;
using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Oracle;

// ReSharper disable once UnusedType.Global
public class Startup: TestCommon.Startup<OracleTestContainerDatabase, OracleExternalDatabase, OracleGrateTestContext>
{
    protected override void ConfigureExtraServices(IServiceCollection services, HostBuilderContext context)
    {
        services
            .AddGrate()
            .UseOracle();
    }
}

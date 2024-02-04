using grate.Configuration;
using grate.DependencyInjection;
using grate.Infrastructure;
using grate.mariadb.DependencyInjection;
using grate.Migration;
using grate.oracle.DependencyInjection;
using grate.Oracle.Infrastructure;
using grate.Oracle.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Oracle;

// ReSharper disable once UnusedType.Global
public class Startup
{

    // ReSharper disable once UnusedMember.Global
    public void ConfigureServices(IServiceCollection services, HostBuilderContext context)
    {
        services.AddLogging(
            lb => lb
                .AddXUnit()
                .AddConsole()
                .SetMinimumLevel(TestConfig.GetLogLevel())
        );
    
        services
            .AddGrate()
            .UseOracle();
    
        services.TryAddSingleton<OracleTestContainer>();
        services.TryAddTransient<IGrateTestContext, OracleGrateTestContext>();
    }

}

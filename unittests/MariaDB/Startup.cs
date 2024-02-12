using grate.DependencyInjection;
using grate.mariadb.DependencyInjection;
using grate.MariaDb.Migration;
using MariaDB.TestInfrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestCommon.TestInfrastructure;

namespace MariaDB;

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
            .UseMariaDb();
        
        services.TryAddSingleton<MariaDbTestContainer>();
        services.TryAddTransient<IGrateTestContext, MariaDbGrateTestContext>();
        services.TryAddTransient<InspectableMariaDbDatabase>();
    }
}

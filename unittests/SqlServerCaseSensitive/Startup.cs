using grate.DependencyInjection;
using grate.sqlserver.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlServerCaseSensitive.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace SqlServerCaseSensitive;

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
            .UseSqlServer();

        services.TryAddSingleton<SqlServerTestContainer>();
        services.TryAddTransient<IGrateTestContext, SqlServerGrateTestContext>();
        services.TryAddTransient<InspectableSqlServerDatabase>();
    }

}

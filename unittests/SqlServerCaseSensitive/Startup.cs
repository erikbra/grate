using grate.DependencyInjection;
using grate.sqlserver.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlServerCaseSensitive.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace SqlServer;

// ReSharper disable once UnusedType.Global
public class Startup
{
    public void ConfigureHost(IHostBuilder hostBuilder) =>
        hostBuilder
            .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables() )
            .ConfigureAppConfiguration((context, builder) => { });

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

        var adminConnectionString = context.Configuration["GrateTest_AdminConnectionString"];
        if (adminConnectionString != null)
        {
            services.TryAddSingleton<ITestDatabase>(new SqlServerExternalDatabase(adminConnectionString));
        }
        else
        {
            services.TryAddSingleton<ITestDatabase, SqlServerTestContainerDatabase>();
        }
        
        services.TryAddTransient<InspectableSqlServerDatabase>();
    }

}

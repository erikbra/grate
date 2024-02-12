using grate.DependencyInjection;
using grate.postgresql.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace PostgreSQL;

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
            .UsePostgreSQL();
    
        services.TryAddSingleton<PostgreSqlTestContainer>();
        services.TryAddTransient<IGrateTestContext, PostgreSqlGrateTestContext>();
    }

}

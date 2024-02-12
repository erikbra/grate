using grate.Configuration;
using grate.DependencyInjection;
using grate.Infrastructure;
using grate.Migration;
using grate.sqlite.DependencyInjection;
using grate.Sqlite.Infrastructure;
using grate.Sqlite.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite;

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
            .UseSqlite();
    
        services.TryAddSingleton<SqliteTestContainer>();
        services.TryAddTransient<IGrateTestContext, SqliteGrateTestContext>();
    }
    
}

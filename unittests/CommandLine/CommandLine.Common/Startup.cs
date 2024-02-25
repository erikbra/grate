using CommandLine.Common.TestInfrastructure;
using grate.Configuration;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestCommon.TestInfrastructure;

namespace CommandLine.Common;

// ReSharper disable once UnusedType.Global
public abstract class Startup
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

        services.AddSingleton<IGrateMigrator>(new CommandLineGrateMigrator(DatabaseType));
        
        services.TryAddSingleton(TestContextType);
        services.TryAddSingleton(TestContainerType);
        
        services.TryAddTransient<IGrateTestContext>(provider => 
            new CommandLineGrateTestContext(
                provider.GetRequiredService<IGrateMigrator>(),
                (IGrateTestContext) provider.GetRequiredService(TestContextType)));
    }
    
    protected abstract DatabaseType DatabaseType { get; }
    protected abstract Type TestContainerType { get; }
    protected abstract Type TestContextType { get; }
    
}

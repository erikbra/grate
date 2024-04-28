using grate.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestCommon.TestInfrastructure;

namespace TestCommon;

// ReSharper disable once UnusedType.Global
public abstract class Startup
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

        
        ConfigureExtraServices(services, context);
        
        services.TryAddSingleton(TestContextType);
        RegisterTestDatabase(services, context.Configuration);
    }
    
    protected abstract Type TestContainerDatabaseType { get; }
    protected abstract Type ExternalTestDatabaseType { get; }
    protected abstract Type TestContextType { get; }

    protected void RegisterTestDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var testConfig = new GrateTestConfig();
        configuration.Bind("GrateTestConfig", testConfig);
        services.TryAddSingleton(testConfig);

        if (testConfig.AdminConnectionString != null)
        {
            services.TryAddSingleton(typeof(ITestDatabase), ExternalTestDatabaseType);
        }
        else
        {
            services.TryAddSingleton(typeof(ITestDatabase), TestContainerDatabaseType);
        }
    }
    
    protected virtual void ConfigureExtraServices(IServiceCollection services, HostBuilderContext context) { }
    
}

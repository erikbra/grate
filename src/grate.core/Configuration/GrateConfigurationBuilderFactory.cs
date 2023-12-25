using Microsoft.Extensions.DependencyInjection;

namespace grate.Configuration;
public class GrateConfigurationBuilderFactory
{
    public static GrateConfigurationBuilder Create(IServiceCollection serviceCollection, GrateConfiguration grateConfiguration)
    {
        return new GrateConfigurationBuilder(serviceCollection, grateConfiguration);
    }
    public static GrateConfigurationBuilder Create(IServiceCollection serviceCollection)
    {
        return new GrateConfigurationBuilder(serviceCollection, GrateConfiguration.Default with { NonInteractive = true });
    }
}

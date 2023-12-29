using Microsoft.Extensions.DependencyInjection;

namespace grate.Configuration;
public class GrateConfigurationBuilderFactory
{
    /// <summary>
    /// Create the grate configuration builder with existing service collection and grate configuration.
    /// </summary>
    /// <param name="serviceCollection">Service collection</param>
    /// <param name="grateConfiguration">GrateConfiguration</param>
    /// <returns>GrateConfigurationBuilder</returns>
    public static GrateConfigurationBuilder Create(IServiceCollection serviceCollection, GrateConfiguration grateConfiguration)
    {
        return new GrateConfigurationBuilder(serviceCollection, grateConfiguration);
    }

    /// <summary>
    /// Create the default grate configuration builder with existing service collection.
    /// </summary>
    /// <param name="serviceCollection">Service collection</param>
    /// <returns>GrateConfigurationBuilder</returns>
    public static GrateConfigurationBuilder Create(IServiceCollection serviceCollection)
    {
        return new GrateConfigurationBuilder(serviceCollection, GrateConfiguration.Default with { NonInteractive = true });
    }
}

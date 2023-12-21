using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;

namespace grate;

public static class DependencyInjectionRegistrationExtensions
{
    public static IServiceCollection AddGrate(this IServiceCollection collection, Action<GrateConfiguration>? configure = null)
    {
        AddGrateService(collection);
        var configurator = GrateConfiguration.Default;
        configurator.ServiceCollection = collection;
        configure?.Invoke(configurator);
        collection.AddSingleton(configurator);
        return collection;
    }
    private static IServiceCollection AddGrateService(IServiceCollection collection)
    {
        collection.AddTransient<IDbMigrator, DbMigrator>();
        collection.AddTransient<IHashGenerator, HashGenerator>();
        collection.AddTransient<IGrateMigrator, GrateMigrator>();
        return collection;
    }
}

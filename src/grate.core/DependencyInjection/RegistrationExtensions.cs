using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;

namespace grate;

public static class RegistrationExtensions
{
    public static IServiceCollection AddGrate(this IServiceCollection serviceCollection, GrateConfiguration presetGrateConfiguration, Action<GrateConfigurationBuilder>? builder = null)
    {
        var configurationBuilder = GrateConfigurationBuilderFactory.Create(serviceCollection, presetGrateConfiguration);
        builder?.Invoke(configurationBuilder);
        var grateConfiguration = configurationBuilder.Build();
        serviceCollection.AddSingleton(grateConfiguration);
        AddGrateService(serviceCollection);
        return serviceCollection;
    }
    public static IServiceCollection AddGrate(this IServiceCollection serviceCollection, Action<GrateConfigurationBuilder>? builder = null)
    {
        var configurationBuilder = GrateConfigurationBuilderFactory.Create(serviceCollection);
        builder?.Invoke(configurationBuilder);
        var grateConfiguration = configurationBuilder.Build();
        serviceCollection.AddSingleton(grateConfiguration);
        AddGrateService(serviceCollection);
        return serviceCollection;
    }
    private static IServiceCollection AddGrateService(IServiceCollection collection)
    {
        collection.AddTransient<IDbMigrator, DbMigrator>();
        collection.AddTransient<IHashGenerator, HashGenerator>();
        collection.AddTransient<IGrateMigrator, GrateMigrator>();
        collection.AddTransient(service =>
        {
            var database = service.GetService<IDatabase>()!;
            return new BatchSplitterReplacer(database.StatementSeparatorRegex, StatementSplitter.BatchTerminatorReplacementString);
        });
        collection.AddTransient(service =>
        {
            var database = service.GetService<IDatabase>()!;
            return new StatementSplitter(database.StatementSeparatorRegex);
        });
        return collection;
    }
}

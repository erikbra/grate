using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;

namespace grate;

public static class DependencyInjectionRegistrationExtensions
{
    public static IServiceCollection AddGrate(this IServiceCollection collection, GrateConfiguration grateConfiguration)
    {
        grateConfiguration.ServiceCollection = collection;
        collection.AddSingleton(grateConfiguration);
        AddGrateService(collection);
        return collection;
    }
    public static IServiceCollection AddGrate(this IServiceCollection collection, Action<GrateConfiguration>? configure = null)
    {
        var configurator = GrateConfiguration.Default;
        configurator.ServiceCollection = collection;
        configure?.Invoke(configurator);
        collection.AddSingleton(configurator);
        AddGrateService(collection);
        return collection;
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

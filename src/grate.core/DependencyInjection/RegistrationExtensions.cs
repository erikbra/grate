using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace grate.DependencyInjection;

public static class RegistrationExtensions
{
    public static IServiceCollection AddGrate(this IServiceCollection serviceCollection, GrateConfiguration presetGrateConfiguration, Action<GrateConfigurationBuilder>? builder = null)
    {
        var configurationBuilder = GrateConfigurationBuilder.Create(presetGrateConfiguration);
        builder?.Invoke(configurationBuilder);
        var grateConfiguration = configurationBuilder.Build();
        
        // Remove any existing grate configuration in the service collection, 
        // and overwrite with this one.
        serviceCollection.RemoveAll(grateConfiguration.GetType());
        serviceCollection.TryAddSingleton(grateConfiguration);
        
        return AddGrateService(serviceCollection);
    }
    
    public static IServiceCollection AddGrate(this IServiceCollection serviceCollection, Action<GrateConfigurationBuilder>? builder = null)
    {
        var configurationBuilder = GrateConfigurationBuilder.Create();
        builder?.Invoke(configurationBuilder);
        var grateConfiguration = configurationBuilder.Build();
        
        // Remove any existing grate configuration in the service collection, 
        // and overwrite with this one.
        serviceCollection.RemoveAll(grateConfiguration.GetType());
        serviceCollection.TryAddSingleton(grateConfiguration);
        
        return AddGrateService(serviceCollection);
    }
    
    public static IServiceCollection AddGrate<TDatabase>(
        this IServiceCollection serviceCollection, GrateConfiguration presetGrateConfiguration, Action<GrateConfigurationBuilder>? builder = null)
        where TDatabase : class, IDatabase
    {
        return serviceCollection
            .AddGrate(presetGrateConfiguration, builder)
            .UseDatabase<TDatabase>();
    }
    
    public static IServiceCollection AddGrate<TDatabase>(
        this IServiceCollection serviceCollection, Action<GrateConfigurationBuilder>? builder = null)
        where TDatabase : class, IDatabase
    {
        return serviceCollection
            .AddGrate(builder)
            .UseDatabase<TDatabase>();
    }

    public static IServiceCollection UseDatabase<TDatabase>(this IServiceCollection serviceCollection)
        where TDatabase : class, IDatabase
    {
        serviceCollection.TryAddTransient<IDatabase, TDatabase>();
        return serviceCollection;
    }
    
    public static IServiceCollection UseDatabase(this IServiceCollection serviceCollection, Type databaseType)
    {
        serviceCollection.TryAddTransient(typeof(IDatabase), databaseType);
        return serviceCollection;
    }
    
    
    private static IServiceCollection AddGrateService(IServiceCollection collection)
    {
        collection.TryAddTransient<IDbMigrator, DbMigrator>();
        collection.TryAddTransient<IHashGenerator, HashGenerator>();
        collection.TryAddTransient<IGrateMigrator, GrateMigrator>();
        // collection.TryAddTransient(service =>
        // {
        //     var database = service.GetRequiredService<IDatabase>();
        //     return new BatchSplitterReplacer(database.StatementSeparatorRegex, StatementSplitter.BatchTerminatorReplacementString);
        // });
        // collection.TryAddTransient(service =>
        // {
        //     var database = service.GetRequiredService<IDatabase>();
        //     return new StatementSplitter(database.StatementSeparatorRegex);
        // });
        
        return collection;
    }
}

using grate.DependencyInjection;
using grate.Migration;
using grate.Sqlite.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace grate.sqlite.DependencyInjection;

public static class RegistrationExtensions
{
    // ReSharper disable once InconsistentNaming
    public static void UseSqlite(this IServiceCollection services)
    {
        services.TryAddTransient<IDatabase, SqliteDatabase>();
    }
    
    // ReSharper disable once InconsistentNaming
    public static void AddGrateWithSqlite(this IServiceCollection services)
    {
        services
            .AddGrate()
            .UseSqlite();
    }
}

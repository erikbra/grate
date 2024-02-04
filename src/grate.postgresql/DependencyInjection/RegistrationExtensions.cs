using grate.DependencyInjection;
using grate.Migration;
using grate.PostgreSql.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace grate.postgresql.DependencyInjection;

public static class RegistrationExtensions
{
    // ReSharper disable once InconsistentNaming
    public static void UsePostgreSQL(this IServiceCollection services)
    {
        services.TryAddTransient<IDatabase, PostgreSqlDatabase>();
    }
    
    // ReSharper disable once InconsistentNaming
    public static void AddGrateWithPostgreSQL(this IServiceCollection services)
    {
        services
            .AddGrate()
            .UsePostgreSQL();
    }
}

using grate.DependencyInjection;
using grate.Migration;
using grate.SqlServer.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace grate.sqlserver.DependencyInjection;

public static class RegistrationExtensions
{
    // ReSharper disable once InconsistentNaming
    public static void UseSqlServer(this IServiceCollection services)
    {
        services.TryAddTransient<IDatabase, SqlServerDatabase>();
    }
    
    // ReSharper disable once InconsistentNaming
    public static void AddGrateUseSqlServer(this IServiceCollection services)
    {
        services
            .AddGrate()
            .UseSqlServer();
    }
}

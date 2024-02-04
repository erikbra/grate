using grate.DependencyInjection;
using grate.MariaDb.Migration;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace grate.mariadb.DependencyInjection;

public static class RegistrationExtensions
{
    public static void UseMariaDb(this IServiceCollection services)
    {
        services.TryAddTransient<IDatabase, MariaDbDatabase>();
    }
    
    public static void AddGrateWithMariaDb(this IServiceCollection services)
    {
        services
            .AddGrate()
            .UseMariaDb();
    }
}

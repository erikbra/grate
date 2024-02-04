using grate.DependencyInjection;
using grate.Migration;
using grate.Oracle.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace grate.oracle.DependencyInjection;

public static class RegistrationExtensions
{
    public static void UseOracle(this IServiceCollection services)
    {
        services.TryAddTransient<IDatabase, OracleDatabase>();
    }
    
    public static void AddGrateWithOracle(this IServiceCollection services)
    {
        services
            .AddGrate()
            .UseOracle();
    }
}

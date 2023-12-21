using grate.Configuration;
using grate.Migration;
using grate.Oracle.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace grate.Oracle;

public static class DependencyInjectionRegistrationExtensions
{
    public static void UseOracle(this GrateConfiguration configuration)
    {
        configuration.DatabaseType = DatabaseType.Name;
        configuration.ServiceCollection!.AddKeyedTransient<IDatabase, OracleDatabase>(configuration.DatabaseType);
    }
}

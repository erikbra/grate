using grate.Configuration;
using grate.Migration;
using grate.PostgreSql.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace grate.PostgreSql;

public static class DependencyInjectionRegistrationExtensions
{
    public static void UsePostgreSql(this GrateConfiguration configuration)
    {
        configuration.DatabaseType = DatabaseType.Name;
        configuration.ServiceCollection!.AddKeyedTransient<IDatabase, PostgreSqlDatabase>(configuration.DatabaseType);
    }
}

using grate.Configuration;
using grate.Migration;
using grate.PostgreSql.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using grate.PostgreSql.Migration;
using grate.Infrastructure;
namespace grate.PostgreSql;

public static class DependencyInjectionRegistrationExtensions
{
    public static void UsePostgreSql(this GrateConfiguration configuration)
    {
        configuration.DatabaseType = PostgreSqlDatabase.Type;
        configuration.ServiceCollection!.AddTransient<IDatabase, PostgreSqlDatabase>();
        configuration.ServiceCollection!.AddSingleton<ISyntax, PostgreSqlSyntax>();
    }
}

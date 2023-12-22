using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.PostgreSql.Infrastructure;
using grate.PostgreSql.Migration;
using Microsoft.Extensions.DependencyInjection;
namespace grate.PostgreSql;

public static class DependencyInjectionRegistrationExtensions
{
    public static void UsePostgreSql(this GrateConfiguration configuration)
    {
        configuration.DatabaseType = PostgreSqlDatabase.Type;
        configuration.ServiceCollection!.AddTransient<IDatabase, PostgreSqlDatabase>();
        configuration.ServiceCollection!.AddTransient<ISyntax, PostgreSqlSyntax>();
    }
}

using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.PostgreSql.Infrastructure;
using grate.PostgreSql.Migration;
using Microsoft.Extensions.DependencyInjection;
namespace grate.PostgreSql;

public static class RegistrationExtensions
{
    public static void UsePostgreSql(this GrateConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.WithDatabaseType(PostgreSqlDatabase.Type);
        configurationBuilder.ServiceCollection.AddTransient<IDatabase, PostgreSqlDatabase>();
        configurationBuilder.ServiceCollection.AddTransient<ISyntax, PostgreSqlSyntax>();
    }
}

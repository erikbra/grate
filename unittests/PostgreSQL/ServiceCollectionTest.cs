using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.PostgreSql;
using grate.PostgreSql.Infrastructure;
using grate.PostgreSql.Migration;
using Microsoft.Extensions.DependencyInjection;
namespace PostgreSQL.DependencyInjection;
public class ServiceCollectionTest : TestCommon.DependencyInjection.GrateServiceCollectionTest
{
    protected override void ConfigureService(GrateConfiguration grateConfiguration)
    {
        grateConfiguration.UsePostgreSql();
    }

    protected override void ValidateDatabaseService(ServiceCollection serviceCollection)
    {
        ValidateService(serviceCollection, typeof(IDatabase), ServiceLifetime.Transient, typeof(PostgreSqlDatabase));
        ValidateService(serviceCollection, typeof(ISyntax), ServiceLifetime.Transient, typeof(PostgreSqlSyntax));
    }
}

using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.PostgreSql;
using grate.PostgreSql.Infrastructure;
using grate.PostgreSql.Migration;
using Microsoft.Extensions.DependencyInjection;
using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;
namespace PostgreSQL.DependencyInjection;

[Collection(nameof(PostgreSqlTestContainer))]
public class ServiceCollectionTest(PostgreSqlTestContainer postgreSqlTestContainer)
    : TestCommon.DependencyInjection.GrateServiceCollectionTest
{
    protected override void ConfigureService(GrateConfigurationBuilder grateConfigurationBuilder)
    {
        var connectionString = $"Host={postgreSqlTestContainer.TestContainer!.Hostname};Port={postgreSqlTestContainer.TestContainer!.GetMappedPublicPort(postgreSqlTestContainer.Port)};Database={TestConfig.RandomDatabase()};Username=postgres;Password={postgreSqlTestContainer.AdminPassword};Include Error Detail=true;Pooling=false";
        grateConfigurationBuilder.WithConnectionString(connectionString);
        grateConfigurationBuilder.UsePostgreSql();
        grateConfigurationBuilder.ServiceCollection.AddSingleton<IDatabaseConnectionFactory, PostgreSqlConnectionFactory>();
    }
    protected override void ValidateDatabaseService(IServiceCollection serviceCollection)
    {
        ValidateService(serviceCollection, typeof(IDatabase), ServiceLifetime.Transient, typeof(PostgreSqlDatabase));
        ValidateService(serviceCollection, typeof(ISyntax), ServiceLifetime.Transient, typeof(PostgreSqlSyntax));
    }
}

using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.PostgreSql;
using grate.PostgreSql.Infrastructure;
using grate.PostgreSql.Migration;
using Microsoft.Extensions.DependencyInjection;
using PostgreSQL.TestInfrastructure;
using TestCommon.Generic.Running_MigrationScripts;
using TestCommon.TestInfrastructure;
using static grate.Configuration.KnownFolderKeys;
namespace PostgreSQL.DependencyInjection;

[Collection(nameof(PostgreSqlTestContainer))]
public class ServiceCollectionTest : TestCommon.DependencyInjection.GrateServiceCollectionTest
{
    private readonly PostgreSqlTestContainer _postgreSqlTestContainer;
    public ServiceCollectionTest(PostgreSqlTestContainer postgreSqlTestContainer)
    {
        _postgreSqlTestContainer = postgreSqlTestContainer;
    }
    protected override void ConfigureService(GrateConfigurationBuilder grateConfigurationBuilder)
    {
        var connectionString = $"Host={_postgreSqlTestContainer.TestContainer!.Hostname};Port={_postgreSqlTestContainer.TestContainer!.GetMappedPublicPort(_postgreSqlTestContainer.Port)};Database={TestConfig.RandomDatabase()};Username=postgres;Password={_postgreSqlTestContainer.AdminPassword};Include Error Detail=true;Pooling=false";
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

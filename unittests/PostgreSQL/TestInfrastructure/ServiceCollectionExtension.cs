using grate.Configuration;
using grate.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using TestCommon.TestInfrastructure;
namespace PostgreSQL.TestInfrastructure;
public static class ServiceCollectionExtension
{
    public static void ConfigureService(this GrateConfigurationBuilder grateConfigurationBuilder, PostgreSqlTestContainer _postgreSqlTestContainer)
    {
        var connectionString = $"Host={_postgreSqlTestContainer.TestContainer!.Hostname};Port={_postgreSqlTestContainer.TestContainer!.GetMappedPublicPort(_postgreSqlTestContainer.Port)};Database={TestConfig.RandomDatabase()};Username=postgres;Password={_postgreSqlTestContainer.AdminPassword};Include Error Detail=true;Pooling=false";
        grateConfigurationBuilder.WithConnectionString(connectionString);
        grateConfigurationBuilder.UsePostgreSql();
        grateConfigurationBuilder.ServiceCollection.AddSingleton<IDatabaseConnectionFactory, PostgreSqlConnectionFactory>();
    }
}

using grate.Configuration;
using grate.Infrastructure;
using grate.MariaDb;
using grate.MariaDb.Infrastructure;
using grate.MariaDb.Migration;
using grate.Migration;
using MariaDB.TestInfrastructure;
using Microsoft.Extensions.DependencyInjection;
using TestCommon.TestInfrastructure;
namespace MariaDB.DependencyInjection;

[Collection(nameof(MariaDbTestContainer))]
public class ServiceCollectionTest : TestCommon.DependencyInjection.GrateServiceCollectionTest
{
    private readonly MariaDbTestContainer _mariaDbTestContainer;

    public ServiceCollectionTest(MariaDbTestContainer mariaDbTestContainer)
    {
        _mariaDbTestContainer = mariaDbTestContainer; ;
    }
    protected override void ConfigureService(GrateConfigurationBuilder grateConfigurationBuilder)
    {
        var connectionString = $"Server={_mariaDbTestContainer.TestContainer!.Hostname};Port={_mariaDbTestContainer.TestContainer!.GetMappedPublicPort(_mariaDbTestContainer.Port)};Database={TestConfig.RandomDatabase()};Uid=root;Pwd={_mariaDbTestContainer.AdminPassword}";
        grateConfigurationBuilder.WithConnectionString(connectionString);
        grateConfigurationBuilder.UseMariaDb();
        grateConfigurationBuilder.ServiceCollection.AddSingleton<IDatabaseConnectionFactory, MariaDbConnectionFactory>();
    }
    protected override void ValidateDatabaseService(IServiceCollection serviceCollection)
    {
        ValidateService(serviceCollection, typeof(IDatabase), ServiceLifetime.Transient, typeof(MariaDbDatabase));
        ValidateService(serviceCollection, typeof(ISyntax), ServiceLifetime.Transient, typeof(MariaDbSyntax));
    }
}

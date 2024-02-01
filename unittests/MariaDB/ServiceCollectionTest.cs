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
// ReSharper disable once UnusedType.Global
public class ServiceCollectionTest(MariaDbTestContainer mariaDbTestContainer)
    : TestCommon.DependencyInjection.GrateServiceCollectionTest
{
    protected override void ConfigureService(GrateConfigurationBuilder grateConfigurationBuilder)
    {
        var connectionString = $"Server={mariaDbTestContainer.TestContainer!.Hostname};Port={mariaDbTestContainer.TestContainer!.GetMappedPublicPort(mariaDbTestContainer.Port)};Database={TestConfig.RandomDatabase()};Uid=root;Pwd={mariaDbTestContainer.AdminPassword}";
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

using grate.Configuration;
using grate.MariaDb;
using Microsoft.Extensions.DependencyInjection;
using TestCommon.TestInfrastructure;
namespace MariaDB.TestInfrastructure;
public static class ServiceCollectionExtension
{
    public static void ConfigureService(this GrateConfigurationBuilder grateConfigurationBuilder, MariaDbTestContainer mariaDbTestContainer)
    {
        var connectionString = $"Server={mariaDbTestContainer.TestContainer!.Hostname};Port={mariaDbTestContainer.TestContainer!.GetMappedPublicPort(mariaDbTestContainer.Port)};Database={TestConfig.RandomDatabase()};Uid=root;Pwd={mariaDbTestContainer.AdminPassword}";
        grateConfigurationBuilder.WithConnectionString(connectionString);
        grateConfigurationBuilder.UseMariaDb();
        grateConfigurationBuilder.ServiceCollection.AddSingleton<IDatabaseConnectionFactory, MariaDbConnectionFactory>();
    }
}

using grate.Configuration;
using grate.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using TestCommon.TestInfrastructure;
namespace Sqlite.TestInfrastructure;
public static class ServiceCollectionExtension
{
    public static void ConfigureService(this GrateConfigurationBuilder grateConfigurationBuilder)
    {
        var connectionString = $"Data Source={TestConfig.RandomDatabase()}.db";
        grateConfigurationBuilder.WithConnectionString(connectionString);
        grateConfigurationBuilder.UseSqlite();
        grateConfigurationBuilder.ServiceCollection.AddSingleton<IDatabaseConnectionFactory, SqliteConnectionFactory>();
    }
}

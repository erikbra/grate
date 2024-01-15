using grate.Configuration;
using grate.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using TestCommon.TestInfrastructure;
namespace SqlServerCaseSensitive.TestInfrastructure;
public static class ServiceCollectionExtension
{
    public static void ConfigureService(this GrateConfigurationBuilder grateConfigurationBuilder, SqlServerTestContainer sqlServerTestContainer)
    {
        var connectionString = $"Data Source={sqlServerTestContainer.TestContainer!.Hostname},{sqlServerTestContainer.TestContainer!.GetMappedPublicPort(sqlServerTestContainer.Port)};Initial Catalog={TestConfig.RandomDatabase()};User Id=sa;Password={sqlServerTestContainer.AdminPassword};Encrypt=false;Pooling=false";
        grateConfigurationBuilder.WithConnectionString(connectionString);
        grateConfigurationBuilder.UseSqlServer();
        grateConfigurationBuilder.ServiceCollection.AddSingleton<IDatabaseConnectionFactory, SqlServerConnectionFactory>();
    }
}

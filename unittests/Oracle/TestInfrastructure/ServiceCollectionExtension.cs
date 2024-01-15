using grate.Configuration;
using grate.Oracle;
using Microsoft.Extensions.DependencyInjection;
using TestCommon.TestInfrastructure;
namespace Oracle.TestInfrastructure;
public static class ServiceCollectionExtension
{
    public static void ConfigureService(this GrateConfigurationBuilder grateConfigurationBuilder, OracleTestContainer oracleTestContainer)
    {
        var connectionString = $@"Data Source={oracleTestContainer.TestContainer!.Hostname}:{oracleTestContainer.TestContainer!.GetMappedPublicPort(oracleTestContainer.Port)}/XEPDB1;User ID=oracle;Password={oracleTestContainer.AdminPassword};Pooling=False";
        var adminConnectionString = $@"Data Source={oracleTestContainer.TestContainer!.Hostname}:{oracleTestContainer.TestContainer!.GetMappedPublicPort(oracleTestContainer.Port)}/XEPDB1;User ID=system;Password={oracleTestContainer.AdminPassword};Pooling=False";

        grateConfigurationBuilder.WithConnectionString(connectionString);
        grateConfigurationBuilder.WithAdminConnectionString(adminConnectionString);
        grateConfigurationBuilder.UseOracle();
        grateConfigurationBuilder.ServiceCollection.AddSingleton<IDatabaseConnectionFactory, OracleConnectionFactory>();
    }
}

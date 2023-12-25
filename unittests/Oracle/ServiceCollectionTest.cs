using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.Oracle;
using grate.Oracle.Infrastructure;
using grate.Oracle.Migration;
using Microsoft.Extensions.DependencyInjection;
using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Oracle.DependencyInjection;

[Collection(nameof(OracleTestContainer))]
public class ServiceCollectionTest : TestCommon.DependencyInjection.GrateServiceCollectionTest
{
    private readonly OracleTestContainer _oracleTestContainer;

    public ServiceCollectionTest(OracleTestContainer oracleTestContainer)
    {
        _oracleTestContainer = oracleTestContainer;
    }
    protected override void ConfigureService(GrateConfigurationBuilder grateConfigurationBuilder)
    {
        var connectionString = $@"Data Source={_oracleTestContainer.TestContainer!.Hostname}:{_oracleTestContainer.TestContainer!.GetMappedPublicPort(_oracleTestContainer.Port)}/XEPDB1;User ID=oracle;Password={_oracleTestContainer.AdminPassword};Pooling=False";
        var adminConnectionString = $@"Data Source={_oracleTestContainer.TestContainer!.Hostname}:{_oracleTestContainer.TestContainer!.GetMappedPublicPort(_oracleTestContainer.Port)}/XEPDB1;User ID=system;Password={_oracleTestContainer.AdminPassword};Pooling=False";

        grateConfigurationBuilder.WithConnectionString(connectionString);
        grateConfigurationBuilder.WithAdminConnectionString(adminConnectionString);
        grateConfigurationBuilder.UseOracle();
        grateConfigurationBuilder.ServiceCollection.AddSingleton<IDatabaseConnectionFactory, OracleConnectionFactory>();
    }


    protected override void ValidateDatabaseService(IServiceCollection serviceCollection)
    {
        ValidateService(serviceCollection, typeof(IDatabase), ServiceLifetime.Transient, typeof(OracleDatabase));
        ValidateService(serviceCollection, typeof(ISyntax), ServiceLifetime.Transient, typeof(OracleSyntax));
    }
}

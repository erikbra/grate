using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.SqlServer;
using grate.SqlServer.Infrastructure;
using grate.SqlServer.Migration;
using Microsoft.Extensions.DependencyInjection;
using SqlServer.TestInfrastructure;
using TestCommon.Generic.Running_MigrationScripts;
using TestCommon.TestInfrastructure;
using static grate.Configuration.KnownFolderKeys;
namespace SqlServer.DependencyInjection;

[Collection(nameof(SqlServerTestContainer))]
public class ServiceCollectionTest : TestCommon.DependencyInjection.GrateServiceCollectionTest
{
    private readonly SqlServerTestContainer _sqlServerTestContainer;

    public ServiceCollectionTest(SqlServerTestContainer sqlServerTestContainer)
    {
        _sqlServerTestContainer = sqlServerTestContainer;
    }
    protected override void ConfigureService(GrateConfigurationBuilder grateConfigurationBuilder)
    {
        var connectionString = $"Data Source={_sqlServerTestContainer.TestContainer!.Hostname},{_sqlServerTestContainer.TestContainer!.GetMappedPublicPort(_sqlServerTestContainer.Port)};Initial Catalog={TestConfig.RandomDatabase()};User Id=sa;Password={_sqlServerTestContainer.AdminPassword};Encrypt=false;Pooling=false";
        grateConfigurationBuilder.WithConnectionString(connectionString);
        grateConfigurationBuilder.UseSqlServer();
        grateConfigurationBuilder.ServiceCollection.AddSingleton<IDatabaseConnectionFactory, SqlServerConnectionFactory>();
    }

    protected override void ValidateDatabaseService(IServiceCollection serviceCollection)
    {
        ValidateService(serviceCollection, typeof(IDatabase), ServiceLifetime.Transient, typeof(SqlServerDatabase));
        ValidateService(serviceCollection, typeof(ISyntax), ServiceLifetime.Transient, typeof(SqlServerSyntax));
    }
}

using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.Sqlite;
using grate.Sqlite.Infrastructure;
using grate.Sqlite.Migration;
using Microsoft.Extensions.DependencyInjection;
using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;
namespace Sqlite.DependencyInjection;
public class ServiceCollectionTest : TestCommon.DependencyInjection.GrateServiceCollectionTest
{
    protected override void ConfigureService(GrateConfigurationBuilder grateConfigurationBuilder)
    {
        var connectionString = $"Data Source={TestConfig.RandomDatabase()}.db";
        grateConfigurationBuilder.WithConnectionString(connectionString);
        grateConfigurationBuilder.UseSqlite();
        grateConfigurationBuilder.ServiceCollection.AddSingleton<IDatabaseConnectionFactory, SqliteConnectionFactory>();
    }

    protected override void ValidateDatabaseService(IServiceCollection serviceCollection)
    {
        ValidateService(serviceCollection, typeof(IDatabase), ServiceLifetime.Transient, typeof(SqliteDatabase));
        ValidateService(serviceCollection, typeof(ISyntax), ServiceLifetime.Transient, typeof(SqliteSyntax));
    }
}

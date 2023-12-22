using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.Sqlite;
using grate.Sqlite.Infrastructure;
using grate.Sqlite.Migration;
using Microsoft.Extensions.DependencyInjection;
namespace Sqlite.DependencyInjection;
public class ServiceCollectionTest : TestCommon.DependencyInjection.GrateServiceCollectionTest
{
    protected override void ConfigureService(GrateConfiguration grateConfiguration)
    {
        grateConfiguration.UseSqlite();
    }

    protected override void ValidateDatabaseService(ServiceCollection serviceCollection)
    {
        ValidateService(serviceCollection, typeof(IDatabase), ServiceLifetime.Transient, typeof(SqliteDatabase));
        ValidateService(serviceCollection, typeof(ISyntax), ServiceLifetime.Transient, typeof(SqliteSyntax));
    }
}

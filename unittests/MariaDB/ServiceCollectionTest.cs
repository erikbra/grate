using grate.Configuration;
using grate.Infrastructure;
using grate.MariaDb;
using grate.MariaDb.Infrastructure;
using grate.MariaDb.Migration;
using grate.Migration;
using grate.Sqlite;
using Microsoft.Extensions.DependencyInjection;
namespace MariaDB.DependencyInjection;
public class ServiceCollectionTest : TestCommon.DependencyInjection.GrateServiceCollectionTest
{
    protected override void ConfigureService(GrateConfiguration grateConfiguration)
    {
        grateConfiguration.UseMariaDb();
    }

    protected override void ValidateDatabaseService(ServiceCollection serviceCollection)
    {
        ValidateService(serviceCollection, typeof(IDatabase), ServiceLifetime.Transient, typeof(MariaDbDatabase));
        ValidateService(serviceCollection, typeof(ISyntax), ServiceLifetime.Transient, typeof(MariaDbSyntax));
    }
}

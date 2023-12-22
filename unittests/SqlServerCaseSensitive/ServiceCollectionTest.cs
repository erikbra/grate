using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.SqlServer;
using grate.SqlServer.Infrastructure;
using grate.SqlServer.Migration;
using Microsoft.Extensions.DependencyInjection;
namespace SqlServerCaseSensitiveCaseSensitive.DependencyInjection;
public class ServiceCollectionTest : TestCommon.DependencyInjection.GrateServiceCollectionTest
{
    protected override void ConfigureService(GrateConfiguration grateConfiguration)
    {
        grateConfiguration.UseSqlServer();
    }

    protected override void ValidateDatabaseService(ServiceCollection serviceCollection)
    {
        ValidateService(serviceCollection, typeof(IDatabase), ServiceLifetime.Transient, typeof(SqlServerDatabase));
        ValidateService(serviceCollection, typeof(ISyntax), ServiceLifetime.Transient, typeof(SqlServerSyntax));
    }
}

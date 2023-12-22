using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.Oracle;
using grate.Oracle.Infrastructure;
using grate.Oracle.Migration;
using Microsoft.Extensions.DependencyInjection;
namespace Oracle.DependencyInjection;
public class ServiceCollectionTest : TestCommon.DependencyInjection.GrateServiceCollectionTest
{
    protected override void ConfigureService(GrateConfiguration grateConfiguration)
    {
        grateConfiguration.UseOracle();
    }

    protected override void ValidateDatabaseService(ServiceCollection serviceCollection)
    {
        ValidateService(serviceCollection, typeof(IDatabase), ServiceLifetime.Transient, typeof(OracleDatabase));
        ValidateService(serviceCollection, typeof(ISyntax), ServiceLifetime.Transient, typeof(OracleSyntax));
    }
}

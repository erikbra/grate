using grate.DependencyInjection;
using grate.postgresql.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace PostgreSQL;

// ReSharper disable once UnusedType.Global
public class Startup: TestCommon.Startup<PostgreSqlTestContainerDatabase, PostgreSqlExternalDatabase, PostgreSqlGrateTestContext>
{
    protected override void ConfigureExtraServices(IServiceCollection services, HostBuilderContext context)
    {
        services
            .AddGrate()
            .UsePostgreSQL();
    }
}

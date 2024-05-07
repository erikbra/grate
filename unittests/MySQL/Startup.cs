using grate.DependencyInjection;
using grate.mariadb.DependencyInjection;
using MySQL.TestInfrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace MySQL;

// ReSharper disable once UnusedType.Global
public class Startup: TestCommon.Startup<MySqlTestContainerDatabase, MySQLExternalDatabase, MySqlGrateTestContext>
{
    protected override void ConfigureExtraServices(IServiceCollection services, HostBuilderContext context)
    {
        services
            .AddGrate()
            .UseMariaDb();
        services.TryAddTransient<InspectableMySqlDatabase>();
    }
}

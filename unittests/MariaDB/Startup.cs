using grate.Configuration;
using grate.DependencyInjection;
using grate.mariadb.DependencyInjection;
using MariaDB.TestInfrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace MariaDB;

// ReSharper disable once UnusedType.Global
public class Startup: TestCommon.Startup<MariaDbTestContainerDatabase, MariaDBExternalDatabase, MariaDbGrateTestContext>
{
    protected override void ConfigureExtraServices(IServiceCollection services, HostBuilderContext context)
    {
        services
            .AddGrate()
            .UseMariaDb();
        services.TryAddTransient<InspectableMariaDbDatabase>();
    }
}

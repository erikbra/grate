using grate.DependencyInjection;
using grate.sqlserver.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using SqlServer.TestInfrastructure;

namespace SqlServer;

// ReSharper disable once UnusedType.Global
public class Startup: TestCommon.Startup<SqlServerTestContainerDatabase, SqlServerExternalDatabase, SqlServerGrateTestContext>
{
    protected override void ConfigureExtraServices(IServiceCollection services, HostBuilderContext context)
    {
        services
            .AddGrate()
            .UseSqlServer();

        services.TryAddTransient<InspectableSqlServerDatabase>();
    }
}

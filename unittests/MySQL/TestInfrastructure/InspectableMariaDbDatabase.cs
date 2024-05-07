using System.Data.Common;
using grate.MariaDb.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MySQL.TestInfrastructure;

public record InspectableMySqlDatabase : MariaDbDatabase
{
    public InspectableMySqlDatabase(IServiceProvider serviceProvider) 
        : base(serviceProvider.GetRequiredService<ILogger<InspectableMySqlDatabase>>())
    {
    }

    public DbConnection GetConnection() => base.Connection;
}

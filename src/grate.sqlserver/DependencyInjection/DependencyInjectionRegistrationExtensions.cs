using grate.Configuration;
using grate.Migration;
using grate.SqlServer.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace grate.SqlServer;

public static class DependencyInjectionRegistrationExtensions
{
    public static void UseSqlServer(this GrateConfiguration configuration)
    {
        configuration.DatabaseType = DatabaseType.Name;
        configuration.ServiceCollection!.AddKeyedTransient<IDatabase, SqlServerDatabase>(configuration.DatabaseType);
    }
}

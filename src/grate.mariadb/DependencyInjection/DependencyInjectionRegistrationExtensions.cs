using grate.Configuration;
using grate.MariaDb.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;

namespace grate.MariaDb;

public static class DependencyInjectionRegistrationExtensions
{
    public static void UseMariaDb(this GrateConfiguration configuration)
    {
        configuration.DatabaseType = DatabaseType.Name;
        configuration.ServiceCollection!.AddKeyedTransient<IDatabase, MariaDbDatabase>(configuration.DatabaseType);
    }
}

using grate.Configuration;
using grate.Migration;
using grate.Sqlite.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace grate.Sqlite;

public static class DependencyInjectionRegistrationExtensions
{
    public static void UseSqlite(this GrateConfiguration configuration)
    {
        configuration.DatabaseType = DatabaseType.Name;
        configuration.ServiceCollection!.AddKeyedTransient<IDatabase, SqliteDatabase>(configuration.DatabaseType);
    }
}

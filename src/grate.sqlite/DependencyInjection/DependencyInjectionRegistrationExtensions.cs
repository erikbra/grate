using grate.Configuration;
using grate.Migration;
using grate.Sqlite.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using grate.Sqlite.Migration;
using grate.Infrastructure;
namespace grate.Sqlite;

public static class DependencyInjectionRegistrationExtensions
{
    public static void UseSqlite(this GrateConfiguration configuration)
    {
        configuration.DatabaseType = SqliteDatabase.Type;
        if (configuration.ServiceCollection == null)
        {
            throw new ArgumentNullException(nameof(configuration), "ServiceCollection is null, please use AddGrate to initilize GrateConfiguration");
        }
        configuration.ServiceCollection.AddKeyedTransient<IDatabase, SqliteDatabase>(configuration.DatabaseType);
        configuration.ServiceCollection.AddSingleton<ISyntax, SqliteSyntax>();
    }
}

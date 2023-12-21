using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.Sqlite.Infrastructure;
using grate.Sqlite.Migration;
using Microsoft.Extensions.DependencyInjection;
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
        configuration.ServiceCollection.AddTransient<IDatabase, SqliteDatabase>();
        configuration.ServiceCollection.AddTransient<ISyntax, SqliteSyntax>();
    }
}

using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.Sqlite.Infrastructure;
using grate.Sqlite.Migration;
using Microsoft.Extensions.DependencyInjection;
namespace grate.Sqlite;

public static class RegistrationExtensions
{
    public static void UseSqlite(this GrateConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.WithDatabaseType(SqliteDatabase.Type);
        configurationBuilder.ServiceCollection.AddTransient<IDatabase, SqliteDatabase>();
        configurationBuilder.ServiceCollection.AddTransient<ISyntax, SqliteSyntax>();
    }
}

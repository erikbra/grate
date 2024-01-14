using grate.Configuration;
using grate.Infrastructure;
using grate.MariaDb.Infrastructure;
using grate.MariaDb.Migration;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
namespace grate.MariaDb;

public static class RegistrationExtensions
{
    public static void UseMariaDb(this GrateConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.WithDatabaseType(MariaDbDatabase.Type);
        configurationBuilder.ServiceCollection.AddTransient<IDatabase, MariaDbDatabase>();
        configurationBuilder.ServiceCollection.AddTransient<ISyntax, MariaDbSyntax>();
    }
}

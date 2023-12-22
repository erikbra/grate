using grate.Configuration;
using grate.Infrastructure;
using grate.MariaDb.Infrastructure;
using grate.MariaDb.Migration;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
namespace grate.MariaDb;

public static class DependencyInjectionRegistrationExtensions
{
    public static void UseMariaDb(this GrateConfiguration configuration)
    {
        configuration.DatabaseType = MariaDbDatabase.Type;
        configuration.ServiceCollection!.AddTransient<IDatabase, MariaDbDatabase>();
        configuration.ServiceCollection!.AddTransient<ISyntax, MariaDbSyntax>();
    }
}

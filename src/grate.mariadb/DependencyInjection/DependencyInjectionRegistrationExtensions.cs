using grate.Configuration;
using grate.MariaDb.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
using grate.MariaDb.Migration;
using grate.Infrastructure;
namespace grate.MariaDb;

public static class DependencyInjectionRegistrationExtensions
{
    public static void UseMariaDb(this GrateConfiguration configuration)
    {
        configuration.DatabaseType = MariaDbDatabase.Type;
        configuration.ServiceCollection!.AddTransient<IDatabase, MariaDbDatabase>();
        configuration.ServiceCollection!.AddSingleton<ISyntax, MariaDbSyntax>();
    }
}

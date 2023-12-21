using grate.Configuration;
using grate.Migration;
using grate.SqlServer.Infrastructure;
using grate.SqlServer.Migration;
using Microsoft.Extensions.DependencyInjection;
using grate.Infrastructure;
namespace grate.SqlServer;

public static class DependencyInjectionRegistrationExtensions
{
    public static void UseSqlServer(this GrateConfiguration configuration)
    {
        configuration.DatabaseType = SqlServerDatabase.Type;
        configuration.ServiceCollection!.AddTransient<IDatabase, SqlServerDatabase>();
        configuration.ServiceCollection!.AddSingleton<ISyntax, SqlServerSyntax>();
    }
}

using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.SqlServer.Infrastructure;
using grate.SqlServer.Migration;
using Microsoft.Extensions.DependencyInjection;
namespace grate.SqlServer;

public static class RegistrationExtensions
{
    public static void UseSqlServer(this GrateConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.WithDatabaseType(SqlServerDatabase.Type);
        configurationBuilder.ServiceCollection.AddTransient<IDatabase, SqlServerDatabase>();
        configurationBuilder.ServiceCollection.AddTransient<ISyntax, SqlServerSyntax>();
    }
}

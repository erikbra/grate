using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.Oracle.Infrastructure;
using grate.Oracle.Migration;
using Microsoft.Extensions.DependencyInjection;

namespace grate.Oracle;

public static class RegistrationExtensions
{
    public static void UseOracle(this GrateConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.WithDatabaseType(OracleDatabase.Type);
        configurationBuilder.ServiceCollection.AddTransient<IDatabase, OracleDatabase>();
        configurationBuilder.ServiceCollection.AddTransient<ISyntax, OracleSyntax>();
    }
}

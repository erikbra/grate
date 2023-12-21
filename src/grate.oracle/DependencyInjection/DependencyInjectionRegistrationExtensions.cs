using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.Oracle.Infrastructure;
using grate.Oracle.Migration;
using Microsoft.Extensions.DependencyInjection;

namespace grate.Oracle;

public static class DependencyInjectionRegistrationExtensions
{
    public static void UseOracle(this GrateConfiguration configuration)
    {
        configuration.DatabaseType = OracleDatabase.Type;
        configuration.ServiceCollection!.AddTransient<IDatabase, OracleDatabase>();
        configuration.ServiceCollection!.AddSingleton<ISyntax, OracleSyntax>();
    }
}

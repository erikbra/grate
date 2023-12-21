using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestCommon.TestInfrastructure;
public class SimpleService
{
    public IServiceProvider ServiceProvider { get; }
    public SimpleService()
    {
        ServiceProvider = new ServiceCollection()
            .AddLogging(opt =>
            {
                opt.AddConsole();
                opt.SetMinimumLevel(TestConfig.GetLogLevel());
            })
            .AddKeyedSingleton<IDatabase, SqlServerDatabase>(grate.SqlServer.Infrastructure.DatabaseType.Name)
            .AddKeyedSingleton<IDatabase, PostgreSqlDatabase>(grate.PostgreSql.Infrastructure.DatabaseType.Name)
            .AddKeyedSingleton<IDatabase, MariaDbDatabase>(grate.MariaDb.Infrastructure.DatabaseType.Name)
            .AddKeyedSingleton<IDatabase, OracleDatabase>(grate.Oracle.Infrastructure.DatabaseType.Name)
            .AddKeyedSingleton<IDatabase, SqliteDatabase>(grate.Sqlite.Infrastructure.DatabaseType.Name)
            .BuildServiceProvider();
    }
}

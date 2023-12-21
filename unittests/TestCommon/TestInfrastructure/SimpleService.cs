using grate.Infrastructure;
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
            .AddKeyedSingleton<IDatabase, SqlServerDatabase>(DatabaseType.SqlServer)
            .AddKeyedSingleton<IDatabase, PostgreSqlDatabase>(DatabaseType.PostgreSql)
            .AddKeyedSingleton<IDatabase, MariaDbDatabase>(DatabaseType.MariaDb)
            .AddKeyedSingleton<IDatabase, OracleDatabase>(DatabaseType.Oracle)
            .AddKeyedSingleton<IDatabase, SqliteDatabase>(DatabaseType.Sqlite)
            .BuildServiceProvider();
    }
}

using grate;
using grate.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestCommon.TestInfrastructure;
namespace Sqlite.TestInfrastructure;
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
            .AddGrate(cfg =>
            {
                cfg.UseSqlite();
            })
            .AddSingleton<IDatabaseConnectionFactory, SqliteConnectionFactory>()
            .BuildServiceProvider();
    }
}

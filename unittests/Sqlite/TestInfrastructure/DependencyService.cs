using grate;
using grate.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestCommon.TestInfrastructure;
namespace Sqlite.TestInfrastructure;
public class DependencyService
{
    public IServiceProvider ServiceProvider { get; }
    public DependencyService()
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
            .BuildServiceProvider();
    }
}

using grate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using grate.SqlServer;
using TestCommon.TestInfrastructure;
namespace SqlServer.TestInfrastructure;
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
                cfg.UseSqlServer();
            })
            .BuildServiceProvider();
    }
}

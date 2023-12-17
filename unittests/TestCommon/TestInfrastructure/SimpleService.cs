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
            .BuildServiceProvider();
    }
}

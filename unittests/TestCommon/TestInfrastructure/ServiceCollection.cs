using Microsoft.Extensions.DependencyInjection;

namespace TestCommon.TestInfrastructure;
public class SimpleService
{
    public IServiceProvider ServiceProvider { get; }
    public SimpleService()
    {
        ServiceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
    }
}

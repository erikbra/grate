using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;

namespace TestCommon.TestInfrastructure;
public abstract class ContainerFixture : IAsyncLifetime
{
    public abstract string DockerImage { get; } 
    public abstract int Port { get;  }
    
    private readonly Random _random = new();
    
    private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
    private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "1234567890";
    
    public string AdminPassword { get; private set; }

    protected ContainerFixture(ILogger logger)
    {
        AdminPassword = _random.GetString(10, UpperCase) +
                       _random.GetString(10, LowerCase) +
                       _random.GetString(10, Digits);

        // ReSharper disable once VirtualMemberCallInConstructor
        TestContainer = InitializeTestContainer();
        TestcontainersSettings.Logger = logger;
    }

    public IContainer TestContainer { get; }

    public async Task DisposeAsync()
    {
        await TestContainer.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await TestContainer.StartAsync();
    }

    protected abstract IContainer InitializeTestContainer();
}

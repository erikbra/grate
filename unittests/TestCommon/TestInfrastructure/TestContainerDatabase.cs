using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;

namespace TestCommon.TestInfrastructure;
public abstract class TestContainerDatabase : ITestDatabase, IAsyncLifetime
{
    public abstract string DockerImage { get; } 
    
    private readonly Random _random = new();

    public string AdminPassword { get; }
    public int Port => TestContainer.GetMappedPublicPort(InternalPort);
    
    protected abstract int InternalPort { get; }

    private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
    private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "1234567890";
    
    protected TestContainerDatabase(ILogger logger)
    {
        AdminPassword = _random.GetString(10, UpperCase) +
                       _random.GetString(10, LowerCase) +
                       _random.GetString(10, Digits);

        // ReSharper disable once VirtualMemberCallInConstructor
        TestContainer = InitializeTestContainer();
        TestcontainersSettings.Logger = logger;
    }

    public IContainer TestContainer { get; }
    public abstract string AdminConnectionString { get; }

    public async Task DisposeAsync()
    {
        await TestContainer.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await TestContainer.StartAsync();
    }

    protected abstract IContainer InitializeTestContainer();
    public abstract string ConnectionString(string database);
    public abstract string UserConnectionString(string database);
}

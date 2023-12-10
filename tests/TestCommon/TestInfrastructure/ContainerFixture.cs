using DotNet.Testcontainers.Containers;

namespace TestCommon.TestInfrastructure;
public abstract class ContainerFixture : IAsyncLifetime
{
    private readonly Random _random = new();
    private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
    private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "1234567890";
    public string AdminPassword { get; private set; }
    public ContainerFixture()
    {
        AdminPassword = _random.GetString(10, UpperCase) +
                       _random.GetString(10, LowerCase) +
                       _random.GetString(10, Digits);
    }
    public IContainer? TestContainer { get; protected set; }

    public async Task DisposeAsync()
    {
        if (TestContainer is not null)
        {
            await TestContainer.DisposeAsync();
        }
    }

    public async Task InitializeAsync()
    {
        if (TestContainer is not null)
        {
            await TestContainer.StartAsync();
        }
    }
}

namespace TestCommon.TestInfrastructure;

public class TestDatabaseFixture(ITestDatabase testDatabase) : IAsyncLifetime
{
    public ITestDatabase TestDatabase { get; } = testDatabase;

    public async Task InitializeAsync()
    {
        if (TestDatabase is IAsyncLifetime asyncLifetime)
        {
            await asyncLifetime.InitializeAsync();
        }
    }

    public async Task DisposeAsync()
    {
        if (TestDatabase is IAsyncLifetime asyncLifetime)
        {
            await asyncLifetime.DisposeAsync();
        }
    }
}

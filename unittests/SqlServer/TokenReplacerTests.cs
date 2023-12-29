using FluentAssertions;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
using SqlServer.TestInfrastructure;

namespace Basic_tests.Infrastructure;


public class TokenReplacerTests : IClassFixture<SimpleService>
{
    private readonly IServiceProvider _serviceProvider;
    public TokenReplacerTests(SimpleService sqlServersimpleService)
    {
        _serviceProvider = sqlServersimpleService.ServiceProvider;
    }

    [Fact]
    public void EnsureDbMakesItToTokens()
    {
        var config = new GrateConfiguration()
        {
            ConnectionString = "Server=(LocalDb)\\mssqllocaldb;Database=TestDb;",
            Folders = FoldersConfiguration.Default(null)
        };


        var db = _serviceProvider.GetRequiredService<IDatabase>();
        db.InitializeConnections(config);

        var provider = new TokenProvider(config, db);
        var tokens = provider.GetTokens();

        tokens["DatabaseName"].Should().Be("TestDb");
        tokens["ServerName"].Should().Be("(LocalDb)\\mssqllocaldb");
    }
}

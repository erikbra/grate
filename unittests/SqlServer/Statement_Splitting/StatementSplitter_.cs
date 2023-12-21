using FluentAssertions;
using grate.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SqlServer.TestInfrastructure;

namespace Basic_tests.Infrastructure.SqlServer.Statement_Splitting;


// ReSharper disable once InconsistentNaming
public class StatementSplitter_ : IClassFixture<DependencyService>
{
    private StatementSplitter Splitter;

    public StatementSplitter_(DependencyService dependencyService)
    {
        Splitter = dependencyService.ServiceProvider.GetRequiredService<StatementSplitter>()!;
    }

    [Fact]
    public void Splits_and_removes_GO_statements()
    {
        var original = @"
SELECT @@VERSION;


GO
SELECT 1
";
        var batches = Splitter.Split(original);

        batches.Should().HaveCount(2);
    }

}

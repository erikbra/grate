using FluentAssertions;
using grate.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Oracle.TestInfrastructure;

namespace Basic_tests.Infrastructure.Oracle.Statement_Splitting;


// ReSharper disable once InconsistentNaming
public class StatementSplitter_ : IClassFixture<SimpleService>
{
    private StatementSplitter Splitter;

    public StatementSplitter_(SimpleService simpleService)
    {
        Splitter = simpleService.ServiceProvider.GetRequiredService<StatementSplitter>()!;
    }
    [Fact]
    public void Splits_and_removes_GO_statements()
    {
        var original = @"
SELECT * FROM v$version WHERE banner LIKE 'Oracle%';


/
SELECT 1
";
        var batches = Splitter.Split(original);

        batches.Should().HaveCount(2);
    }

}

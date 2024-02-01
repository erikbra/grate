using FluentAssertions;
using grate.Infrastructure;

namespace SqlServer.Statement_Splitting;


// ReSharper disable once InconsistentNaming
public class StatementSplitter_(StatementSplitter splitter)
{
    [Fact]
    public void Splits_and_removes_GO_statements()
    {
        var original = @"
SELECT @@VERSION;


GO
SELECT 1
";
        var batches = splitter.Split(original);

        batches.Should().HaveCount(2);
    }

}

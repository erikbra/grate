using FluentAssertions;
using grate.Infrastructure;

namespace Basic_tests.Infrastructure.Oracle.Statement_Splitting;


// ReSharper disable once InconsistentNaming
public class StatementSplitter_
{
    private readonly StatementSplitter _splitter;

    public StatementSplitter_(StatementSplitter splitter)
    {
        _splitter = splitter;
    }
    
    [Fact]
    public void Splits_and_removes_GO_statements()
    {
        var original = @"
SELECT * FROM v$version WHERE banner LIKE 'Oracle%';


/
SELECT 1
";
        var batches = _splitter.Split(original);

        batches.Should().HaveCount(2);
    }

}

using FluentAssertions;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Basic.Infrastructure.Oracle.Statement_Splitting;

[TestFixture]
[Category("Basic")]
// ReSharper disable once InconsistentNaming
public class StatementSplitter_
{
    
#pragma warning disable NUnit1032
    private static readonly IDatabase Database = new OracleDatabase(NullLogger<OracleDatabase>.Instance);
#pragma warning restore NUnit1032
    
    private static readonly StatementSplitter Splitter = new(Database.StatementSeparatorRegex);

    [Test]
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

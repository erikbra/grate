using FluentAssertions;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Basic_tests.Infrastructure.SqlServer.Statement_Splitting;


// ReSharper disable once InconsistentNaming
public class StatementSplitter_
{

#pragma warning disable NUnit1032
    private static readonly IDatabase Database = new SqlServerDatabase(NullLogger<SqlServerDatabase>.Instance);
    private static readonly StatementSplitter Splitter = new(Database.StatementSeparatorRegex);
#pragma warning restore NUnit1032

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

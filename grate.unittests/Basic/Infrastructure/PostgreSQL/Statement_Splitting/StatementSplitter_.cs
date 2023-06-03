using FluentAssertions;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace grate.unittests.Basic.Infrastructure.PostgreSQL.Statement_Splitting;

[TestFixture]
[Category("Basic")]
// ReSharper disable once InconsistentNaming
public class StatementSplitter_
{
    private static readonly IDatabase Database = new PostgreSqlDatabase(NullLogger<PostgreSqlDatabase>.Instance);
    private static readonly StatementSplitter Splitter = new(Database.StatementSeparatorRegex);

    [Test]
    public void Splits_and_removes_semicolons()
    {
        var original = @"
DROP INDEX IF EXISTS IX_column1 CASCADE;
CREATE INDEX CONCURRENTLY IX_column1 ON public.table1
	USING btree
	(
	  column1
	);

DROP INDEX IF EXISTS IX_column2 CASCADE;
CREATE INDEX CONCURRENTLY IX_column2 ON public.table1
	USING btree
	(
	  column2
	);
";
        var batches = Splitter.Split(original);

        batches.Should().HaveCount(4);
    }

}

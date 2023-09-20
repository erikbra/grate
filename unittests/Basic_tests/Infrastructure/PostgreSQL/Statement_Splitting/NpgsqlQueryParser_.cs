using FluentAssertions;
using grate.Infrastructure.Npgsql;

namespace PostgreSQL.Infrastructure;

public class NpgsqlQueryParser_
{
    [Test]
    public void Can_split_create_index_concurrently()
    {
        //var statements = ReflectionNpgsqlQueryParser.Split(sqlStatement);
        var statements = NativeSqlQueryParser.Split(sqlStatement);
        statements.Should().HaveCount(4);
    }

    private const string sqlStatement = @"
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

}

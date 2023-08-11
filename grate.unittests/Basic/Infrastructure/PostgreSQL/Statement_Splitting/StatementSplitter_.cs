﻿using System.Linq;
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
    
    [Test]
    public void Ignores_semicolons_in_backslash_escaped_strings()
    {
        var original = @"
DO E'
BEGIN
    IF NOT EXISTS(
        SELECT schema_name
        FROM information_schema.schemata
        WHERE schema_name = \'random\'
    )
    THEN
        EXECUTE \'CREATE SCHEMA random\';
    END IF;
END
';
";
        var batches = Splitter.Split(original);

        batches.Should().HaveCount(1);
    }

    [TestCase("$$")]
    [TestCase("$sometag$")]
    public void Ignores_semicolons_in_dollar_quoted_strings(string tag)
    {
        var original = @$"
DO 
{tag}
BEGIN
    IF NOT EXISTS(
        SELECT schema_name
        FROM information_schema.schemata
        WHERE schema_name = 'random'
    )
    THEN
        EXECUTE 'CREATE SCHEMA random';
    END IF;
END
{tag};
";
        var batches = Splitter.Split(original);
        batches.Should().HaveCount(1);
    }
    
    [Test]
    public void Splits_on_semicolon_after_single_quotes_when_there_is_another_semicolon_in_the_quote()
    {
        var original = @"SELECT 1 WHERE whatnot = '; ' ; MOO";
        var batches = Splitter.Split(original).ToList();
        batches.Should().HaveCount(2);

        batches.First().Should().Be("SELECT 1 WHERE whatnot = '; ' ");
        batches.Last().Should().Be(" MOO");
    }

    [Test]
    public void Ignores_semicolon_in_single_quotes_when_there_is_no_other_semicolon()
    {
        var original = @"SELECT 1 WHERE whatnot = '; '";
        var batches = Splitter.Split(original);
        batches.Should().HaveCount(1);
    }

    [Test]
    public void Ignores_semicolons_in_strings()
    {
        var original = @"
DO '
BEGIN
    IF NOT EXISTS(
        SELECT schema_name
        FROM information_schema.schemata
        WHERE schema_name = ''random''
    )
    THEN
        EXECUTE ''CREATE SCHEMA random'';
    END IF;
END
';
";
        var batches = Splitter.Split(original);

        batches.Should().HaveCount(1);
    }
}

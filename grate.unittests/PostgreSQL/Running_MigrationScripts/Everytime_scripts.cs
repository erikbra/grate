using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;
using static grate.Configuration.KnownFolderKeys;

namespace grate.unittests.PostgreSQL.Running_MigrationScripts;

[TestFixture]
[Category("PostgreSQL")]
// ReSharper disable once InconsistentNaming
public class Everytime_scripts: Generic.Running_MigrationScripts.Everytime_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;

    [Test]
    public async Task Create_index_concurrently_works()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        WriteSql(parent, knownFolders[Up]!.Path, "create_table_dummy.sql", @"
CREATE TABLE public.table1(
    column1 INT PRIMARY KEY,
    column2 INT
)
");

        WriteSql(parent, knownFolders[Indexes]!.Path, "create_index_concurrently.sql", @"
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
");

        await using (migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            await migrator.Migrate();
        }

        string[] scripts;
        string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

        await using (var conn = Context.CreateDbConnection(db))
        {
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.Should().HaveCount(2);
    }
}

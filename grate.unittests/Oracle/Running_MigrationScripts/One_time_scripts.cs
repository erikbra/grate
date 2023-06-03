using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;
using static grate.Configuration.KnownFolderKeys;

namespace grate.unittests.Oracle.Running_MigrationScripts;

[TestFixture]
[Category("Oracle")]
// ReSharper disable once InconsistentNaming
public class One_time_scripts: Generic.Running_MigrationScripts.One_time_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;

    protected override string CreateView1 => base.CreateView1 + " FROM DUAL";
    protected override string CreateView2 => base.CreateView2 + " FROM DUAL";

    [Test]
    public async Task Can_run_scripts_with_semicolon_in_them()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        WriteSql(parent, knownFolders[Up]!.Path, "create_table.sql", @"
        CREATE TABLE actor (
            actor_id numeric NOT NULL ,
            first_name VARCHAR(45) NOT NULL,
            last_name VARCHAR(45) NOT NULL,
            last_update DATE NOT NULL,
            CONSTRAINT pk_actor PRIMARY KEY (actor_id)
        );");

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

        scripts.Should().HaveCount(1);
     
    }
    
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

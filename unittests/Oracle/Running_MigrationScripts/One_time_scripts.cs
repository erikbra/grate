using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;
using static grate.Configuration.KnownFolderKeys;

namespace Oracle.Running_MigrationScripts;


[Collection(nameof(OracleGrateTestContext))]
public class One_time_scripts(OracleGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.One_time_scripts(testContext, testOutput)
{
    protected override string CreateView1 => base.CreateView1 + " FROM DUAL";
    protected override string CreateView2 => base.CreateView2 + " FROM DUAL";

    [Fact]
    public async Task Can_run_scripts_with_semicolon_in_them()
    {
        var db = TestConfig.RandomDatabase();

        IGrateMigrator? migrator;

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default();

        WriteSql(parent, knownFolders[Up]!.Path, "create_table.sql", @"
        CREATE TABLE actor (
            actor_id numeric NOT NULL ,
            first_name VARCHAR(45) NOT NULL,
            last_name VARCHAR(45) NOT NULL,
            last_update DATE NOT NULL,
            CONSTRAINT pk_actor PRIMARY KEY (actor_id)
        );");

        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithSqlFilesDirectory(parent)
            .WithFolders(knownFolders)
            .Build();

        await using (migrator = Context.Migrator.WithConfiguration(config))
        {
            await migrator.Migrate();
        }

        string[] scripts;
        string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

        using (var conn = Context.GetDbConnection(Context.ConnectionString(db)))
        {
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.Should().HaveCount(1);
    }

    [Fact]
    public async Task Create_index_concurrently_works()
    {
        var db = TestConfig.RandomDatabase();

        IGrateMigrator? migrator;

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

        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithSqlFilesDirectory(parent)
            .WithFolders(knownFolders)
            .Build();

        await using (migrator = Context.Migrator.WithConfiguration(config))
        {
            await migrator.Migrate();
        }

        string[] scripts;
        string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

        using (var conn = Context.GetDbConnection(Context.ConnectionString(db)))
        {
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.Should().HaveCount(2);
    }
}


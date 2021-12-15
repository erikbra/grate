using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using FluentAssertions.Execution;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Generic.Running_MigrationScripts;

[TestFixture]
public abstract class Anytime_scripts : MigrationsScriptsBase
{
    [Test]
    public async Task Are_not_run_more_than_once_when_unchanged()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
        CreateDummySql(knownFolders.Sprocs);

        await using (migrator = Context.GetMigrator(db, knownFolders))
        {
            await migrator.Migrate();
        }
        await using (migrator = Context.GetMigrator(db, knownFolders))
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
    public async Task Are_run_again_if_changed_between_runs()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
        CreateDummySql(knownFolders.Sprocs);

        await using (migrator = Context.GetMigrator(db, knownFolders))
        {
            await migrator.Migrate();
        }

        WriteSomeOtherSql(knownFolders.Sprocs);

        await using (migrator = Context.GetMigrator(db, knownFolders))
        {
            await migrator.Migrate();
        }

        string[] scripts;
        string sql = $"SELECT text_of_script FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

        await using (var conn = Context.CreateDbConnection(db))
        {
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.Should().HaveCount(2);

        using (new AssertionScope())
        {
            scripts.First().Should().Be(Context.Sql.SelectVersion);
            scripts.Last().Should().Be(Context.Syntax.CurrentDatabase);
        }
    }

    [Test]
    public async Task Do_not_have_text_logged_if_flag_set()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
        CreateDummySql(knownFolders.Sprocs);

        var config = Context.GetConfiguration(db, knownFolders) with
        {
            DoNotStoreScriptsRunText = true, // important
        };
           
        await using (migrator = Context.GetMigrator(config))
        {
            await migrator.Migrate();
        }
            
        string[] scripts;
        string sql = $"SELECT text_of_script FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

        await using (var conn = Context.CreateDbConnection(db))
        {
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.Should().HaveCount(1);
        scripts.Single().Should().Be(null);
    }

    [Test]
    public async Task Do_have_text_logged_by_default()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
        CreateDummySql(knownFolders.Sprocs);
            
        var config = Context.GetConfiguration(db, knownFolders) with
        {
            DoNotStoreScriptsRunText = false, // important
        };
           
        await using (migrator = Context.GetMigrator(config))
        {
            await migrator.Migrate();
        }

        string[] scripts;
        string sql = $"SELECT text_of_script FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

        await using (var conn = Context.CreateDbConnection(db))
        {
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.Should().HaveCount(1);
        scripts.Single().Should().Be(Context.Sql.SelectVersion);
    }

    [Test]
    public async Task Are_run_more_than_once_when_unchanged_but_flag_set()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
        CreateDummySql(knownFolders.Sprocs);
            
        var config = Context.GetConfiguration(db, knownFolders) with
        {
            RunAllAnyTimeScripts = true, // important
        };

        await using (migrator = Context.GetMigrator(config))
        {
            await migrator.Migrate();
        }
        await using (migrator = Context.GetMigrator(config))
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
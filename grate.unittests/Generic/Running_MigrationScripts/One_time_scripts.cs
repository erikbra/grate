using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Generic.Running_MigrationScripts;

[TestFixture]
// ReSharper disable once InconsistentNaming
public abstract class One_time_scripts: MigrationsScriptsBase
{
    [Test]
    public async Task Are_not_run_more_than_once_when_unchanged()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;
            
        var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
        CreateDummySql(knownFolders.Up);
            
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
    public async Task Fails_if_changed_between_runs()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;
            
        var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
        CreateDummySql(knownFolders.Up);
            
        await using (migrator = Context.GetMigrator(db, knownFolders))
        {
            await migrator.Migrate();
        }
            
        WriteSomeOtherSql(knownFolders.Up);
            
        await using (migrator = Context.GetMigrator(db, knownFolders))
        {
            Assert.ThrowsAsync<OneTimeScriptChanged>(() => migrator.Migrate());
        }

        string[] scripts;
        string sql = $"SELECT text_of_script FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";
            
        await using (var conn = Context.CreateDbConnection(db))
        {
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.Should().HaveCount(1);
        scripts.First().Should().Be(Context.Sql.SelectVersion);
    }

    [Test]
    public async Task Runs_and_warns_if_changed_between_runs_and_flag_set()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
        CreateDummySql(knownFolders.Up);
            
        var config = Context.GetConfiguration(db, knownFolders) with
        {
            WarnOnOneTimeScriptChanges = true, // this is important!
        };

        await using (migrator = Context.GetMigrator(config))
        {
            await migrator.Migrate();
        }

        WriteSomeOtherSql(knownFolders.Up);

        await using (migrator = Context.GetMigrator(config))
        {
            await migrator.Migrate(); // no exceptions this time
        }

        string[] scripts;
        string sql = $"SELECT text_of_script FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")} order by id";

        await using (var conn = Context.CreateDbConnection(db))
        {
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.Should().HaveCount(2); //script run twice
        scripts.Last().Should().Be(Context.Syntax.CurrentDatabase); // the script was re-run
    }

    protected virtual string CreateView1 => "create view grate as select '1' as col";
    protected virtual string CreateView2 => "create view grate as select '2' as col";

    [Test]
    public async Task Ignores_and_warns_if_changed_between_runs_and_flag_set()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
        var path = knownFolders.Up?.Path ?? throw new Exception("Config Fail");

        WriteSql(path, "token.sql", CreateView1);

        var config = Context.GetConfiguration(db, knownFolders) with
        {
            WarnAndIgnoreOnOneTimeScriptChanges = true, // this is important!
        };

        await using (migrator = Context.GetMigrator(config))
        {
            await migrator.Migrate();
        }

        WriteSql(path, "token.sql", CreateView2);

        await using (migrator = Context.GetMigrator(config))
        {
            await migrator.Migrate(); // no exceptions this time
        }

        string sql = $"SELECT text_of_script FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")} order by id";

        await using var conn = Context.CreateDbConnection(db);
        var scripts = await conn.QueryAsync<string>(sql);
        var result = (await conn.QueryAsync<string>("select col from grate")).Single();


        scripts.Should().HaveCount(2); //script marked as run twice
        result.Should().Be("1"); // but still the first version of the view
    }
}

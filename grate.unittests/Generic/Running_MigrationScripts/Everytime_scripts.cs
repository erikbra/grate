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
public abstract class Everytime_scripts : MigrationsScriptsBase
{
    [Test]
    public async Task Are_run_every_time_even_when_unchanged()
    {
        var db = TestConfig.RandomDatabase();

        var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
        CreateDummySql(knownFolders.Permissions);

        for (var i = 0; i < 3; i++)
        {
            await using var migrator = Context.GetMigrator(db, knownFolders);
            await migrator.Migrate();
        }

        string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

        await using var conn = Context.CreateDbConnection(db);
        var scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        scripts.Should().HaveCount(3);
    }

    [Test]
    public async Task Are_not_run_in_dryrun()
    {
        var db = TestConfig.RandomDatabase();

        var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
        CreateDummySql(knownFolders.Permissions);

        var config = Context.GetConfiguration(db, knownFolders) with
        {
            DryRun = true, // this is important!
        };

        await using var migrator = Context.GetMigrator(config);
        await migrator.Migrate();
        // this helper takes into account whether the grate versioning table exists or not.
        Assert.False(await migrator.DbMigrator.Database.HasRun("1_jalla.sql"));
    }

    [Test]
    public async Task Are_recognized_by_script_name()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var knownFolders = KnownFolders.In(CreateRandomTempDirectory());

        var folder = knownFolders.Up;// not an everytime folder

        CreateDummySql(folder);
        CreateEveryTimeScriptFile(folder);
        CreateOtherEveryTimeScriptFile(folder);

        for (var i = 0; i < 3; i++)
        {
            await using (migrator = Context.GetMigrator(db, knownFolders))
            {
                await migrator.Migrate();
            }
        }

        string[] scripts;
        string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

        await using (var conn = Context.CreateDbConnection(db))
        {
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.Should().HaveCount(7); // one time script ran once, the two everytime scripts ran every time.
    }

    [Test]
    public async Task Are_not_run_in_baseline()
    {
        var db = TestConfig.RandomDatabase();

        var knownFolders = KnownFolders.In(CreateRandomTempDirectory());

        var config = Context.GetConfiguration(db, knownFolders) with
        {
            Baseline = true, // this is important!
        };

        var path = knownFolders?.Views?.Path ?? throw new Exception("Config Fail");

        WriteSql(path, "view.sql", "create view grate as select '1' as col");

        await using (var migrator = Context.GetMigrator(config))
        {
            await migrator.Migrate();
        }

        string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

        await using var conn = Context.CreateDbConnection(db);
        var scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        scripts.Should().HaveCount(1); //marked as run

        // but doesn't exist
        Assert.ThrowsAsync(Context.DbExceptionType, async () => await conn.QueryAsync<string>("select * from grate"));
    }

    private void CreateEveryTimeScriptFile(MigrationsFolder? folder)
    {
        var dummySql = Context.Syntax.CurrentDatabase;
        var path = MakeSurePathExists(folder);
        WriteSql(path, "everytime.1_jalla.sql", dummySql);
    }

    private void CreateOtherEveryTimeScriptFile(MigrationsFolder? folder)
    {
        var dummySql = Context.Syntax.CurrentDatabase;
        var path = MakeSurePathExists(folder);
        WriteSql(path, "1_jalla.everytime.and.always.sql", dummySql);
    }

}

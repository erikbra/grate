using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using TestCommon.TestInfrastructure;
using static grate.Configuration.KnownFolderKeys;

namespace TestCommon.Generic.Running_MigrationScripts;

//[TestFixture]
// ReSharper disable once InconsistentNaming
public abstract class Everytime_scripts : MigrationsScriptsBase
{
    [Fact]
    public async Task Are_run_every_time_even_when_unchanged()
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        CreateDummySql(parent, knownFolders[Permissions]);

        for (var i = 0; i < 3; i++)
        {
            await using var migrator = Context.GetMigrator(db, parent, knownFolders);
            await migrator.Migrate();
        }

        string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

        await using var conn = Context.CreateDbConnection(db);
        var scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        scripts.Should().HaveCount(3);
    }

    [Fact]
    public async Task Are_not_run_in_dryrun()
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        CreateDummySql(parent, knownFolders[Permissions]);

        var config = Context.GetConfiguration(db, parent, knownFolders) with
        {
            DryRun = true, // this is important!
        };

        await using var migrator = Context.GetMigrator(config);
        await migrator.Migrate();

        string[] scripts;
        string sql = $"SELECT 1 FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")} " +
                     $"WHERE script_name = '1_jalla.sql'";

        await using (var conn = Context.CreateDbConnection(db))
        {
            try
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }
            catch (Exception) when (config.DryRun)
            {
                scripts = Array.Empty<string>();
            }
        }

        scripts.Should().BeEmpty();
    }

    [Fact]
    public async Task Are_recognized_by_script_name()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        var folder = knownFolders[Up];// not an everytime folder

        CreateDummySql(parent, folder);
        CreateEveryTimeScriptFile(parent, folder);
        CreateOtherEveryTimeScriptFile(parent, folder);

        for (var i = 0; i < 3; i++)
        {
            await using (migrator = Context.GetMigrator(db, parent, knownFolders))
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

    [Fact]
    public async Task Are_not_run_in_baseline()
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        var config = Context.GetConfiguration(db, parent, knownFolders) with
        {
            Baseline = true, // this is important!
        };

        var path = Wrap(parent, knownFolders[Views]?.Path ?? throw new Exception("Config Fail"));

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
        await Assert.ThrowsAsync(Context.DbExceptionType, async () => await conn.QueryAsync<string>("select * from grate"));
    }

    private void CreateEveryTimeScriptFile(DirectoryInfo root, MigrationsFolder? folder)
    {
        var dummySql = Context.Syntax.CurrentDatabase;
        var path = MakeSurePathExists(root, folder);
        WriteSql(path, "everytime.1_jalla.sql", dummySql);
    }

    private void CreateOtherEveryTimeScriptFile(DirectoryInfo root, MigrationsFolder? folder)
    {
        var dummySql = Context.Syntax.CurrentDatabase;
        var path = MakeSurePathExists(root, folder);
        WriteSql(path, "1_jalla.everytime.and.always.sql", dummySql);
    }

}

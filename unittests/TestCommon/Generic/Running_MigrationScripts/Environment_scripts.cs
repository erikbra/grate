using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using TestCommon.TestInfrastructure;
using static grate.Configuration.KnownFolderKeys;

namespace TestCommon.Generic.Running_MigrationScripts;

//[TestFixture]
// ReSharper disable once InconsistentNaming
public abstract class Environment_scripts : MigrationsScriptsBase
{
    [Fact]
    public async Task Are_not_run_if_not_in_environment()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        CreateDummySql(parent, knownFolders[Up], "1_.OTHER.filename.ENV.sql");

        await using (migrator = Context.GetMigrator(db, parent, knownFolders, "TEST"))
        {
            await migrator.Migrate();
        }

        string[] scripts;
        string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

        await using (var conn = Context.CreateDbConnection(db))
        {
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.Should().BeEmpty();
    }

    [Fact]
    public async Task Are_not_run_by_default() //Bug #101
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        CreateDummySql(parent, knownFolders[Up], "1_.OTHER.filename.ENV.sql");

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

        scripts.Should().BeEmpty();
    }

    [Fact]
    public async Task Are_run_if_in_environment()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;
        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        CreateDummySql(parent, knownFolders[Up], "1_.TEST.filename.ENV.sql");
        CreateDummySql(parent, knownFolders[Up], "2_.TEST.ENV.otherfilename.sql");

        await using (migrator = Context.GetMigrator(db, parent, knownFolders, "TEST"))
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

    [Fact]
    public async Task Non_environment_scripts_are_always_run()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        CreateDummySql(parent, knownFolders[Up], "1_.filename.sql");
        CreateDummySql(parent, knownFolders[Up], "2_.TEST.ENV.otherfilename.sql");
        CreateDummySql(parent, knownFolders[Up], "2_.TEST.ENV.somethingelse.sql");

        await using (migrator = Context.GetMigrator(db, parent, knownFolders, "PROD"))
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
}

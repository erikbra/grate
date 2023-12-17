using Dapper;
using FluentAssertions;
using grate.Configuration;
using TestCommon.TestInfrastructure;
using static grate.Configuration.KnownFolderKeys;

namespace TestCommon.Generic.Running_MigrationScripts;

public abstract class DropDatabase : MigrationsScriptsBase
{
    [Fact]
    public async Task Ensure_database_gets_dropped()
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        CreateDummySql(parent, knownFolders[Sprocs]);

        var dropConfig = Context.GetConfiguration(db, parent, knownFolders) with
        {
            Drop = true, // This is important!
        };

        await using (var migrator = Context.GetMigrator(dropConfig))
        {
            await migrator.Migrate();
        }

        WriteSomeOtherSql(parent, knownFolders[Sprocs]);

        await using (var migrator = Context.GetMigrator(dropConfig))
        {
            // This second migration should drop and recreate, so only one script run afterwards
            await migrator.Migrate();
        }

        string[] scripts;
        string sql = $"SELECT text_of_script FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

        await using (var conn = Context.CreateDbConnection(db))
        {
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.Should().HaveCount(1); // only one script because the database was dropped after the first migration...
    }
}

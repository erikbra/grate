using Dapper;
using FluentAssertions;
using FluentAssertions.Execution;
using grate.Configuration;
using grate.Migration;
using TestCommon.TestInfrastructure;
using static grate.Configuration.KnownFolderKeys;

namespace TestCommon.Generic.Running_MigrationScripts;

// ReSharper disable once InconsistentNaming
public abstract class Versioning_The_Database : MigrationsScriptsBase
{
    [Fact]
    public async Task Returns_the_new_version_id()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;
        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        CreateDummySql(parent, knownFolders[Sprocs]);

        await using (migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            await migrator.Migrate();

            using (new AssertionScope())
            {
                // Version again
                var version = await migrator.DbMigrator.VersionTheDatabase("1.2.3.4");
                version.Should().Be(2);

                // And again
                version = await migrator.DbMigrator.VersionTheDatabase("1.2.3.4");
                version.Should().Be(3);
            }
        }
    }

    [Fact]
    public async Task Does_not_create_versions_on_DryRun()
    {
        //for bug #204 - when running --baseline and --dryrun on a new db it shouldn't create the grate schema's etc
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        CreateDummySql(parent, knownFolders[Sprocs]); // make sure there's something that could be logged...

        var grateConfig = Context.GetConfiguration(db, parent, knownFolders) with
        {
            Baseline = true, // don't run the sql
            DryRun = true // and don't actually _touch_ the DB in any way
        };

        await using var migrator = Context.GetMigrator(grateConfig);
        await migrator.Migrate(); // shouldn't touch anything because of --dryrun
        var addedTable = await migrator.DbMigrator.Database.VersionTableExists();
        addedTable.Should().Be(false); // we didn't even add the grate infrastructure
    }

    [Fact]
    public async Task Creates_a_new_version_with_status_InProgress()
    {
        var db = TestConfig.RandomDatabase();
        var dbVersion = "1.2.3.4";

        GrateMigrator? migrator;

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        CreateDummySql(parent, knownFolders[Up]);

        long newVersionId = 0;

        await using (migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            //Calling migrate here to setup the database and such.
            await migrator.Migrate();
            newVersionId = await migrator.DbMigrator.VersionTheDatabase(dbVersion);
        }

        IEnumerable<(string version, string status)> entries;
        string sql = $"SELECT version, status FROM {Context.Syntax.TableWithSchema("grate", "Version")}";

        await using (var conn = Context.CreateDbConnection(db))
        {
            entries = await conn.QueryAsync<(string version, string status)>(sql);
        }

        entries.Should().HaveCount(2);
        var version = entries.Single(x => x.version == dbVersion);
        version.status.Should().Be(MigrationStatus.InProgress);
    }
}

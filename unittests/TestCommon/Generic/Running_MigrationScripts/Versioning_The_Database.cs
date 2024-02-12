using Dapper;
using FluentAssertions;
using FluentAssertions.Execution;
using grate.Configuration;
using TestCommon.TestInfrastructure;
using Xunit.Abstractions;
using static grate.Configuration.KnownFolderKeys;

namespace TestCommon.Generic.Running_MigrationScripts;

// ReSharper disable once InconsistentNaming
public abstract class Versioning_The_Database(IGrateTestContext context, ITestOutputHelper testOutput) 
    : MigrationsScriptsBase(context, testOutput)
{
    protected Versioning_The_Database(): this(null!, null!)
    {
    }
    
    [Fact]
    public async Task Returns_the_new_version_id()
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        CreateDummySql(parent, knownFolders[Sprocs]);
        
        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithSqlFilesDirectory(parent)
            .Build();
        
        await using (var migrator = Context.Migrator.WithConfiguration(config))
        {
            await migrator.Migrate();

            using (new AssertionScope())
            {
                // Version again
                var version = await migrator.GetDbMigrator().VersionTheDatabase("1.2.3.4");
                version.Should().Be(2);

                // And again
                version = await migrator.GetDbMigrator().VersionTheDatabase("1.2.3.4");
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

        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithSqlFilesDirectory(parent)
            .Baseline()  // don't run the sql
            .DryRun()  // and don't actually _touch_ the DB in any way
            .Build();

        await using var migrator = Context.Migrator.WithConfiguration(config);
        await migrator.Migrate(); // shouldn't touch anything because of --dryrun
        var addedTable = await migrator.GetDbMigrator().Database.VersionTableExists();
        addedTable.Should().Be(false); // we didn't even add the grate infrastructure
    }

    [Fact]
    public async Task Creates_a_new_version_with_status_InProgress()
    {
        var db = TestConfig.RandomDatabase();
        var dbVersion = "1.2.3.4";

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        CreateDummySql(parent, knownFolders[Up]);

        long newVersionId = 0;
        
        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithSqlFilesDirectory(parent)
            .Build();

        await using (var migrator = Context.Migrator.WithConfiguration(config))
        {
            //Calling migrate here to setup the database and such.
            await migrator.Migrate();
            newVersionId = await migrator.GetDbMigrator().VersionTheDatabase(dbVersion);
        }

        IEnumerable<(string version, string status)> entries;
        string sql = $"SELECT version, status FROM {Context.Syntax.TableWithSchema("grate", "Version")}";

        using (var conn = Context.CreateDbConnection(db))
        {
            entries = await conn.QueryAsync<(string version, string status)>(sql);
        }

        entries.Should().HaveCount(2);
        var version = entries.Single(x => x.version == dbVersion);
        version.status.Should().Be(MigrationStatus.InProgress);
    }
}

using Dapper;
using FluentAssertions;
using FluentAssertions.Execution;
using grate;
using grate.Configuration;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
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



    // [Fact]
    // [Trait("Category", "Versioning")]
    // [Trait("Bug", "388")]
    // public async Task Should_reset_the_version_table_to_desire_value()
    // {
    //     var serviceCollection = new ServiceCollection().AddGrate(ConfigureService).AddLogging(opt =>
    //     {
    //         opt.AddConsole();
    //         opt.SetMinimumLevel(TestConfig.GetLogLevel());
    //     });
    //     var serviceProvider = serviceCollection.BuildServiceProvider();
    //     var dbMigrator = serviceProvider.GetRequiredService<IDbMigrator>();
    //     await dbMigrator.InitializeConnections();
    //     dbMigrator.SetDefaultConnectionActive();
    //     await dbMigrator.CreateDatabase();
    //     // insert into version table
    //     var v1 = await dbMigrator.Database.VersionTheDatabase("1.0.0.1");
    //     var v2 = await dbMigrator.Database.VersionTheDatabase("1.0.0.2");
    //     var v3 = await dbMigrator.Database.VersionTheDatabase("1.0.0.3");

    //     // should reset to v2 
    //     await dbMigrator.Database.DeleteVersionRecord(v3);
    //     var newV3 = await dbMigrator.Database.VersionTheDatabase("1.0.0.3-new");
    //     newV3.Should().Be(v3);

    // }
    [Fact]
    [Trait("Category", "Versioning")]
    [Trait("Bug", "388")]
    public async Task Does_not_create_versions_when_no_script_existing()
    {
        var sqlFolder = CreateRandomTempDirectory();
        var originalVersion = "1.0.0.0-alpha";
        var serviceProvider = new ServiceCollection().AddLogging().AddGrate(builder =>
        {
            builder.WithSqlFilesDirectory(sqlFolder);
            builder.WithVersion(originalVersion);
            ConfigureService(builder);
        }).BuildServiceProvider();

        var migrator = serviceProvider.GetRequiredService<IGrateMigrator>();
        await migrator.Migrate();
        var currentVersion = await migrator.DbMigrator.Database.GetCurrentVersion();
        currentVersion.Should().NotBe(originalVersion);
        currentVersion.Should().Be(AnsiSqlDatabase.NotVersioning);
    }

    [Fact]
    [Trait("Category", "Versioning")]
    [Trait("Bug", "388")]
    public async Task Does_not_create_versions_when_no_script_changed()
    {
        //for bug #388 - no script change, should not create new version entry
        var sqlFolder = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        CreateDummySql(sqlFolder, knownFolders[Up]);
        var originalVersion = "1.0.0.0-alpha";
        var migrator = new ServiceCollection().AddLogging().AddGrate(builder =>
        {
            builder.WithSqlFilesDirectory(sqlFolder);
            builder.WithVersion(originalVersion);
            ConfigureService(builder);
        }).BuildServiceProvider().GetRequiredService<IGrateMigrator>();

        await migrator.Migrate();

        var serviceProvider = new ServiceCollection().AddLogging().AddGrate(builder =>
        {
            builder.WithSqlFilesDirectory(sqlFolder);
            builder.WithVersion("1.0.0.2");
            ConfigureService(builder);
        }).BuildServiceProvider();

        migrator = serviceProvider.GetRequiredService<IGrateMigrator>();
        await migrator.Migrate();

        var grateConfig = serviceProvider.GetRequiredService<GrateConfiguration>();

        // migrate again, but don't change the script, shouldn't create a new version record
        var currrentVersion = await migrator.DbMigrator.Database.GetCurrentVersion();
        currrentVersion.Should().Be(grateConfig.Version, "DB version should not be changed due to no new script detected");
    }
    protected abstract void ConfigureService(GrateConfigurationBuilder grateConfiguration);
}

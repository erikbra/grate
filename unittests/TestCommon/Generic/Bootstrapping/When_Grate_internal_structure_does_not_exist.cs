using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using TestCommon.Generic.Running_MigrationScripts;
using TestCommon.TestInfrastructure;
using Xunit.Abstractions;
using static grate.Configuration.KnownFolderKeys;

namespace TestCommon.Generic.Bootstrapping;

public abstract class When_Grate_internal_structure_does_not_exist(IGrateTestContext context, ITestOutputHelper testOutput) 
: MigrationsScriptsBase(context, testOutput)
{
    [Fact]
    public async Task GrateScriptsRun_Table_Is_Created()
    {
        var db = TestConfig.RandomDatabase();
        var grateScriptsRunTableName = "GrateScriptsRun";
        var parent = CreateRandomTempDirectory();

        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(FoldersConfiguration.Default())
            .WithSqlFilesDirectory(parent)
            .Build();

        await using var migrator = Context.Migrator.WithConfiguration(config);
        await RunMigration(migrator);

        var grateScriptsRunTable = await migrator.GetDbMigrator().Database.ExistingTable(config.SchemaName, grateScriptsRunTableName);
        grateScriptsRunTable.Should().NotBeNull();
        grateScriptsRunTable.Should().Be(grateScriptsRunTableName);
    }
    
    [Fact]
    public async Task ScriptsRunError_Table_Is_Created()
    {
        var db = TestConfig.RandomDatabase();
        var grateScriptsErrorTableName = "GrateScriptsRunErrors";
        var parent = CreateRandomTempDirectory();

        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(FoldersConfiguration.Default())
            .WithSqlFilesDirectory(parent)
            .Build();

        await using var migrator = Context.Migrator.WithConfiguration(config);
        await RunMigration(migrator);
        
        var scriptsErrorTable = await migrator.GetDbMigrator().Database.ExistingTable(config.SchemaName, grateScriptsErrorTableName);
        scriptsErrorTable.Should().NotBeNull();
        scriptsErrorTable.Should().Be(grateScriptsErrorTableName);
    }
    
     
    [Fact]
    public async Task Version_Table_Is_Created()
    {
        var db = TestConfig.RandomDatabase();
        var grateVersionTableName = "GrateVersion";
        var parent = CreateRandomTempDirectory();

        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(FoldersConfiguration.Default())
            .WithSqlFilesDirectory(parent)
            .Build();

        await using var migrator = Context.Migrator.WithConfiguration(config);
        await RunMigration(migrator);

        var grateVersionTable = await migrator.GetDbMigrator().Database.ExistingTable(config.SchemaName, grateVersionTableName);
        grateVersionTable.Should().NotBeNull();
        grateVersionTable.Should().Be(grateVersionTableName);
    }
    
    
    private async Task RunMigration(IGrateMigrator migrator)
    {
        var config = migrator.Configuration;
        CreateDummySql(config.SqlFilesDirectory, config.Folders![Sprocs]);
        await migrator.Migrate();
    }

}

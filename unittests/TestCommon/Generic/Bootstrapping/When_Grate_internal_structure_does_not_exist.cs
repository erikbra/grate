﻿using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using TestCommon.Generic.Running_MigrationScripts;
using TestCommon.TestInfrastructure;
using Xunit.Abstractions;
using static System.StringSplitOptions;
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
        
        // Not all databases are case-sensitive, so we can't guarantee the case of the table name
        grateScriptsRunTable!.ToUpper().Should().Be(grateScriptsRunTable.ToUpper());
        
        //await Context.DropDatabase(db);
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
        
        // Not all databases are case-sensitive, so we can't guarantee the case of the table name
        scriptsErrorTable!.ToUpper().Should().Be(scriptsErrorTable.ToUpper());
        
        //await Context.DropDatabase(db);
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
        
        // Not all databases are case-sensitive, so we can't guarantee the case of the table name
        grateVersionTable!.ToUpper().Should().Be(grateVersionTable.ToUpper());
        
        //await Context.DropDatabase(db);
    }
    
    [Theory]
    [InlineData("02_create_scripts_run_table.sql")]
    [InlineData("03_create_scripts_run_errors_table.sql")]
    [InlineData("04_create_version_table.sql")]
    public async Task Logs_internal_scripts_run_in_own_structure(string name)
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

        string[] scriptNames;
        string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", grateScriptsRunTableName)}";

        using (var conn = Context.CreateDbConnection(db))
        {
            scriptNames = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        // The scripts should have been logged twice, once for the creation of the "meta" tables, with the prefix "bootstrap/"
        // (GrateScriptsRun, GrateScriptsRunErrors and GrateVersion), and once for the creation of the actual table
        // (ScriptsRun, ScriptsRunErrors and Version)
        scriptNames.Should().Contain(name);
        scriptNames.Should().Contain($"grate-internal/{name}");
        
        //await Context.DropDatabase(db);
    }

    // This  was failing, because the tokens were already replaced before the internal scripts were run,
    // and the tokens were lazily initialized. So we had to clear the tokens when running the internal scripts.
    // https://github.com/erikbra/grate/issues/512
    [Fact]
    public virtual async Task GrateScriptsRun_table_is_created_even_with_custom_create_database_script()
    {
        var db = TestConfig.RandomDatabase();
        var grateScriptsRunTableName = "GrateScriptsRun";
        var parent = CreateRandomTempDirectory();

        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(FoldersConfiguration.Default())
            .WithSqlFilesDirectory(parent)
            .Build();

        var password = Context.AdminConnectionString
            .Split(";", TrimEntries | RemoveEmptyEntries)
            .SingleOrDefault(entry => entry.StartsWith("Password") || entry.StartsWith("Pwd"))?
            .Split("=", TrimEntries | RemoveEmptyEntries)
            .Last();

        var customScript = Context.Syntax.CreateDatabase(db, password);
        TestConfig.WriteContent(Wrap(config.SqlFilesDirectory, config.Folders?.CreateDatabase?.Path), "createDatabase.sql", customScript);

        await using var migrator = Context.Migrator.WithConfiguration(config);
        await RunMigration(migrator);

        // Make sure the Scripts run table is still created even though the custom script was run (and as a side effect, the tokens were initialized)
        var grateScriptsRunTable = await migrator.GetDbMigrator().Database.ExistingTable(config.SchemaName, grateScriptsRunTableName);
        grateScriptsRunTable.Should().NotBeNull();

        grateScriptsRunTable!.ToUpper().Should().Be(grateScriptsRunTable.ToUpper());
    }

    private async Task RunMigration(IGrateMigrator migrator)
    {
        var config = migrator.Configuration;
        CreateDummySql(config.SqlFilesDirectory, config.Folders![Sprocs]);
        await migrator.Migrate();
    }

}

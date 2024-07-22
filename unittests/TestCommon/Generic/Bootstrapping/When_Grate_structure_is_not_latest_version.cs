using System.Data;
using System.Text.RegularExpressions;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using TestCommon.Generic.Running_MigrationScripts;
using TestCommon.TestInfrastructure;
using Xunit.Abstractions;
using static System.StringSplitOptions;

namespace TestCommon.Generic.Bootstrapping;

// ReSharper disable once UnusedType.Global
// ReSharper disable once InconsistentNaming
public abstract class When_Grate_structure_is_not_latest_version(IGrateTestContext context, ITestOutputHelper testOutput)
    : MigrationsScriptsBase(context, testOutput)
{

    [Theory]
    [MemberData(nameof(VersionTableWithDifferentCasings))]
    public virtual async Task The_latest_version_is_applied(string versionTableName)
    {
        var db = TestConfig.RandomDatabase();
        var parent = CreateRandomTempDirectory();
        
        // This will create the version table without the status column, with different casings

        var cfg = Context.DefaultConfiguration with
        {
            VersionTableName = versionTableName
        };
           
        var config = GrateConfigurationBuilder.Create(cfg)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(FoldersConfiguration.Default())
            .WithSqlFilesDirectory(parent)
            .Build();
        
        // Create database
        var password = Context.AdminConnectionString
            .Split(";", TrimEntries | RemoveEmptyEntries)
            .SingleOrDefault(entry => entry.StartsWith("Password") || entry.StartsWith("Pwd"))?
            .Split("=", TrimEntries | RemoveEmptyEntries)
            .Last();

        var createDatabaseSql = Context.Syntax.CreateDatabase(db, password);
        using (var adminConn = Context.CreateAdminDbConnection())
        {
            await adminConn.ExecuteAsync(createDatabaseSql);
        }
        
        var conn = Context.CreateDbConnection(db);

        var resources = TestInfrastructure.Bootstrapping.GetBootstrapScripts(this.Context.DatabaseType, "Baseline");
        
        foreach (var resource in resources)
        {
            var resourceText = await TestInfrastructure.Bootstrapping.GetContent(this.Context.DatabaseType.Assembly, resource);
            if (resource.Contains("04_create_version_table"))
            {
                resourceText = Regex.Replace(
                    resourceText,
                    @"^(\s*entered_by[^,\n]*),?\s*status[^,\n]*(,?\n)", "$1$2",
                    RegexOptions.Multiline
                    );
            }
            
            resourceText = resourceText.Replace("{{ScriptsRunTable}}", config.ScriptsRunTableName);
            resourceText = resourceText.Replace("{{ScriptsRunErrorsTable}}", config.ScriptsRunErrorsTableName);
            resourceText = resourceText.Replace("{{VersionTable}}", config.VersionTableName);
            resourceText = resourceText.Replace("{{SchemaName}}", config.SchemaName);
            
            await conn.ExecuteAsync(resourceText);
        }

        conn.Close();
     
        // Check that the status column is not there
        var tableWithSchema = Context.Syntax.TableWithSchema(config.SchemaName, config.VersionTableName);
        var selectSql = $"SELECT * FROM {tableWithSchema}";
        
        conn = Context.CreateDbConnection(db);
        var reader = await conn.ExecuteReaderAsync(selectSql);
        
        // Not all databases are case-sensitive, so we can't guarantee the case of the table name
        var columns = GetColumns(reader).Select(column => column.ToUpper());
        TryClose(conn);
        columns.Should().NotContain("status".ToUpper());
        
        // Reset the config to use the default column casings, to make sure that the status column is added even if the
        // already "existing" tables (which we just created above, with varying casings), are updated, even if the
        // casing of the default configuration is different from the standard one
        var standardConfig = config with
        {
            VersionTableName = GrateConfiguration.Default.VersionTableName,
        };
        
        // Run the migration
        await using (var migrator = Context.Migrator.WithConfiguration(standardConfig))
        {
            await RunMigration(migrator);
        }
        
        // Check that the status column has been added
        tableWithSchema = Context.Syntax.TableWithSchema(standardConfig.SchemaName, standardConfig.VersionTableName);
        selectSql = $"SELECT * FROM {tableWithSchema}";

        conn = Context.CreateDbConnection(db);
        reader = await conn.ExecuteReaderAsync(selectSql);
        
        // Not all databases are case-sensitive, so we can't guarantee the case of the table name
        columns = GetColumns(reader).Select(column => column.ToUpper());
        TryClose(conn);
        columns.Should().Contain("status".ToUpper());
        
        //await Context.DropDatabase(db);
    }
    
    
    [Theory]
    [MemberData(nameof(VersionTableWithDifferentCasings))]
    public virtual async Task The_table_name_casings_are_converted_if_needed(string versionTableName)
    {
        var db = TestConfig.RandomDatabase();
        var parent = CreateRandomTempDirectory();
        
        // This will create the version table with the supplied casing
        var cfg = Context.DefaultConfiguration with
        {
            VersionTableName = versionTableName
        };
           
        var config = GrateConfigurationBuilder.Create(cfg)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(FoldersConfiguration.Default())
            .WithSqlFilesDirectory(parent)
            .Build();
        
        // Create database
        var password = Context.AdminConnectionString
            .Split(";", TrimEntries | RemoveEmptyEntries)
            .SingleOrDefault(entry => entry.StartsWith("Password") || entry.StartsWith("Pwd"))?
            .Split("=", TrimEntries | RemoveEmptyEntries)
            .Last();

        var createDatabaseSql = Context.Syntax.CreateDatabase(db, password);
        using (var adminConn = Context.CreateAdminDbConnection())
        {
            await adminConn.ExecuteAsync(createDatabaseSql);
        }
        
        var conn = Context.CreateDbConnection(db);

        // Manually create the script tables, with varying casing (supplied as Theory attributes)
        var resources = TestInfrastructure.Bootstrapping.GetBootstrapScripts(this.Context.DatabaseType, "Baseline");
        foreach (var resource in resources)
        {
            var resourceText = await TestInfrastructure.Bootstrapping.GetContent(this.Context.DatabaseType.Assembly, resource);
            
            resourceText = resourceText.Replace("{{ScriptsRunTable}}", config.ScriptsRunTableName);
            resourceText = resourceText.Replace("{{ScriptsRunErrorsTable}}", config.ScriptsRunErrorsTableName);
            resourceText = resourceText.Replace("{{VersionTable}}", config.VersionTableName);
            resourceText = resourceText.Replace("{{SchemaName}}", config.SchemaName);
            
            await conn.ExecuteAsync(resourceText);
        }

        conn.Close();
     
        // Check that the table exists in the DB, with their various casing
        // - Selecting from the table with a non-existing table name would throw an exception
        var tableWithSchema = Context.Syntax.TableWithSchema(config.SchemaName, config.VersionTableName);
        var selectSql = $"SELECT * FROM {tableWithSchema}";
        
        conn = Context.CreateDbConnection(db);
        var reader = await conn.ExecuteReaderAsync(selectSql);
        
        var columns = GetColumns(reader).Select(column => column.ToUpper());
        TryClose(conn);
        columns.Should().HaveCountGreaterThan(2);
        
        // Reset the config to use the default column names, and run the migration. Then, select from the version table
        // with the default table name, and see that it has been converted to the default casing (if needed by the DB provider),
        // i.e., we can still select from it.
        
        var standardConfig = config with
        {
            VersionTableName = GrateConfiguration.Default.VersionTableName,
        };
        
        // Run the migration
        await using (var migrator = Context.Migrator.WithConfiguration(standardConfig))
        {
            await RunMigration(migrator);
        }
        
        // Check that the tables can be selected from, using the standard table names.
        tableWithSchema = Context.Syntax.TableWithSchema(standardConfig.SchemaName, standardConfig.VersionTableName);
        selectSql = $"SELECT * FROM {tableWithSchema}";

        conn = Context.CreateDbConnection(db);
        reader = await conn.ExecuteReaderAsync(selectSql);
        
        columns = GetColumns(reader);
        TryClose(conn);
        columns.Should().HaveCountGreaterThan(2);
    }

    public static TheoryData<string> VersionTableWithDifferentCasings()
    {
        var def = GrateConfiguration.Default;
        return new TheoryData<string>
        {
            def.VersionTableName,
            def.VersionTableName.ToLower()
        };
    }

    private async Task RunMigration(IGrateMigrator migrator)
    {
        var config = migrator.Configuration;
        CreateDummySql(config.SqlFilesDirectory, config.Folders![KnownFolderKeys.Up]);
        await migrator.Migrate();
    }

    private static List<string> GetColumns(IDataReader reader)
    {
        var columns = Enumerable.Range(0, reader.FieldCount)
            .Select(reader.GetName)
            .ToList();
        return columns;
    }
    
    private static void TryClose(IDbConnection conn)
    {
        try
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
    
}


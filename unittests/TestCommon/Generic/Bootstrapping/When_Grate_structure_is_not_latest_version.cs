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
public abstract class When_Grate_structure_is_not_latest_version(IGrateTestContext context, ITestOutputHelper testOutput)
    : MigrationsScriptsBase(context, testOutput)
{

    [Fact]
    public virtual async Task The_latest_version_is_applied()
    {
        var db = TestConfig.RandomDatabase();
        var parent = CreateRandomTempDirectory();
        
        // This will create the version table without the status column
           
        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
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
        
        // Run the migration
        await using (var migrator = Context.Migrator.WithConfiguration(config))
        {
            await RunMigration(migrator);
        }
        
        // Check that the status column has been added
        conn = Context.CreateDbConnection(db);
        reader = await conn.ExecuteReaderAsync(selectSql);
        
        // Not all databases are case-sensitive, so we can't guarantee the case of the table name
        columns = GetColumns(reader).Select(column => column.ToUpper());
        TryClose(conn);
        columns.Should().Contain("status".ToUpper());
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


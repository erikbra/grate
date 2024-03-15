using System.Data;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using TestCommon.Generic.Running_MigrationScripts;
using TestCommon.TestInfrastructure;
using Xunit.Abstractions;

namespace TestCommon.Generic.Bootstrapping;

// ReSharper disable once UnusedType.Global
public class When_Grate_structure_is_not_latest_version(IGrateTestContext context, ITestOutputHelper testOutput)
    : MigrationsScriptsBase(context, testOutput)
{
    [Fact]
    public async Task The_latest_version_is_applied()
    {
        var db = TestConfig.RandomDatabase();
        var parent = CreateRandomTempDirectory();

        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(FoldersConfiguration.Default())
            .WithSqlFilesDirectory(parent)
            .Build();

        await using (var migrator = Context.Migrator.WithConfiguration(config))
        {
            // This will create the version table
            await RunMigration(migrator);
        }
        
        var tableWithSchema = Context.Syntax.TableWithSchema(config.SchemaName, config.VersionTableName);
        
        // Check that the status column is there    
        var selectSql = $"SELECT * FROM {tableWithSchema}";

        var conn = Context.CreateDbConnection(db);
        var reader = await conn.ExecuteReaderAsync(selectSql);
        var columns = GetColumns(reader);
        TryClose(conn);
        columns.Should().Contain("status");

        // Remove the status column from the table 
        var sql = $"ALTER TABLE {tableWithSchema} DROP COLUMN status";
        
        conn = Context.CreateDbConnection(db);
        await conn.ExecuteAsync(sql);
        
        // Check that the status column has been removed
        conn = Context.CreateDbConnection(db);
        reader = await conn.ExecuteReaderAsync(selectSql);
        columns = GetColumns(reader);
        TryClose(conn);
        columns.Should().NotContain("status");
        
        await using (var migrator = Context.Migrator.WithConfiguration(config))
        {
            // Run the migration again
            await RunMigration(migrator);
        }
        
        // Check that the status column has been added back
        conn = Context.CreateDbConnection(db);
        reader = await conn.ExecuteReaderAsync(selectSql);
        columns = GetColumns(reader);
        TryClose(conn);
        columns.Should().Contain("status");
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


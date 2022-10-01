using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Exceptions;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Generic;

[TestFixture]
public abstract class GenericMigrationTables
{
    protected abstract IGrateTestContext Context { get; }
        
    [TestCase("ScriptsRun")]
    [TestCase("ScriptsRunErrors")]
    [TestCase("Version")]
    public async Task Is_created_if_it_does_not_exist(string tableName)
    {
        var db = "MonoBonoJono";
        var fullTableName = Context.Syntax.TableWithSchema("grate", tableName);

        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        await using (var migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            await migrator.Migrate();
        }

        IEnumerable<string> scripts;
        string sql = $"SELECT modified_date FROM {fullTableName}";
            
        await using (var conn = Context.GetDbConnection(Context.ConnectionString(db)))
        {
            scripts = await conn.QueryAsync<string>(sql);
        }
        scripts.Should().NotBeNull();
    }
        
        
    [TestCase("ScriptsRun")]
    [TestCase("ScriptsRunErrors")]
    [TestCase("Version")]
    public async Task Is_created_even_if_scripts_fail(string tableName)
    {
        var db = "DatabaseWithFailingScripts";
        var fullTableName = Context.Syntax.TableWithSchema("grate", tableName);

        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        CreateInvalidSql(parent, knownFolders[KnownFolderKeys.Up]);

        await using (var migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            try
            {
                await migrator.Migrate();
            }
            catch (MigrationFailed)
            {
            }
        }

        IEnumerable<string> scripts;
        string sql = $"SELECT modified_date FROM {fullTableName}";
            
        await using (var conn = Context.GetDbConnection(Context.ConnectionString(db)))
        {
            scripts = await conn.QueryAsync<string>(sql);
        }
        scripts.Should().NotBeNull();
    }
        
    [TestCase("ScriptsRun")]
    [TestCase("ScriptsRunErrors")]
    [TestCase("Version")]
    public async Task Migration_does_not_fail_if_table_already_exists(string tableName)
    {
        var db = "MonoBonoJono";

        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
            
        await using (var migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            await migrator.Migrate();
        }
            
        // Run migration again - make sure it does not throw an exception
        await using (var migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            Assert.DoesNotThrowAsync(() => migrator.Migrate());
        }
    }
    
    [TestCase("version")]
    [TestCase("vErSiON")]
    public async Task Does_not_create_Version_table_if_it_exists_with_another_casing(string existingTable)
    {
        await CheckTableCasing("Version", existingTable, (config, name) => config.VersionTableName = name);
    }
    
    [TestCase("scriptsrun")]
    [TestCase("SCRiptSrUN")]
    public async Task Does_not_create_ScriptsRun_table_if_it_exists_with_another_casing(string existingTable)
    {
        await CheckTableCasing("ScriptsRun", existingTable, (config, name) => config.ScriptsRunTableName = name);
    }
    
    [TestCase("scriptsrunerrors")]
    [TestCase("ScripTSRunErrors")]
    public async Task Does_not_create_ScriptsRunErrors_table_if_it_exists_with_another_casing(string existingTable)
    {
        await CheckTableCasing("ScriptsRunErrors", existingTable, (config, name) => config.ScriptsRunErrorsTableName = name);
    }
    
    protected virtual async Task CheckTableCasing(string tableName, string funnyCasing, Action<GrateConfiguration, string> setTableName)
    {
        var db = TestConfig.RandomDatabase();

        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default();

        // Set the version table name to be lower-case first, and run one migration.
        var config = Context.GetConfiguration(db, parent, knownFolders);
        
        setTableName(config, funnyCasing);
            
        await using (var migrator = Context.GetMigrator(config))
        {
            await migrator.Migrate();
        }

        // Check that the table is indeed created with lower-case
        var errorCaseCountAfterFirstMigration = await TableCountIn(db, funnyCasing);
        var normalCountAfterFirstMigration = await TableCountIn(db, tableName);
        Assert.Multiple(() =>
        {
            errorCaseCountAfterFirstMigration.Should().Be(1);
            normalCountAfterFirstMigration.Should().Be(0);
        });

        // Run migration again - make sure it does not create the table with different casing too
        setTableName(config, tableName);
        await using (var migrator = Context.GetMigrator(config))
        {
            await migrator.Migrate();
        }
        
        var errorCaseCountAfterSecondMigration = await TableCountIn(db, funnyCasing);
        var normalCountAfterSecondMigration = await TableCountIn(db, tableName);
        Assert.Multiple(() =>
        {
            errorCaseCountAfterSecondMigration.Should().Be(1);
            normalCountAfterSecondMigration.Should().Be(0);
        });
        
    }

    private async Task<int> TableCountIn(string db, string tableName)
    {
        var schemaName = Context.DefaultConfiguration.SchemaName;
        var supportsSchemas = Context.DatabaseMigrator.SupportsSchemas;
        
        var fullTableName = supportsSchemas ? tableName : Context.Syntax.TableWithSchema(schemaName, tableName);
        var tableSchema = supportsSchemas ? schemaName : db;
        
        int count;
        string countSql = CountTableSql(tableSchema, fullTableName);

        await using (var conn = Context.GetDbConnection(Context.ConnectionString(db)))
        {
            count = await conn.ExecuteScalarAsync<int>(countSql);
        }
        
        return count;
    }

    [Test()]
    public async Task Inserts_version_in_version_table()
    {
        var db = "BooYaTribe";

        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
            
        await using (var migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            await migrator.Migrate();
        }

        IEnumerable<(string version, string status)> entries;
        string sql = $"SELECT version, status FROM {Context.Syntax.TableWithSchema("grate", "Version")}";

        await using (var conn = Context.GetDbConnection(Context.ConnectionString(db)))
        {
            entries = await conn.QueryAsync<(string version, string status)>(sql);
        }

        var versions = entries.ToList();
        versions.Should().HaveCount(1);
        var version = versions.Single();
        version.version.Should().Be("a.b.c.d");
        // Validate the version is finished after running without errors
        version.status.Should().Be(MigrationStatus.Finished);
    }
      
    private static void CreateInvalidSql(DirectoryInfo root, MigrationsFolder? folder)
    {
        var dummySql = "SELECT TOP";
        var path = MakeSurePathExists(root, folder);
        WriteSql(path, "2_failing.sql", dummySql);
    }

    private static void WriteSql(DirectoryInfo path, string filename, string? sql)
    {
        File.WriteAllText(Path.Combine(path.ToString(), filename), sql);
    }

    private static DirectoryInfo MakeSurePathExists(DirectoryInfo root, MigrationsFolder? folder)
        => MakeSurePathExists(Wrap(root, folder?.Path));
        
    protected static DirectoryInfo MakeSurePathExists(DirectoryInfo? path)
    {
        ArgumentNullException.ThrowIfNull(path);
        if (!path.Exists)
        {
            path.Create();
        }
        return path;
    }

    private static DirectoryInfo Wrap(DirectoryInfo root, string? relativePath) =>
        new(Path.Combine(root.ToString(), relativePath ?? ""));

    protected virtual string CountTableSql(string schemaName, string tableName)
    {
        return $@"
SELECT count(table_name) FROM information_schema.tables 
WHERE 
table_schema = '{schemaName}' AND
table_name = '{tableName}'
";
    }

}

using System;
using System.Collections.Generic;
using System.Data.Common;
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
            
        var knownFolders = KnownFolders.In(TestConfig.CreateRandomTempDirectory());

        await using (var migrator = Context.GetMigrator(db, knownFolders))
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
            
        var knownFolders = KnownFolders.In(TestConfig.CreateRandomTempDirectory());
        CreateInvalidSql(knownFolders.Up);

        await using (var migrator = Context.GetMigrator(db, knownFolders))
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

        var knownFolders = KnownFolders.In(TestConfig.CreateRandomTempDirectory());
            
        await using (var migrator = Context.GetMigrator(db, knownFolders))
        {
            await migrator.Migrate();
        }
            
        // Run migration again - make sure it does not throw an exception
        await using (var migrator = Context.GetMigrator(db, knownFolders))
        {
            Assert.DoesNotThrowAsync(() => migrator.Migrate());
        }
    }
        
    [Test()]
    public async Task Inserts_version_in_version_table()
    {
        var db = "BooYaTribe";

        var knownFolders = KnownFolders.In(TestConfig.CreateRandomTempDirectory());
            
        await using (var migrator = Context.GetMigrator(db, knownFolders))
        {
            await migrator.Migrate();
        }

        IEnumerable<string> entries;
        var fullTableName = Context.Syntax.TableWithSchema("grate", "Version");
        string sql = $"SELECT version FROM {fullTableName}";
            
        await using (var conn = Context.GetDbConnection(Context.ConnectionString(db)))
        {
            entries = await conn.QueryAsync<string>(sql);
        }

        var versions = entries.ToList();
        versions.Should().HaveCount(1);
        versions.FirstOrDefault().Should().Be("a.b.c.d");
    }
      
    private static void CreateInvalidSql(MigrationsFolder? folder)
    {
        var dummySql = "SELECT TOP";
        var path = MakeSurePathExists(folder);
        WriteSql(path, "2_failing.sql", dummySql);
    }

    private static void WriteSql(DirectoryInfo path, string filename, string? sql)
    {
        File.WriteAllText(Path.Combine(path.ToString(), filename), sql);
    }

    private static DirectoryInfo MakeSurePathExists(MigrationsFolder? folder)
        => MakeSurePathExists(folder?.Path);
        
    protected static DirectoryInfo MakeSurePathExists(DirectoryInfo? path)
    {
        ArgumentNullException.ThrowIfNull(path);
        if (!path.Exists)
        {
            path.Create();
        }
        return path;
    }

        
}

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using FluentAssertions.Execution;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Generic.Running_MigrationScripts;

[TestFixture]
// ReSharper disable once InconsistentNaming
public abstract class Order_Of_Scripts: MigrationsScriptsBase
{
    [Test()]
    public async Task Is_as_expected()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;
        await using (migrator = GetMigrator(db, true))
        {
            await migrator.Migrate();
        }

        string[] scripts;
        string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";
            
        await using (var conn = Context.CreateDbConnection(db))
        {
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        var expectation = new[]
        {
            "1_beforemigration.sql",
            "1_alterdatabase.sql",
            "1_aftercreate.sql",
            "1_beforeup.sql",
            "1_up.sql",
            "1_firstafterup.sql",
            "1_functions.sql",
            "1_views.sql",
            "1_sprocs.sql",
            "1_triggers.sql",
            "1_indexes.sql",
            "1_afterotherany.sql",
            "1_permissions.sql",
            "1_aftermigration.sql",
        };
            
        scripts.Should().BeEquivalentTo(expectation);
        scripts.Should().HaveCount(14);

        using (new AssertionScope())
        {
            for (int i = 0; i < expectation.Length; i++)
            {
                scripts[i].Should().Be(expectation[i]);
            }
        }
    }


    private GrateMigrator GetMigrator(string databaseName, bool createDatabase)
    {
        var dummyFile = Path.GetTempFileName();
        File.Delete(dummyFile);

        var scriptsDir = Directory.CreateDirectory(dummyFile);
            
        var config = Context.DefaultConfiguration with
        {
            CreateDatabase = createDatabase, 
            ConnectionString = Context.ConnectionString(databaseName),
            KnownFolders = KnownFolders.In(scriptsDir),
            Transaction = true
        };

        CreateDummySql(config.KnownFolders.AfterMigration, "1_aftermigration.sql");
        CreateDummySql(config.KnownFolders.AlterDatabase, "1_alterdatabase.sql");
        CreateDummySql(config.KnownFolders.BeforeMigration, "1_beforemigration.sql");
        CreateDummySql(config.KnownFolders.Functions, "1_functions.sql");
        CreateDummySql(config.KnownFolders.Indexes, "1_indexes.sql");
        CreateDummySql(config.KnownFolders.Permissions, "1_permissions.sql");
        CreateDummySql(config.KnownFolders.RunAfterCreateDatabase, "1_aftercreate.sql");
        CreateDummySql(config.KnownFolders.RunAfterOtherAnyTimeScripts, "1_afterotherany.sql");
        CreateDummySql(config.KnownFolders.RunBeforeUp, "1_beforeup.sql");
        CreateDummySql(config.KnownFolders.RunFirstAfterUp, "1_firstafterup.sql");
        CreateDummySql(config.KnownFolders.Sprocs, "1_sprocs.sql");
        CreateDummySql(config.KnownFolders.Triggers, "1_triggers.sql");
        CreateDummySql(config.KnownFolders.Up, "1_up.sql");
        CreateDummySql(config.KnownFolders.Views, "1_views.sql");

        return Context.GetMigrator(config);

    }
}

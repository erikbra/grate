using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Exceptions;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;
using static grate.Configuration.KnownFolderKeys;

namespace grate.unittests.Generic.Running_MigrationScripts;

[TestFixture]
// ReSharper disable once InconsistentNaming
public abstract class Failing_Scripts : MigrationsScriptsBase
{
    protected abstract string ExpectedErrorMessageForInvalidSql { get; }

    [Test]
    public async Task Aborts_the_run_giving_an_error_message()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        CreateInvalidSql(parent, knownFolders[Up]);

        await using (migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            var ex = Assert.ThrowsAsync<MigrationFailed>(migrator.Migrate);
            ex?.Message.Should().Be($"Migration failed due to errors ({ExpectedErrorMessageForInvalidSql})");
        }
    }

    [Test]
    public async Task Inserts_Failed_Scripts_Into_ScriptRunErrors_Table()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        CreateInvalidSql(parent, knownFolders[Up]);

        await using (migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            try
            {
                await migrator.Migrate();
            }
            catch (MigrationFailed)
            {
            }
        }

        string[] scripts;
        string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRunErrors")}";

        using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        {
            await using var conn = Context.CreateDbConnection(db);
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.Should().HaveCount(1);
    }
    
    [Test]
    public void Ensure_Command_Timeout_Fires()
    {
        var sql = Context.Sql.SleepTwoSeconds;

        if (sql == default)
        {
            Assert.Ignore("DBMS doesn't support sleep() for testing");
        }

        var db = TestConfig.RandomDatabase();
        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        var path = MakeSurePathExists(parent, knownFolders[Up]);
        WriteSql(path, "goodnight.sql", sql);

        // run it with a timeout shorter than the 1 second sleep, should timeout
        var config = Context.GetConfiguration(db, parent, knownFolders) with
        {
            CommandTimeout = 1, // shorter than the script runs for
        };

        Assert.CatchAsync(async () =>
        {
            await using var migrator = Context.GetMigrator(config);
            await migrator.Migrate();
            Assert.Fail("Should have thrown a timeout exception prior to this!");
        });
    }

    [Test]
    public void Ensure_AdminCommand_Timeout_Fires()
    {
        var sql = Context.Sql.SleepTwoSeconds;

        if (sql == default)
        {
            Assert.Ignore("DBMS doesn't support sleep() for testing");
        }

        var db = TestConfig.RandomDatabase();
        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        var path = MakeSurePathExists(parent, knownFolders[AlterDatabase]); //so it's run on the admin connection
        WriteSql(path, "goodnight.sql", sql);

        // run it with a timeout shorter than the 1 second sleep, should timeout
        var config = Context.GetConfiguration(db, parent, knownFolders) with
        {
            AdminCommandTimeout = 1, // shorter than the script runs for
        };

        Assert.CatchAsync(async () =>
        {
            await using var migrator = Context.GetMigrator(config);
            await migrator.Migrate();
            Assert.Fail("Should have thrown a timeout exception prior to this!");
        });
    }

    [Test]
    [TestCaseSource(nameof(ShouldStillBeRunOnRollback))]
    public virtual async Task Still_Runs_The_Scripts_In(MigrationsFolder folder)
    {
        var filename = folder.Name + "_jalla1.sql";
        var scripts = await RunMigration(folder, filename);
        scripts.Should().Contain(filename);
    }

    [Test]
    [TestCaseSource(nameof(ShouldNotBeRunOnRollback))]
    public virtual async Task Rolls_Back_Runs_Of_Scripts_In(MigrationsFolder folder)
    {
        if (!Context.SupportsTransaction)
        {
            Assert.Ignore("DDL transactions not supported, skipping tests");
        }
        
        var filename = folder.Name + "_jalla1.sql";
        var scripts = await RunMigration(folder, filename);
        scripts.Should().NotContain(filename);
    }


    private async Task<string[]> RunMigration(MigrationsFolder folder, string filename)
    {
        string[] scripts;

        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var root = CreateRandomTempDirectory();

        var knownFolders = Folders;
        CreateDummySql(root, folder, filename);
        CreateInvalidSql(root, knownFolders[Up]);

        await using (migrator = Context.GetMigrator(db, root, knownFolders, true))
        {
            try
            {
                await migrator.Migrate();
            }
            catch (MigrationFailed)
            {
            }
        }

        string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

        await using (var conn = Context.CreateDbConnection(db))
        {
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        return scripts;
    }

    protected static void CreateInvalidSql(DirectoryInfo root, MigrationsFolder? folder)
    {
        var dummySql = "SELECT TOP";
        var path = MakeSurePathExists(root, folder);
        WriteSql(path, "2_failing.sql", dummySql);
    }

    private static readonly DirectoryInfo Root = TestConfig.CreateRandomTempDirectory();
    private static readonly IFoldersConfiguration Folders = FoldersConfiguration.Default(null);

    private static readonly object?[] ShouldStillBeRunOnRollback =
    {
        GetTestCase(Folders[BeforeMigration]), GetTestCase(Folders[AlterDatabase]), GetTestCase(Folders[Permissions]),
        GetTestCase(Folders[AfterMigration]),
    };

    private static readonly object?[] ShouldNotBeRunOnRollback =
    {
        GetTestCase(Folders[RunAfterCreateDatabase]), GetTestCase(Folders[RunBeforeUp]), GetTestCase(Folders[Up]),
        GetTestCase(Folders[RunFirstAfterUp]), GetTestCase(Folders[Functions]), GetTestCase(Folders[Views]),
        GetTestCase(Folders[Sprocs]), GetTestCase(Folders[Triggers]), GetTestCase(Folders[Indexes]),
        GetTestCase(Folders[RunAfterOtherAnyTimeScripts]),
    };

    private static TestCaseData GetTestCase(
        MigrationsFolder? folder,
        [CallerArgumentExpression("folder")] string migrationsFolderDefinitionName = ""
    ) =>
        new TestCaseData(folder)
            .SetArgDisplayNames(
                migrationsFolderDefinitionName
            );
}

using System.Transactions;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Exceptions;
using grate.Migration;
using TestCommon.TestInfrastructure;
using static grate.Configuration.KnownFolderKeys;
using static TestCommon.TestInfrastructure.DescriptiveTestObjects;

namespace TestCommon.Generic.Running_MigrationScripts;


// ReSharper disable once InconsistentNaming
public abstract class Failing_Scripts : MigrationsScriptsBase
{
    protected abstract string ExpectedErrorMessageForInvalidSql { get; }

    [Fact]
    public async Task Aborts_the_run_giving_an_error_message()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        CreateInvalidSql(parent, knownFolders[Up]);

        await using (migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            var ex = await Assert.ThrowsAsync<MigrationFailed>(migrator.Migrate);
            ex?.Message.Should().Be($"Migration failed due to errors:\n * {ExpectedErrorMessageForInvalidSql}");
        }
    }

    [Fact]
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

    [Fact]
    public async Task Inserts_Large_Failed_Scripts_Into_ScriptRunErrors_Table()
    {
        var db = TestConfig.RandomDatabase();

        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        GrateMigrator? migrator;

        CreateLongInvalidSql(parent, knownFolders[Up]);

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

        string fileContent = await File.ReadAllTextAsync(Path.Combine(parent.ToString(), knownFolders[Up]!.Path, "2_failing.sql"));

        string[] scripts;
        string sql = $"SELECT text_of_script FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRunErrors")}";

        using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        {
            await using var conn = Context.CreateDbConnection(db);
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.First().Should().Be(fileContent);
    }

    [Fact]
    public void Ensure_Command_Timeout_Fires()
    {
        var sql = Context.Sql.SleepTwoSeconds;

        if (sql == default)
        {
            TestOutput.WriteLine("DBMS doesn't support sleep() for testing");
            return;
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

        var exception = Record.ExceptionAsync(async () =>
        {
            await using var migrator = Context.GetMigrator(config);
            await migrator.Migrate();
            Assert.Fail("Should have thrown a timeout exception prior to this!");
        });

        exception.Should().NotBeNull();
    }

    [Fact]
    public void Ensure_AdminCommand_Timeout_Fires()
    {
        var sql = Context.Sql.SleepTwoSeconds;

        if (sql == default)
        {
            TestOutput.WriteLine("DBMS doesn't support sleep() for testing");
            return;
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

        var exception = Record.ExceptionAsync(async () =>
        {
            await using var migrator = Context.GetMigrator(config);
            await migrator.Migrate();
            Assert.Fail("Should have thrown a timeout exception prior to this!");
        });
        
        exception.Should().NotBeNull();
    }

    [Theory]
    [MemberData(nameof(ShouldStillBeRunOnRollback))]
    public virtual async Task Still_Runs_The_Scripts_In(MigrationsFolder folder)
    {
        var filename = folder.Name + "_jalla1.sql";
        var scripts = await RunMigration(folder, filename);
        scripts.Should().Contain(filename);
    }

    [Theory]
    [MemberData(nameof(ShouldNotBeRunOnRollback))]
    public virtual async Task Rolls_Back_Runs_Of_Scripts_In(MigrationsFolder folder)
    {
        if (!Context.SupportsTransaction)
        {
            TestOutput.WriteLine("DDL transactions not supported, skipping tests");
            return;
        }

        var filename = folder.Name + "_jalla1.sql";
        var scripts = await RunMigration(folder, filename);
        scripts.Should().NotContain(filename);
    }

    [Fact]
    public async Task Create_a_version_in_error_if_ran_without_transaction()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        CreateInvalidSql(parent, knownFolders[Up]);

        var config = Context.GetConfiguration(db, parent, knownFolders) with
        {
            Transaction = false
        };

        await using (migrator = Context.GetMigrator(config))
        {
            try
            {
                await migrator.Migrate();
            }
            catch (MigrationFailed)
            {
            }
        }


        string[] versions;
        string sql = $"SELECT status FROM {Context.Syntax.TableWithSchema("grate", "Version")}";

        using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        {
            await using var conn = Context.CreateDbConnection(db);
            versions = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        versions.Should().HaveCount(1);
        versions.Single().Should().Be(MigrationStatus.Error);
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

    protected void CreateLongInvalidSql(DirectoryInfo root, MigrationsFolder? folder)
    {
        var dummySql = CreateLongComment(8192) + Environment.NewLine + "SELECT TOP";
        var path = MakeSurePathExists(root, folder);
        WriteSql(path, "2_failing.sql", dummySql);
    }

    //private static readonly DirectoryInfo Root = TestConfig.CreateRandomTempDirectory();
    private static readonly IFoldersConfiguration Folders = FoldersConfiguration.Default(null);

    public static TheoryData<MigrationsFolderWithDescription> ShouldStillBeRunOnRollback() =>
        new()
        {
            { Describe(Folders[BeforeMigration])! },
            { Describe(Folders[AlterDatabase])! },
            { Describe(Folders[Permissions])! },
            { Describe(Folders[AfterMigration])! }
        };

    public static TheoryData<MigrationsFolderWithDescription> ShouldNotBeRunOnRollback() =>
        new()
        {
            { Describe(Folders[RunAfterCreateDatabase])! },
            { Describe(Folders[RunBeforeUp])! },
            { Describe(Folders[Up])! },
            { Describe(Folders[RunFirstAfterUp])! },
            { Describe(Folders[Functions])! },
            { Describe(Folders[Views])! },
            { Describe(Folders[Sprocs])! },
            { Describe(Folders[Triggers])! },
            { Describe(Folders[Indexes])! },
            { Describe(Folders[RunAfterOtherAnyTimeScripts])! }
        };

}

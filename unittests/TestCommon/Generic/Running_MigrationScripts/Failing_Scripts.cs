using System.Text.Json;
using System.Transactions;
using Dapper;
using FluentAssertions;
using FluentAssertions.Execution;
using grate.Configuration;
using grate.Exceptions;
using grate.Migration;
using TestCommon.TestInfrastructure;
using Xunit.Abstractions;
using Xunit.Sdk;
using static grate.Configuration.KnownFolderKeys;
using static TestCommon.TestInfrastructure.DescriptiveTestObjects;

namespace TestCommon.Generic.Running_MigrationScripts;


// ReSharper disable once InconsistentNaming
public abstract class Failing_Scripts(IGrateTestContext context, ITestOutputHelper testOutput) 
    : MigrationsScriptsBase(context, testOutput)
{
    protected Failing_Scripts(): this(null!, null!)
    {
    }
    
    protected abstract string ExpectedErrorMessageForInvalidSql { get; }

    [Fact]
    public virtual async Task Aborts_the_run_giving_an_error_message()
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = global::grate.Configuration.Folders.Default;
        CreateInvalidSql(parent, knownFolders[Up]);

        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithSqlFilesDirectory(parent)
            .Build();

        await using var migrator = Context.Migrator.WithConfiguration(config);
        var ex = await Assert.ThrowsAnyAsync<MigrationFailed>(migrator.Migrate);
        ex.Message.Should().Be($"Migration failed due to the following errors:\n\n{ExpectedErrorMessageForInvalidSql}");
    }
    
        
    [Fact]
    public virtual async Task Exception_includes_details_on_the_failed_script()
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default();
        CreateInvalidSql(parent, knownFolders[Up]);

        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithSqlFilesDirectory(parent)
            .Build();

        await using var migrator = Context.Migrator.WithConfiguration(config);
        var ex = await Assert.ThrowsAnyAsync<MigrationFailed>(migrator.Migrate);

        try
        {
            using (new AssertionScope())
            {
                ex.MigrationErrors.Should().Contain(ExpectedErrorDetails);
            }
        } catch (XunitException)
        {
            // Write the expected and actual error details to output, to be able to compare them in the test output
            TestOutput.WriteLine("Expected error details: " + JsonSerializer.Serialize(ExpectedErrorDetails));
            TestOutput.WriteLine("Actual migration error details: " + JsonSerializer.Serialize(ex.MigrationErrors));
            TestOutput.WriteLine("Properties of inner exception: " +
                                 JsonSerializer.Serialize(ex.InnerException!.GetType().GetProperties().Select(prop => prop.Name)));
            throw;
        }
        
    }
    
    // TODO: Improve this test to throw both transient and non-transient exceptions, and check the result
    [Fact]
    public virtual async Task Exception_is_set_to_transient_based_on_inner_exceptions()
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default();
        CreateInvalidSql(parent, knownFolders[Up]);

        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithSqlFilesDirectory(parent)
            .Build();

        await using var migrator = Context.Migrator.WithConfiguration(config);
        var ex = await Assert.ThrowsAnyAsync<MigrationFailed>(migrator.Migrate);
        ex.IsTransient.Should().BeFalse();
    }

    protected abstract IDictionary<string, object?> ExpectedErrorDetails { get; }


    [Fact]
    public async Task Inserts_Failed_Scripts_Into_ScriptRunErrors_Table()
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = global::grate.Configuration.Folders.Default;
        CreateInvalidSql(parent, knownFolders[Up]);

        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithSqlFilesDirectory(parent)
            .Build();

        await using (var migrator = Context.Migrator.WithConfiguration(config))
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
            using var conn = Context.CreateDbConnection(db);
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task Inserts_RepositoryPath_Into_ScriptRunErrors_Table()
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = global::grate.Configuration.Folders.Default;
        CreateInvalidSql(parent, knownFolders[Up]);
        
        var repositoryPath = "https://github.com/blah/blah.git";

        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithSqlFilesDirectory(parent)
            .WithRepositoryPath(repositoryPath)
            .Build();

        await using (var migrator = Context.Migrator.WithConfiguration(config))
        {
            try
            {
                await migrator.Migrate();
            }
            catch (MigrationFailed)
            {
            }
        }

        string? loggedRepositoryPath;
        string sql = $"SELECT repository_path FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRunErrors")}";

        using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        {
            using var conn = Context.CreateDbConnection(db);
            loggedRepositoryPath = (await conn.QuerySingleOrDefaultAsync<string>(sql));
        }

        loggedRepositoryPath.Should().Be(repositoryPath);
    }
    

    [Fact]
    public async Task Inserts_Large_Failed_Scripts_Into_ScriptRunErrors_Table()
    {
        var db = TestConfig.RandomDatabase();

        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = global::grate.Configuration.Folders.Default;

        CreateLongInvalidSql(parent, knownFolders[Up]);

        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithSqlFilesDirectory(parent)
            .Build();

        await using (var migrator = Context.Migrator.WithConfiguration(config))
        {
            await Assert.ThrowsAsync<MigrationFailed>(() => migrator.Migrate());
        }

        string fileContent = await File.ReadAllTextAsync(Path.Combine(parent.ToString(), knownFolders[Up]!.Path, "2_failing.sql"));

        string[] scripts;
        string sql = $"SELECT text_of_script FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRunErrors")}";

        using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        {
            using var conn = Context.CreateDbConnection(db);
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.First().Should().Be(fileContent);
    }

    [Fact]
    public async Task Ensure_Command_Timeout_Fires()
    {
        var sql = Context.Sql.SleepTwoSeconds;

        if (sql == default)
        {
            TestOutput.WriteLine("DBMS doesn't support sleep() for testing");
            return;
        }

        var db = TestConfig.RandomDatabase();
        var parent = CreateRandomTempDirectory();
        var knownFolders = global::grate.Configuration.Folders.Default;
        var path = MakeSurePathExists(parent, knownFolders[Up]);
        WriteSql(path, "goodnight.sql", sql);
        
        // run it with a timeout shorter than the 1 second sleep, should timeout
        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithSqlFilesDirectory(parent)
            .CommandTimeout(1) // shorter than the script runs for
            .Build();

        await using var migrator = Context.Migrator.WithConfiguration(config);

        // For some reason, the Assert.ThrowAnyAsync<MigrationFailed> fails, and
        // says that we get the Sql Exception instead of the MigratorFailed exception,
        // but when we assert that we get any exception, and _then_ check on the type of the exception,
        // it works. I don't know why, but I'm leaving it like this for now.
        //var ex = await Assert.ThrowsAnyAsync<MigrationFailed>(migrator.Migrate);
        var ex = await Assert.ThrowsAnyAsync<Exception>(migrator.Migrate);
        ex.Should().BeOfType<MigrationFailed>();
    }

    [Fact]
    public async Task Ensure_AdminCommand_Timeout_Fires()
    {
        var sql = Context.Sql.SleepTwoSeconds;

        if (sql is null)
        {
            TestOutput.WriteLine("DBMS doesn't support sleep() for testing");
            return;
        }

        var db = TestConfig.RandomDatabase();
        var parent = CreateRandomTempDirectory();
        var knownFolders = global::grate.Configuration.Folders.Default;
        var path = MakeSurePathExists(parent, knownFolders[AlterDatabase]); //so it's run on the admin connection
        WriteSql(path, "goodnight.sql", sql);
        
        // run it with a timeout shorter than the 1 second sleep, should timeout
        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithSqlFilesDirectory(parent)
            .AdminCommandTimeout(1) // shorter than the script runs for
            .Build();

        await using var migrator = Context.Migrator.WithConfiguration(config);

        // For some reason, the Assert.ThrowAnyAsync<MigrationFailed> fails, and
        // says that we get the Sql Exception instead of the MigratorFailed exception,
        // but when we assert that we get any exception, and _then_ check on the type of the exception,
        // it works. I don't know why, but I'm leaving it like this for now.
        //var ex = await Assert.ThrowsAnyAsync<MigrationFailed>(migrator.Migrate);
        var ex = await Assert.ThrowsAnyAsync<Exception>(migrator.Migrate);
        ex.Should().BeOfType<MigrationFailed>();
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

        IGrateMigrator? migrator;

        var parent = CreateRandomTempDirectory();
        var knownFolders = global::grate.Configuration.Folders.Default;
        CreateInvalidSql(parent, knownFolders[Up]);
        
        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithSqlFilesDirectory(parent)
            .WithTransaction(false) // This is the important bit
            .Build();

        await using (migrator = Context.Migrator.WithConfiguration(config))
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
            using var conn = Context.CreateDbConnection(db);
            versions = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        versions.Should().HaveCount(1);
        versions.Single().Should().Be(MigrationStatus.Error);
    }

    private async Task<string[]> RunMigration(MigrationsFolder folder, string filename)
    {
        string[] scripts;

        var db = TestConfig.RandomDatabase();

        var root = CreateRandomTempDirectory();

        var knownFolders = Folders;
        CreateDummySql(root, folder, filename);
        CreateInvalidSql(root, knownFolders[Up]);
        
        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithTransaction()
            .WithSqlFilesDirectory(root)
            .Build();

        await using (var migrator = Context.Migrator.WithConfiguration(config))
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

        using (var conn = Context.CreateDbConnection(db))
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
    private static readonly IFoldersConfiguration Folders = global::grate.Configuration.Folders.Default;

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

using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TestCommon.TestInfrastructure;

namespace Basic_tests;

public class Migration
{
    private readonly ILogger<GrateMigrator> _logger;

    public Migration()
    {
        _logger = Substitute.For<ILogger<GrateMigrator>>();
    }

    [Fact]
    public async Task Does_not_output_no_sql_run_in_dryrun_mode()
    {
        var dbMigrator = GetDbMigrator(true);
        var migrator = new GrateMigrator(_logger, dbMigrator);
        await migrator.Migrate();
        _logger.DidNotReceive().LogInformation(" No sql run, either an empty folder, or all files run against destination previously.");
    }

    [Fact]
    public async Task Outputs_no_sql_run_in_live_mode()
    {
        var dbMigrator = GetDbMigrator(false);
        var migrator = new GrateMigrator(_logger, dbMigrator);
        await migrator.Migrate();
        _logger.Received().LogInformation(" No sql run, either an empty folder, or all files run against destination previously.");
    }

    protected static DirectoryInfo Wrap(DirectoryInfo root, string? subFolder) =>
        new(Path.Combine(root.ToString(), subFolder ?? ""));

    private static IDbMigrator GetDbMigrator(bool dryRun)
    {
        var dbMigrator = Substitute.For<IDbMigrator>();
        dbMigrator.RunSql(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<MigrationType>(),
            Arg.Any<long>(),
            Arg.Any<GrateEnvironment>(),
            Arg.Any<ConnectionType>(),
            Arg.Any<TransactionHandling>()
        ).ReturnsForAnyArgs(false);
        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default();

        var path = Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path);

        var folder1 = new DirectoryInfo(Path.Combine(path.ToString(), "01_sub", "folder", "long", "way"));
        string filename1 = "01_any_filename.sql";
        TestConfig.WriteContent(folder1, filename1, "Whatever");

        var configuration = new GrateConfiguration()
        {
            NonInteractive = true,
            SqlFilesDirectory = parent,
            DryRun = dryRun
        };
        dbMigrator.Configuration.Returns(configuration);

        return dbMigrator;
    }

}

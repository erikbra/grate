using Basic_tests.Infrastructure;
using FluentAssertions;
using grate.Configuration;
using grate.Infrastructure.FileSystem;
using grate.Migration;
using TestCommon.TestInfrastructure;

namespace Basic_tests;

public class Migration
{
    private readonly MockGrateLogger  _logger;

    public Migration()
    {
        _logger = new MockGrateLogger();
    }

    [Fact]
    public async Task Does_not_output_no_sql_run_in_dryrun_mode()
    {
        var dbMigrator = GetDbMigrator(true);
        var migrator = new GrateMigrator(_logger, dbMigrator, new PhysicalFileSystem());
        await migrator.Migrate();
        _logger.LoggedMessages.Should().NotContain(" No sql run, either an empty folder, or all files run against destination previously.");
    }

    [Fact]
    public async Task Outputs_no_sql_run_in_live_mode()
    {
        var dbMigrator = GetDbMigrator(false);
        var migrator = new GrateMigrator(_logger, dbMigrator, new PhysicalFileSystem());
        await migrator.Migrate();
        _logger.LoggedMessages.Should().Contain(" No sql run, either an empty folder, or all files run against destination previously.");
    }

    private IDirectoryInfo Wrap(IDirectoryInfo root, string? subFolder) =>
        new PhysicalDirectoryInfo(Path.Combine(root.ToString(), subFolder ?? ""));

    private IDbMigrator GetDbMigrator(bool dryRun)
    {
        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default();

        var path = Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path);

        var folder1 = new PhysicalDirectoryInfo(Path.Combine(path.ToString(), "01_sub", "folder", "long", "way"));
        string filename1 = "01_any_filename.sql";
        IFileSystem fileSystem = new PhysicalFileSystem();
        fileSystem.WriteContent(folder1, filename1, "Whatever");

        var configuration = new GrateConfiguration()
        {
            NonInteractive = true,
            SqlFilesDirectory = parent,
            DryRun = dryRun
        };
        var dbMigrator = new MockDbMigrator() { Configuration = configuration };

        return dbMigrator;
    }

}

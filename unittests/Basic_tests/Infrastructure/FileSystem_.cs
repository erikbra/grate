using FluentAssertions;
using grate.Configuration;
using grate.Infrastructure.FileSystem;
using grate.Migration;
using TestCommon.TestInfrastructure;

namespace Basic_tests.Infrastructure;


public class FileSystem_
{
    public FileSystem_()
    {
        this.FileSystem = new PhysicalFileSystem();
    }

    public IFileSystem FileSystem { get; set; }

    [Fact]
    public void Sorts_enumerated_files_on_filename_when_no_subfolders()
    {
        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        var path = Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path);

        string filename1 = "01_any_filename.sql";
        string filename2 = "02_any_filename.sql";

        FileSystem.WriteContent(path, filename1, "Whatever");
        FileSystem.WriteContent(path, filename2, "Whatever");

        var files = FileSystem.GetFiles(path, "*.sql").ToList();

        files.First().FullName.Should().Be(Path.Combine(path.ToString(), filename1));
        files.Last().FullName.Should().Be(Path.Combine(path.ToString(), filename2));
    }

    [Fact]
    public void Sorts_enumerated_files_on_filename_without_extension_when_no_subfolders()
    {
        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        var path = Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path);

        string filename1 = "01_any_filename_and_a_bit_longer.sql";
        string filename2 = "01_any_filename.sql";

        FileSystem.WriteContent(path, filename1, "Whatever");
        FileSystem.WriteContent(path, filename2, "Whatever");

        var files = FileSystem.GetFiles(path, "*.sql").ToList();

        files.First().FullName.Should().Be(Path.Combine(path.ToString(), filename2));
        files.Last().FullName.Should().Be(Path.Combine(path.ToString(), filename1));
    }

    [Fact]
    public void Sorts_enumerated_files_on_sub_path_when_subfolders_are_used()
    {
        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        var path = Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path);

        var folder1 = new PhysicalDirectoryInfo(Path.Combine(path.ToString(), "01_sub", "folder", "long", "way"));
        var folder2 = new PhysicalDirectoryInfo(Path.Combine(path.ToString(), "02_sub", "folder", "long", "way"));

        string filename1 = "01_any_filename.sql";
        string filename2 = "02_any_filename.sql";

        FileSystem.WriteContent(folder1, filename2, "Whatever");
        FileSystem.WriteContent(folder2, filename1, "Whatever");

        var files = FileSystem.GetFiles(path, "*.sql").ToList();

        files.First().FullName.Should().Be(Path.Combine(folder1.ToString(), filename2));
        files.Last().FullName.Should().Be(Path.Combine(folder2.ToString(), filename1));
    }

    [Fact]
    public void Sorts_enumerated_files_on_filename_when_directory_names_are_ignored()
    {
        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        var path = Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path);

        var folder1 = new PhysicalDirectoryInfo(Path.Combine(path.ToString(), "Init"));
        var folder2 = new PhysicalDirectoryInfo(Path.Combine(path.ToString(), "1.0"));

        string filename1 = "01_Schema.sql";
        string filename2 = "02_SomeChanges.sql";

        FileSystem.WriteContent(folder1, filename1, "Whatever");
        FileSystem.WriteContent(folder2, filename2, "Whatever");

        var files = FileSystem.GetFiles(path, "*.sql", true).ToList();

        files.First().FullName.Should().Be(Path.Combine(folder1.ToString(), filename1));
        files.Last().FullName.Should().Be(Path.Combine(folder2.ToString(), filename2));
    }

    protected IDirectoryInfo Wrap(IDirectoryInfo root, string? subFolder) =>
        FileSystem.Wrap(root, subFolder);

}

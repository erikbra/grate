using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace grate.unittests.Basic.Infrastructure;

[TestFixture]
[Category("Basic")]
public class FileSystemEx_
{
    private const string RegexPattern = @"^(\-\-[ \t]*Dependencies[\t ]*:[\t ]*)(([\\*?#\w.]+\.sql[\t ]*)+)$";
    private const string RegexPatternSplitter = @"[\s]+";
    private const string CyclicExceptionErrorMessage1 = "{0} has a cyclic dependency loop on itself!";
    private const string CyclicExceptionErrorMessage2 = "Files {0} & {1} share cyclic dependency loop!";

    private ILogger<GrateMigrator> _logger;
    public FileSystemEx_()
    {
        _logger = new ServiceCollection().AddLogging().BuildServiceProvider()
            .GetService<ILoggerFactory>()
            .CreateLogger<GrateMigrator>();
    }

    [Test]
    public void Sorts_enumerated_files_on_filename_when_no_subfolders_with_undeclared_dependencies()
    {
        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        var path = Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path);

        const string filename1 = "01_any_filename.sql";
        const string filename2 = "02_any_filename.sql";

        TestConfig.WriteContent(path, filename1, @"
-- This file depends on 02_any_filename.sql!
-- Dependencies: 02_any_filename.sql");
        TestConfig.WriteContent(path, filename2, "Whatever");

        var files = FileSystemEx.GetFiles(path, "*.sql", _logger,false, RegexPattern, RegexPatternSplitter).ToList();

        files.First().FullName.Should().Be(Path.Combine(path.ToString(), filename1));
        files.Last().FullName.Should().Be(Path.Combine(path.ToString(), filename2));
    }

    [Test]
    public void Sorts_enumerated_files_on_filename_when_no_subfolders_with_dependencies()
    {
        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        var path = Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path);

        const string filename1 = "01_any_filename.sql";
        const string filename2 = "02_any_filename.sql";

        TestConfig.WriteContent(path, filename1, @"
-- This file depends on 02_any_filename.sql!
-- Dependencies: 02_any_filename.sql");
        TestConfig.WriteContent(path, filename2, "Whatever");

        var files = FileSystemEx.GetFiles(path, "*.sql", _logger, true, RegexPattern, RegexPatternSplitter).ToList();

        files.First().FullName.Should().Be(Path.Combine(path.ToString(), filename2));
        files.Last().FullName.Should().Be(Path.Combine(path.ToString(), filename1));
    }

    [Test]

    public void Fail_with_cyclic_dependencies()
    {
        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        var path = Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path);

        const string filename1 = "01_any_filename.sql";
        const string filename2 = "02_any_filename.sql";

        TestConfig.WriteContent(path, filename1, @"
-- This file depends on 02_any_filename.sql!
-- Dependencies: 02_any_filename.sql");
        TestConfig.WriteContent(path, filename2, @"
-- This file depends on 01_any_filename.sql!
-- Dependencies: 01_any_filename.sql");

        var applicationException = Assert.Throws<ApplicationException>(() =>FileSystemEx.GetFiles(path, "*.sql", _logger, true, RegexPattern, RegexPatternSplitter).ToList());
        Assert.NotNull(applicationException);
        if (applicationException == null) return;
        Assert.That(applicationException.Message, Is.EqualTo(string.Format(CyclicExceptionErrorMessage2, filename2, filename1)));

    }

    [Test]
    public void Fail_with_self_reference_Dependency()
    {
        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        var path = Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path);

        const string filename1 = "01_any_filename.sql";
        const string filename2 = "02_any_filename.sql";
        const string filename3 = "03_any_filename.sql";

        TestConfig.WriteContent(path, filename1, "Whatever");
        TestConfig.WriteContent(path, filename3, "Whatever");

        TestConfig.WriteContent(path, filename2, @"
-- This file depends on itself!
-- Dependencies: 02_any_filename.sql");

        var applicationException = Assert.Throws<ApplicationException>(() =>FileSystemEx.GetFiles(path, "*.sql", _logger, true, RegexPattern, RegexPatternSplitter).ToList());
        Assert.NotNull(applicationException);
        if (applicationException == null) return;
        Assert.That(applicationException.Message, Is.EqualTo(string.Format(CyclicExceptionErrorMessage1, filename2)));

    }
    [Test]
    public void Sorts_enumerated_files_on_filename_when_no_subfolders_with_multiple_dependencies()
    {
        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        var path = Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path);

        const string filename1 = "01_any_filename.sql";
        const string filename2 = "02_any_filename.sql";
        const string filename3 = "03_any_filename.sql";

        TestConfig.WriteContent(path, filename1, @"
-- This file depends on 02_any_filename.sql!
-- Dependencies: 02_any_filename.sql 03_any_filename.sql");
        TestConfig.WriteContent(path, filename2, "Whatever");
        TestConfig.WriteContent(path, filename3, "Whatever");

        var files = FileSystemEx.GetFiles(path, "*.sql", _logger, true, RegexPattern, RegexPatternSplitter).ToList();

        files.First().FullName.Should().Be(Path.Combine(path.ToString(), filename2));
        files[1].FullName.Should().Be(Path.Combine(path.ToString(), filename3));
        files.Last().FullName.Should().Be(Path.Combine(path.ToString(), filename1));
    }

    [Test]
    public void Sorts_enumerated_files_on_sub_path_when_subfolders_are_used_and_dependencies()
    {
        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        var path = Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path);

        var folder1 = new DirectoryInfo(Path.Combine(path.ToString(), "folder", "sub1"));
        var folder2 = new DirectoryInfo(Path.Combine(path.ToString(), "folder", "sub2"));


        const string filename1 = "01_any_filename.sql";
        const string filename2 = "02_any_filename.sql";

        // folder/sub1/01 -> folder/sub1/02 -> folder/sub2/01 -> folder/sub2/02

        TestConfig.WriteContent(folder1, filename1, @"
-- Dependencies: folder\sub1\02_any_filename.sql");
        TestConfig.WriteContent(folder1, filename2, @"
-- Dependencies: folder\sub2\01_any_filename.sql");
        TestConfig.WriteContent(folder2, filename1, @"
-- Dependencies: folder\sub2\02_any_filename.sql");
        TestConfig.WriteContent(folder2, filename2, "Whatever");

        var files = FileSystemEx.GetFiles(path, "*.sql", _logger, true, RegexPattern, RegexPatternSplitter).ToList();

        files.First().FullName.Should().Be(Path.Combine(folder2.ToString(), filename2));
        files[1].FullName.Should().Be(Path.Combine(folder2.ToString(), filename1));
        files[2].FullName.Should().Be(Path.Combine(folder1.ToString(), filename2));
        files.Last().FullName.Should().Be(Path.Combine(folder1.ToString(), filename1));

    }

    [Test]
    public void Sorts_enumerated_files_on_sub_path_when_subfolders_are_used_and_wildcard_dependencies()
    {
        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        var path = Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path);

        var folder1 = new DirectoryInfo(Path.Combine(path.ToString(), "folder", "sub1"));
        var folder2 = new DirectoryInfo(Path.Combine(path.ToString(), "folder", "sub2"));


        const string filename1 = "01_any_filename.sql";
        const string filename2 = "02_any_filename.sql";

        // folder/sub1/01 -> folder/sub1/02, folder/sub2/01 & folder/sub2/02

        TestConfig.WriteContent(folder1, filename1, @"
-- Dependencies: folder\sub1\02_any_filename.sql folder\sub2\0#_any_filename.sql");
        TestConfig.WriteContent(folder1, filename2, "Whatever");
        TestConfig.WriteContent(folder2, filename1, "Whatever");
        TestConfig.WriteContent(folder2, filename2, "Whatever");

        var files = FileSystemEx.GetFiles(path, "*.sql", _logger, true, RegexPattern, RegexPatternSplitter).ToList();

        files.Last().FullName.Should().Be(Path.Combine(folder1.ToString(), filename1));

    }


    [Test]
    public void Fail_with_cyclic_dependencies_and_subfolders()
    {
        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        var path = Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path);

        var folder1 = new DirectoryInfo(Path.Combine(path.ToString(), "folder", "sub1"));
        var folder2 = new DirectoryInfo(Path.Combine(path.ToString(), "folder", "sub2"));

        const string filename1 = "01_any_filename.sql";
        const string filename2 = "02_any_filename.sql";

        TestConfig.WriteContent(folder1, filename1, @"
-- Dependencies: folder\sub1\02_any_filename.sql");
        TestConfig.WriteContent(folder1, filename2, @"
-- Dependencies: folder\sub2\01_any_filename.sql");
        TestConfig.WriteContent(folder2, filename1, @"
-- Dependencies: folder\sub2\02_any_filename.sql");
        TestConfig.WriteContent(folder2, filename2, @"
-- Dependencies: folder\sub1\01_any_filename.sql");

        var applicationException = Assert.Throws<ApplicationException>(() =>FileSystemEx.GetFiles(path, "*.sql", _logger, true, RegexPattern, RegexPatternSplitter).ToList());
        Assert.NotNull(applicationException);
        if (applicationException == null) return;
        Assert.That(applicationException.Message, Is.EqualTo(string.Format(CyclicExceptionErrorMessage2, Path.Join("folder", "sub2", filename2), Path.Join("folder", "sub1", filename1))));

    }

    protected static DirectoryInfo Wrap(DirectoryInfo root, string? subFolder) =>
        new DirectoryInfo(Path.Combine(root.ToString(), subFolder ?? ""));

}

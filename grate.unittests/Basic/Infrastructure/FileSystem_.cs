﻿using System.IO;
using System.Linq;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Basic.Infrastructure;

[TestFixture]
[Category("Basic")]
public class FileSystem_
{
    [Test]
    public void Sorts_enumerated_files_on_filename_when_no_subfolders()
    {
        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        var path = Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path);

        string filename1 = "01_any_filename.sql";
        string filename2 = "02_any_filename.sql";
        
        TestConfig.WriteContent(path, filename1, "Whatever");
        TestConfig.WriteContent(path, filename2, "Whatever");

        var files = FileSystem.GetFiles(path, "*.sql").ToList();

        files.First().FullName.Should().Be(Path.Combine(path.ToString(), filename1));
        files.Last().FullName.Should().Be(Path.Combine(path.ToString(), filename2));
    }
        
    [Test]
    public void Sorts_enumerated_files_on_sub_path_when_subfolders_are_used()
    {
        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        var path = Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path);
        
        var folder1 = new DirectoryInfo(Path.Combine(path.ToString(), "01_sub", "folder", "long", "way"));
        var folder2 = new DirectoryInfo(Path.Combine(path.ToString(), "02_sub", "folder", "long", "way"));
        
        string filename1 = "01_any_filename.sql";
        string filename2 = "02_any_filename.sql";
        
        TestConfig.WriteContent(folder1, filename2, "Whatever");
        TestConfig.WriteContent(folder2, filename1, "Whatever");

        var files = FileSystem.GetFiles(path, "*.sql").ToList();

        files.First().FullName.Should().Be(Path.Combine(folder1.ToString(), filename2));
        files.Last().FullName.Should().Be(Path.Combine(folder2.ToString(), filename1));
    }
    
    protected static DirectoryInfo Wrap(DirectoryInfo root, string? subFolder) =>
        new DirectoryInfo(Path.Combine(root.ToString(), subFolder ?? ""));
    
}

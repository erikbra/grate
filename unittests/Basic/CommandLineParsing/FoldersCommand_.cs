﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using grate.Commands;
using grate.Configuration;
using grate.Migration;
using NUnit.Framework;

namespace Basic.CommandLineParsing;

// ReSharper disable once InconsistentNaming
[TestOf(nameof(FoldersCommand))]
public class FoldersCommand_
{
    [Test]
    [TestCaseSource(nameof(FoldersCommandLines))]
    [TestCaseSource(nameof(FileFoldersCommandLines))]
    public void Can_Parse(string argument, IFoldersConfiguration expected)
    {
        var actual = FoldersCommand.Parse(argument);

        actual.Should().NotBeNull();

        var expectedArr = expected.ToArray();
        var actualArr = actual.ToArray();

        actualArr.Should().HaveCount(expectedArr.Length);
        
        Assert.Multiple(() =>
        {
            for (int i = 0; i < expected.Count; i++)
            {
                var act = actualArr?[i];
                var exp = expectedArr[i];
                
                act?.Key.Should().Be(exp.Key);
                AssertEquivalent(exp.Value, act?.Value);
            }
        });
    }
    
    private static void AssertEquivalent(MigrationsFolder? expected, MigrationsFolder? actual)
    {
        Assert.Multiple(() =>
        {
            actual?.Name.Should().Be(expected?.Name);
            actual?.Path.Should().Be(expected?.Path);
            actual?.Type.Should().Be(expected?.Type);
            actual?.ConnectionType.Should().Be(expected?.ConnectionType);
            actual?.TransactionHandling.Should().Be(expected?.TransactionHandling);
        });
    }

    private static readonly List<(string name, string config, IFoldersConfiguration expected)> TestCases =
        new()
        {
            ("Default, with overrides", 
                "up=blup;afterMigration=æfter",
                FoldersConfiguration.Default(
                    KnownFolderNames.Default with{ Up = "blup", AfterMigration = "æfter"})
                ),
            
            ("Fully customised", 
             "folder1=type:Once;folder2=type:EveryTime;folder3=type:AnyTime",
             new FoldersConfiguration(
                new MigrationsFolder("folder1", MigrationType.Once),
                new MigrationsFolder("folder2", MigrationType.EveryTime),
                new MigrationsFolder("folder3", MigrationType.AnyTime)
             )),
            
            ("Fully customised, only one folder", 
                "folder1=type:Once", 
                new FoldersConfiguration(
                    new MigrationsFolder("folder1", MigrationType.Once)
                )),
            
            ("Fully customised, one folder with standard name",
                "folder1=type:Once;up=tables",
                new FoldersConfiguration(
                    new MigrationsFolder("folder1", MigrationType.Once),
                    new MigrationsFolder("up", "tables", MigrationType.Once)
                )),

            ("Fully customised, only folder names, should have defaults",
                "folder1;up",
                new FoldersConfiguration(
                    new MigrationsFolder("folder1"),
                    new MigrationsFolder("up")
                )),

            ("Fully customised, more properties", 
                "folder1=path:a/sub/folder/here,type:Once,connectionType:Admin;folder2=type:EveryTime;folder3=type:AnyTime", 
                new FoldersConfiguration(
                    new MigrationsFolder("folder1", "a/sub/folder/here", MigrationType.Once, ConnectionType.Admin),
                    new MigrationsFolder("folder2", MigrationType.EveryTime),
                    new MigrationsFolder("folder3", MigrationType.AnyTime)
                )),
            
            ("Fully customised, with newline separators", 
@"folder1=type:Once
folder2=type:EveryTime
folder3=type:AnyTime", 
                new FoldersConfiguration(
                    new MigrationsFolder("folder1", MigrationType.Once),
                    new MigrationsFolder("folder2", MigrationType.EveryTime),
                    new MigrationsFolder("folder3", MigrationType.AnyTime)
                )),
            
            ("Fully customised - With only migration type",
                "folderA=Everytime;folderB=Once;folderC=AnyTime",
                new FoldersConfiguration(
                    new MigrationsFolder("folderA", MigrationType.EveryTime),
                    new MigrationsFolder("folderB", MigrationType.Once),
                    new MigrationsFolder("folderC", MigrationType.AnyTime)
                )),
        
            ("Fully customised - With only folder name",
                "folderA=hello;folderB=you;folderC=fool",
                new FoldersConfiguration(
                    new MigrationsFolder("folderA", "hello", MigrationType.Once),
                    new MigrationsFolder("folderB", "you", MigrationType.Once),
                    new MigrationsFolder("folderC", "fool", MigrationType.Once)
                )),
        };

    private static readonly TestCaseData[] FoldersCommandLines = TestCases.Select(c =>
        GetTestCase("Text - " + c.name, c.config, c.expected)).ToArray();

    private static readonly TestCaseData[] FileFoldersCommandLines =
    new []{
        GetTestCase("File - NonExistent file", "/tmp/this/does/not/exist" , FoldersConfiguration.Empty),
        GetTestCase("File - Empty file", CreateFile(""), FoldersConfiguration.Empty),
    }.Concat(TestCases.Select( c => GetTestCase("File - " + c.name, CreateFile(c.config), c.expected))).ToArray();

    private static string CreateFile(string content)
    {
        var tmpFile = Path.GetTempFileName();
        File.WriteAllText(tmpFile, content);
        return tmpFile;
    }

    private static TestCaseData GetTestCase(
        string testCaseName,
        string folderArg,
        IFoldersConfiguration expected
    ) => new TestCaseData(folderArg, expected).SetArgDisplayNames(testCaseName, folderArg);

    
}

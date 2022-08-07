using System.IO;
using System.Linq;
using FluentAssertions;
using grate.Commands;
using grate.Configuration;
using NUnit.Framework;

namespace grate.unittests.Basic.CommandLineParsing;

// ReSharper disable once InconsistentNaming
public class CustomFoldersCommand_
{
    [Test]
    [TestCaseSource(nameof(FoldersCommandLines))]
    [TestCaseSource(nameof(FileFoldersCommandLines))]
    public void Can_Parse(string argument, IFoldersConfiguration expected)
    {
        var actual = CustomFoldersCommand.Parse(argument);

        actual.Should().NotBeNull();

        var expectedArr = expected.ToArray();
        var actualArr = actual?.ToArray();

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
            actual?.RelativePath?.Should().Be(expected?.RelativePath);
            actual?.Type.Should().Be(expected?.Type);
            actual?.ConnectionType.Should().Be(expected?.ConnectionType);
            actual?.TransactionHandling.Should().Be(expected?.TransactionHandling);
        });
    }

    private static readonly object?[] FoldersCommandLines =
    {
        GetTestCase("Text - Empty", "{}", CustomFoldersConfiguration.Empty),
        GetTestCase("Text - Mostly defaults",
@"{ 
    ""folders"": {
        ""folder1"": { ""type"": ""Once"" },
        ""folder2"": { ""type"": ""EveryTime"" },
        ""folder3"": { ""type"": ""AnyTime"" }
    }
}", 
            new CustomFoldersConfiguration(
                new MigrationsFolder("folder1", MigrationType.Once),
                new MigrationsFolder("folder2", MigrationType.EveryTime),
                new MigrationsFolder("folder3", MigrationType.AnyTime)
                )),
        GetTestCase("Text - With only migration type",
@"{
    ""folders"": {
        ""folderA"": ""Everytime"",
        ""folderB"": ""Once"",
        ""folderC"": ""AnyTime""
    }
}",
            new CustomFoldersConfiguration(
                new MigrationsFolder("folderA", MigrationType.EveryTime),
                new MigrationsFolder("folderB", MigrationType.Once),
                new MigrationsFolder("folderC", MigrationType.AnyTime)
                )),
        
        GetTestCase("Text - Without root - relative folders",
            @"{
    ""folders"": {
        ""folderA"": ""Everytime""
    }
}",
            new CustomFoldersConfiguration(
                new MigrationsFolder("folderA", MigrationType.EveryTime)
            )),
        
    };

    private static readonly object?[] FileFoldersCommandLines =
    {
        GetTestCase("File - NonExistant file", "/tmp/this/does/not/exist" , CustomFoldersConfiguration.Empty),
        GetTestCase("File - Empty file", CreateFile(""), KnownFolders.UnRooted(KnownFolderNames.Default)),
        GetTestCase("File - Empty Json", CreateFile("{}"), CustomFoldersConfiguration.Empty),
        GetTestCase("File - Mostly defaults",
            CreateFile(@"{ 
    ""folders"": {
        ""folder1"": { ""type"": ""Once"" },
        ""folder2"": { ""type"": ""EveryTime"" },
        ""folder3"": { ""type"": ""AnyTime"" }
    }
}"),
            new CustomFoldersConfiguration(
                new MigrationsFolder("folder1", MigrationType.Once),
                new MigrationsFolder("folder2", MigrationType.EveryTime),
                new MigrationsFolder("folder3", MigrationType.AnyTime)
            )),
        GetTestCase("File - With only migration type",
            CreateFile(@"{
    ""folders"": {
        ""folderA"": ""Everytime"",
        ""folderB"": ""Once"",
        ""folderC"": ""AnyTime""
    }
}"),
            new CustomFoldersConfiguration(
                new MigrationsFolder("folderA", MigrationType.EveryTime),
                new MigrationsFolder("folderB", MigrationType.Once),
                new MigrationsFolder("folderC", MigrationType.AnyTime)
            )),
    };

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

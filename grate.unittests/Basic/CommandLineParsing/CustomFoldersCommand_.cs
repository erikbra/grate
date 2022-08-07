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
        GetTestCase("Text - Empty", "{}", FoldersConfiguration.Empty),
        GetTestCase("Text - Mostly defaults",
@"{ 
        ""folder1"": { ""type"": ""Once"" },
        ""folder2"": { ""type"": ""EveryTime"" },
        ""folder3"": { ""type"": ""AnyTime"" }
}", 
            new FoldersConfiguration(
                new MigrationsFolder("folder1", MigrationType.Once),
                new MigrationsFolder("folder2", MigrationType.EveryTime),
                new MigrationsFolder("folder3", MigrationType.AnyTime)
                )),
        GetTestCase("Text - With only migration type",
@"{
        ""folderA"": ""Everytime"",
        ""folderB"": ""Once"",
        ""folderC"": ""AnyTime""
}",
            new FoldersConfiguration(
                new MigrationsFolder("folderA", MigrationType.EveryTime),
                new MigrationsFolder("folderB", MigrationType.Once),
                new MigrationsFolder("folderC", MigrationType.AnyTime)
                )),
        
        GetTestCase("Text - Without root - relative folders",
            @"{
        ""folderA"": ""Everytime""
}",
            new FoldersConfiguration(
                new MigrationsFolder("folderA", MigrationType.EveryTime)
            )),
        
    };

    private static readonly object?[] FileFoldersCommandLines =
    {
        GetTestCase("File - NonExistant file", "/tmp/this/does/not/exist" , FoldersConfiguration.Empty),
        GetTestCase("File - Empty file", CreateFile(""), FoldersConfiguration.Default(KnownFolderNames.Default)),
        GetTestCase("File - Empty Json", CreateFile("{}"), FoldersConfiguration.Empty),
        GetTestCase("File - Mostly defaults",
            CreateFile(@"{ 
        ""folder1"": { ""type"": ""Once"" },
        ""folder2"": { ""type"": ""EveryTime"" },
        ""folder3"": { ""type"": ""AnyTime"" }
}"),
            new FoldersConfiguration(
                new MigrationsFolder("folder1", MigrationType.Once),
                new MigrationsFolder("folder2", MigrationType.EveryTime),
                new MigrationsFolder("folder3", MigrationType.AnyTime)
            )),
        GetTestCase("File - With only migration type",
            CreateFile(@"{
        ""folderA"": ""Everytime"",
        ""folderB"": ""Once"",
        ""folderC"": ""AnyTime""
}"),
            new FoldersConfiguration(
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

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
        actual?.Root?.ToString().Should().Be(expected.Root?.ToString());

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
            actual?.Path?.ToString().Should().Be(expected?.Path?.ToString());
            actual?.Type.Should().Be(expected?.Type);
            actual?.ConnectionType.Should().Be(expected?.ConnectionType);
            actual?.TransactionHandling.Should().Be(expected?.TransactionHandling);
        });
    }

    private static readonly object?[] FoldersCommandLines =
    {
        GetTestCase("Text - Empty", "{}", CustomFoldersConfiguration.Empty),
        GetTestCase("Text - Only Root", "{ \"root\": \"/tmp/jallajalla\" }", new CustomFoldersConfiguration(new DirectoryInfo("/tmp/jallajalla"))),
        GetTestCase("Text - Mostly defaults",
@"{ 
    ""root"": ""/tmp/jalla"",
    ""folders"": {
        ""folder1"": { ""type"": ""Once"" },
        ""folder2"": { ""type"": ""EveryTime"" },
        ""folder3"": { ""type"": ""AnyTime"" }
    }
}", 
            new CustomFoldersConfiguration(
                new DirectoryInfo("/tmp/jalla"),
                new MigrationsFolder(new DirectoryInfo("/tmp/jalla"), "folder1", MigrationType.Once),
                new MigrationsFolder(new DirectoryInfo("/tmp/jalla"), "folder2", MigrationType.EveryTime),
                new MigrationsFolder(new DirectoryInfo("/tmp/jalla"), "folder3", MigrationType.AnyTime)
                )),
        GetTestCase("Text - With only migration type",
@"{
    ""root"": ""/tmp/somewhere"",
    ""folders"": {
        ""folderA"": ""Everytime"",
        ""folderB"": ""Once"",
        ""folderC"": ""AnyTime""
    }
}",
            new CustomFoldersConfiguration(
                new DirectoryInfo("/tmp/somewhere"),
                new MigrationsFolder(new DirectoryInfo("/tmp/somewhere"), "folderA", MigrationType.EveryTime),
                new MigrationsFolder(new DirectoryInfo("/tmp/somewhere"), "folderB", MigrationType.Once),
                new MigrationsFolder(new DirectoryInfo("/tmp/somewhere"), "folderC", MigrationType.AnyTime)
                )),
        
        GetTestCase("Text - Without root - relative folders",
            @"{
    ""folders"": {
        ""folderA"": ""Everytime""
    }
}",
            new CustomFoldersConfiguration(
                null,
                new MigrationsFolder(new DirectoryInfo(Directory.GetCurrentDirectory()), "folderA", MigrationType.EveryTime)
            )),
        
        GetTestCase("Text - Without root - absolute folders",
            @"{
    ""folders"": {
        ""folderA"": { ""path"": ""/tmp/gorilla"", ""type"": ""Everytime"" },
    }
}",
            new CustomFoldersConfiguration(
                null,
                new MigrationsFolder("folderA", new DirectoryInfo("/tmp/gorilla"), MigrationType.EveryTime)
            )),
    };

    private static readonly object?[] FileFoldersCommandLines =
    {
        GetTestCase("File - NonExistant file", "/tmp/this/does/not/exist" , CustomFoldersConfiguration.Empty),
        GetTestCase("File - Empty file", CreateFile(""), CustomFoldersConfiguration.Empty),
        GetTestCase("File - Empty Json", CreateFile("{}"), CustomFoldersConfiguration.Empty),
        GetTestCase("File - Only Root", CreateFile("{ \"root\": \"/tmp/jallajalla\" }"),
            new CustomFoldersConfiguration(new DirectoryInfo("/tmp/jallajalla"))),
        GetTestCase("File - Mostly defaults",
            CreateFile(@"{ 
    ""root"": ""/tmp/jalla"",
    ""folders"": {
        ""folder1"": { ""type"": ""Once"" },
        ""folder2"": { ""type"": ""EveryTime"" },
        ""folder3"": { ""type"": ""AnyTime"" }
    }
}"),
            new CustomFoldersConfiguration(
                new DirectoryInfo("/tmp/jalla"),
                new MigrationsFolder(new DirectoryInfo("/tmp/jalla"), "folder1", MigrationType.Once),
                new MigrationsFolder(new DirectoryInfo("/tmp/jalla"), "folder2", MigrationType.EveryTime),
                new MigrationsFolder(new DirectoryInfo("/tmp/jalla"), "folder3", MigrationType.AnyTime)
            )),
        GetTestCase("File - With only migration type",
            CreateFile(@"{
    ""root"": ""/tmp/somewhere"",
    ""folders"": {
        ""folderA"": ""Everytime"",
        ""folderB"": ""Once"",
        ""folderC"": ""AnyTime""
    }
}"),
            new CustomFoldersConfiguration(
                new DirectoryInfo("/tmp/somewhere"),
                new MigrationsFolder(new DirectoryInfo("/tmp/somewhere"), "folderA", MigrationType.EveryTime),
                new MigrationsFolder(new DirectoryInfo("/tmp/somewhere"), "folderB", MigrationType.Once),
                new MigrationsFolder(new DirectoryInfo("/tmp/somewhere"), "folderC", MigrationType.AnyTime)
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

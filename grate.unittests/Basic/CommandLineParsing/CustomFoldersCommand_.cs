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
    public void Can_Parse(string argument, IFoldersConfiguration expected)
    {
        var actual = CustomFoldersCommand.Parse(argument);

        actual.Should().NotBeNull();
        actual?.Root?.ToString().Should().Be(expected.Root.ToString());

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
        GetTestCase("Empty", "{}", CustomFoldersConfiguration.Empty),
        GetTestCase("Only Root", "{ \"root\": \"/tmp/jallajalla\" }", new CustomFoldersConfiguration(new DirectoryInfo("/tmp/jallajalla"))),
        GetTestCase("Mostly defaults",
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
        GetTestCase("With only migration type",
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
    };

    private static TestCaseData GetTestCase(
        string testCaseName,
        string folderArg,
        IFoldersConfiguration expected
    ) => new TestCaseData(folderArg, expected).SetArgDisplayNames(testCaseName, folderArg);

    
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentAssertions;
using grate.Commands;
using grate.Configuration;
using NUnit.Framework;

namespace Unit_tests.Basic_tests.CommandLineParsing;

[TestFixture]
[Category("Basic")]
// ReSharper disable once InconsistentNaming
public class FolderConfiguration_
{
    [Test()]
    public async Task Default()
    {
        var cfg = await ParseGrateConfiguration("--sqlfilesdirectory=/tmp");
        _ = cfg?.SqlFilesDirectory ?? new DirectoryInfo("/tmp");

        var expected = FoldersConfiguration.Default(null);
        var actual = cfg?.Folders;

        AssertEquivalent(expected.Values, actual?.Values);
    }

    [Test]
    [TestCaseSource(nameof(FoldersCommandLines))]
    public async Task Default_With_Overrides(string commandLine, Func<KnownFolderNames, KnownFolderNames> applyExpectedOverrides)
    {
        var cfg = await ParseGrateConfiguration("--sqlfilesdirectory=/tmp", commandLine);
        _ = cfg?.SqlFilesDirectory ?? new DirectoryInfo("/tmp");
        var folderConfig = applyExpectedOverrides(KnownFolderNames.Default);
        var expected = FoldersConfiguration.Default(folderConfig);
        
        var actual = cfg?.Folders;
        
        AssertEquivalent(expected.Values, actual?.Values);
    }
    
    [Test]
    [TestCaseSource(nameof(FullyCustomFoldersCommandLines))]
    public async Task Fully_Customised(string root, string foldersArgument, IFoldersConfiguration expected)
    {
        var cfg = await ParseGrateConfiguration("--sqlfilesdirectory="+ root, foldersArgument);
        var actual = cfg?.Folders;
        
        AssertEquivalent(expected.Values, actual?.Values);
    }

    private static readonly object?[] FoldersCommandLines =
    {
        GetTestCase("--folders=up=tables", names => names with { Up = "tables" }),
        GetTestCase("--folders=up=tables;views=projections", names => names with { Up = "tables", Views = "projections"})
    };
    
    private static readonly object?[] FullyCustomFoldersCommandLines =
    {
        GetCustomisedTestCase(
        "Mostly defaults",
        "/tmp/jalla",
@"--folders=folder1=type:Once;folder2=type:EveryTime;folder3=type:AnyTime",
        new FoldersConfiguration(
            new MigrationsFolder("folder1", MigrationType.Once),
            new MigrationsFolder("folder2", MigrationType.EveryTime),
            new MigrationsFolder("folder3", MigrationType.AnyTime)
        )),
    };

    private static TestCaseData GetTestCase(
        string folderArg,
        Func<KnownFolderNames, KnownFolderNames> expectedOverrides,
        [CallerArgumentExpression(nameof(expectedOverrides))] string overridesText = ""
    ) => new TestCaseData(folderArg, expectedOverrides).SetArgDisplayNames(folderArg, overridesText);
    
    private static TestCaseData GetCustomisedTestCase(
        string caseName,
        string root,
        string folderArg,
        IFoldersConfiguration expected
    ) => new TestCaseData(root, folderArg, expected).SetArgDisplayNames(caseName, folderArg);

    private static void AssertEquivalent(
        IEnumerable<MigrationsFolder?> expected,
        IEnumerable<MigrationsFolder?>? actual)
    {
        var expectedArr = expected.ToImmutableArray();
        var actualArr = (actual ?? Enumerable.Empty<MigrationsFolder>()).ToImmutableArray();

        Assert.Multiple(() =>
        {
            actualArr.Length.Should().Be(expectedArr.Length);
            for (var i = 0; i < expectedArr.Length; i++)
            {
                AssertEquivalent(expectedArr[i], actualArr[i]);
            }
        });
    }

    private static void AssertEquivalent(MigrationsFolder? expected, MigrationsFolder? actual)
    {
        Assert.Multiple(() =>
        {
            actual?.Name.Should().Be(expected?.Name);
            actual?.Path?.Should().Be(expected?.Path);
            actual?.Type.Should().Be(expected?.Type);
            actual?.ConnectionType.Should().Be(expected?.ConnectionType);
            actual?.TransactionHandling.Should().Be(expected?.TransactionHandling);
        });
    }


    private static async Task<GrateConfiguration?> ParseGrateConfiguration(params string[] commandline)
    {
        GrateConfiguration? cfg = null;
        var cmd = CommandHandler.Create((GrateConfiguration config) => cfg = config);

        var migrateCommand = new MigrateCommand(null!);
        ParseResult p = new Parser(migrateCommand).Parse(commandline);
        await cmd.InvokeAsync(new InvocationContext(p));
        
        return cfg;
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.CommandLine;
using System.CommandLine.Help;
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
using grate.Infrastructure;
using grate.Migration;
using NUnit.Framework;

namespace grate.unittests.Basic.CommandLineParsing;

[TestFixture]
[Category("Basic")]
// ReSharper disable once InconsistentNaming
public class FolderConfiguration_
{
    [Test()]
    public async Task Default()
    {
        var cfg = await ParseGrateConfiguration("");

        var parent = cfg?.SqlFilesDirectory ?? new DirectoryInfo("/tmp");

        var expected = KnownFolders.In(parent);
        var actual = cfg?.KnownFolders;

        AssertEquivalent(expected.Values, actual?.Values);
    }

    [Test]
    [TestCaseSource(nameof(FoldersCommandLines))]
    public async Task Default_With_Overrides(string commandLine, Func<KnownFolderNames, KnownFolderNames> applyExpectedOverrides)
    {
        var cfg = await ParseGrateConfiguration(commandLine);
        
        var parent = cfg?.SqlFilesDirectory ?? new DirectoryInfo("/tmp");
        var folderConfig = applyExpectedOverrides(KnownFolderNames.Default);
        var expected = KnownFolders.In(parent, folderConfig);
        
        var actual = cfg?.KnownFolders;
        
        AssertEquivalent(expected.Values, actual?.Values);
    }

    private static readonly object?[] FoldersCommandLines =
    {
        GetTestCase("--folders=up:tables", names => names with { Up = "tables" }),
        GetTestCase("--folders=up:tables,views:projections", names => names with { Up = "tables", Views = "projections"})
    };

    private static TestCaseData GetTestCase(
        string folderArg,
        Func<KnownFolderNames, KnownFolderNames> expectedOverrides,
        [CallerArgumentExpression("expectedOverrides")] string overridesText = ""
    ) => new TestCaseData(folderArg, expectedOverrides).SetArgDisplayNames(folderArg, overridesText);

    private static void AssertEquivalent(
        IEnumerable<MigrationsFolder?> expected,
        IEnumerable<MigrationsFolder?>? actual)
    {
        var expectedArr = expected.ToImmutableArray();
        var actualArr = (actual ?? Enumerable.Empty<MigrationsFolder>()).ToImmutableArray();

        Assert.Multiple(() =>
        {
            Assert.AreEqual(expectedArr.Length, actualArr.Length);
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
            actual?.Path?.ToString().Should().Be(expected?.Path?.ToString());
            actual?.Type.Should().Be(expected?.Type);
            actual?.ConnectionType.Should().Be(expected?.ConnectionType);
            actual?.TransactionHandling.Should().Be(expected?.TransactionHandling);
        });
    }


    private static async Task<GrateConfiguration?> ParseGrateConfiguration(string commandline)
    {
        GrateConfiguration? cfg = null;
        var cmd = CommandHandler.Create((GrateConfiguration config) => cfg = config);

        ParseResult p =
            new Parser(new MigrateCommand(null!)).Parse(commandline);
        await cmd.InvokeAsync(new InvocationContext(p));
        return cfg;
    }
}

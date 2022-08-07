using System;
using System.Runtime.CompilerServices;
using FluentAssertions;
using grate.Commands;
using grate.Configuration;
using NUnit.Framework;

namespace grate.unittests.Basic.CommandLineParsing;

[TestFixture]
[Category("Basic")]
// ReSharper disable once InconsistentNaming
public class KnownFolderNamesArgument_
{
    [Test]
    [TestCaseSource(nameof(FoldersCommandLines))]
    public void Can_Parse(string argument, Func<KnownFolderNames, KnownFolderNames> applyExpectedOverrides)
    {
        var actual = KnownFolderNamesArgument.Parse(argument);
        var expected = applyExpectedOverrides(KnownFolderNames.Default);

        actual.Should().Be(expected);
    }

    private static readonly object?[] FoldersCommandLines =
    {
        GetTestCase("up=tables", names => names with { Up = "tables" }),
        GetTestCase("up=tables;views=projections", names => names with { Up = "tables", Views = "projections"})
    };

    private static TestCaseData GetTestCase(
        string folderArg,
        Func<KnownFolderNames, KnownFolderNames> expectedOverrides,
        [CallerArgumentExpression("expectedOverrides")] string overridesText = ""
    ) => new TestCaseData(folderArg, expectedOverrides).SetArgDisplayNames(folderArg, overridesText);

}

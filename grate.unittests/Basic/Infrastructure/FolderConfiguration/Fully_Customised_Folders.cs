﻿using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;
using static grate.Configuration.MigrationType;
using static grate.Migration.ConnectionType;

namespace grate.unittests.Basic.Infrastructure.FolderConfiguration;

[TestFixture]
[TestOf(nameof(FoldersConfiguration))]
[Category("Basic")]
// ReSharper disable once InconsistentNaming
public class Fully_Customised_Folders
{
    [Test]
    public void Returns_folders_in_Expected_Order()
    {
        var items = Folders.Values.ToImmutableArray();

        Assert.Multiple(() =>
        {
            items[0].Should().Be(Folders["structure"]);
            items[1].Should().Be(Folders["randomstuff"]);
            items[2].Should().Be(Folders["procedures"]);
            items[3].Should().Be(Folders["security"]);
        });
    }

    [Test]
    [TestCaseSource(nameof(ExpectedKnownFolderNames))]
    public void Has_expected_folder_configuration(
        MigrationsFolder folder,
        string expectedFolderName,
        MigrationType expectedType,
        ConnectionType expectedConnectionType,
        TransactionHandling transactionHandling
    )
    {
        var root = Root.ToString();

        Assert.Multiple(() =>
        {
            folder.Path?.Should().Be(expectedFolderName);
            folder.Type.Should().Be(expectedType);
            folder.ConnectionType.Should().Be(expectedConnectionType);
            folder.TransactionHandling.Should().Be(transactionHandling);
        });
    }

    
    private static readonly DirectoryInfo Root = TestConfig.CreateRandomTempDirectory();

    private static readonly IFoldersConfiguration Folders = new FoldersConfiguration(
        new MigrationsFolder("structure", Once),
        new MigrationsFolder("randomstuff", AnyTime, Admin, TransactionHandling.Autonomous),
        new MigrationsFolder("procedures", "procs", AnyTime),
        new MigrationsFolder("security", "secret", AnyTime)
    );

    private static readonly object?[] ExpectedKnownFolderNames =
    {
        GetTestCase(Folders["structure"], "structure", Once, Default, TransactionHandling.Default),
        GetTestCase(Folders["randomstuff"], "randomstuff", AnyTime, Admin, TransactionHandling.Autonomous),
        GetTestCase(Folders["procedures"], "procs", AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders["security"], "secret", AnyTime, Default, TransactionHandling.Default),
    };

    private static TestCaseData GetTestCase(
        MigrationsFolder? folder,
        string expectedName,
        MigrationType expectedType,
        ConnectionType expectedConnectionType,
        TransactionHandling transactionHandling,
        [CallerArgumentExpression(nameof(folder))] string migrationsFolderDefinitionName = ""
    ) =>
        new TestCaseData(folder, expectedName, expectedType, expectedConnectionType, transactionHandling)
            .SetArgDisplayNames(
                migrationsFolderDefinitionName,
                expectedName,
                expectedType.ToString(),
                "conn: " + expectedConnectionType,
                "tran: " + transactionHandling
            );
}

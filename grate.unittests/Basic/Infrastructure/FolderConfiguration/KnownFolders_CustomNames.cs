using System;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;
using static grate.Configuration.KnownFolderKeys;
using static grate.Configuration.MigrationType;
using static grate.Migration.ConnectionType;

namespace grate.unittests.Basic.Infrastructure.FolderConfiguration;

[TestFixture]
[TestOf(nameof(FoldersConfiguration))]
[Category("Basic")]
// ReSharper disable once InconsistentNaming
public class KnownFolders_CustomNames
{
    private static readonly Random Random = new Random();

    [Test]
    public void Returns_folders_in_same_order_as_default()
    {
        var items = Folders.Values.ToImmutableArray();

        Assert.Multiple(() =>
        {
            items[0].Should().Be(Folders[BeforeMigration]);
            items[1].Should().Be(Folders[AlterDatabase]);
            items[2].Should().Be(Folders[RunAfterCreateDatabase]);
            items[3].Should().Be(Folders[RunBeforeUp]);
            items[4].Should().Be(Folders[Up]);
            items[5].Should().Be(Folders[RunFirstAfterUp]);
            items[6].Should().Be(Folders[Functions]);
            items[7].Should().Be(Folders[Views]);
            items[8].Should().Be(Folders[Sprocs]);
            items[9].Should().Be(Folders[Triggers]);
            items[10].Should().Be(Folders[Indexes]);
            items[11].Should().Be(Folders[RunAfterOtherAnyTimeScripts]);
            items[12].Should().Be(Folders[Permissions]);
            items[13].Should().Be(Folders[AfterMigration]);
        });
    }

    [Test]
    [TestCaseSource(nameof(ExpectedKnownFolderNames))]
    public void Has_expected_folder_configuration(
        MigrationsFolder folder,
        string expectedName,
        MigrationType expectedType,
        ConnectionType expectedConnectionType,
        TransactionHandling transactionHandling
    )
    {
        var root = Root.ToString();

        Assert.Multiple(() =>
        {
            folder.Path.Should().Be(expectedName);
            folder.Type.Should().Be(expectedType);
            folder.ConnectionType.Should().Be(expectedConnectionType);
            folder.TransactionHandling.Should().Be(transactionHandling);
        });
    }

    private static readonly IKnownFolderNames OverriddenFolderNames = new KnownFolderNames()
    {
        BeforeMigration = "beforeMigration" + Random.GetString(8),
        CreateDatabase = "createDatabase" + Random.GetString(8),
        AlterDatabase = "alterDatabase" + Random.GetString(8),
        RunAfterCreateDatabase = "runAfterCreateDatabase" + Random.GetString(8),
        RunBeforeUp = "runBeforeUp" + Random.GetString(8),
        Up = "up" + Random.GetString(8),
        RunFirstAfterUp = "runFirstAfterUp" + Random.GetString(8),
        Functions = "functions" + Random.GetString(8),
        Views = "views" + Random.GetString(8),
        Sprocs = "sprocs" + Random.GetString(8),
        Triggers = "triggers" + Random.GetString(8),
        Indexes = "indexes" + Random.GetString(8),
        RunAfterOtherAnyTimeScripts = "runAfterOtherAnyTimeScripts" + Random.GetString(8),
        Permissions = "permissions" + Random.GetString(8),
        AfterMigration = "afterMigration" + Random.GetString(8),
    };
    
    private static readonly DirectoryInfo Root = TestConfig.CreateRandomTempDirectory();
    private static readonly IFoldersConfiguration Folders = FoldersConfiguration.Default(OverriddenFolderNames);

    private static readonly object?[] ExpectedKnownFolderNames =
    {
        GetTestCase(Folders[BeforeMigration], OverriddenFolderNames.BeforeMigration, EveryTime, Default, TransactionHandling.Autonomous),
        GetTestCase(Folders[AlterDatabase], OverriddenFolderNames.AlterDatabase, AnyTime, Admin, TransactionHandling.Autonomous),
        GetTestCase(Folders[RunAfterCreateDatabase], OverriddenFolderNames.RunAfterCreateDatabase, AnyTime, Default,
            TransactionHandling.Default),
        GetTestCase(Folders[RunBeforeUp], OverriddenFolderNames.RunBeforeUp, AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders[Up], OverriddenFolderNames.Up, Once, Default, TransactionHandling.Default),
        GetTestCase(Folders[RunFirstAfterUp], OverriddenFolderNames.RunFirstAfterUp, Once, Default, TransactionHandling.Default),
        GetTestCase(Folders[Functions], OverriddenFolderNames.Functions, AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders[Views], OverriddenFolderNames.Views, AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders[Sprocs], OverriddenFolderNames.Sprocs, AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders[Triggers], OverriddenFolderNames.Triggers, AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders[Indexes], OverriddenFolderNames.Indexes, AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders[RunAfterOtherAnyTimeScripts], OverriddenFolderNames.RunAfterOtherAnyTimeScripts, AnyTime, Default,
            TransactionHandling.Default),
        GetTestCase(Folders[Permissions], OverriddenFolderNames.Permissions, EveryTime, Default, TransactionHandling.Autonomous),
        GetTestCase(Folders[AfterMigration], OverriddenFolderNames.AfterMigration, EveryTime, Default, TransactionHandling.Autonomous),
    };

    private static TestCaseData GetTestCase(
        MigrationsFolder? folder,
        string expectedName,
        MigrationType expectedType,
        ConnectionType expectedConnectionType,
        TransactionHandling transactionHandling,
        [CallerArgumentExpression("folder")] string migrationsFolderDefinitionName = ""
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

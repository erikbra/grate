using System;
using System.Collections.Immutable;
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
[TestOf(nameof(KnownFolders))]
[Category("Basic")]
// ReSharper disable once InconsistentNaming
public class KnownFolders_CustomNames
{
    private static readonly Random Random = new Random();

    [Test]
    public void Returns_folders_in_same_order_as_default()
    {
        var items = Folders.ToImmutableArray();

        Assert.Multiple(() =>
        {
            items[0].Should().Be(Folders.BeforeMigration);
            items[1].Should().Be(Folders.AlterDatabase);
            items[2].Should().Be(Folders.RunAfterCreateDatabase);
            items[3].Should().Be(Folders.RunBeforeUp);
            items[4].Should().Be(Folders.Up);
            items[5].Should().Be(Folders.RunFirstAfterUp);
            items[6].Should().Be(Folders.Functions);
            items[7].Should().Be(Folders.Views);
            items[8].Should().Be(Folders.Sprocs);
            items[9].Should().Be(Folders.Triggers);
            items[10].Should().Be(Folders.Indexes);
            items[11].Should().Be(Folders.RunAfterOtherAnyTimeScripts);
            items[12].Should().Be(Folders.Permissions);
            items[13].Should().Be(Folders.AfterMigration);
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
            folder.Path.ToString().Should().Be(Path.Combine(root, expectedName));
            folder.Type.Should().Be(expectedType);
            folder.ConnectionType.Should().Be(expectedConnectionType);
            folder.TransactionHandling.Should().Be(transactionHandling);
        });
    }

    private static IKnownFolderNames FolderNames = new KnownFolderNames()
    {
        BeforeMigration = "beforeMigration" + Random.GetString(8),
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
    private static readonly KnownFolders Folders = KnownFolders.In(Root, FolderNames);

    private static readonly object?[] ExpectedKnownFolderNames =
    {
        GetTestCase(Folders.BeforeMigration, FolderNames.BeforeMigration, EveryTime, Default, TransactionHandling.Autonomous),
        GetTestCase(Folders.AlterDatabase, FolderNames.AlterDatabase, AnyTime, Admin, TransactionHandling.Autonomous),
        GetTestCase(Folders.RunAfterCreateDatabase, FolderNames.RunAfterCreateDatabase, AnyTime, Default,
            TransactionHandling.Default),
        GetTestCase(Folders.RunBeforeUp, FolderNames.RunBeforeUp, AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders.Up, FolderNames.Up, Once, Default, TransactionHandling.Default),
        GetTestCase(Folders.RunFirstAfterUp, FolderNames.RunFirstAfterUp, Once, Default, TransactionHandling.Default),
        GetTestCase(Folders.Functions, FolderNames.Functions, AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders.Views, FolderNames.Views, AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders.Sprocs, FolderNames.Sprocs, AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders.Triggers, FolderNames.Triggers, AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders.Indexes, FolderNames.Indexes, AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders.RunAfterOtherAnyTimeScripts, FolderNames.RunAfterOtherAnyTimeScripts, AnyTime, Default,
            TransactionHandling.Default),
        GetTestCase(Folders.Permissions, FolderNames.Permissions, EveryTime, Default, TransactionHandling.Autonomous),
        GetTestCase(Folders.AfterMigration, FolderNames.AfterMigration, EveryTime, Default, TransactionHandling.Autonomous),
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
                "conn: " + expectedConnectionType.ToString(),
                "tran: " + transactionHandling.ToString()
            );
}

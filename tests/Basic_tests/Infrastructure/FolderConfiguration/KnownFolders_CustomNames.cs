using System.Collections.Immutable;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using TestCommon.TestInfrastructure;
using static grate.Configuration.KnownFolderKeys;
using static grate.Configuration.MigrationType;
using static grate.Migration.ConnectionType;

namespace Basic_tests.Infrastructure.FolderConfiguration;

// ReSharper disable once InconsistentNaming
public class KnownFolders_CustomNames
{
    private static readonly Random Random = new();

    [Fact]
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

    [Theory]
    [MemberData(nameof(ExpectedKnownFolderNames))]
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

    public static IEnumerable<object?[]> ExpectedKnownFolderNames()
    {
        yield return new object?[] { Folders[BeforeMigration], OverriddenFolderNames.BeforeMigration, EveryTime, Default, TransactionHandling.Autonomous };
        yield return new object?[] { Folders[AlterDatabase], OverriddenFolderNames.AlterDatabase, AnyTime, Admin, TransactionHandling.Autonomous };
        yield return new object?[] { Folders[RunAfterCreateDatabase], OverriddenFolderNames.RunAfterCreateDatabase, AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[RunBeforeUp], OverriddenFolderNames.RunBeforeUp, AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[Up], OverriddenFolderNames.Up, Once, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[RunFirstAfterUp], OverriddenFolderNames.RunFirstAfterUp, AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[Functions], OverriddenFolderNames.Functions, AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[Views], OverriddenFolderNames.Views, AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[Sprocs], OverriddenFolderNames.Sprocs, AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[Triggers], OverriddenFolderNames.Triggers, AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[Indexes], OverriddenFolderNames.Indexes, AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[RunAfterOtherAnyTimeScripts], OverriddenFolderNames.RunAfterOtherAnyTimeScripts, AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[Permissions], OverriddenFolderNames.Permissions, EveryTime, Default, TransactionHandling.Autonomous };
        yield return new object?[] { Folders[AfterMigration], OverriddenFolderNames.AfterMigration, EveryTime, Default, TransactionHandling.Autonomous };
    }
}

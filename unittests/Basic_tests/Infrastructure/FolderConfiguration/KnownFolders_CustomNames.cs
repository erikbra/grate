using System.Collections.Immutable;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using TestCommon.TestInfrastructure;
using static grate.Configuration.KnownFolderKeys;
using static grate.Configuration.MigrationType;
using static grate.Migration.ConnectionType;
using static TestCommon.TestInfrastructure.DescriptiveTestObjects;

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
        string name,
        MigrationType type,
        ConnectionType conn,
        TransactionHandling tran
    )
    {
        var root = Root.ToString();

        Assert.Multiple(() =>
        {
            folder.Path.Should().Be(name);
            folder.Type.Should().Be(type);
            folder.ConnectionType.Should().Be(conn);
            folder.TransactionHandling.Should().Be(tran);
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
    
    public static TheoryData<MigrationsFolderWithDescription, string, MigrationType, ConnectionType, TransactionHandling> ExpectedKnownFolderNames()
    {
        var data = new TheoryData<MigrationsFolderWithDescription, string, MigrationType, ConnectionType, TransactionHandling>
        {
            { Describe(Folders[BeforeMigration])!, OverriddenFolderNames.BeforeMigration, EveryTime, Default, TransactionHandling.Autonomous },
            { Describe(Folders[AlterDatabase])!, OverriddenFolderNames.AlterDatabase, AnyTime, Admin, TransactionHandling.Autonomous },
            { Describe(Folders[RunAfterCreateDatabase])!, OverriddenFolderNames.RunAfterCreateDatabase, AnyTime, Default, TransactionHandling.Default },
            { Describe(Folders[RunBeforeUp])!, OverriddenFolderNames.RunBeforeUp, AnyTime, Default, TransactionHandling.Default },
            { Describe(Folders[Up])!, OverriddenFolderNames.Up, Once, Default, TransactionHandling.Default },
            { Describe(Folders[RunFirstAfterUp])!, OverriddenFolderNames.RunFirstAfterUp, AnyTime, Default, TransactionHandling.Default },
            { Describe(Folders[Functions])!, OverriddenFolderNames.Functions, AnyTime, Default, TransactionHandling.Default },
            { Describe(Folders[Views])!, OverriddenFolderNames.Views, AnyTime, Default, TransactionHandling.Default },
            { Describe(Folders[Sprocs])!, OverriddenFolderNames.Sprocs, AnyTime, Default, TransactionHandling.Default },
            { Describe(Folders[Triggers])!, OverriddenFolderNames.Triggers, AnyTime, Default, TransactionHandling.Default },
            { Describe(Folders[Indexes])!, OverriddenFolderNames.Indexes, AnyTime, Default, TransactionHandling.Default },
            { Describe(Folders[RunAfterOtherAnyTimeScripts])!, OverriddenFolderNames.RunAfterOtherAnyTimeScripts, AnyTime, Default, TransactionHandling.Default },
            { Describe(Folders[Permissions])!, OverriddenFolderNames.Permissions, EveryTime, Default, TransactionHandling.Autonomous },
            { Describe(Folders[AfterMigration])!, OverriddenFolderNames.AfterMigration, EveryTime, Default, TransactionHandling.Autonomous }
       };
       return data;
    }
}

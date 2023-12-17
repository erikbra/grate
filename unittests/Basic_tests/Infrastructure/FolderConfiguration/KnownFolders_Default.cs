using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using TestCommon.TestInfrastructure;
using Xunit;
using static grate.Configuration.KnownFolderKeys;
using static grate.Configuration.MigrationType;
using static grate.Migration.ConnectionType;

namespace Basic_tests.Infrastructure.FolderConfiguration;


// ReSharper disable once InconsistentNaming
public class KnownFolders_Default
{
    [Fact]
    public void Returns_folders_in_current_order()
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
            folder.Path?.Should().Be(expectedName);
            folder.Type.Should().Be(expectedType);
            folder.ConnectionType.Should().Be(expectedConnectionType);
            folder.TransactionHandling.Should().Be(transactionHandling);
        });
    }

    private static readonly DirectoryInfo Root = TestConfig.CreateRandomTempDirectory();
    private static readonly IFoldersConfiguration Folders = FoldersConfiguration.Default(null);

    public static IEnumerable<object?[]> ExpectedKnownFolderNames()
    {
        yield return new object?[] { Folders[BeforeMigration], "beforeMigration", EveryTime, Default, TransactionHandling.Autonomous };
        yield return new object?[] { Folders[AlterDatabase], "alterDatabase", AnyTime, Admin, TransactionHandling.Autonomous };
        yield return new object?[] { Folders[RunAfterCreateDatabase], "runAfterCreateDatabase", AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[RunBeforeUp], "runBeforeUp", AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[Up], "up", Once, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[RunFirstAfterUp], "runFirstAfterUp", AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[Functions], "functions", AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[Views], "views", AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[Sprocs], "sprocs", AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[Triggers], "triggers", AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[Indexes], "indexes", AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[RunAfterOtherAnyTimeScripts], "runAfterOtherAnyTimeScripts", AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders[Permissions], "permissions", EveryTime, Default, TransactionHandling.Autonomous };
        yield return new object?[] { Folders[AfterMigration], "afterMigration", EveryTime, Default, TransactionHandling.Autonomous };
    }
}

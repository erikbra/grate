using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using TestCommon.TestInfrastructure;
using Xunit;
using static grate.Configuration.MigrationType;
using static grate.Migration.ConnectionType;

namespace Basic_tests.Infrastructure.FolderConfiguration;

// ReSharper disable once InconsistentNaming
public class Fully_Customised_Folders
{
    [Fact]
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

    [Theory]
    [MemberData(nameof(ExpectedKnownFolderNames))]
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

    public static IEnumerable<object?[]> ExpectedKnownFolderNames()
    {
        yield return new object?[] { Folders["structure"], "structure", Once, Default, TransactionHandling.Default };
        yield return new object?[] { Folders["randomstuff"], "randomstuff", AnyTime, Admin, TransactionHandling.Autonomous };
        yield return new object?[] { Folders["procedures"], "procs", AnyTime, Default, TransactionHandling.Default };
        yield return new object?[] { Folders["security"], "secret", AnyTime, Default, TransactionHandling.Default };
    }

}

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
public class KnownFolders_Default
{
    [Test]
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
            folder.Path?.Should().Be(expectedName);
            folder.Type.Should().Be(expectedType);
            folder.ConnectionType.Should().Be(expectedConnectionType);
            folder.TransactionHandling.Should().Be(transactionHandling);
        });
    }

    private static readonly DirectoryInfo Root = TestConfig.CreateRandomTempDirectory();
    private static readonly IFoldersConfiguration Folders = FoldersConfiguration.Default(null);

    private static readonly object?[] ExpectedKnownFolderNames =
    {
        GetTestCase(Folders[BeforeMigration] ,"beforeMigration", EveryTime, Default, TransactionHandling.Autonomous),
        GetTestCase(Folders[AlterDatabase] ,"alterDatabase", AnyTime, Admin, TransactionHandling.Autonomous),
        GetTestCase(Folders[RunAfterCreateDatabase] ,"runAfterCreateDatabase", AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders[RunBeforeUp] ,"runBeforeUp", AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders[Up] ,"up", Once, Default, TransactionHandling.Default),
        GetTestCase(Folders[RunFirstAfterUp] ,"runFirstAfterUp", Once, Default, TransactionHandling.Default),
        GetTestCase(Folders[Functions] ,"functions", AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders[Views] ,"views", AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders[Sprocs] ,"sprocs", AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders[Triggers] ,"triggers", AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders[Indexes] ,"indexes", AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders[RunAfterOtherAnyTimeScripts] ,"runAfterOtherAnyTimeScripts", AnyTime, Default, TransactionHandling.Default),
        GetTestCase(Folders[Permissions] ,"permissions", EveryTime, Default, TransactionHandling.Autonomous),
        GetTestCase(Folders[AfterMigration] ,"afterMigration", EveryTime, Default, TransactionHandling.Autonomous),
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

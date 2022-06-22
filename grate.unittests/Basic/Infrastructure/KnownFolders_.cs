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

namespace grate.unittests.Basic.Infrastructure;

[TestFixture]
[TestOf(nameof(KnownFolders))]
[Category("Basic")]
// ReSharper disable once InconsistentNaming
public class KnownFolders_
{
    [Test]
    public void Returns_folder_in_existing_order()
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
    public void Has_expected_current_folder_configuration(
            MigrationsFolder folder, 
            string expectedName, 
            MigrationType expectedType,
            ConnectionType expectedConnectionType
        )
    {
        var root = Root.ToString();
        
        Assert.Multiple(() =>
        {
            folder.Path.ToString().Should().Be(Path.Combine(root, expectedName));
            folder.Type.Should().Be(expectedType);
            folder.ConnectionType.Should().Be(expectedConnectionType);
        });
    }

    private static readonly DirectoryInfo Root = TestConfig.CreateRandomTempDirectory();
    private static readonly KnownFolders Folders = KnownFolders.In(Root);

    private static readonly object?[] ExpectedKnownFolderNames =
    {
        GetTestCase(Folders.BeforeMigration ,"beforeMigration", EveryTime, Default),
        GetTestCase(Folders.AlterDatabase ,"alterDatabase", AnyTime, Admin),
        GetTestCase(Folders.RunAfterCreateDatabase ,"runAfterCreateDatabase", AnyTime, Default),
        GetTestCase(Folders.RunBeforeUp ,"runBeforeUp", AnyTime, Default),
        GetTestCase(Folders.Up ,"up", Once, Default),
        GetTestCase(Folders.RunFirstAfterUp ,"runFirstAfterUp", Once, Default),
        GetTestCase(Folders.Functions ,"functions", AnyTime, Default),
        GetTestCase(Folders.Views ,"views", AnyTime, Default),
        GetTestCase(Folders.Sprocs ,"sprocs", AnyTime, Default),
        GetTestCase(Folders.Triggers ,"triggers", AnyTime, Default),
        GetTestCase(Folders.Indexes ,"indexes", AnyTime, Default),
        GetTestCase(Folders.RunAfterOtherAnyTimeScripts ,"runAfterOtherAnyTimeScripts", AnyTime, Default),
        GetTestCase(Folders.Permissions ,"permissions", EveryTime, Default),
        GetTestCase(Folders.AfterMigration ,"afterMigration", EveryTime, Default),
    };
    
    private static TestCaseData GetTestCase(
        MigrationsFolder? folder,
        string expectedName,
        MigrationType expectedType,
        ConnectionType expectedConnectionType,
        [CallerArgumentExpression("folder")] string migrationsFolderDefinitionName = ""
    ) =>
        new TestCaseData(folder, expectedName, expectedType, expectedConnectionType)
            .SetArgDisplayNames(
                    migrationsFolderDefinitionName, 
                    expectedName,
                    expectedType.ToString(),
                    expectedConnectionType.ToString()
                );
   
}

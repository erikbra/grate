using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Generic.Running_MigrationScripts;

[TestFixture]
// ReSharper disable once InconsistentNaming
public abstract class Versioning_The_Database : MigrationsScriptsBase
{
    [Test]
    public async Task Returns_The_New_Version_Id()
    {
        var db = TestConfig.RandomDatabase();

        GrateMigrator? migrator;

        var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
        CreateDummySql(knownFolders.Sprocs);
            
        await using (migrator = Context.GetMigrator(db, knownFolders))
        {
            await migrator.Migrate();

            using (new AssertionScope())
            {
                // Version again
                var version = await migrator.DbMigrator.VersionTheDatabase("1.2.3.4");
                version.Should().Be(2);
                
                // And again
                version = await migrator.DbMigrator.VersionTheDatabase("1.2.3.4");
                version.Should().Be(3);
            }
        }
    }
}

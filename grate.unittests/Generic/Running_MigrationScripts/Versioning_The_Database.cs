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
        
        var parent = CreateRandomTempDirectory();
        var knownFolders = KnownFolders.In();
        CreateDummySql(parent, knownFolders.Sprocs);

        await using (migrator = Context.GetMigrator(db, parent, knownFolders))
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

    [Test]
    public async Task Does_Not_Create_Versions_When_Dryrun()
    {
        //for bug #204 - when running --baseline and --dryrun on a new db it shouldn't create the grate schema's etc
        var db = TestConfig.RandomDatabase();
        
        var parent = CreateRandomTempDirectory();
        var knownFolders = KnownFolders.In();

        CreateDummySql(parent, knownFolders.Sprocs); // make sure there's something that could be logged...

        var grateConfig = Context.GetConfiguration(db, parent, knownFolders) with
        {
            Baseline = true, // don't run the sql
            DryRun = true // and don't actually _touch_ the DB in any way
        };

        await using var migrator = Context.GetMigrator(grateConfig);
        await migrator.Migrate(); // shouldn't touch anything because of --dryrun
        var addedTable = await migrator.DbMigrator.Database.VersionTableExists();
        Assert.False(addedTable); // we didn't even add the grate infrastructure
    }
}

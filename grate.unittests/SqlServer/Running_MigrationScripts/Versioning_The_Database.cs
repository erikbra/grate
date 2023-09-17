using System.Threading.Tasks;
using grate.Configuration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

using static grate.Configuration.KnownFolderKeys;

namespace grate.unittests.SqlServer.Running_MigrationScripts;

[TestFixture]
[Category("SqlServer")]
[NonParallelizable]
// ReSharper disable once InconsistentNaming
public class Versioning_The_Database: Generic.Running_MigrationScripts.Versioning_The_Database
{
    protected override IGrateTestContext Context => GrateTestContext.SqlServer;


    [Test]
    public async Task Bug230_Uses_Server_Casing_Rules_For_Schema()
    {
        //for bug #230 - when targeting an existing schema use the servers casing rules, not .Net's
        var db = TestConfig.RandomDatabase();
        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        CreateDummySql(parent, knownFolders[Sprocs]); // make sure there's something that could be logged...

        await using (var migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            await migrator.Migrate();
            Assert.True(await migrator.DbMigrator.Database.VersionTableExists()); // we migrated into the `grate` schema.
        }

        // Now we'll run again with the same name but different cased schema
        var grateConfig = Context.GetConfiguration(db, parent, knownFolders) with
        {
            SchemaName = "GRATE"
        };

        await using (var migrator = Context.GetMigrator(grateConfig))
        {
            await migrator.Migrate(); // should either reuse the existing schema if a case-insensitive server, or create a new second schema for use if case-sensitive.
            Assert.True(await migrator.DbMigrator.Database.VersionTableExists()); // we migrated into the `GRATE` schema, which may be the same as 'grate' depending on server settings.
        }
    }
}

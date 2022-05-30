using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
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

    [Test]
    public async Task Creates_A_New_Version_In_Progress()
    {
        var db = TestConfig.RandomDatabase();
        var dbVersion = "1.2.3.4";

        GrateMigrator? migrator;

        var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
        CreateDummySql(knownFolders.Up);

        long newVersionId = 0;

        await using (migrator = Context.GetMigrator(db, knownFolders))
        {
            //Calling migrate here to setup the database and such.
            await migrator.Migrate();
            newVersionId = await migrator.DbMigrator.VersionTheDatabase(dbVersion);
        }

        IEnumerable<(string version, string status)> entries;
        string sql = $"SELECT version, status FROM {Context.Syntax.TableWithSchema("grate", "Version")}";

        await using (var conn = Context.CreateDbConnection(db))
        {
            entries = await conn.QueryAsync<(string version, string status)>(sql);
        }

        entries.Should().HaveCount(2);
        var version = entries.Single(x => x.version == dbVersion);
        version.status.Should().Be(MigrationStatus.InProgress);
    }
}

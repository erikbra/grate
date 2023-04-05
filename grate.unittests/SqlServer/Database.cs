using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServer;

[TestFixture]
[Category("SqlServer")]
public class Database: Generic.GenericDatabase
{
    protected override IGrateTestContext Context => GrateTestContext.SqlServer;

    [Test]
    public async Task Does_not_needlessly_apply_case_sensitive_database_name_checks_Issue_167()
    {
        // There's a bug where if the database name specified by the user differs from the actual database only by case then
        // Grate currently attempts to create the database again, only for it to fail on the DBMS (Sql Server, case insensitive  bug only).

        var db = "CASEDATABASE";
        await CreateDatabase(db);

        // Check that the database has been created
        IEnumerable<string> databasesBeforeMigration = await GetDatabases();
        databasesBeforeMigration.Should().Contain(db);

        await using var migrator = GetMigrator(GetConfiguration(db.ToLower(), true)); // ToLower is important here, this reproduces the bug in #167
        // There should be no errors running the migration
        Assert.DoesNotThrowAsync(() => migrator.Migrate());
    }
}

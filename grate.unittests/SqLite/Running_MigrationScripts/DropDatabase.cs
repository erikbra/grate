using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.Sqlite.Running_MigrationScripts;

[TestFixture]
[Category("Sqlite")]
public class DropDatabase : Generic.Running_MigrationScripts.DropDatabase
{
    protected override IGrateTestContext Context => GrateTestContext.Sqlite;
}

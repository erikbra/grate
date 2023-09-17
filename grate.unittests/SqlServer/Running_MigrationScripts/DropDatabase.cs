using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.SqlServer.Running_MigrationScripts;

[TestFixture]
[Category("SqlServer")]
public class DropDatabase : Generic.Running_MigrationScripts.DropDatabase
{
    protected override IGrateTestContext Context => GrateTestContext.SqlServer;
}

using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.Sqlite.Running_MigrationScripts;

[TestFixture]
[Category("Sqlite")]
public class TokenScripts : Generic.Running_MigrationScripts.TokenScripts
{
    protected override IGrateTestContext Context => GrateTestContext.Sqlite;
}

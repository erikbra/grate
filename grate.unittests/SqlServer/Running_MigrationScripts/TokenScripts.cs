using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.SqlServer.Running_MigrationScripts;

[TestFixture]
[Category("SqlServer")]
public class TokenScripts : Generic.Running_MigrationScripts.TokenScripts
{
    protected override IGrateTestContext Context => GrateTestContext.SqlServer;
}

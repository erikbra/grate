using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.PostgreSQL.Running_MigrationScripts;

[TestFixture]
[Category("PostgreSQL")]
public class TokenScripts : Generic.Running_MigrationScripts.TokenScripts
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}

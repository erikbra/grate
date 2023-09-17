using NUnit.Framework;
using TestCommon.TestInfrastructure;

namespace TestCommon.PostgreSQL.Running_MigrationScripts;

[TestFixture]
[Category("PostgreSQL")]
public class TokenScripts : Generic.Running_MigrationScripts.TokenScripts
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}

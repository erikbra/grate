using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.Running_MigrationScripts;

[TestFixture]
[Category("PostgreSQL")]
public class TokenScripts : TestCommon.Generic.Running_MigrationScripts.TokenScripts
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}

using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace SqlServer.Running_MigrationScripts;

[TestFixture]
[Category("SqlServer")]
public class TokenScripts : TestCommon.Generic.Running_MigrationScripts.TokenScripts
{
    protected override IGrateTestContext Context => GrateTestContext.SqlServer;
}

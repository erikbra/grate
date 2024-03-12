using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

[Collection(nameof(OracleGrateTestContext))]
public class TokenScripts(OracleGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.TokenScripts(testContext, testOutput)
{
    protected override string CreateDatabaseName => base.CreateDatabaseName + " FROM DUAL";
    protected override string CreateViewMyCustomToken => base.CreateViewMyCustomToken + " FROM DUAL";
}

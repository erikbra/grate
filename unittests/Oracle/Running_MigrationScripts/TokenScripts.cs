using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

[Collection(nameof(OracleTestContainer))]
public class TokenScripts(IGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.TokenScripts(testContext, testOutput)
{
    protected override string CreateDatabaseName => base.CreateDatabaseName + " FROM DUAL";
    protected override string CreateViewMyCustomToken => base.CreateViewMyCustomToken + " FROM DUAL";
}

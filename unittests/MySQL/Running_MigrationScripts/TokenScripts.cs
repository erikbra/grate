using MySQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace MySQL.Running_MigrationScripts;

[Collection(nameof(MySqlGrateTestContext))]
public class TokenScripts(MySqlGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.TokenScripts(testContext, testOutput);


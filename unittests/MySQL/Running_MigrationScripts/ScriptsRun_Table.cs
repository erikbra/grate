using MySQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace MySQL.Running_MigrationScripts;

[Collection(nameof(MySqlGrateTestContext))]
public class ScriptsRun_Table(MySqlGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.ScriptsRun_Table(testContext, testOutput);

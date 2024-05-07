using MySQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace MySQL.Running_MigrationScripts;

[Collection(nameof(MySqlGrateTestContext))]
// ReSharper disable once InconsistentNaming
public class Environment_scripts(MySqlGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Environment_scripts(testContext, testOutput);

using MySQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace MySQL.Running_MigrationScripts;

[Collection(nameof(MySqlGrateTestContext))]
// ReSharper disable once UnusedType.Global
public class DropDatabase(MySqlGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.DropDatabase(testContext, testOutput);

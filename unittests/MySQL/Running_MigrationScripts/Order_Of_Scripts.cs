using MySQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace MySQL.Running_MigrationScripts;

[Collection(nameof(MySqlGrateTestContext))]
// ReSharper disable once InconsistentNaming
public class Order_Of_Scripts(MySqlGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Order_Of_Scripts(testContext, testOutput);


using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

[Collection(nameof(OracleGrateTestContext))]
public class Order_Of_Scripts(OracleGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Order_Of_Scripts(testContext, testOutput);
